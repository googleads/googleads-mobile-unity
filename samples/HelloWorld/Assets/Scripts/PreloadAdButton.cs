using System;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.UI;

namespace GoogleMobileAds.Sample
{
    /// <summary>
    /// Button controller for a specific preloaded ad.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/PreloadAdButton")]
    public class PreloadAdButton : MonoBehaviour
    {
        public Button Button;
        public Text TitleText;
        public Text StatusText;

        public PreloadConfiguration PreloadConfig {get; private set; }

        public void InitializeUI(PreloadConfiguration preloadConfig)
        {
            // Display the platform and format of this preload configuration.
            gameObject.SetActive(true);
            PreloadConfig = preloadConfig;
            bool isAdmob = preloadConfig.AdUnitId.StartsWith("ca-app-pub");
            TitleText.text = isAdmob ? "Admob" : "Ad Manager";
            switch (preloadConfig.Format)
            {
                case AdFormat.APP_OPEN_AD:
                TitleText.text += " App open ad";
                    break;
                case AdFormat.REWARDED:
                TitleText.text += " Rewarded ad";
                    break;
                case AdFormat.INTERSTITIAL:
                TitleText.text += " Interstitial ad";
                    break;
            }

            UpdateStatusUI(preloadConfig);
        }

        public void UpdateStatusUI(PreloadConfiguration preloadConfig)
        {
            // Display the availability of this preload configuration.
            bool isAvailable = false;
            switch (preloadConfig.Format)
            {
                case AdFormat.APP_OPEN_AD:
                    isAvailable = AppOpenAd.IsAdAvailable(preloadConfig.AdUnitId);
                    break;
                case AdFormat.REWARDED:
                    isAvailable = RewardedAd.IsAdAvailable(preloadConfig.AdUnitId);
                    break;
                case AdFormat.INTERSTITIAL:
                    isAvailable = InterstitialAd.IsAdAvailable(preloadConfig.AdUnitId);
                    break;
            }
            Button.interactable = isAvailable;
            StatusText.text = isAvailable ? "Available" : "Exhausted";
            StatusText.color = isAvailable ? Color.green : Color.red;
        }

        public void PollAndShowAd()
        {
             switch (PreloadConfig.Format)
            {
                case AdFormat.APP_OPEN_AD:
                    PollAndShowAppOpenAd();
                    break;
                case AdFormat.REWARDED:
                    PollAndShowRewardedAd();
                    break;
                case AdFormat.INTERSTITIAL:
                    PollAndShowInterstitialAd();
                    break;
            }
        }

        public void PollAndShowAppOpenAd()
        {
            // Verify that a preloaded ad is available before polling for an ad.
            if(!AppOpenAd.IsAdAvailable(PreloadConfig.AdUnitId))
            {
                Debug.LogWarning(
                    $"{PreloadConfig.AdUnitId}'s preloaded interstitial ad is unavailable.");
                return;
            }

            // Polling returns the next available ad and load another ad in the background.
            var ad = AppOpenAd.PollAd(PreloadConfig.AdUnitId);

            // Always show a preloaded ad immediately.
            if(ad != null)
            {
                ad.Show();
            }
        }

        // [START pollAndShowAd]
        public void PollAndShowInterstitialAd()
        {
            // [START isAdAvailable]
            // Verify that a preloaded ad is available before polling for an ad.
            if(!InterstitialAd.IsAdAvailable(PreloadConfig.AdUnitId))
            {
                Debug.LogWarning(
                    $"{PreloadConfig.AdUnitId}'s preloaded app open ad is unavailable.");
                return;
            }
            // [END isAdAvailable]

            // Polling returns the next available ad and load another ad in the background.
            var ad = InterstitialAd.PollAd(PreloadConfig.AdUnitId);

            // Always show a preloaded ad immediately.
            if(ad != null)
            {
                ad.Show();
            }
        }
        // [END pollAndShowAd]


        public void PollAndShowRewardedAd()
        {
            // Verify that a preloaded ad is available before polling for an ad.
            if(!RewardedAd.IsAdAvailable(PreloadConfig.AdUnitId))
            {
                Debug.LogWarning(
                    $"{PreloadConfig.AdUnitId}'s preloaded rewarded ad is unavailable.");
                return;
            }

            // Polling returns the next available ad and load another ad in the background.
            var ad = RewardedAd.PollAd(PreloadConfig.AdUnitId);

            // Always show a preloaded ad immediately.
            if(ad != null)
            {
                ad.Show((Reward reward) =>
                {
                    Debug.Log($"Rewarded ad granted a reward: {reward.Amount} {reward.Type}");
                });
            }
        }

        private void OnDestroy()
        {
            PreloadConfig = null;
        }
    }
}