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
using GoogleMobileAds.Api.AdManager;

namespace GoogleMobileAds.Snippets
{
    internal class PictureInPictureAdSnippets
    {
        // These ad units are configured to always serve test ads.
        #if UNITY_ANDROID
        private string _adUnitId = "ca-app-pub-3940256099942544/9657123429";
        #elif UNITY_IPHONE
        private string _adUnitId = "ca-app-pub-3940256099942544/8810945611";
        #else
        private string _adUnitId = "unused";
        #endif

        private PictureInPictureAd _pictureInPictureAd;

        // [START load_ad]
        public void LoadPictureInPictureAd()
        {
            // Clean up any existing ad before loading a new one.
            if (_pictureInPictureAd != null)
            {
                _pictureInPictureAd.Destroy();
                _pictureInPictureAd = null;
            }

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // Send the request to load the ad.
            PictureInPictureAd.Load(_adUnitId, adRequest,
                (PictureInPictureAd ad, LoadAdError error) =>
                {
                    if (error != null || ad == null)
                    {
                        Debug.LogWarning(
                            "Picture-in-picture ad failed to load: " + error);
                        return;
                    }

                    // Capture the PictureInPictureAd reference for later use.
                    _pictureInPictureAd = ad;
                });
        }
        // [END load_ad]

        // [START show_ad]
        public void ShowPictureInPictureAd()
        {
            // Use the ad reference saved from the Load callback.
            if (_pictureInPictureAd != null)
            {
                _pictureInPictureAd.Show();
            }
            else
            {
                Debug.Log("No ad to show.");
            }
        }
        // [END show_ad]

        // [START position]
        public void ShowPictureInPictureAdTopLeft()
        {
            // Use the ad reference saved from the Load callback.
            if (_pictureInPictureAd != null)
            {
                _pictureInPictureAd.Show(PictureInPictureAdPosition.TopLeft);
            }
            else
            {
                Debug.Log("No ad to show.");
            }
        }
        // [END position]

        // [START ad_events]
        private void RegisterEventHandlers(PictureInPictureAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log($"Picture-in-picture ad paid: {adValue.Value} {adValue.CurrencyCode}");
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Picture-in-picture ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("Picture-in-picture ad recorded a click.");
            };
            // Raised when the picture-in-picture ad is displayed on the screen.
            ad.OnAdShown += () =>
            {
                Debug.Log("Picture-in-picture ad shown.");
            };
            // Raised when the picture-in-picture ad is hidden from the screen.
            ad.OnAdHidden += () =>
            {
                Debug.Log("Picture-in-picture ad hidden.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Picture-in-picture ad opened full screen content.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Picture-in-picture ad closed full screen content.");
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogWarning(
                    "Picture-in-picture ad failed to open full screen " +
                    "content: " + error);
            };
        }
        // [END ad_events]

        // [START hide_ad]
        public void HidePictureInPictureAd()
        {
            // Use the ad reference saved from the Load callback.
            if (_pictureInPictureAd != null)
            {
                _pictureInPictureAd.Hide();
            }
            else
            {
                Debug.Log("No ad to hide.");
            }
        }
        // [END hide_ad]

        // [START destroy_ad]
        public void DestroyPictureInPictureAd()
        {
            // Use the ad reference saved from the Load callback.
            if (_pictureInPictureAd != null)
            {
                _pictureInPictureAd.Destroy();
                _pictureInPictureAd = null;
            }
        }
        // [END destroy_ad]
    }

    internal class AdManagerPictureInPictureAdSnippets
    {
        private string _adUnitId = "unused";
        private PictureInPictureAd _pictureInPictureAd;

        // [START load_ad_gam]
        public void LoadPictureInPictureAd()
        {
            // Clean up any existing ad before loading a new one.
            if (_pictureInPictureAd != null)
            {
                _pictureInPictureAd.Destroy();
                _pictureInPictureAd = null;
            }

            // Create our request used to load the ad.
            var adRequest = new AdManagerAdRequest();

            // Send the request to load the ad.
            PictureInPictureAd.Load(_adUnitId, adRequest,
                (PictureInPictureAd ad, LoadAdError error) =>
                {
                    if (error != null || ad == null)
                    {
                        Debug.LogWarning(
                            "Picture-in-picture ad failed to load: " + error);
                        return;
                    }

                    // Capture the PictureInPictureAd reference for later use.
                    _pictureInPictureAd = ad;
                });
        }
        // [END load_ad_gam]
    }
}
