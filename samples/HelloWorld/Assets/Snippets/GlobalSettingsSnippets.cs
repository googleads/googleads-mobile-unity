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
    }
}
