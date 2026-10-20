// Copyright 2026 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Sample
{
    /// <summary>
    /// Demonstrates how to load, show, hide, and manage Squeezeback ads in Unity.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/SqueezebackAdController")]
    public class SqueezebackAdController : MonoBehaviour
    {
        /// <summary>
        /// UI element activated when an ad is ready to show.
        /// </summary>
        public GameObject AdLoadedStatus;

        // Test ad units for Squeezeback.
#if UNITY_ANDROID
        private const string _adUnitId =
            "ca-app-pub-3940256099942544/2247696110";
#elif UNITY_IPHONE
        private const string _adUnitId =
            "ca-app-pub-3940256099942544/3986624511";
#else
        private const string _adUnitId = "unused";
#endif

        private SqueezebackAd _squeezebackAd;

        /// <summary>
        /// Loads the Squeezeback ad.
        /// </summary>
        public void LoadAd()
        {
            // Clean up old ad before loading a new one.
            if (_squeezebackAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Loading Squeezeback ad.");

            var request = new AdRequest();

            SqueezebackAd.Load(_adUnitId, request,
                (SqueezebackAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError(
                        "Squeezeback ad failed to load with error: " +
                        error);
                    return;
                }

                Debug.Log("Squeezeback ad loaded with response: " +
                    ad.GetResponseInfo());
                _squeezebackAd = ad;
                RegisterEventHandlers(ad);

                // Update UI on the main thread
                MobileAdsEventExecutor.ExecuteInUpdate(() =>
                {
                    AdLoadedStatus?.SetActive(true);
                });
            });
        }

        /// <summary>
        /// Shows the Squeezeback ad on screen.
        /// </summary>
        public void ShowAd()
        {
            if (_squeezebackAd != null)
            {
                Debug.Log("Showing Squeezeback ad.");
                _squeezebackAd.Show();
            }
            else
            {
                Debug.LogError("Squeezeback ad is not ready yet.");
            }
        }

        /// <summary>
        /// Hides the Squeezeback ad from screen and restores the app content.
        /// </summary>
        public void HideAd()
        {
            if (_squeezebackAd != null)
            {
                Debug.Log("Hiding Squeezeback ad.");
                _squeezebackAd.Hide();
            }
            else
            {
                Debug.LogError("Squeezeback ad is null.");
            }
        }

        /// <summary>
        /// Destroys the Squeezeback ad.
        /// </summary>
        public void DestroyAd()
        {
            if (_squeezebackAd != null)
            {
                Debug.Log("Destroying Squeezeback ad.");
                _squeezebackAd.Destroy();
                _squeezebackAd = null;
            }

            AdLoadedStatus?.SetActive(false);
        }

        /// <summary>
        /// Logs the Ad Response Info.
        /// </summary>
        public void LogResponseInfo()
        {
            if (_squeezebackAd != null)
            {
                var responseInfo = _squeezebackAd.GetResponseInfo();
                Debug.Log("Squeezeback Ad Response Info: " +
                    (responseInfo != null ? responseInfo.ToString() : "null"));
            }
            else
            {
                Debug.LogError("Squeezeback ad is null.");
            }
        }

        private void RegisterEventHandlers(SqueezebackAd ad)
        {
            // Raised when the ad is displayed on screen (after squeeze animation).
            ad.OnAdShown += () =>
            {
                Debug.Log("Squeezeback ad shown.");
            };

            // Raised when the ad is hidden from screen (after unsqueeze animation).
            ad.OnAdHidden += () =>
            {
                Debug.Log("Squeezeback ad hidden.");
            };

            // Raised when an impression is recorded for the ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Squeezeback ad recorded an impression.");
            };

            // Raised when a click is recorded for the ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("Squeezeback ad was clicked.");
            };

            // Raised when the ad opens a full-screen overlay.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Squeezeback ad full screen content opened.");
            };

            // Raised when the ad full-screen overlay is dismissed.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Squeezeback ad full screen content closed.");
            };

            // Raised when the ad fails to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError(
                    "Squeezeback ad failed to open full screen " +
                    "content with error: " + error);
            };

            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Squeezeback ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));

                MobileAdsEventExecutor.ExecuteInUpdate(() =>
                {
                    Debug.Log("Squeezeback ad paid callback invoked " +
                        "inside ExecuteInUpdate.");
                });
            };
        }
    }
}
