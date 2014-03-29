using System;
using UnityEngine;
using GoogleMobileAds.Common;

namespace GoogleMobileAds
{
    internal class GoogleMobileAdsClientFactory
    {
        internal static IGoogleMobileAdsBannerClient GetGoogleMobileAdsBannerClient(
                IAdListener listener)
        {
            #if UNITY_EDITOR
                // Testing UNITY_EDITOR first because the editor also responds to UNITY_ANROID.
                return new GoogleMobileAds.Common.DummyClient(listener);
            #elif UNITY_ANDROID
                return new GoogleMobileAds.Android.AndroidBannerClient(listener);
            #elif UNITY_IPHONE
                return new GoogleMobileAds.iOS.IOSBannerClient(listener);
            #else
                return new GoogleMobileAds.Common.DummyClient(listener);
            #endif
        }
    }
}
