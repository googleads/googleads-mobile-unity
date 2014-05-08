#if UNITY_ANDROID

using System;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Android
{
    internal class Utils
    {
        #region Fully-qualified class names

        #region Google Mobile Ads SDK class names

        public const string AdListenerClassName = "com.google.android.gms.ads.AdListener";
        public const string AdRequestClassName = "com.google.android.gms.ads.AdRequest";
        public const string AdRequestBuilderClassName =
                "com.google.android.gms.ads.AdRequest$Builder";
        public const string AdSizeClassName = "com.google.android.gms.ads.AdSize";
        public const string AdMobExtrasClassName =
                "com.google.android.gms.ads.mediation.admob.AdMobExtras";

        #endregion

        #region Google Mobile Ads Unity Plugin class names

        public const string BannerViewClassName = "com.google.unity.ads.Banner";
        public const string InterstitialClassName = "com.google.unity.ads.Interstitial";
        public const string UnityAdListenerClassName = "com.google.unity.ads.UnityAdListener";

        #endregion

        #region Unity class names

        public const string UnityActivityClassName = "com.unity3d.player.UnityPlayer";

        #endregion

        #region Android SDK class names

        public const string BundleClassName = "android.os.Bundle";
        public const string DateClassName = "java.util.Date";

        #endregion

        #endregion

        #region JavaObject creators

        public static AndroidJavaObject GetAdSizeJavaObject(AdSize adSize)
        {
            if (adSize.IsSmartBanner)
            {
                return new AndroidJavaClass(AdSizeClassName)
                        .GetStatic<AndroidJavaObject>("SMART_BANNER");
            }
            else
            {
                return new AndroidJavaObject(AdSizeClassName, adSize.Width, adSize.Height);
            }
        }

        public static AndroidJavaObject GetAdRequestJavaObject(AdRequest request)
        {
            AndroidJavaObject adRequestBuilder = new AndroidJavaObject(AdRequestBuilderClassName);
            foreach (string keyword in request.Keywords)
            {
                adRequestBuilder.Call<AndroidJavaObject>("addKeyword", keyword);
            }
            foreach (string deviceId in request.TestDevices)
            {
                if (deviceId == AdRequest.TestDeviceSimulator) {
                    string emulatorDeviceId = new AndroidJavaClass(AdRequestClassName)
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
                        DateClassName, birthday.Year, birthday.Month, birthday.Day);
                adRequestBuilder.Call<AndroidJavaObject>("setBirthday", birthdayObject);
            }
            if (request.Gender.HasValue)
            {
                int? genderCode = null;
                switch(request.Gender.GetValueOrDefault())
                {
                case Gender.Unknown:
                    genderCode = new AndroidJavaClass(AdRequestClassName)
                            .GetStatic<int>("GENDER_UNKNOWN");
                    break;
                case Gender.Male:
                    genderCode = new AndroidJavaClass(AdRequestClassName)
                            .GetStatic<int>("GENDER_MALE");
                    break;
                case Gender.Female:
                    genderCode = new AndroidJavaClass(AdRequestClassName)
                            .GetStatic<int>("GENDER_FEMALE");
                    break;
                }
                if (genderCode.HasValue)
                {
                    adRequestBuilder.Call<AndroidJavaObject>("setGender", genderCode);
                }
            }
            if (request.TagForChildDirectedTreatment.HasValue) {
                adRequestBuilder.Call<AndroidJavaObject>(
                        "tagForChildDirectedTreatment",
                        request.TagForChildDirectedTreatment.GetValueOrDefault());
            }
            AndroidJavaObject bundle = new AndroidJavaObject(BundleClassName);
            foreach (KeyValuePair<string, string> entry in request.Extras)
            {
                bundle.Call("putString", entry.Key, entry.Value);
            }
            // Denote that the request is coming from this Unity plugin.
            bundle.Call("putInt", "unity", 1);
            AndroidJavaObject extras = new AndroidJavaObject(AdMobExtrasClassName, bundle);
            adRequestBuilder.Call<AndroidJavaObject>("addNetworkExtras", extras);
            return adRequestBuilder.Call<AndroidJavaObject>("build");
        }

        #endregion
    }
}

#endif
