using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoogleMobileAds.Common
{
    [Serializable]
    public struct CuiLoggablePayload
    {
        public Insight unity_gma_sdk_cui_message;
    }
    /// <summary>
    /// A persistent singleton that captures all CUIs and sends them in batches to a backend
    /// service (RCS) based on either a count or time threshold.
    /// </summary>
    public class CuiHandler : RcsClient<Insight>
    {
        private static CuiHandler _instance;
        public static CuiHandler Instance
        {
            get
            {
                if (_instance == null && Application.isPlaying)
                {
                    _instance = FindObjectOfType<CuiHandler>();
                    if (_instance == null)
                    {
                        _instance = new GameObject("CuiHandler")
                                .AddComponent<CuiHandler>();
                    }
                }
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        #region Unity lifecycle methods
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        #endregion

        /// <summary>
        /// Call this to report a CUI.
        /// This method is thread-safe and adds the CUI to the queue.
        /// </summary>
        public void ReportCui(Insight insight)
        {
            if (insight == null) return;
            Enqueue(insight);
        }

        /// <summary>
        /// Builds and sends a batch of CUI reports.
        /// </summary>
        protected override void SendBatch(List<Insight> batch)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log(string.Format("Processing and sending a batch of {0} CUIs...",
                                        batch.Count));
            }

            var payloads = new List<CuiLoggablePayload>();
            foreach (var report in batch)
            {
                payloads.Add(new CuiLoggablePayload
                {
                    unity_gma_sdk_cui_message = report
                });
            }

            var request = new LoggableRemoteCaptureRequest<CuiLoggablePayload>
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
                if (Debug.isDebugBuild)
                {
                    Debug.Log("rcs jspb payload is not null: " + jspbPayload);
                }
                SendToRcs(jspbPayload);
            }
        }
    }
}