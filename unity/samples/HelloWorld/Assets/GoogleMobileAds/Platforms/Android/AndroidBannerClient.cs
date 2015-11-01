#if UNITY_ANDROID

using System;
using System.Collections.Generic;

using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    internal class AndroidBannerClient : IGoogleMobileAdsBannerClient
    {
        private AndroidJavaObject bannerView;

        public AndroidBannerClient(IAdListener listener)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity =
                    playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            bannerView = new AndroidJavaObject(
                    Utils.BannerViewClassName, activity, new AdListener(listener));
        }

        // Creates a banner view.
        public void CreateBannerView(String adUnitId, AdSize adSize, AdPosition position) {
            bannerView.Call("create",
                    new object[3] { adUnitId, Utils.GetAdSizeJavaObject(adSize), (int)position });
        }

        // Load an ad.
        public void LoadAd(AdRequest request)
        {
            bannerView.Call("loadAd", Utils.GetAdRequestJavaObject(request));
        }

        // Show the banner view on the screen.
        public void ShowBannerView() {
            bannerView.Call("show");
        }

        // Hide the banner view from the screen.
        public void HideBannerView()
        {
            bannerView.Call("hide");
        }

        public void DestroyBannerView()
        {
            bannerView.Call("destroy");
        }
    }
}

#endif
