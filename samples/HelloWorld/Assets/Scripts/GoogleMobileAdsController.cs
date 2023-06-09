using System;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.SceneManagement;

namespace GoogleMobileAds.Samples
{
    /// <summary>
    /// Demonstrates how to use the Google Mobile Ads MobileAds Instance.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/GoogleMobileAdsController")]
    public class GoogleMobileAdsController : MonoBehaviour
    {
        private static bool _isInitialized;

        /// <summary>
        /// Initializes the MobileAds SDK
        /// </summary>
        private void Start()
        {
            // Demonstrates how to configure Google Mobile Ads.
            // Google Mobile Ads needs to be run only once and before loading any ads.
            if (_isInitialized)
            {
                return;
            }

            // On Android, Unity is paused when displaying interstitial or rewarded video.
            // This setting makes iOS behave consistently with Android.
            MobileAds.SetiOSAppPauseOnBackground(true);

            // When true all events raised by GoogleMobileAds will be raised
            // on the Unity main thread. The default value is false.
            // https://developers.google.com/admob/unity/quick-start#raise_ad_events_on_the_unity_main_thread
            MobileAds.RaiseAdEventsOnUnityMainThread = true;

            // Set your test devices.
            // https://developers.google.com/admob/unity/test-ads
            List<string> deviceIds = new List<string>()
            {
                AdRequest.TestDeviceSimulator,
                // Add your test device IDs (replace with your own device IDs).
                #if UNITY_IPHONE
                "96e23e80653bb28980d3f40beb58915c"
                #elif UNITY_ANDROID
                "75EF8D155528C04DACBBA6F36F433035"
                #endif
            };

            // Configure your RequestConfiguration with Child Directed Treatment
            // and the Test Device Ids.
            RequestConfiguration requestConfiguration = new RequestConfiguration
            {
                TestDeviceIds = deviceIds
            };
            MobileAds.SetRequestConfiguration(requestConfiguration);

            // Initialize the Google Mobile Ads SDK.
            Debug.Log("Google Mobile Ads Initializing.");
            MobileAds.Initialize((InitializationStatus initstatus) =>
            {
                if (initstatus == null)
                {
                    Debug.LogError("Google Mobile Ads initialization failed.");
                    return;
                }

                // If you use mediation, you can check the status of each adapter.
                var adapterStatusMap = initstatus.getAdapterStatusMap();
                if (adapterStatusMap != null)
                {
                    foreach (var item in adapterStatusMap)
                    {
                        Debug.Log(string.Format("Adapter {0} is {1}",
                            item.Key,
                            item.Value.InitializationState));
                    }
                }

                Debug.Log("Google Mobile Ads initialization complete.");
                _isInitialized = true;
            });
        }

        /// <summary>
        /// Opens the AdInspector.
        /// </summary>
        public void OpenAdInspector()
        {
            Debug.Log("Opening ad Inspector.");
            MobileAds.OpenAdInspector((AdInspectorError error) =>
            {
                // If the operation failed, an error is returned.
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
