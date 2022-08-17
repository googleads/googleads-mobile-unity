using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleMobileAds.Sample
{
    [AddComponentMenu("GoogleMobileAds/Samples/RewardedAdManager")]
    public class RewardedAdManager : MonoBehaviour
    {
        [Tooltip("Displays the ad loading status.")]
        public Text statusText;

        private RewardedAd _ad;

        /// <summary>
        /// Loads the ad.
        /// </summary>
        public void LoadAd()
        {
            // These ad units are configured to always serve test ads.
    #if UNITY_EDITOR
            string adUnitId = "unused";
    #elif UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/5224354917";
    #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/1712485313";
    #else
            string adUnitId = "unexpected_platform";
    #endif

            if (_ad != null)
            {
                _ad.Destroy();
                _ad = null;
            }

            Log("Loading rewarded ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest.Builder()
                .AddKeyword("unity-admob-sample")
                .Build();

            // send the request to load the ad.
            RewardedAd.LoadAd(adUnitId, adRequest, OnLoad);
        }

        /// <summary>
        /// Shows the ad.
        /// </summary>
        public void ShowAd()
        {
            if (_ad != null)
            {
                Log("Showing rewarded ad.");
                _ad.Show(OnUserEarnedReward);
            }
            else
            {
                LogError("Rewarded ad is not ready yet.");
            }
        }

        /// <summary>
        /// Destroys the ad.
        /// </summary>
        public void DestroyAd()
        {
            if (_ad != null)
            {
                Log("Destroying rewarded ad.");
                _ad.Destroy();
                _ad = null;
            }
        }

        private void OnLoad(RewardedAd ad, LoadAdError error)
        {
            if (error != null)
            {
                LogError("Rewarded ad failed to load with error : " + error);
                return;
            }

            Log("Rewarded ad loaded with response : " + ad.GetResponseInfo()));

            _ad = ad;
            RegisterEventHandlers(ad);
        }

        protected void RegisterEventHandlers(RewardedAd ad)
        {
            // Raised when a click is recorded for an ad.
            ad.OnAdClickRecorded += OnAdClickRecorded;
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += OnAdImpressionRecorded;
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += OnAdPaid;
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;
        }

        private void OnUserEarnedReward(Reward reward)
        {
            Log(String.Format("Rewarded ad rewarded the user {0} {1}.",
                              reward.Amount,
                              reward.Type));
        }

        private void OnAdPaid(AdValue adValue)
        {
            Log(String.Format("Rewarded ad paid {0} {1}.",
                              adValue.Value,
                              adValue.CurrencyCode));
        }

        private void OnAdImpressionRecorded()
        {
            Log("Rewarded ad recorded an impression.");
        }

        private void OnAdClickRecorded()
        {
            Log("Rewarded ad recorded a click.");
        }

        private void OnAdFullScreenContentFailed(AdError error)
        {
            LogError("Rewarded ad failed to open full screen content with error : " + error);
        }

        private void OnAdFullScreenContentOpened()
        {
            Log("Rewarded ad full screen content opened.");
        }

        private void OnAdFullScreenContentClosed()
        {
            Log("Rewarded ad full screen content closed.");
        }

        private void Log(string message)
        {
            Debug.Log(message);
            // Api Events are not thread safe and may cause exceptions if they touch the UI thread.
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                statusText.text = message;
            });
        }

        private void LogError(string message)
        {
            Debug.LogError(message);
            // Api Events are not thread safe and may cause exceptions if they touch the UI thread.
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                statusText.text = message;
            });
        }
    }
}
