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
    public class InsightsEmitter : RcsClient<Insight>, IInsightsEmitter
    {
        private static InsightsEmitter _instance;
        public static InsightsEmitter Instance
        {
            get
            {
                if (_instance == null && Application.isPlaying)
                {
                    _instance = FindObjectOfType<InsightsEmitter>();
                    if (_instance == null)
                    {
                        _instance = new GameObject("InsightsEmitter")
                                .AddComponent<InsightsEmitter>();
                    }
                }
                return _instance;
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
        public void Emit(Insight insight)
        {
            if (insight == null) return;
            Enqueue(insight);
        }

        /// <summary>
        /// Builds and sends a batch of CUIs.
        /// </summary>
        protected override void SendBatch(List<Insight> batch)
        {
            var payloads = new List<CuiLoggablePayload>();
            foreach (var insight in batch)
            {
                payloads.Add(new CuiLoggablePayload
                {
                    unity_gma_sdk_cui_message = insight
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
                SendToRcs(jspbPayload);
            }
        }
    }
}
