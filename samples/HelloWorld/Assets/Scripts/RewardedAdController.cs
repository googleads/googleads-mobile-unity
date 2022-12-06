using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Sample
{
    /// <summary>
    /// Demonstrates how to use the Google Mobile Ads Rewarded ad.
    /// </summary>
    /// <remarks>
    /// Before loading ads, initialize the Mobile Ads SDK  by calling MobileAds.Initialize();
    /// This needs to be done only once, ideally at app launch.
    /// <remarks>
    [AddComponentMenu("GoogleMobileAds/Samples/RewardedAdController")]
    public class RewardedAdController : MonoBehaviour
    {
        // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
        private string _adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
        private string _adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        private string _adUnitId = "unused";
#endif

        private RewardedAd _rewardedAd;

        /// <summary>
        /// Loads the ad.
        /// </summary>
        public void LoadAd()
        {
            // Clean up the old ad before loading a new one.
            if (_rewardedAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Loading rewarded ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest.Builder()
                .AddKeyword("unity-admob-sample")
                .Build();

            // send the request to load the ad.
            RewardedAd.Load(_adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
            {
                // If the operation completed successfully, no error is returned.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());

                _rewardedAd = ad;

                // Register to ad events to extend functionality.
                RegisterEventHandlers(ad);
            });
        }

        /// <summary>
        /// Shows the ad.
        /// </summary>
        public void ShowAd()
        {
            if (_rewardedAd != null && _rewardedAd.IsLoaded())
            {
                Debug.Log("Showing rewarded ad.");
                _rewardedAd.Show((Reward reward) =>
                {
                    Debug.Log("Rewarded ad rewarded : " + reward.Amount);
                });
            }
            else
            {
                Debug.LogError("Rewarded ad is not ready yet.");
            }
        }

        /// <summary>
        /// Destroys the ad.
        /// </summary>
        public void DestroyAd()
        {
            if (_rewardedAd != null)
            {
                Debug.Log("Destroying rewarded ad.");
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }
        }

        private void RegisterEventHandlers(RewardedAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Rewarded ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("Rewarded ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += ()
            {
                Debug.Log("Rewarded ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += ()
            {
                Debug.Log("Rewarded ad full screen content closed.");
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error)
            {
                Debug.LogError("Rewarded ad failed to open full screen content with error : "
                    + error);
            };
        }
    }
}
