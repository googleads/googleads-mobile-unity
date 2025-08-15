using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Snippets
{
    internal class RewardedAdSnippets
    {
#if UNITY_ANDROID
        private const string AD_UNIT_ID = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
        private const string AD_UNIT_ID = "ca-app-pub-3940256099942544/1712485313";
#else
        private const string AD_UNIT_ID = "unused";
#endif

        void LoadAd()
        {
            // [START load_ad]
            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // Send the request to load the ad.
            RewardedAd.Load(AD_UNIT_ID, adRequest, (RewardedAd ad, LoadAdError error) =>
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

        void ServerSideVerification(RewardedAd rewardedAd)
        {
            // [START ssv]
            // Create and pass the SSV options to the rewarded ad.
            var options = new ServerSideVerificationOptions
            {
                CustomData = "SAMPLE_CUSTOM_DATA_STRING"
            };

            rewardedAd.SetServerSideVerificationOptions(options);
            // [END ssv]
        }

        void ShowAd(RewardedAd rewardedAd)
        {
            // [START show_ad]
            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                rewardedAd.Show((Reward reward) =>
                {
                    // The ad was showen and the user earned a reward.
                });
            }
            // [END show_ad]]
        }

        void ListenToAdEvents(RewardedAd rewardedAd)
        {
            // [START ad_events]
            rewardedAd.OnAdPaid += (AdValue adValue) =>
            {
                // Raised when the ad is estimated to have earned money.
            };
            rewardedAd.OnAdImpressionRecorded += () =>
            {
                // Raised when an impression is recorded for an ad.
            };
            rewardedAd.OnAdClicked += () =>
            {
                // Raised when a click is recorded for an ad.
            };
            rewardedAd.OnAdFullScreenContentOpened += () =>
            {
                // Raised when the ad opened full screen content.
            };
            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                // Raised when the ad closed full screen content.
            };
            rewardedAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                // Raised when the ad failed to open full screen content.
            };
            // [END ad_events]]
        }

        void DestroyAd(RewardedAd rewardedAd)
        {
            // [START destroy_ad]
            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
            }
            // [END destroy_ad]]
        }

        void ReloadAd(RewardedAd rewardedAd)
        {
            // [START reload_ad]
            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                // Reload the ad so that we can show another as soon as possible.
                var adRequest = new AdRequest();
                RewardedAd.Load(AD_UNIT_ID, adRequest, (RewardedAd ad, LoadAdError error) =>
                {
                    // Handle ad loading here.
                });
            };
            // [END reload_ad]]
        }
    }
}
