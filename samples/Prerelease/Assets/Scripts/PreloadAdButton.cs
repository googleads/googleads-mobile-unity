using System;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.UI;

namespace GoogleMobileAds.Sample
{
    /// <summary>
    /// UI element for a preloaded ad. Controlled by the PreloadAdsViewController.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/PreloadAdButton")]
    public class PreloadAdButton : MonoBehaviour
    {
        // UI elements set in the editor.
        [Tooltip("Serialized instance of the Button component.")]
        public Button Button;
        [Tooltip("Serialized instance of the title text component.")]
        public Text TitleText;
        [Tooltip("Serialized instance of the status text component.")]
        public Text AvailableText;

        /// <summary>
        /// The PreloadConfiguration associated with this UI element.
        /// </summary>
        public PreloadConfiguration PreloadConfig {get; private set; }

        /// <summary>
        /// Activates this UI element and associates it with a PreloadConfiguration.
        /// </summary>
        public void InitializeUI(PreloadConfiguration preloadConfig)
        {
            gameObject.SetActive(true);
            PreloadConfig = preloadConfig;

            // Display the configurations platform and format.
            string platform = preloadConfig.AdUnitId.StartsWith("ca-app-pub")
                            ? "Admob" : "Ad Manager";
            TitleText.text = $"{platform} {preloadConfig.Format}";

            UpdateUI(preloadConfig);
        }

        /// <summary>
        /// Updates the available text UI element for this PreloadConfiguration.
        /// </summary>
        public void UpdateUI(PreloadConfiguration preloadConfig)
        {
            // Display the configurations availability.
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
            AvailableText.text = isAvailable ? "Available" : "Exhausted";
            AvailableText.color = isAvailable ? Color.green : Color.red;
        }

        /// <summary>
        /// Shows the preloaded ad for this PreloadConfiguration, if available.
        /// </summary>
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

        private void PollAndShowAppOpenAd()
        {
            // Verify that a preloaded ad is available before polling for an ad.
            if(!AppOpenAd.IsAdAvailable(PreloadConfig.AdUnitId))
            {
                Debug.LogWarning(
                    $"{PreloadConfig.AdUnitId}'s preloaded interstitial ad is unavailable.");
                return;
            }

            // Polling returns the next available ad and loads another ad in the background.
            var ad = AppOpenAd.PollAd(PreloadConfig.AdUnitId);

            // Do not hold onto preloaded ads, always show a preloaded ad immediately.
            if(ad != null)
            {
                ad.Show();
            }
        }

        // [START pollAndShowAd]
        private void PollAndShowInterstitialAd()
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

            // Polling returns the next available ad and loads another ad in the background.
            var ad = InterstitialAd.PollAd(PreloadConfig.AdUnitId);

            // Do not hold onto preloaded ads, always show a preloaded ad immediately.
            if(ad != null)
            {
                ad.Show();
            }
        }
        // [END pollAndShowAd]


        private void PollAndShowRewardedAd()
        {
            // Verify that a preloaded ad is available before polling for an ad.
            if(!RewardedAd.IsAdAvailable(PreloadConfig.AdUnitId))
            {
                Debug.LogWarning(
                    $"{PreloadConfig.AdUnitId}'s preloaded rewarded ad is unavailable.");
                return;
            }

            // Polling returns the next available ad and loads another ad in the background.
            var ad = RewardedAd.PollAd(PreloadConfig.AdUnitId);

            // Do not hold onto preloaded ads, always show a preloaded ad immediately.
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