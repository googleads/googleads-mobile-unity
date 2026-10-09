using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Api.AdManager;

namespace GoogleMobileAds.Snippets
{
    internal class BannerViewSnippets
    {
#if UNITY_ANDROID
        private const string AD_UNIT_ID = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
        private const string AD_UNIT_ID = "ca-app-pub-3940256099942544/2934735716";
#else
        private const string AD_UNIT_ID = "unused";
#endif

#if UNITY_ANDROID
        private const string ANCHORED_ADAPTIVE_AD_UNIT_ID = "ca-app-pub-3940256099942544/9214589741";
#elif UNITY_IPHONE
        private const string ANCHORED_ADAPTIVE_AD_UNIT_ID = "ca-app-pub-3940256099942544/2435281174";
#else
        private const string ANCHORED_ADAPTIVE_AD_UNIT_ID = "unused";
#endif

        BannerView bannerView;

        private void CreateBannerView()
        {
            // [START create_banner_view]
            // Create a 320x50 banner at top of the screen.
            bannerView = new BannerView(AD_UNIT_ID, AdSize.Banner, AdPosition.Top);
            // [END create_banner_view]
        }

        private void CreateBannerViewWithCustomPostition()
        {
            // [START create_banner_view_position]
            // Create a 320x50 banner views at coordinate (0,50) on screen.
            bannerView = new BannerView(AD_UNIT_ID, AdSize.Banner, 0, 50);
            // [END create_banner_view_position]
        }

        private void CreateBannerViewWithCustomSize()
        {
            // [START create_banner_view_size]
            // Create a 250x250 banner at the bottom of the screen.
            AdSize adSize = new AdSize(250, 250);
            bannerView = new BannerView(AD_UNIT_ID, adSize, AdPosition.Bottom);
            // [END create_banner_view_size]
        }

        private void CreateAdManagerBannerViewWithCustomSizes(
            AdManagerBannerView adManagerBannerView)
        {
            // [START create_ad_manager_banner_view_sizes]
            // Create a 250x250 banner at the bottom of the screen.
            adManagerBannerView = new AdManagerBannerView(AD_UNIT_ID, AdSize.Banner, AdPosition.Top);

            // Add multiple ad sizes.
            adManagerBannerView.ValidAdSizes = new List<AdSize>
            {
                AdSize.Banner,
                new AdSize(120, 20),
                new AdSize(250, 250),
            };
            // [END create_ad_manager_banner_view_sizes]
        }

        private void LoadBannerView()
        {
            // [START load_banner_view]
            // Send a request to load an ad into the banner view.
            bannerView.LoadAd(new AdRequest());
            // [END load_banner_view]
        }

        private void ListenToBannerViewEvents()
        {
            // [START listen_to_events]
            bannerView.OnBannerAdLoaded += () =>
            {
                // Raised when an ad is loaded into the banner view.
            };
            bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                // Raised when an ad fails to load into the banner view.
            };
            bannerView.OnAdPaid += (AdValue adValue) =>
            {
                // Raised when the ad is estimated to have earned money.
            };
            bannerView.OnAdImpressionRecorded += () =>
            {
                // Raised when an impression is recorded for an ad.
            };
            bannerView.OnAdClicked += () =>
            {
                // Raised when a click is recorded for an ad.
            };
            bannerView.OnAdFullScreenContentOpened += () =>
            {
                // Raised when an ad opened full screen content.
            };
            bannerView.OnAdFullScreenContentClosed += () =>
            {
                // Raised when the ad closed full screen content.
            };
            // [END listen_to_events]
        }

        private void DestroyBannerView()
        {
            // [START destroy_banner_view]
            if (bannerView != null)
            {
                // Always destroy the banner view when no longer needed.
                bannerView.Destroy();
                bannerView = null;
            }
            // [END destroy_banner_view]
        }

        private void CreateAnchoredAdaptiveBannerView()
        {
            // [START create_anchored_adaptive_banner_view]
            // Get the device safe width in density-independent pixels.
            int deviceWidth = MobileAds.Utils.GetDeviceSafeWidth();

            // Define the anchored adaptive ad size.
            AdSize adaptiveSize =
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(deviceWidth);

            // Create an anchored adaptive banner view.
            bannerView = new BannerView(ANCHORED_ADAPTIVE_AD_UNIT_ID, adaptiveSize, AdPosition.Bottom);
            // [END create_anchored_adaptive_banner_view]
        }
    }
}