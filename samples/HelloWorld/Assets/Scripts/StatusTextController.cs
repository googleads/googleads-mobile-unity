using UnityEngine;
using UnityEngine.UI;

namespace GoogleMobileAds.Samples
{
    /// <summary>
    /// Utility for printing status text.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/StatusTextController")]
    [RequireComponent(typeof(Text))]
    public class StatusTextController : MonoBehaviour
    {
        private Text _statusText;

        private void Awake()
        {
            _statusText = GetComponent<Text>();
            Application.logMessageReceivedThreaded += OnlogMessageReceived;
        }

        private void OnDestroy()
        {
            Application.logMessageReceivedThreaded -= OnlogMessageReceived;
        }

        private void OnlogMessageReceived(string condition, string stackTrace, LogType type)
        {
            // Google Mobile Ads is not thread safe. There is a chance that events being raised
            // are not on the main Unity thread. Please dispatch to the Unity main thread.
            Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                // Guarding against race conditions from scene loading.
                if (this != null && _statusText != null)
                {
                    switch (type)
                    {
                        case LogType.Warning:
                            _statusText.color = Color.red * Color.yellow;
                            break;
                        case LogType.Error:
                        case LogType.Exception:
                            _statusText.color = Color.red;
                            break;
                        default:
                            _statusText.color = Color.white;
                            break;
                    }
                    _statusText.text = condition;
                }
            });
        }
    }
}