using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleMobileAds.Sample
{
    [AddComponentMenu("GoogleMobileAds/Samples/BannerAdManager")]
    public class BannerAdManager : MonoBehaviour
    {
        [Tooltip("Displays the ad loading status.")]
        public Text statusText;

        private BannerView _bannerView;

        /// <summary>
        /// Loads the ad.
        /// </summary>
        public void LoadAd()
        {
            // These ad units are configured to always serve test ads.
    #if UNITY_EDITOR
            string adUnitId = "unused";
    #elif UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/6300978111";
    #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
    #else
            string adUnitId = "unexpected_platform";
    #endif

            // Clean up banner before reusing
            if (_bannerView != null)
            {
                _bannerView.Destroy();
                _bannerView = null;
            }

            Log("Loading banner view");

            // Create a 320x50 banner at top of the screen
            _bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

            // listen to events the banner may raise.
            RegisterEventHandlers(_bannerView);

            // create our request used to load the ad.
            var adRequest = new AdRequest.Builder()
                .AddKeyword("unity-admob-sample")
                .Build();

            // send the request to load the ad.
            _bannerView.LoadAd(adRequest);
        }

        /// <summary>
        /// Destroys the ad.
        /// </summary>
        public void DestroyAd()
        {
            if (_bannerView != null)
            {
                Log("Destroying banner view.");
                _bannerView.Destroy();
                _bannerView = null;
            }
        }

        private void RegisterEventHandlers(BannerView ad)
        {
            // Raised when  an ad is loaded into the banner view.
            ad.OnBannerAdLoaded += OnBannerAdLoaded;
            // Raised when an ad fails to load into the banner view.
            ad.OnBannerAdLoadFailed += OnBannerAdLoadFailed;
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
        }

        private void OnAdPaid(AdValue adValue)
        {
            Log(String.Format("Banner view paid {0} {1}.",
                              adValue.Value,
                              adValue.CurrencyCode));
        }

        private void OnAdImpressionRecorded()
        {
            Log("Banner view recorded an impression.");
        }

        private void OnAdClickRecorded()
        {
            Log("Banner view recorded a click.");
        }

        private void OnBannerAdLoadFailed(LoadAdError error)
        {
            LogError("Banner view failed to load an ad with error : " + error);
        }

        private void OnBannerAdLoaded()
        {
            Log("Banner view loaded an ad with response : " + _bannerView.GetResponseInfo());
        }

        private void OnAdFullScreenContentOpened()
        {
            Log("Banner view full screen content opened.");
        }

        private void OnAdFullScreenContentClosed()
        {
            Log("Banner view full screen content closed.");
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
