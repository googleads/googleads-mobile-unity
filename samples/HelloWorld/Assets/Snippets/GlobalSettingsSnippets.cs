using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Snippets
{
    /// <summary>
    /// Code snippets used for the developer guides covering global settings.
    /// </summary>
    internal class GlobalSettingsSnippets
    {
        internal GameObject _myGameObject;

        private void HandleAdEventsOnMainThread()
        {
            // [START execute_in_update]
            // Google Mobile Ads events are raised off the Unity main thread.

            // This log is executed off the Unity main thread.
            // Write all time-sensitive code before ExecuteInUpdate().
            Debug.Log("Executing off the Unity main thread.");

            // Use ExecuteInUpdate to run code on the main thread, allowing you to
            // interact with Unity UI and GameObjects.
            // Changed to fully-qualified name to resolve CS0103
            GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                // This callback may be delayed on Android until the user returns to the app.
                Debug.Log("Executing on the Unity main thread.");

                // Place all code that interacts with Unity UI and GameObjects inside this callback.
                if (_myGameObject != null)
                {
                    _myGameObject.SetActive(true);
                }
            });
            // [END execute_in_update]
        }

        private void RaiseAdEventsOnUnityMainThread()
        {
            // [START raise_ad_events_on_unity_main_thread]
            // When true, all events raised by GoogleMobileAds will be raised
            // on the Unity main thread. The default value is false.
            MobileAds.RaiseAdEventsOnUnityMainThread = true;
            // [END raise_ad_events_on_unity_main_thread]
        }

        private void SetApplicationVolume()
        {
            // [START set_application_volume]
            // Set app volume to be half of current device volume.
            MobileAds.SetApplicationVolume(0.5f);
            // [END set_application_volume]
        }

        private void SetApplicationMuted()
        {
            // [START set_application_muted]
            // Set app to be muted.
            MobileAds.SetApplicationMuted(true);
            // [END set_application_muted]
        }

        private void SetConsentForCookies()
        {
            // [START set_consent_for_cookies]
            // Enable limited ads
            ApplicationPreferences.SetInt("gad_has_consent_for_cookies", 0);
            // [END set_consent_for_cookies]
        }

        private void DisableSDKCrashReporting()
        {
            // [START disable_sdk_crash_reporting]
            MobileAds.DisableSDKCrashReporting();
            // [END disable_sdk_crash_reporting]
        }

        private void GetVersion()
        {
            // [START get_version]
            // Get the Unity SDK version.
            Debug.Log("Unity SDK Version: " + MobileAds.GetVersion());
            // [END get_version]
        }

        private void GetPlatformVersion()
        {
            // [START get_platform_version]
            // Get the underlying platform SDK version.
            Debug.Log("Platform SDK Version: " + MobileAds.GetPlatformVersion());
            // [END get_platform_version]
        }
    }
}
