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

namespace GoogleMobileAds.Snippets
{
    /// <summary>
    /// Code snippets for the rewarded interstitial ad developer guide.
    /// </summary>
    internal class RewardedInterstitialAdSnippets
    {
#if UNITY_ANDROID
        private const string AD_UNIT_ID = "ca-app-pub-3940256099942544/5354046379";
#elif UNITY_IPHONE
        private const string AD_UNIT_ID = "ca-app-pub-3940256099942544/6978759866";
#else
        private const string AD_UNIT_ID = "unused";
#endif

        void LoadAd()
        {
            // [START load_ad]
            // Create the request used to load the ad.
            var adRequest = new AdRequest();

            // Send the request to load the ad.
            RewardedInterstitialAd.Load(AD_UNIT_ID, adRequest,
                (RewardedInterstitialAd ad, LoadAdError error) =>
                {
                    if (error != null)
                    {
                        // The ad failed to load.
                        Debug.LogError($"Rewarded interstitial ad failed to load: {error}");
                        return;
                    }
                    // The ad loaded successfully.
                    Debug.Log("Rewarded interstitial ad loaded.");
                });
            // [END load_ad]
        }

        void ServerSideVerification(RewardedInterstitialAd rewardedInterstitialAd)
        {
            // [START ssv]
            // Create and pass the SSV options to the rewarded interstitial ad.
            var options = new ServerSideVerificationOptions
            {
                CustomData = "SAMPLE_CUSTOM_DATA_STRING"
            };

            rewardedInterstitialAd.SetServerSideVerificationOptions(options);
            // [END ssv]
        }

        void ShowAd(RewardedInterstitialAd rewardedInterstitialAd)
        {
            // [START show_ad]
            if (rewardedInterstitialAd != null && rewardedInterstitialAd.CanShowAd())
            {
                rewardedInterstitialAd.Show((Reward reward) =>
                {
                    // The ad was shown and the user earned a reward.
                    Debug.Log($"Rewarded interstitial ad rewarded user: {reward.Type}, {reward.Amount}");
                });
            }
            // [END show_ad]
        }

        void ListenToAdEvents(RewardedInterstitialAd rewardedInterstitialAd)
        {
            // [START ad_events]
            rewardedInterstitialAd.OnAdPaid += (AdValue adValue) =>
            {
                // Raised when the ad is estimated to have earned money.
                Debug.Log($"Rewarded interstitial ad paid {adValue.Value} {adValue.CurrencyCode}.");
            };
            rewardedInterstitialAd.OnAdImpressionRecorded += () =>
            {
                // Raised when an impression is recorded for an ad.
                Debug.Log("Rewarded interstitial ad recorded an impression.");
            };
            rewardedInterstitialAd.OnAdClicked += () =>
            {
                // Raised when a click is recorded for an ad.
                Debug.Log("Rewarded interstitial ad was clicked.");
            };
            rewardedInterstitialAd.OnAdFullScreenContentOpened += () =>
            {
                // Raised when the ad opened full screen content.
                Debug.Log("Rewarded interstitial ad full screen content opened.");
            };
            rewardedInterstitialAd.OnAdFullScreenContentClosed += () =>
            {
                // Raised when the ad closed full screen content.
                Debug.Log("Rewarded interstitial ad full screen content closed.");
            };
            rewardedInterstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                // Raised when the ad failed to open full screen content.
                Debug.LogError($"Rewarded interstitial ad failed to open full screen content: {error}");
            };
            // [END ad_events]
        }

        void DestroyAd(RewardedInterstitialAd rewardedInterstitialAd)
        {
            // [START destroy_ad]
            if (rewardedInterstitialAd != null)
            {
                rewardedInterstitialAd.Destroy();
            }
            // [END destroy_ad]
        }

        void ReloadAd(RewardedInterstitialAd rewardedInterstitialAd)
        {
            // [START reload_ad]
            rewardedInterstitialAd.OnAdFullScreenContentClosed += () =>
            {
                // Reload the ad so that you can show another as soon as possible.
                var adRequest = new AdRequest();
                RewardedInterstitialAd.Load(AD_UNIT_ID, adRequest,
                    (RewardedInterstitialAd ad, LoadAdError error) =>
                    {
                        // Handle ad loading here.
                        Debug.Log("Rewarded interstitial ad reloaded.");
                    });
            };
            // [END reload_ad]
        }
    }
}
