using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace GoogleMobileAds.Common
{
    #region Exception payload definition
    [Serializable]
    public struct ExceptionLoggablePayload
    {
        public ExceptionReport unity_gma_sdk_exception_message;
    }

    /// <summary>
    /// A data structure to hold all relevant info for a single exception event.
    /// </summary>
    [Serializable]
    public class ExceptionReport
    {
        // JSPB compatibility: 64-bit integers must be sent as strings to avoid precision loss.
        public string time_msec;
        public bool trapped;
        public string name;
        public string exception_class;
        public string top_exception;
        public string stacktrace;
        public string stacktrace_hash;

        // Static metadata.
        public string session_id;
        public string app_id;
        public string app_version_name;
        public string platform;
        public string unity_version;
        public string os_version;
        public string device_model;
        public string country;
        public int total_cpu;
        public string total_memory_bytes;

        // Dynamic metadata.
        public string network_type;
        public string orientation;
    }
    #endregion

    /// <summary>
    /// A persistent singleton that captures all trapped and untrapped C# exceptions.
    /// It enriches them with device metadata and sends them in batches to a backend service (RCS)
    /// based on either a count or time threshold.
    /// </summary>
    public class GlobalExceptionHandler : RcsClient<ExceptionReport>
    {
        private static GlobalExceptionHandler _instance;
        public static GlobalExceptionHandler Instance
        {
            get
            {
                if (_instance == null && Application.isPlaying)
                {
                    _instance = FindObjectOfType<GlobalExceptionHandler>();
                    if (_instance == null)
                    {
                        _instance = new GameObject("GlobalExceptionHandler")
                                .AddComponent<GlobalExceptionHandler>();
                    }
                }
                return _instance;
            }
        }

        #region Unity lifecycle methods
        private void Awake()
        {
            // Enforce the singleton pattern.
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            Application.logMessageReceivedThreaded += OnLogMessageReceivedThreaded;
        }

        private void OnDisable()
        {
            Application.logMessageReceivedThreaded -= OnLogMessageReceivedThreaded;
        }
        #endregion

        #region Public reporting method
        /// <summary>
        /// Call this from any 'try-catch' block to report a TRAPPED exception.
        /// This method is thread-safe and adds the exception to the queue.
        /// </summary>
        public void ReportTrappedException(Exception e, string name = null)
        {
            if (e == null) return;

            var report = new ExceptionReport
            {
                time_msec = GetEpochMillis(),
                trapped = true,
                name = name,
                exception_class = e.GetType().FullName,
                top_exception = e.Message,
                stacktrace = e.StackTrace ?? "",
                stacktrace_hash = Sha256Hash(e.StackTrace ?? ""),
            };
            Enqueue(report);
        }
        #endregion

        #region Core logic
        /// <summary>
        /// This callback handles UNTRAPPED exceptions from *any* thread.
        /// It must be thread-safe and very fast.
        /// </summary>
        internal void OnLogMessageReceivedThreaded(string logString, string stackTrace, LogType type)
        {
            if (type != LogType.Exception) return;

            // Parse exception details from the log string.
            string topException = logString.Split(new[] { '\n' }, 2)[0].Trim();
            string exceptionClass = topException.Split(':')[0].Trim();

            var report = new ExceptionReport
            {
                time_msec = GetEpochMillis(),
                trapped = false,
                exception_class = exceptionClass,
                top_exception = topException,
                stacktrace = stackTrace ?? "",
                stacktrace_hash = Sha256Hash(stackTrace ?? ""),
            };
            Enqueue(report);
        }

        private string Sha256Hash(string rawData)
        {
            try
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                    // Convert each byte of the computed hash into a two-character hexadecimal
                    // string.
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("X2"));
                    }
                    return builder.ToString();
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Builds and sends a batch of exception reports.
        /// </summary>
        protected override void SendBatch(List<ExceptionReport> batch)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log(string.Format("Processing and sending a batch of {0} exceptions...",
                                        batch.Count));
            }

            var staticMetadata = RcsPayload.GetStaticMetadata();
            var dynamicMetadata = RcsPayload.GetDynamicMetadata();

            foreach(var report in batch)
            {
                report.session_id = staticMetadata.session_id;
                report.app_id = staticMetadata.app_id;
                report.app_version_name = staticMetadata.app_version_name;
                report.platform = staticMetadata.platform;
                report.unity_version = staticMetadata.unity_version;
                report.os_version = staticMetadata.os_version;
                report.device_model = staticMetadata.device_model;
                report.country = staticMetadata.country;
                report.total_cpu = staticMetadata.total_cpu;
                report.total_memory_bytes = staticMetadata.total_memory_bytes;
                report.network_type = dynamicMetadata.network_type;
                report.orientation = dynamicMetadata.orientation;
            }

            var payloads = new List<ExceptionLoggablePayload>();
            foreach (var report in batch)
            {
                payloads.Add(new ExceptionLoggablePayload
                {
                    unity_gma_sdk_exception_message = report
                });
            }

            var request = new LoggableRemoteCaptureRequest<ExceptionLoggablePayload>
            {
                payloads = payloads,
                client_ping_metadata = new ClientPingMetadata
                {
                    binary_name = 21, // UNITY_GMA_SDK
                }
            };
            string jspbPayload = JspbConverter.ToJspb(request);
            if (jspbPayload != null)
            {
                SendToRcs(jspbPayload);
            }
        }
        #endregion
    }
}
