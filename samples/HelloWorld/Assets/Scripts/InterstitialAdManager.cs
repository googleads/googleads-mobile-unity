using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleMobileAds.Sample
{
    [AddComponentMenu("GoogleMobileAds/Samples/InterstitialAdManager")]
    public class InterstitialAdManager : MonoBehaviour
    {
        [Tooltip("Displays the ad loading status.")]
        public Text statusText;

        private InterstitialAd _ad;

        /// <summary>
        /// Loads the ad.
        /// </summary>
        public void LoadAd()
        {
            // These ad units are configured to always serve test ads.
    #if UNITY_EDITOR
            string adUnitId = "unused";
    #elif UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/1033173712";
    #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/4411468910";
    #else
            string adUnitId = "unexpected_platform";
    #endif

            if (_ad != null)
            {
                _ad.Destroy();
                _ad = null;
            }

            Log("Loading interstitial ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest.Builder()
                .AddKeyword("unity-admob-sample")
                .Build();

            // send the request to load the ad.
            InterstitialAd.LoadAd(adUnitId, adRequest, OnLoad);
        }

        /// <summary>
        /// Shows the ad.
        /// </summary>
        public void ShowAd()
        {
            if (_ad != null)
            {
                Log("Showing interstitial ad.");
                _ad.Show();
            }
            else
            {
                LogError("Interstitial ad is not ready yet.");
            }
        }

        /// <summary>
        /// Destroys the ad.
        /// </summary>
        public void DestroyAd()
        {
            if (_ad != null)
            {
                Log("Destroying interstitial ad.");
                _ad.Destroy();
                _ad = null;
            }
        }

        private void OnLoad(InterstitialAd ad, LoadAdError error)
        {
            if (error != null)
            {
                LogError("Interstitial ad failed to load an ad with error : " + error);
                return;
            }

            Log("Interstitial ad loaded with response : " + ad.GetResponseInfo()));

            _ad = ad;
            RegisterEventHandlers(ad);
        }

        private void RegisterEventHandlers(InterstitialAd ad)
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

        private void OnAdPaid(AdValue adValue)
        {
            Log(String.Format("Interstitial ad paid {0} {1}.",
                              adValue.Value,
                              adValue.CurrencyCode));
        }

        private void OnAdImpressionRecorded()
        {
            Log("Interstitial ad recorded an impression.");
        }

        private void OnAdClickRecorded()
        {
            Log("Interstitial ad recorded a click.");
        }

        private void OnAdFullScreenContentFailed(AdError error)
        {
            LogError("Interstitial ad failed to open full screen content with error : " + error);
        }

        private void OnAdFullScreenContentOpened()
        {
            Log("Interstitial ad full screen content opened.");
        }

        private void OnAdFullScreenContentClosed()
        {
            Log("Interstitial ad full screen content closed.");
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
