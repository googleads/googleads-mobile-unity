using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace GoogleMobileAds.Samples
{
    public class GoogleAdMobController : MonoBehaviour
    {
        [Tooltip("Enables the FPS meter visibility.")]
        public bool showFpsMeter = true;

        [Tooltip("Displays the current frames per second.")]
        public Text fpsMeter;

        [Tooltip("Displays the ad loading status.")]
        public Text statusText;

        private float _deltaTime;

        private void Start()
        {
            Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.None);
            MobileAds.SetiOSAppPauseOnBackground(true);

            List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };

            // Add some test device IDs (replace with your own device IDs).
    #if UNITY_IPHONE
            deviceIds.Add("96e23e80653bb28980d3f40beb58915c");
    #elif UNITY_ANDROID
            deviceIds.Add("75EF8D155528C04DACBBA6F36F433035");
    #endif
            // Configure TagForChildDirectedTreatment and test device IDs.
            RequestConfiguration requestConfiguration =
                new RequestConfiguration.Builder()
                .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
                .SetTestDeviceIds(deviceIds).build();
            MobileAds.SetRequestConfiguration(requestConfiguration);

            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(HandleInitCompleteAction);
        }

        private void HandleInitCompleteAction(InitializationStatus initstatus)
        {
            Debug.Log("Initialization complete.");

            // Callbacks from GoogleMobileAds are not guaranteed to be called on
            // the main thread.
            // In this example we use MobileAdsEventExecutor to schedule these calls on
            // the next Update() loop.
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                statusText.text = "Initialization complete.";
            });
        }

        private void Update()
        {
            if (showFpsMeter)
            {
                fpsMeter.gameObject.SetActive(true);
                _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
                float fps = 1.0f / _deltaTime;
                fpsMeter.text = string.Format("{0:0.} fps", fps);
            }
            else
            {
                fpsMeter.gameObject.SetActive(false);
            }
        }

        #region AD INSPECTOR

        public void OpenAdInspector()
        {
            Debug.Log("Open ad Inspector.");

            MobileAds.OpenAdInspector((error) =>
            {
                if (error != null)
                {
                    Debug.Log("Ad Inspector failed to open with error: " + error);
                }
                else
                {
                    Debug.Log("Ad Inspector opened successfully.");
                }
            });
        }

        #endregion
    }
}
