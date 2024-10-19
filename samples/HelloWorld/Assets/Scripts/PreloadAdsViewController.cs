using System;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System.Linq;

namespace GoogleMobileAds.Sample
{
    /// <summary>
    /// Demonstrates how to use Google Mobile Ads preload APIs.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/PreloadAdsViewController")]
    public class PreloadAdsViewController : MonoBehaviour
    {
        /// <summary>
        /// UI element activated when an ad is ready to show.
        /// </summary>
        public GameObject ButtonPrefab;

        /// <summary>
        /// A list of child views associated with preload configurations.
        /// </summary>
        private List<PreloadAdButton> _preloadAdButtons = new List<PreloadAdButton>();

        void Start()
        {
            // [START start_preload]
            // Define a list of PreloadConfigurations, specifying the ad unit ID and ad format for
            // each ad unit to be preloaded. These ad units are configured to always serve test ads.
            List<PreloadConfiguration> preloadConfigs = new List<PreloadConfiguration>
            {
            #if UNITY_ANDROID
                new PreloadConfiguration{Format = AdFormat.APP_OPEN_AD,
                    AdUnitId = "ca-app-pub-3940256099942544/9257395921"},
                new PreloadConfiguration{Format = AdFormat.INTERSTITIAL,
                    AdUnitId = "ca-app-pub-3940256099942544/6300978111"},
                new PreloadConfiguration{Format = AdFormat.REWARDED,
                    AdUnitId = "ca-app-pub-3940256099942544/5224354917"},
            #else
                new PreloadConfiguration{Format = AdFormat.APP_OPEN_AD,
                    AdUnitId = "ca-app-pub-3940256099942544/5575463023"},
                new PreloadConfiguration{Format = AdFormat.INTERSTITIAL,
                    AdUnitId = "ca-app-pub-3940256099942544/4411468910"},
                new PreloadConfiguration{Format = AdFormat.REWARDED,
                    AdUnitId = "ca-app-pub-3940256099942544/5224354917"},
            #endif
                new PreloadConfiguration{Format = AdFormat.APP_OPEN_AD,
                    AdUnitId = "/21775744923/example/app-open"},
                new PreloadConfiguration{Format = AdFormat.INTERSTITIAL,
                    AdUnitId = "/21775744923/example/interstitial"},
                new PreloadConfiguration{Format = AdFormat.REWARDED,
                    AdUnitId = "/21775744923/example/rewarded"}
            };

            // Define a callback to receive preload availability events.
            MobileAds.OnAdsAvailable += (PreloadConfiguration preloadConfig) =>
            {
                Debug.Log($"Preload ad for configuration ${preloadConfig.Format} is available.");
                UpdateUI(preloadConfig);
            };
            MobileAds.OnAdsExhausted += (PreloadConfiguration preloadConfig) =>
            {
                Debug.Log($"Preload ad for configuration ${preloadConfig.Format} is exhausted.");
                UpdateUI(preloadConfig);
            };

            // Start the preloading initialization process after MobileAds.Initialize().
            MobileAds.Preload(preloadConfigs);
            // [END start_preload]

            InitializeUI(preloadConfigs);
        }

        void InitializeUI(List<PreloadConfiguration> preloadConfigs)
        {
            // For each preload config, instantiate and initialize the PreloadAdButton.
            _preloadAdButtons = new List<PreloadAdButton>();
            foreach (var preloadConfig in preloadConfigs)
            {
                var instance = Instantiate(ButtonPrefab, ButtonPrefab.transform.parent);
                var view = instance.GetComponent<PreloadAdButton>();
                view.InitializeUI(preloadConfig);
                _preloadAdButtons.Add(view);
            }
        }

        void UpdateUI(PreloadConfiguration preloadConfig)
        {
            // Find the PreloadAdButton for this preload config and update it.
            var view = _preloadAdButtons
                .Where(o=> o.PreloadConfig == preloadConfig)
                .FirstOrDefault();
            if(view == null)
            {
                return;
            }
            view.UpdateStatusUI(preloadConfig);
        }
    }
}
