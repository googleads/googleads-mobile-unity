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
        private const string AdListenerClass = "com.google.android.gms.ads.AdListener";
        private const string AdRequestClass = "com.google.android.gms.ads.AdRequest";
        private const string AdRequestBuilderClass = "com.google.android.gms.ads.AdRequest$Builder";
        private const string AdSizeClass = "com.google.android.gms.ads.AdSize";
        private const string AdMobExtrasClass =
                "com.google.android.gms.ads.mediation.admob.AdMobExtras";
        private const string BannerViewClass = "com.google.unity.ads.Banner";
        private const string BundleClass = "android.os.Bundle";
        private const string DateClass = "java.util.Date";

        private AndroidJavaObject bannerView;

        public AndroidBannerClient(IAdListener listener)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity =
                    playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            bannerView = new AndroidJavaObject(BannerViewClass, activity, new AdListener(listener));
        }

        // Creates a banner view.
        public void CreateBannerView(String adUnitId, AdSize adSize, AdPosition position) {
            bannerView.Call("create",
                    new object[3] { adUnitId, GetAdSizeJavaObject(adSize), (int)position });
        }

        // Load an ad.
        public void LoadAd(AdRequest request)
        {
            bannerView.Call("loadAd", GetAdRequestJavaObject(request));
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

        #region JavaObject creators

        private static AndroidJavaObject GetAdSizeJavaObject(AdSize adSize)
        {
            if (adSize.IsSmartBanner)
            {
                return new AndroidJavaClass(AdSizeClass)
                        .GetStatic<AndroidJavaObject>("SMART_BANNER");
            }
            else
            {
                return new AndroidJavaObject(AdSizeClass, adSize.Width, adSize.Height);
            }
        }

        private static AndroidJavaObject GetAdRequestJavaObject(AdRequest request)
        {
            AndroidJavaObject adRequestBuilder = new AndroidJavaObject(AdRequestBuilderClass);
            foreach (string keyword in request.Keywords)
            {
                adRequestBuilder.Call<AndroidJavaObject>("addKeyword", keyword);
            }
            foreach (string deviceId in request.TestDevices)
            {
                if (deviceId == AdRequest.TestDeviceSimulator) {
                    string emulatorDeviceId = new AndroidJavaClass(AdRequestClass)
                            .GetStatic<string>("DEVICE_ID_EMULATOR");
                    adRequestBuilder.Call<AndroidJavaObject>("addTestDevice", emulatorDeviceId);
                }
                else
                {
                    adRequestBuilder.Call<AndroidJavaObject>("addTestDevice", deviceId);
                }
            }
            if (request.Birthday.HasValue)
            {
                DateTime birthday = request.Birthday.GetValueOrDefault();
                AndroidJavaObject birthdayObject = new AndroidJavaObject(
                    DateClass, birthday.Year, birthday.Month, birthday.Day);
                adRequestBuilder.Call<AndroidJavaObject>("setBirthday", birthdayObject);
            }
            if (request.Gender.HasValue)
            {
                int? genderCode = null;
                switch(request.Gender.GetValueOrDefault())
                {
                    case Gender.Unknown:
                        genderCode = new AndroidJavaClass(AdRequestClass)
                                .GetStatic<int>("GENDER_UNKNOWN");
                        break;
                    case Gender.Male:
                        genderCode = new AndroidJavaClass(AdRequestClass)
                                .GetStatic<int>("GENDER_MALE");
                        break;
                    case Gender.Female:
                        genderCode = new AndroidJavaClass(AdRequestClass)
                                .GetStatic<int>("GENDER_FEMALE");
                        break;
                }
                if (genderCode.HasValue)
                {
                    adRequestBuilder.Call<AndroidJavaObject>("setGender", genderCode);
                }
            }
            if (request.TagForChildDirectedTreatment.HasValue) {
                adRequestBuilder.Call<AndroidJavaObject>("tagForChildDirectedTreatment",
                        request.TagForChildDirectedTreatment.GetValueOrDefault());
            }
            AndroidJavaObject bundle = new AndroidJavaObject(BundleClass);
            foreach (KeyValuePair<string, string> entry in request.Extras)
            {
                bundle.Call("putString", entry.Key, entry.Value);
            }
            bundle.Call("putInt", "unity", 1);
            AndroidJavaObject extras = new AndroidJavaObject(AdMobExtrasClass, bundle);
            adRequestBuilder.Call<AndroidJavaObject>("addNetworkExtras", extras);

            return adRequestBuilder.Call<AndroidJavaObject>("build");
        }

        #endregion
    }
}

#endif
