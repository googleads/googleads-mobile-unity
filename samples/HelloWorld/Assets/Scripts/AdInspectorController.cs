using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Samples
{
    /// <summary>
    /// Demonstrates how to use the Google Mobile Ads MobileAds Instance.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/AdInspectorController")]
    public class AdInspectorController : MonoBehaviour
    {
        /// <summary>
        /// Opens the AdInspector.
        /// </summary>
        public void OpenAdInspector()
        {
            Debug.Log("Opening ad Inspector.");

            MobileAds.OpenAdInspector((AdInspectorError error) =>
            {
                // If the operation completed successfully, no error is returned.
                if (error != null)
                {
                    Debug.Log("Ad Inspector failed to open with error: " + error);
                    return;
                }

                Debug.Log("Ad Inspector opened successfully.");
            });
        }
    }
}
