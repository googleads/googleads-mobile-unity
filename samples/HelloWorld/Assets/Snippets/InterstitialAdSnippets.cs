using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Snippets
{
    internal class InterstitialAdSnippets
    {
#if UNITY_ANDROID
        private const string AD_UNIT_ID = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        private const string AD_UNIT_ID = "ca-app-pub-3940256099942544/4411468910";
#else
        private const string AD_UNIT_ID = "unused";
#endif

        void LoadAd()
        {
            // [START load_ad]
            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // Send the request to load the ad.
            InterstitialAd.Load(AD_UNIT_ID, adRequest, (InterstitialAd ad, LoadAdError error) =>
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

        void ShowAd(InterstitialAd interstitialAd)
        {
            // [START show_ad]
            if (interstitialAd != null && interstitialAd.CanShowAd())
            {
                interstitialAd.Show();
            }
            // [END show_ad]]
        }

        void ListenToAdEvents(InterstitialAd interstitialAd)
        {
            // [START ad_events]
            interstitialAd.OnAdPaid += (AdValue adValue) =>
            {
                // Raised when the ad is estimated to have earned money.
            };
            interstitialAd.OnAdImpressionRecorded += () =>
            {
                // Raised when an impression is recorded for an ad.
            };
            interstitialAd.OnAdClicked += () =>
            {
                // Raised when a click is recorded for an ad.
            };
            interstitialAd.OnAdFullScreenContentOpened += () =>
            {
                // Raised when the ad opened full screen content.
            };
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                // Raised when the ad closed full screen content.
            };
            interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                // Raised when the ad failed to open full screen content.
            };
            // [END ad_events]]
        }

        void DestroyAd(InterstitialAd interstitialAd)
        {
            // [START destroy_ad]
            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
            }
            // [END destroy_ad]]
        }

        void ReloadAd(InterstitialAd interstitialAd)
        {
            // [START reload_ad]
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                // Reload the ad so that we can show another as soon as possible.
                var adRequest = new AdRequest();
                InterstitialAd.Load(AD_UNIT_ID, adRequest, (InterstitialAd ad, LoadAdError error) =>
                {
                    // Handle ad loading here.
                });
            };
            // [END reload_ad]]
        }
    }
}
