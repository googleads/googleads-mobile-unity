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

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Api.AdManager;
using GoogleMobileAds.Api.Mediation;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    internal class Utils
    {
        #region Fully-qualified class names

        #region Google Mobile Ads SDK class names

        public const string AdMobAdapterClassName =
                "com.google.ads.mediation.admob.AdMobAdapter";

        public const string AdListenerClassName = "com.google.android.gms.ads.AdListener";

        public const string AdRequestClassName = "com.google.android.gms.ads.AdRequest";

        public const string AdRequestBuilderClassName =
                "com.google.android.gms.ads.AdRequest$Builder";

        public const string AdManagerAdRequestBuilderClassName =
                "com.google.android.gms.ads.admanager.AdManagerAdRequest$Builder";

        public const string AdSizeClassName = "com.google.android.gms.ads.AdSize";

        public const string AppOpenAdClassName =
                "com.google.android.gms.ads.appopen.AppOpenAd";

        public const string PlayStorePurchaseListenerClassName =
                "com.google.android.gms.ads.purchase.PlayStorePurchaseListener";

        public const string PublisherPrivacyPersonalizationStateEnumName =
            "com.google.android.gms.ads.RequestConfiguration$PublisherPrivacyPersonalizationState";

        public const string MobileAdsClassName = "com.google.android.gms.ads.MobileAds";

        public const string PreloadConfigurationClassName = "com.google.android.gms.ads.preload.PreloadConfiguration";

        public const string PreloadConfigurationBuilderClassName = "com.google.android.gms.ads.preload.PreloadConfiguration$Builder";

        public const string PreloadListenerClassname = "com.google.android.gms.ads.preload.PreloadCallback";

        public const string PreloadCallbackClassname = "com.google.unity.ads.UnityPreloadCallback";

        public const string AdFormatEnumName = "com.google.android.gms.ads.AdFormat";

        public const string RequestConfigurationClassName = "com.google.android.gms.ads.RequestConfiguration";

        public const string RequestConfigurationBuilderClassName = "com.google.android.gms.ads.RequestConfiguration$Builder";

        public const string ServerSideVerificationOptionsClassName =
                "com.google.android.gms.ads.rewarded.ServerSideVerificationOptions";

        public const string ServerSideVerificationOptionsBuilderClassName =
                "com.google.android.gms.ads.rewarded.ServerSideVerificationOptions$Builder";

        public const string NativeAdOptionsBuilderClassName =
            "com.google.android.gms.ads.nativead.NativeAdOptions$Builder";

        public const string VideoOptionsBuilderClassName =
            "com.google.android.gms.ads.VideoOptions$Builder";

        #endregion

        #region Google Mobile Ads Unity Plugin class names

        public const string UnityAdSizeClassName = "com.google.unity.ads.UnityAdSize";

        public const string BannerViewClassName = "com.google.unity.ads.Banner";

        public const string InterstitialClassName = "com.google.unity.ads.Interstitial";

        public const string RewardBasedVideoClassName = "com.google.unity.ads.RewardBasedVideo";

        public const string UnityRewardedAdClassName = "com.google.unity.ads.UnityRewardedAd";

        public const string UnityRewardedAdPreloaderClassName =
                "com.google.unity.ads.UnityRewardedAdPreloader";

        public const string UnityAdListenerClassName = "com.google.unity.ads.UnityAdListener";

        public const string UnityAdManagerAdListenerClassName =
                "com.google.unity.ads.admanager.UnityAdManagerAdListener";

        public const string UnityAppStateEventNotifierClassName =
                "com.google.unity.ads.UnityAppStateEventNotifier";

        public const string UnityAppStateEventCallbackClassName =
                "com.google.unity.ads.UnityAppStateEventCallback";

        public const string UnityRewardedAdCallbackClassName =
                "com.google.unity.ads.UnityRewardedAdCallback";

        public const string UnityAdManagerBannerViewClassName =
                "com.google.unity.ads.admanager.UnityAdManagerBannerView";

        public const string UnityInterstitialAdCallbackClassName = "com.google.unity.ads.UnityInterstitialAdCallback";

        public const string UnityAdManagerInterstitialAdClassName =
                "com.google.unity.ads.admanager.UnityAdManagerInterstitialAd";

        public const string UnityAdManagerInterstitialAdCallbackClassName =
                "com.google.unity.ads.admanager.UnityAdManagerInterstitialAdCallback";

        public const string UnityFullScreenContentCallbackClassName = "com.google.unity.ads.UnityFullScreenContentCallback";

        public const string UnityAdapterStatusEnumName =
                "com.google.android.gms.ads.initialization.AdapterStatus$State";

        public const string UnityAppOpenAdClassName = "com.google.unity.ads.UnityAppOpenAd";

        public const string UnityAppOpenAdCallbackClassName =
                "com.google.unity.ads.UnityAppOpenAdCallback";

        public const string UnityAppOpenAdPreloaderClassName = "com.google.unity.ads.UnityAppOpenAdPreloader";

        public const string OnInitializationCompleteListenerClassName =
                "com.google.android.gms.ads.initialization.OnInitializationCompleteListener";

        public const string UnityAdLoaderListenerClassName =
                "com.google.unity.ads.UnityAdLoaderListener";

        public const string UnityAdInspectorClassName =
                "com.google.unity.ads.UnityAdInspector";

        public const string UnityAdInspectorListenerClassname =
                "com.google.unity.ads.UnityAdInspectorListener";

        public const string UnityPaidEventListenerClassName =
                "com.google.unity.ads.UnityPaidEventListener";

        public const string UnityRewardedInterstitialAdClassName =
                "com.google.unity.ads.UnityRewardedInterstitialAd";

        public const string UnityInterstitialAdPreloaderClassName =
                "com.google.unity.ads.UnityInterstitialAdPreloader";

        public const string UnityRewardedInterstitialAdCallbackClassName =
                "com.google.unity.ads.UnityRewardedInterstitialAdCallback";

        public const string PluginUtilsClassName = "com.google.unity.ads.PluginUtils";

        public const string UnityApplicationPreferencesClassName =
                "com.google.unity.ads.UnityApplicationPreferences";

        public const string UnityNativeTemplateAdClassName =
            "com.google.unity.ads.nativead.UnityNativeTemplateAd";

        public const string UnityNativeTemplateAdCallbackClassName =
            "com.google.unity.ads.nativead.UnityNativeTemplateAdCallback";

        public const string UnityNativeTemplateFontStyleClassName =
            "com.google.unity.ads.nativead.UnityNativeTemplateFontStyle";

        public const string UnityNativeTemplateStyleClassName =
            "com.google.unity.ads.nativead.UnityNativeTemplateStyle";

        public const string UnityNativeTemplateTextStyleClassName =
            "com.google.unity.ads.nativead.UnityNativeTemplateTextStyle";

        public const string UnityNativeTemplateTypeClassName =
            "com.google.unity.ads.nativead.UnityNativeTemplateType";

        #endregion

        #region Unity class names

        public const string UnityActivityClassName = "com.unity3d.player.UnityPlayer";

        #endregion

        #region Android SDK class names

        public const string ColorClassName = "android.graphics.Color";
        public const string ColorDrawableClassName = "android.graphics.drawable.ColorDrawable";
        public const string BundleClassName = "android.os.Bundle";
        public const string PreferenceManagerClassName = "android.preference.PreferenceManager";
        public const string DisplayMetricsClassName = "android.util.DisplayMetrics";
        public const string DoubleClassName = "java.lang.Double";
        public const string DateClassName = "java.util.Date";

        #endregion

        #endregion

        #region JavaObject creators

        public static AndroidJavaObject GetAdSizeJavaObject(AdSize adSize)
        {
            AndroidJavaClass adSizeClass = new AndroidJavaClass(UnityAdSizeClassName);
            switch (adSize.AdType)
            {
                case AdSize.Type.SmartBanner:
                    return adSizeClass.CallStatic<AndroidJavaObject>("getSmartBannerAdSize");
                case AdSize.Type.AnchoredAdaptive:

                    AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
                    AndroidJavaObject activity =
                        playerClass.GetStatic<AndroidJavaObject>("currentActivity");
                    switch (adSize.Orientation)
                    {
                        case Orientation.Landscape:
                            return adSizeClass.CallStatic<AndroidJavaObject>("getLandscapeAnchoredAdaptiveBannerAdSize", activity, adSize.Width);
                        case Orientation.Portrait:
                            return adSizeClass.CallStatic<AndroidJavaObject>("getPortraitAnchoredAdaptiveBannerAdSize", activity, adSize.Width);
                        case Orientation.Current:
                            return adSizeClass.CallStatic<AndroidJavaObject>("getCurrentOrientationAnchoredAdaptiveBannerAdSize", activity, adSize.Width);
                        default:
                            throw new ArgumentException("Invalid Orientation provided for ad size.");
                    }
                case AdSize.Type.Standard:
                    return new AndroidJavaObject(AdSizeClassName, adSize.Width, adSize.Height);
                default:
                    throw new ArgumentException("Invalid AdSize.Type provided for ad size.");
            }
        }

        public static Dictionary<string, string> GetDictionary(AndroidJavaObject androidBundle)
        {
            AndroidJavaObject bundleKeySet = androidBundle.Call<AndroidJavaObject>("keySet");
            int length = bundleKeySet.Call<int>("size");

            AndroidJavaObject bundleKeyArray = bundleKeySet.Call<AndroidJavaObject>("toArray");
            IntPtr bundleKeyArrayPtr = bundleKeyArray.GetRawObject();

            Dictionary<string, string> dict = new Dictionary<string, string>();
            for (int i = 0; i < length; i++)
            {
                IntPtr keyPtr = AndroidJNI.GetObjectArrayElement(bundleKeyArrayPtr, i);
                string key = AndroidJNI.GetStringUTFChars(keyPtr);
                string val = androidBundle.Call<string>("getString", key);
                dict.Add(key, val);
            }
            return dict;
        }

        internal static int GetScreenWidth()
        {
            DisplayMetrics metrics = new DisplayMetrics();
            return (int)(metrics.WidthPixels / metrics.Density);
        }

        /// <summary>
        /// Converts the plugin AdRequest object to a native java proxy object for use by the sdk.
        /// </summary>
        /// <param name="AdRequest">the AdRequest from the unity plugin.</param>
        /// <param name="nativePluginVersion">the version string of the native plugin.</param>
        public static AndroidJavaObject GetAdRequestJavaObject(AdRequest request,
                                                               string nativePluginVersion = null)
        {
            AndroidJavaObject adRequestBuilder = new AndroidJavaObject(AdRequestBuilderClassName);
            foreach (string keyword in request.Keywords)
            {
                adRequestBuilder.Call<AndroidJavaObject>("addKeyword", keyword);
            }

            foreach (KeyValuePair<string, string> entry in request.CustomTargeting)
            {
                adRequestBuilder.Call<AndroidJavaObject>("addCustomTargeting",
                                                         entry.Key, entry.Value);
            }

            // Denote that the request is coming from this Unity plugin.
            adRequestBuilder.Call<AndroidJavaObject>(
                    "setRequestAgent",
                    AdRequest.BuildVersionString(nativePluginVersion));
            AndroidJavaObject bundle = new AndroidJavaObject(BundleClassName);
            foreach (KeyValuePair<string, string> entry in request.Extras)
            {
                bundle.Call("putString", entry.Key, entry.Value);
            }

            bundle.Call("putString", "is_unity", "1");

            // Makes ads that contain WebP ad assets ineligible.
            bundle.Call("putString", "adw", "true");

            AndroidJavaClass adMobAdapter = new AndroidJavaClass(AdMobAdapterClassName);
            adRequestBuilder.Call<AndroidJavaObject>(
                "addNetworkExtrasBundle",
                adMobAdapter,
                bundle);

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

        /// <summary>
        /// Converts the plugin AdRequest or AdManagerAdRequest object to a native java proxy object
        /// for use by the sdk.
        /// </summary>
        /// <param name="AdRequest">the AdRequest from the unity plugin.</param>
        /// <param name="nativePluginVersion">the version string of the native plugin.</param>
        public static AndroidJavaObject GetAdManagerAdRequestJavaObject(AdRequest request,
                string nativePluginVersion = null)
        {
            if (!typeof(AdManagerAdRequest).IsInstanceOfType(request))
            {
                return GetAdRequestJavaObject(request, nativePluginVersion);
            }

            AdManagerAdRequest adManagerAdRequest = (AdManagerAdRequest)request;
            AndroidJavaObject adManagerAdRequestBuilder =
                    new AndroidJavaObject(AdManagerAdRequestBuilderClassName);
            foreach (string keyword in adManagerAdRequest.Keywords)
            {
                adManagerAdRequestBuilder.Call<AndroidJavaObject>("addKeyword", keyword);
            }

            // Denote that the request is coming from this Unity plugin.
            adManagerAdRequestBuilder.Call<AndroidJavaObject>(
                    "setRequestAgent",
                    AdRequest.BuildVersionString(nativePluginVersion));
            AndroidJavaObject bundle = new AndroidJavaObject(BundleClassName);
            foreach (KeyValuePair<string, string> entry in adManagerAdRequest.Extras)
            {
                bundle.Call("putString", entry.Key, entry.Value);
            }

            bundle.Call("putString", "is_unity", "1");

            // Makes ads that contain WebP ad assets ineligible.
            bundle.Call("putString", "adw", "true");

            AndroidJavaClass adMobAdapter = new AndroidJavaClass(AdMobAdapterClassName);
            adManagerAdRequestBuilder.Call<AndroidJavaObject>(
                "addNetworkExtrasBundle",
                adMobAdapter,
                bundle);

            foreach (MediationExtras mediationExtra in adManagerAdRequest.MediationExtras)
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
                    adManagerAdRequestBuilder.Call<AndroidJavaObject>(
                        "addNetworkExtrasBundle",
                        mediationExtrasBundleBuilder.Call<AndroidJavaClass>("getAdapterClass"),
                        mediationExtras);

                    adManagerAdRequestBuilder.Call<AndroidJavaObject>(
                        "addCustomEventExtrasBundle",
                        mediationExtrasBundleBuilder.Call<AndroidJavaClass>("getAdapterClass"),
                        mediationExtras);
                }
            }

            adManagerAdRequestBuilder.Call<AndroidJavaObject>("setPublisherProvidedId",
                    adManagerAdRequest.PublisherProvidedId);
            foreach (string category in adManagerAdRequest.CategoryExclusions)
            {
                adManagerAdRequestBuilder.Call<AndroidJavaObject>("addCategoryExclusion", category);
            }

            foreach (KeyValuePair<string, string> entry in adManagerAdRequest.CustomTargeting)
            {
                adManagerAdRequestBuilder.Call<AndroidJavaObject>("addCustomTargeting",
                                                                   entry.Key, entry.Value);
            }

            return adManagerAdRequestBuilder.Call<AndroidJavaObject>("build");
        }

        public static AndroidJavaObject GetPreloadConfigurationJavaObject(
                PreloadConfiguration preloadConfiguration)
        {
            if (preloadConfiguration.AdUnitId == null)
            {
                throw new ArgumentNullException("PreloadConfiguration.AdUnitId");
            }
            AndroidJavaClass adFormat = new AndroidJavaClass(Utils.AdFormatEnumName);
            AndroidJavaObject adFormatEnum = adFormat.GetStatic<AndroidJavaObject>(
#pragma warning disable 0612
                    preloadConfiguration.Format.ToString());
#pragma warning restore 0612
            AndroidJavaObject preloadConfigurationBuilder =
                    new AndroidJavaObject(Utils.PreloadConfigurationBuilderClassName,
                                          preloadConfiguration.AdUnitId,
                                          adFormatEnum);
            if (preloadConfiguration.Request != null)
            {
                preloadConfigurationBuilder =
                        preloadConfigurationBuilder.Call<AndroidJavaObject>("setAdRequest",
                                Utils.GetAdManagerAdRequestJavaObject(
                                        preloadConfiguration.Request));
            }
            if (preloadConfiguration.BufferSize > 0)
            {
                preloadConfigurationBuilder =
                        preloadConfigurationBuilder.Call<AndroidJavaObject>("setBufferSize",
                                Convert.ToInt32(preloadConfiguration.BufferSize));
            }
            return preloadConfigurationBuilder.Call<AndroidJavaObject>("build");
        }

        public static PreloadConfiguration GetPreloadConfiguration(
                AndroidJavaObject configurationJavaObject)
        {
            if (configurationJavaObject == null)
            {
                return null;
            }
            string adUnitId = configurationJavaObject.Call<string>("getAdUnitId");
            AndroidJavaObject format =
                    configurationJavaObject.Call<AndroidJavaObject>("getAdFormat");
            string enumValue = format.Call<string>("name");
            AdFormat adFormat = (AdFormat)Enum.Parse(typeof(AdFormat), enumValue);
            uint bufferSize = Convert.ToUInt32(configurationJavaObject.Call<int>("getBufferSize"));
            return new PreloadConfiguration() {
                AdUnitId = adUnitId,
#pragma warning disable 0612
                Format = adFormat,
#pragma warning restore 0612
                BufferSize = bufferSize
            };
        }

        public static AndroidJavaObject GetJavaListObject(List<String> csTypeList)
        {
            AndroidJavaObject javaTypeArrayList = new AndroidJavaObject("java.util.ArrayList");
            foreach (string itemList in csTypeList)
            {
                javaTypeArrayList.Call<bool>("add", itemList);
            }
            return javaTypeArrayList;
        }

        public static List<String> GetCsTypeList(AndroidJavaObject javaTypeList)
        {
            List<String> csTypeList = new List<String>();
            int length = javaTypeList.Call<int>("size");
            for (int i = 0; i < length; i++)
            {
                csTypeList.Add(javaTypeList.Call<string>("get", i));
            }

            return csTypeList;
        }

        public static AndroidJavaObject GetServerSideVerificationOptionsJavaObject(ServerSideVerificationOptions serverSideVerificationOptions)
        {
            AndroidJavaObject serverSideVerificationOptionsBuilder = new AndroidJavaObject(ServerSideVerificationOptionsBuilderClassName);
            serverSideVerificationOptionsBuilder.Call<AndroidJavaObject>("setUserId", serverSideVerificationOptions.UserId);
            serverSideVerificationOptionsBuilder.Call<AndroidJavaObject>("setCustomData", serverSideVerificationOptions.CustomData);

            return serverSideVerificationOptionsBuilder.Call<AndroidJavaObject>("build");
        }

        public static AndroidJavaObject GetNativeAdOptionsJavaObject(
            NativeAdOptions nativeAdOptions)
        {
            AndroidJavaObject nativeAdOptionsBuilder =
                new AndroidJavaObject(NativeAdOptionsBuilderClassName);
            nativeAdOptionsBuilder.Call<AndroidJavaObject>("setAdChoicesPlacement",
                                                        (int)nativeAdOptions.AdChoicesPlacement);
            nativeAdOptionsBuilder.Call<AndroidJavaObject>("setMediaAspectRatio",
                                                        (int)nativeAdOptions.MediaAspectRatio);

            AndroidJavaObject videoOptions =
                Utils.GetVideoOptionsJavaObject(nativeAdOptions.VideoOptions);
            nativeAdOptionsBuilder.Call<AndroidJavaObject>("setVideoOptions", videoOptions);
            return nativeAdOptionsBuilder.Call<AndroidJavaObject>("build");
        }

        public static AndroidJavaObject GetVideoOptionsJavaObject(VideoOptions videoOptions)
        {
            AndroidJavaObject videoOptionsBuilder =
                new AndroidJavaObject(VideoOptionsBuilderClassName);
            videoOptionsBuilder.Call<AndroidJavaObject>("setClickToExpandRequested",
                                                      (bool)videoOptions.ClickToExpandRequested);
            videoOptionsBuilder.Call<AndroidJavaObject>("setCustomControlsRequested",
                                                      (bool)videoOptions.CustomControlsRequested);
            videoOptionsBuilder.Call<AndroidJavaObject>("setStartMuted",
                                                      (bool)videoOptions.StartMuted);
            return videoOptionsBuilder.Call<AndroidJavaObject>("build");
        }

        #endregion

        #region Internal utility methods

        internal static AndroidJavaObject GetCurrentActivityAndroidJavaObject()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass(UnityActivityClassName);
            AndroidJavaObject currentActivity =
                    unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            return currentActivity;
        }

        #endregion
    }
}
