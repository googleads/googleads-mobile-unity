using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Snippets
{
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
            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // Send the request to load the ad.
            RewardedInterstitialAd.Load(AD_UNIT_ID, adRequest,
                (RewardedInterstitialAd ad, LoadAdError error) =>
                {
                    if (error != null)
                    {
                        // The ad failed to load.
                        return;
                    }
                    // The ad loaded successfully.
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
            };
            rewardedInterstitialAd.OnAdImpressionRecorded += () =>
            {
                // Raised when an impression is recorded for an ad.
            };
            rewardedInterstitialAd.OnAdClicked += () =>
            {
                // Raised when a click is recorded for an ad.
            };
            rewardedInterstitialAd.OnAdFullScreenContentOpened += () =>
            {
                // Raised when the ad opened full screen content.
            };
            rewardedInterstitialAd.OnAdFullScreenContentClosed += () =>
            {
                // Raised when the ad closed full screen content.
            };
            rewardedInterstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                // Raised when the ad failed to open full screen content.
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
                // Reload the ad so that we can show another as soon as possible.
                var adRequest = new AdRequest();
                RewardedInterstitialAd.Load(AD_UNIT_ID, adRequest,
                    (RewardedInterstitialAd ad, LoadAdError error) =>
                    {
                        // Handle ad loading here.
                    });
            };
            // [END reload_ad]
        }
    }
}
