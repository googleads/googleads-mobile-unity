// Copyright 2026 Google LLC

using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.UI;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Sample
{
    /// <summary>
    /// Demonstrates how to use Google Mobile Ads app open preload APIs.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/Preload/AppOpenAdPreloadController")]
    public class AppOpenAdPreloadController : MonoBehaviour
    {

#if UNITY_ANDROID
        string AD_UNIT_ID = "ca-app-pub-3940256099942544/9257395921";
#elif UNITY_IOS
        string AD_UNIT_ID = "ca-app-pub-3940256099942544/5575463023";
#else
        string AD_UNIT_ID = "editor-ad-unit-id";
#endif

        /// <summary> Shows the ad. </summary>
        [Tooltip("Shows the App open ad.")]
        public Button ShowButton;

        /// <summary> Displays the ad preload status. </summary>
        [Tooltip("Displays the app open ad preload status.")]
        public Text StatusText;

        void Start()
        {
            if (ShowButton != null)
            {
                ShowButton.onClick.AddListener(PollAndShowAd);
            }
            StartPreload();
            UpdateUI();
        }

        void OnDestroy()
        {
            // [Optional] Stop the preloading process for this configuration.
            AppOpenAdPreloader.Destroy(AD_UNIT_ID);
        }

        void StartPreload()
        {
            var preloadConfiguration = new PreloadConfiguration
            {
                AdUnitId = AD_UNIT_ID,
                Request = new AdRequest(),
            };

            // Start the preloading initialization process after MobileAds.Initialize().
            AppOpenAdPreloader.Preload(
                // The Preload ID can be any unique string to identify this configuration.
                AD_UNIT_ID,
                preloadConfiguration,
                onAdPreloaded,
                onAdFailedToPreload,
                onAdsExhausted);
        }

        void PollAndShowAd()
        {
            // DequeueAd returns the next available ad and loads another ad in the background.
            var ad = AppOpenAdPreloader.DequeueAd(AD_UNIT_ID);

            if(ad != null)
            {
                // [Optional] Interact with the ad object as needed.
                ad.OnAdPaid += (AdValue value) =>
                {
                    Debug.Log($"Ad paid: {value.CurrencyCode} {value.Value}");
                    // [Optional] Send the impression-level ad revenue information to your preferred
                    // analytics server directly within this callback.
                };

                // Do not hold onto preloaded ads, always show a preloaded ad immediately.
                ad.Show();
            }
        }

        void onAdPreloaded(string preloadId, ResponseInfo responseInfo)
        {
            Debug.Log($"Preload ad configuration {preloadId} was preloaded.");
            UpdateUI();
        }

        void onAdFailedToPreload(string preloadId, AdError adError)
        {
            string errorMessage = $"Preload ad configuration {preloadId} failed to " +
                                  $"preload with error : {adError.GetMessage()}.";
            Debug.Log(errorMessage);
        }

        void onAdsExhausted(string preloadId)
        {
            Debug.Log($"Preload ad configuration {preloadId} was exhausted");
            UpdateUI();
        }

        void UpdateUI()
        {
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                var isAdAvailable = AppOpenAdPreloader.IsAdAvailable(AD_UNIT_ID);
                if (ShowButton != null)
                {
                    ShowButton.interactable = isAdAvailable;
                }
                if (StatusText != null)
                {
                    StatusText.text = isAdAvailable ? "Available" : "Exhausted";
                }
            });
        }
    }
}
