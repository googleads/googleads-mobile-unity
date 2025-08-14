using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Api.AdManager;

namespace GoogleMobileAds.Snippets
{
    internal class AnchoredAdaptiveBannerViewSnippets
    {
#if UNITY_ANDROID
        private const string AD_UNIT_ID = "ca-app-pub-3940256099942544/9214589741";
#elif UNITY_IPHONE
        private const string AD_UNIT_ID = "ca-app-pub-3940256099942544/2435281174";
#else
        private const string AD_UNIT_ID = "unused";
#endif
        BannerView bannerView;

        private void CreateAnchoredAdaptiveBannerView()
        {
            // [START get_device_full_width]
            // Get the device full width in density-independent pixels.
            int deviceFullWidth = Mathf.RoundToInt(Screen.width / MobileAds.Utils.GetDeviceScale());
            // [END get_device_full_width]

            // [START get_device_width]
            // Get the device safe width in density-independent pixels.
            int deviceWidth = MobileAds.Utils.GetDeviceSafeWidth();
            // [END get_device_width]

            // [START get_adaptive_size]
            // Define the anchored adaptive ad size.
            AdSize adaptiveSize =
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(deviceWidth);
            // [END get_adaptive_size]

            // [START create_anchored_adaptive_banner_view]
            // Create an anchored adaptive banner view.
            bannerView = new BannerView(AD_UNIT_ID, adaptiveSize, AdPosition.Bottom);
            // [END create_anchored_adaptive_banner_view]
        }
    }
}