using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace GoogleMobileAds.Common
{
    [Serializable]
    public struct LoggableRemoteCaptureRequest<TPayload>
    {
        public List<TPayload> payloads;
        public ClientPingMetadata client_ping_metadata;
    }

    [Serializable]
    public struct ClientPingMetadata
    {
        public int binary_name;
    }

    /// <summary>
    /// An abstract base class for clients that send batches of items to RCS.
    /// It handles queueing, batching triggers (count or time), and POSTing data.
    /// </summary>
    public abstract class RcsClient<TReport> : MonoBehaviour where TReport : class
    {
        private class BypassCertificateHandler : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                return true;
            }
        }

        // Batching triggers can be overridden by subclasses. We don't need to expose them in Unity
        // Editor. If any trigger fires, a batch of items will get sent.
        protected virtual int CountThreshold => 20;
        protected virtual float TimeThresholdInSeconds => 120.0f;

        // RCS endpoint for reporting. The `e=1` URL parameter defines JSPB encoding.
        private const string ProdRcsUrl = "https://pagead2.googlesyndication.com/pagead/ping?e=1";

        internal static readonly Queue<TReport> _queue = new Queue<TReport>();
        private static readonly object _queueLock = new object();
        private float _timeOfNextBatch;

        /// <summary>
        /// Initializes the client when it is enabled.
        /// </summary>
        private void Start()
        {
            RcsPayload.InitializeStaticMetadata();
            ResetBatchTimer();
        }

        /// <summary>
        /// Runs every frame to check if either of our batching triggers has been met.
        /// </summary>
        private void Update()
        {
            int count;
            lock (_queueLock)
            {
                count = _queue.Count;
            }
            bool isCountThresholdMet = count >= CountThreshold;
            bool isTimeThresholdMet = Time.time >= _timeOfNextBatch;
            if (isCountThresholdMet || isTimeThresholdMet)
            {
                ProcessAndSendBatch();
            }
        }

        /// <summary>
        /// Sends pending items before the application quits.
        /// </summary>
        private void OnApplicationQuit()
        {
            ProcessAndSendBatch();
        }

        /// <summary>
        /// Adds an item to the queue. This method is thread-safe.
        /// </summary>
        protected void Enqueue(TReport item)
        {
            if (item == null) return;
            lock (_queueLock)
            {
                _queue.Enqueue(item);
            }
        }

        /// <summary>
        /// Returns the Unix epoch in milliseconds.
        /// </summary>
        protected string GetEpochMillis()
        {
            return ((long)DateTime.UtcNow
                    .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                    .TotalMilliseconds).ToString();
        }

        /// <summary>
        /// Sends the batch of items to RCS.
        /// </summary>
        protected void SendToRcs(string jspbPayload)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("RCS JSPB payload: " + jspbPayload);
            }
            StartCoroutine(PostRequest(ProdRcsUrl, jspbPayload));
        }

        /// <summary>
        /// Drains the queue and passes the resulting batch to be sent.
        /// </summary>
        internal void ProcessAndSendBatch()
        {
            ResetBatchTimer();
            List<TReport> batch = new List<TReport>();
            lock (_queueLock)
            {
                if (_queue.Count == 0) return;
                while(_queue.Count > 0)
                {
                    batch.Add(_queue.Dequeue());
                }
            }

            if (batch.Count > 0)
            {
                SendBatch(batch);
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
        /// Coroutine to send a JSPB payload via HTTP POST.
        /// </summary>
        private IEnumerator PostRequest(string url, string jspbPayload)
        {
            using (UnityWebRequest uwr = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jspbPayload);
                uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
                uwr.certificateHandler = new BypassCertificateHandler();
                uwr.downloadHandler = new DownloadHandlerBuffer();
                uwr.SetRequestHeader("Content-Type", "application/json+protobuf");

                yield return uwr.SendWebRequest();

                if (Debug.isDebugBuild)
                {

#if UNITY_2020_2_OR_NEWER
                    if (uwr.result != UnityWebRequest.Result.Success)
#else
                    if (uwr.isHttpError || uwr.isNetworkError)
#endif
                    {
                        Debug.Log(string.Format(
                            "Error sending batch: {0} | Response code: {1}.",
                            uwr.error, uwr.responseCode));
                    }
                    else
                    {
                        // This only guarantees transport, not that the request is fully processed
                        // as RCS will just drop unknown fields if it can't otherwise parse them.
                        Debug.Log("Batch sent successfully.");
                    }
                }
            }
        }

        /// <summary>
        /// Concrete classes must implement this to process and send a batch of items.
        /// </summary>
        protected abstract void SendBatch(List<TReport> batch);
    }
}
