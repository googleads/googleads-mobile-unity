using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace GoogleMobileAds.Common
{
    /// <summary>
    /// A persistent singleton that captures all trapped and untrapped C# exceptions.
    /// It enriches them with device metadata and sends them in batches to a backend service (RCS)
    /// based on either a count or time threshold.
    /// </summary>
    public class GlobalExceptionHandler : MonoBehaviour
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
            private set
            {
                _instance = value;
            }
        }

        // Batching triggers are hardcoded constants. We don't need to expose them in Unity Editor.
        // If any trigger fires, a batch of exceptions will get sent.
        private const int CountThreshold = 50;
        private const float TimeThresholdInSeconds = 30.0f;

        // RCS endpoint for exception reporting. The `e=1` URL parameter defines JSPB encoding.
        // The `f=1` URL parameter indicates that this client is forward compatible with unplanned
        // changes to the service's response format.
        private const string ProdRcsUrl = "https://pagead2.googlesyndication.com/pagead/ping?e=1&f=1";

        private static readonly Queue<ExceptionReport> _exceptionQueue =
            new Queue<ExceptionReport>();
        private static readonly object _queueLock = new object();
        private float _timeOfNextBatch;

        #region Exception payload definition
        [Serializable]
        public struct LoggableRemoteCaptureRequest
        {
            public List<LoggablePayload> payloads;
            public ClientPingMetadata client_ping_metadata;
        }

        [Serializable]
        public struct LoggablePayload
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

        [Serializable]
        public struct ClientPingMetadata
        {
            public int binary_name;
        }
        #endregion

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

        private void Start()
        {
            RcsPayload.InitializeStaticMetadata();
            ResetBatchTimer();
        }

        private void OnEnable()
        {
            Application.logMessageReceivedThreaded += OnLogMessageReceivedThreaded;
        }

        private void OnDisable()
        {
            Application.logMessageReceivedThreaded -= OnLogMessageReceivedThreaded;
        }

        /// <summary>
        /// Runs every frame to check if either of our batching triggers has been met.
        /// </summary>
        private void Update()
        {
            int count;
            lock (_queueLock)
            {
                count = _exceptionQueue.Count;
            }
            bool isCountThresholdMet = count >= CountThreshold;
            bool isTimeThresholdMet = Time.time >= _timeOfNextBatch;
            if (isCountThresholdMet || isTimeThresholdMet)
            {
                ProcessAndSendBatch();
            }
        }

        /// <summary>
        /// Sends pending exceptions before the application quits.
        /// </summary>
        private void OnApplicationQuit()
        {
            ProcessAndSendBatch();
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
            lock (_queueLock)
            {
                _exceptionQueue.Enqueue(report);
            }

            Debug.Log("Trapped exception queued for batch.");
        }
        #endregion

        #region Private core logic
        /// <summary>
        /// This callback handles UNTRAPPED exceptions from *any* thread.
        /// It must be thread-safe and very fast.
        /// </summary>
        private void OnLogMessageReceivedThreaded(string logString, string stackTrace, LogType type)
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
            lock (_queueLock)
            {
                _exceptionQueue.Enqueue(report);
            }

            Debug.Log("Untrapped exception queued for batch.");
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
        /// Drains the queue and passes the resulting batch to be sent.
        /// </summary>
        private void ProcessAndSendBatch()
        {
            Debug.Log("Processing and sending a batch of exceptions...");

            ResetBatchTimer();

            List<ExceptionReport> batch = new List<ExceptionReport>();
            lock (_queueLock)
            {
                if (_exceptionQueue.Count == 0) return;
                while(_exceptionQueue.Count > 0)
                {
                    batch.Add(_exceptionQueue.Dequeue());
                }
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

            if (batch.Count > 0)
            {
                SendToRcs(batch);
            }
        }

        /// <summary>
        /// Resets the batch timer to the current time plus the threshold.
        /// </summary>
        private void ResetBatchTimer()
        {
            _timeOfNextBatch = Time.time + TimeThresholdInSeconds;
        }

        /// <summary>
        /// Builds the final JSON payload (conforming to JSPB rules) and sends it to RCS.
        /// </summary>
        private void SendToRcs(List<ExceptionReport> batch)
        {
            Debug.Log(string.Format("Sending a batch of {0} exception(s)...", batch.Count));

            var payloads = new List<LoggablePayload>();
            foreach (var report in batch)
            {
                payloads.Add(new LoggablePayload
                {
                    unity_gma_sdk_exception_message = report
                });
            }

            var request = new LoggableRemoteCaptureRequest
            {
                payloads = payloads,
                client_ping_metadata = new ClientPingMetadata
                {
                    binary_name = 21, // UNITY_GMA_SDK
                }
            };
            // TODO(jochac): Use http://go/jspb-wireformat#message-layout instead.
            string jsonPayload = JsonUtility.ToJson(request);
            Debug.Log("RCS JSON payload: " + jsonPayload);

            StartCoroutine(PostRequest(ProdRcsUrl, jsonPayload));
        }

        /// <summary>
        /// Coroutine to send a JSON payload via HTTP POST.
        /// </summary>
        private IEnumerator PostRequest(string url, string jsonPayload)
        {
            using (UnityWebRequest uwr = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
                uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
                uwr.downloadHandler = new DownloadHandlerBuffer();
                uwr.SetRequestHeader("Content-Type", "application/json");

                yield return uwr.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
                if (uwr.result != UnityWebRequest.Result.Success)
#else
                if (uwr.isHttpError || uwr.isNetworkError)
#endif
                {
                    Debug.LogError(string.Format(
                        "Error sending exception batch: {0} | Response code: {1}.",
                        uwr.error, uwr.responseCode));
                }
                else
                {
                    Debug.Log("Exception batch sent successfully.");
                }
            }
        }

        private string GetEpochMillis()
        {
            return ((long)DateTime.UtcNow
                    .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                    .TotalMilliseconds).ToString();
        }
        #endregion
    }
}
