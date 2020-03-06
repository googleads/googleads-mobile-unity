// Copyright (C) 2015 Google, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using System;
using System.Collections.Generic;

using GoogleMobileAds.Api;
using GoogleMobileAds.Api.Mediation;
using GoogleMobileAds.Common;

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

        public const string PlayStorePurchaseListenerClassName =
            "com.google.android.gms.ads.purchase.PlayStorePurchaseListener";

        public const string MobileAdsClassName = "com.google.android.gms.ads.MobileAds";

        public const string ServerSideVerificationOptionsClassName =
            "com.google.android.gms.ads.rewarded.ServerSideVerificationOptions";

        public const string ServerSideVerificationOptionsBuilderClassName =
            "com.google.android.gms.ads.rewarded.ServerSideVerificationOptions$Builder";

        #endregion

        #region Google Mobile Ads Unity Plugin class names

        public const string BannerViewClassName = "com.google.unity.ads.Banner";

        public const string InterstitialClassName = "com.google.unity.ads.Interstitial";

        public const string RewardBasedVideoClassName = "com.google.unity.ads.RewardBasedVideo";

        public const string UnityRewardedAdClassName = "com.google.unity.ads.UnityRewardedAd";

        public const string NativeAdLoaderClassName = "com.google.unity.ads.NativeAdLoader";

        public const string UnityAdListenerClassName = "com.google.unity.ads.UnityAdListener";

        public const string UnityRewardBasedVideoAdListenerClassName =
            "com.google.unity.ads.UnityRewardBasedVideoAdListener";

        public const string UnityRewardedAdCallbackClassName =
            "com.google.unity.ads.UnityRewardedAdCallback";

        public const string UnityAdapterStatusEnumName =
                "com.google.android.gms.ads.initialization.AdapterStatus$State";

        public const string OnInitializationCompleteListenerClassName =
            "com.google.android.gms.ads.initialization.OnInitializationCompleteListener";

        public const string UnityAdLoaderListenerClassName =
            "com.google.unity.ads.UnityAdLoaderListener";

        public const string UnityPaidEventListenerClassName =
            "com.google.unity.ads.UnityPaidEventListener";


        public const string PluginUtilsClassName = "com.google.unity.ads.PluginUtils";

        #endregion

        #region Unity class names

        public const string UnityActivityClassName = "com.unity3d.player.UnityPlayer";

        #endregion

        #region Android SDK class names

        public const string BundleClassName = "android.os.Bundle";
        public const string DateClassName = "java.util.Date";
        public const string DisplayMetricsClassName = "android.util.DisplayMetrics";

        #endregion

        #endregion

        #region JavaObject creators

        public static AndroidJavaObject GetAdSizeJavaObject(AdSize adSize)
        {
            switch (adSize.AdType) {
                case AdSize.Type.SmartBanner:
                    // AndroidJavaClass.GetStatic<AndroidJavaObject>() returns null since Unity 2019.2.
                    // Creates an AdSize object by directly calling the constructor, as a workaround.
                    return new AndroidJavaObject(AdSizeClassName, -1, -2)
                            .GetStatic<AndroidJavaObject>("SMART_BANNER");
                case AdSize.Type.AnchoredAdaptive:
                    AndroidJavaClass adSizeClass = new AndroidJavaClass(AdSizeClassName);
                    AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
                    AndroidJavaObject activity =
                        playerClass.GetStatic<AndroidJavaObject>("currentActivity");
                    switch (adSize.Orientation)
                    {
                        case Orientation.Landscape:
                            return adSizeClass.CallStatic<AndroidJavaObject>("getLandscapeBannerAdSizeWithWidth", activity, adSize.Width);
                        case Orientation.Portrait:
                            return adSizeClass.CallStatic<AndroidJavaObject>("getPortraitBannerAdSizeWithWidth", activity, adSize.Width);
                        case Orientation.Current:
                            return adSizeClass.CallStatic<AndroidJavaObject>("getCurrentOrientationBannerAdSizeWithWidth", activity, adSize.Width);
                        default:
                            throw new ArgumentException("Invalid Orientation provided for ad size.");
                    }
                case AdSize.Type.Standard:
                    return new AndroidJavaObject(AdSizeClassName, adSize.Width, adSize.Height);
                default:
                    throw new ArgumentException("Invalid AdSize.Type provided for ad size.");
  }
        }

        internal static int GetScreenWidth() {
          DisplayMetrics metrics = new DisplayMetrics();
          return (int) (metrics.WidthPixels / metrics.Density);
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
                if (deviceId == AdRequest.TestDeviceSimulator)
                {
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
                switch (request.Gender.GetValueOrDefault())
                {
                    case Api.Gender.Unknown:
                        genderCode = new AndroidJavaClass(AdRequestClassName)
                                .GetStatic<int>("GENDER_UNKNOWN");
                        break;
                    case Api.Gender.Male:
                        genderCode = new AndroidJavaClass(AdRequestClassName)
                                .GetStatic<int>("GENDER_MALE");
                        break;
                    case Api.Gender.Female:
                        genderCode = new AndroidJavaClass(AdRequestClassName)
                                .GetStatic<int>("GENDER_FEMALE");
                        break;
                }

                if (genderCode.HasValue)
                {
                    adRequestBuilder.Call<AndroidJavaObject>("setGender", genderCode);
                }
            }

            if (request.TagForChildDirectedTreatment.HasValue)
            {
                adRequestBuilder.Call<AndroidJavaObject>(
                        "tagForChildDirectedTreatment",
                        request.TagForChildDirectedTreatment.GetValueOrDefault());
            }

            // Denote that the request is coming from this Unity plugin.
            adRequestBuilder.Call<AndroidJavaObject>(
                    "setRequestAgent",
                    "unity-" + AdRequest.Version);
            AndroidJavaObject bundle = new AndroidJavaObject(BundleClassName);
            foreach (KeyValuePair<string, string> entry in request.Extras)
            {
                bundle.Call("putString", entry.Key, entry.Value);
            }

            bundle.Call("putString", "is_unity", "1");

            AndroidJavaObject extras = new AndroidJavaObject(AdMobExtrasClassName, bundle);
            adRequestBuilder.Call<AndroidJavaObject>("addNetworkExtras", extras);

            foreach (MediationExtras mediationExtra in request.MediationExtras)
            {
                AndroidJavaObject mediationExtrasBundleBuilder =
                    new AndroidJavaObject(mediationExtra.AndroidMediationExtraBuilderClassName);
                AndroidJavaObject map = new AndroidJavaObject("java.util.HashMap");

                foreach (KeyValuePair<string, string> entry in mediationExtra.Extras)
                {
                    map.Call<AndroidJavaObject>("put", entry.Key, entry.Value);
                }

                AndroidJavaObject mediationExtras =
                        mediationExtrasBundleBuilder.Call<AndroidJavaObject>("buildExtras", map);

                if (mediationExtras != null)
                {
                    adRequestBuilder.Call<AndroidJavaObject>(
                        "addNetworkExtrasBundle",
                        mediationExtrasBundleBuilder.Call<AndroidJavaClass>("getAdapterClass"),
                        mediationExtras);

                    adRequestBuilder.Call<AndroidJavaObject>(
                        "addCustomEventExtrasBundle",
                        mediationExtrasBundleBuilder.Call<AndroidJavaClass>("getAdapterClass"),
                        mediationExtras);
                }
            }

            return adRequestBuilder.Call<AndroidJavaObject>("build");
        }

        public static AndroidJavaObject GetServerSideVerificationOptionsJavaObject(ServerSideVerificationOptions serverSideVerificationOptions)
        {
            AndroidJavaObject serverSideVerificationOptionsBuilder = new AndroidJavaObject(ServerSideVerificationOptionsBuilderClassName);
            serverSideVerificationOptionsBuilder.Call<AndroidJavaObject>("setUserId", serverSideVerificationOptions.UserId);
            serverSideVerificationOptionsBuilder.Call<AndroidJavaObject>("setCustomData", serverSideVerificationOptions.CustomData);

            return serverSideVerificationOptionsBuilder.Call<AndroidJavaObject>("build");
        }
        #endregion
    }
}
