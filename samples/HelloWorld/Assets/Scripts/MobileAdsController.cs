using System;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Samples
{
    /// <summary>
    /// Demonstrates how to use the Google Mobile Ads MobileAds Instance.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/MobileAdsController")]
    public class MobileAdsController : MonoBehaviour
    {
        private static bool _isInitialized;

        private void Start()
        {
            // Here we are demonstrating how to configure Google Mobile Ads.
            // This needs to be run only once and before loading any ads.
            if (_isInitialized)
            {
                return;
            }

            // On Android, Unity is paused when displaying interstitial or rewarded video.
            // This behavior should be made consistent with iOS.
            MobileAds.SetiOSAppPauseOnBackground(true);

            // If you click on too many ads without being in test mode,
            // you risk your account being flagged for invalid activity.
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
            RequestConfiguration requestConfiguration =
                new RequestConfiguration.Builder()
                .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
                .SetTestDeviceIds(deviceIds).build();
            MobileAds.SetRequestConfiguration(requestConfiguration);

            // Initialize the Google Mobile Ads SDK.
            Debug.Log("Google Mobile Ads Initializing.");
            MobileAds.Initialize(OnInitialize);
        }

        private static void OnInitialize(InitializationStatus initstatus)
        {
            if (initstatus != null)
            {
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
            }
        }
    }
}
