// Copyright 2026 Google LLC

using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Snippets
{
    /// <summary>
    /// Code snippets used for the developer guides.
    /// </summary>
    internal class AdPreloaderSnippets
    {

#if UNITY_ANDROID
        string AD_UNIT_ID = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IOS
        string AD_UNIT_ID = "ca-app-pub-3940256099942544/4411468910";
#else
        string AD_UNIT_ID = "editor-ad-unit-id";
#endif

        void StartPreload()
        {
            // [START start_preload]
            var preloadConfiguration = new PreloadConfiguration
            {
                AdUnitId = AD_UNIT_ID,
                Request = new AdRequest(),
            };

            // Start the preloading initialization process after MobileAds.Initialize().
            InterstitialAdPreloader.Preload(
                // The Preload ID can be any unique string to identify this configuration.
                AD_UNIT_ID,
                preloadConfiguration);
            // [END start_preload]
        }

        void SetBufferSize()
        {
            // [START set_buffer_size]
            new PreloadConfiguration
            {
                AdUnitId = AD_UNIT_ID,
                Request = new AdRequest(),
                BufferSize = 5
            };
            // [END set_buffer_size]
        }

        void StopPreload()
        {
            // [START stop_preload]
            InterstitialAdPreloader.Destroy(AD_UNIT_ID);
            InterstitialAdPreloader.DestroyAll();
            // [END stop_preload]
        }


        void PollAndShowAd()
        {
            // [START poll_and_show]
            // DequeueAd returns the next available ad and loads another ad in the background.
            var ad = InterstitialAdPreloader.DequeueAd(AD_UNIT_ID);

            if (ad != null)
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
            // [END poll_and_show]
        }

        // [START set_callback]

        void StartPreloadWithCallbacks()
        {
            var preloadConfiguration = new PreloadConfiguration
            {
                AdUnitId = AD_UNIT_ID,
                Request = new AdRequest(),
            };

            // Start the preloading initialization process after MobileAds.Initialize().
            InterstitialAdPreloader.Preload(
                // The Preload ID can be any unique string to identify this configuration.
                AD_UNIT_ID,
                preloadConfiguration,
                onAdPreloaded,
                onAdFailedToPreload,
                onAdsExhausted);
        }

        void onAdPreloaded(string preloadId, ResponseInfo responseInfo)
        {
            Debug.Log($"Preload ad configuration {preloadId} was preloaded.");
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
            // [Important] Don't call Preload() or DequeueAd() from onAdsExhausted.
        }
        // [END set_callback]

        void IsAdAvailable()
        {
            // [START is_ad_available]
            var isAdAvailable = InterstitialAdPreloader.IsAdAvailable(AD_UNIT_ID);
            // [END is_ad_available]
        }
    }
}
