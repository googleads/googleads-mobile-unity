using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Api.AdManager;

namespace GoogleMobileAds.Snippets
{
    internal class AdManagerBannerViewSnippets
    {
        private const string AD_UNIT_ID = "/21775744923/example/adaptive-banner";

        AdManagerBannerView adManagerBannerView;

        private void CreateBannerView()
        {
            // [START create_banner_view]
            // Create a 320x50 banner at top of the screen.
            adManagerBannerView = new AdManagerBannerView(AD_UNIT_ID, AdSize.Banner, AdPosition.Top);
            // [END create_banner_view]
        }

        private void CreateBannerViewWithCustomPostition()
        {
            // [START create_banner_view_position]
            // Create a 320x50 banner views at coordinate (0,50) on screen.
            adManagerBannerView = new AdManagerBannerView(AD_UNIT_ID, AdSize.Banner, 0, 50);
            // [END create_banner_view_position]
        }

        private void CreateBannerViewWithCustomSize()
        {
            // [START create_banner_view_size]
            // Create a 250x250 banner at the bottom of the screen.
            AdSize adSize = new AdSize(250, 250);
            adManagerBannerView = new AdManagerBannerView(AD_UNIT_ID, adSize, AdPosition.Bottom);
            // [END create_banner_view_size]
        }

        private void CreateAdManagerBannerViewWithCustomSizes()
        {
            // [START create_banner_view_sizes]
            // Create a 250x250 banner at the bottom of the screen.
            adManagerBannerView = new AdManagerBannerView(AD_UNIT_ID, AdSize.Banner, AdPosition.Top);

            // Add multiple ad sizes.
            adManagerBannerView.ValidAdSizes = new List<AdSize>
            {
                AdSize.Banner,
                new AdSize(120, 20),
                new AdSize(250, 250),
            };
            // [END create_banner_view_sizes]
        }

        private void LoadBannerView()
        {
            // [START load_banner_view]
            // Send a request to load an ad into the banner view.
            adManagerBannerView.LoadAd(new AdManagerAdRequest());
            // [END load_banner_view]
        }

        private void ListenToBannerViewEvents()
        {
            // [START listen_to_events]
            adManagerBannerView.OnBannerAdLoaded += () =>
            {
                // Raised when an ad is loaded into the banner view.
            };
            adManagerBannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                // Raised when an ad fails to load into the banner view.
            };
            adManagerBannerView.OnAdPaid += (AdValue adValue) =>
            {
                // Raised when the ad is estimated to have earned money.
            };
            adManagerBannerView.OnAdImpressionRecorded += () =>
            {
                // Raised when an impression is recorded for an ad.
            };
            adManagerBannerView.OnAdClicked += () =>
            {
                // Raised when a click is recorded for an ad.
            };
            adManagerBannerView.OnAdFullScreenContentOpened += () =>
            {
                // Raised when an ad opened full screen content.
            };
            adManagerBannerView.OnAdFullScreenContentClosed += () =>
            {
                // Raised when the ad closed full screen content.
            };
            // [END listen_to_events]
        }

        private void DestroyBannerView()
        {
            // [START destroy_banner_view]
            if (adManagerBannerView != null)
            {
                // Always destroy the banner view when no longer needed.
                adManagerBannerView.Destroy();
                adManagerBannerView = null;
            }
            // [END destroy_banner_view]
        }

        private void HandleAppEvent()
        {
            // [START handle_app_event]
            adManagerBannerView.OnAppEventReceived += (AppEvent args) =>
            {
                Debug.Log($"Received app event from the ad: {args.Name}, {args.Data}.");
            };
            // [END handle_app_event]
        }

        private void HandleAppEventExample(Renderer renderer)
        {
            // [START handle_app_event_example]
            adManagerBannerView.OnAppEventReceived += (AppEvent args) =>
            {
                if (args.Name == "color")
                {
                    Color color;
                    if (ColorUtility.TryParseHtmlString(args.Data, out color))
                    {
                        renderer.material.color = color;
                    }
                }
            };
            // [END handle_app_event_example]
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
            adManagerBannerView = new AdManagerBannerView(AD_UNIT_ID, adaptiveSize, AdPosition.Bottom);
            // [END create_anchored_adaptive_banner_view]
        }
    }
}