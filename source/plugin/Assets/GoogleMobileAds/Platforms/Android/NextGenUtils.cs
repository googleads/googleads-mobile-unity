
// Copyright (C) 2025 Google, Inc.
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
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Api.AdManager;

namespace GoogleMobileAds.Android {
  internal class NextGenUtils {
#region Fully - qualified NextGen Mobile Ads SDK class names

#region RequestConfiguration
    public const string MaxAdContentRatingClassName =
        "com.google.android.libraries.ads.mobile.sdk.common.RequestConfiguration$MaxAdContentRating";
    public const string PublisherPrivacyPersonalizationStateEnumName =
        "com.google.android.libraries.ads.mobile.sdk.common.RequestConfiguration$PublisherPrivacyPersonalizationState";
    public const string RequestConfigurationBuilderClassName =
        "com.google.android.libraries.ads.mobile.sdk.common.RequestConfiguration$Builder";
    public const string TagForChildDirectedTreatmentClassName =
        "com.google.android.libraries.ads.mobile.sdk.common.RequestConfiguration$TagForChildDirectedTreatment";
    public const string TagForUnderAgeOfConsentClassName =
        "com.google.android.libraries.ads.mobile.sdk.common.RequestConfiguration$TagForUnderAgeOfConsent";
    public const string AgeRestrictedTreatmentClassName =
        "com.google.android.libraries.ads.mobile.sdk.common.AgeRestrictedTreatment";
#endregion

#region MobileAds
    public const string InitializationConfigBuilderClassName =
        "com.google.android.libraries.ads.mobile.sdk.initialization.InitializationConfig$Builder";
    public const string AdapterStatusInitializationStateName =
        "com.google.android.libraries.ads.mobile.sdk.initialization.AdapterStatus$InitializationState";
    public const string MobileAdsClassName =
        "com.google.android.libraries.ads.mobile.sdk.MobileAds";
    public const string OnInitializationCompleteListenerClassName =
        "com.google.android.libraries.ads.mobile.sdk.initialization.OnAdapterInitializationCompleteListener";
#endregion

#region AdRequest
    public const string AdRequestBuilderClassName =
        "com.google.android.libraries.ads.mobile.sdk.common.AdRequest$Builder";
    public const string BannerAdRequestBuilderClassName =
        "com.google.android.libraries.ads.mobile.sdk.banner.BannerAdRequest$Builder";
    public const string NativeAdRequestBuilderClassName =
        "com.google.android.libraries.ads.mobile.sdk.nativead.NativeAdRequest$Builder";
#endregion

#region AdSize
    public const string AdSizeClassName =
        "com.google.android.libraries.ads.mobile.sdk.banner.AdSize";
#endregion

#region PreloadConfiguration
    public const string PreloadConfigurationClassName =
        "com.google.android.libraries.ads.mobile.sdk.common.PreloadConfiguration";
#endregion

#region NativeAd
    public const string NativeAdTypeClassName =
        "com.google.android.libraries.ads.mobile.sdk.nativead.NativeAd$NativeAdType";
    public const string NativeMediaAspectRatioClassName =
        "com.google.android.libraries.ads.mobile.sdk.nativead.NativeAd$NativeMediaAspectRatio";
#endregion

#region MediationAdError
    public const string MediationAdErrorClassName =
        "com.google.android.libraries.ads.mobile.sdk.common.MediationAdError";
#endregion

#region ServerSideVerificationOptions
    public const string ServerSideVerificationOptionsClassName =
        "com.google.android.libraries.ads.mobile.sdk.rewarded.ServerSideVerificationOptions";
#endregion

#region AdChoicesPlacement
    public const string AdChoicesPlacementClassName =
        "com.google.android.libraries.ads.mobile.sdk.common.AdChoicesPlacement";
#endregion

#region VideoOptions
    public const string VideoOptionsBuilderClassName =
        "com.google.android.libraries.ads.mobile.sdk.common.VideoOptions$Builder";
#endregion

#region Java util
    public const string ArrayListClassName = "java.util.ArrayList";
#endregion

#endregion

#region Fully - qualified Unity NextGen Bridge class names

    public const string UnityMobileAdsClassName = "com.google.unity.ads.nextgen.UnityMobileAds";
    public const string UnityAdInspectorClassName = "com.google.unity.ads.nextgen.UnityAdInspector";
    public const string UnityAdInspectorListenerClassName =
        "com.google.unity.ads.nextgen.UnityAdInspectorListener";
    public const string UnityPreloadCallbackClassName =
        "com.google.unity.ads.nextgen.UnityPreloadCallback";

    public const string UnityAppOpenAdClassName = "com.google.unity.ads.nextgen.UnityAppOpenAd";
    public const string UnityAppOpenAdCallbackClassName =
        "com.google.unity.ads.nextgen.UnityAppOpenAdCallback";
    public const string UnityAppOpenAdPreloaderClassName =
        "com.google.unity.ads.nextgen.UnityAppOpenAdPreloader";

    public const string UnityBannerAdClassName = "com.google.unity.ads.nextgen.UnityBannerAd";
    public const string UnityBannerAdCallbackClassName =
        "com.google.unity.ads.nextgen.UnityBannerAdCallback";

    public const string UnityInterstitialAdClassName =
        "com.google.unity.ads.nextgen.UnityInterstitialAd";
    public const string UnityInterstitialAdCallbackClassName =
        "com.google.unity.ads.nextgen.UnityInterstitialAdCallback";

    public const string UnityRewardedAdClassName = "com.google.unity.ads.nextgen.UnityRewardedAd";
    public const string UnityRewardedAdCallbackClassName =
        "com.google.unity.ads.nextgen.UnityRewardedAdCallback";
    public const string UnityRewardedAdPreloaderClassName =
        "com.google.unity.ads.nextgen.UnityRewardedAdPreloader";

    public const string UnityRewardedInterstitialAdClassName =
        "com.google.unity.ads.nextgen.UnityRewardedInterstitialAd";
    public const string UnityRewardedInterstitialAdCallbackClassName =
        "com.google.unity.ads.nextgen.UnityRewardedInterstitialAdCallback";
    public const string UnityRewardedInterstitialAdPreloaderClassName =
        "com.google.unity.ads.nextgen.UnityRewardedInterstitialAdPreloader";
    public const string UnityInterstitialAdPreloaderClassName =
        "com.google.unity.ads.nextgen.UnityInterstitialAdPreloader";

    public const string UnityNativeTemplateAdClassName =
        "com.google.unity.ads.nextgen.UnityNativeTemplateAd";
    public const string UnityNativeTemplateAdCallbackClassName =
        "com.google.unity.ads.nextgen.UnityNativeTemplateAdCallback";
#endregion

    /// <summary>
    /// Converts the plugin AdRequest object to a native java proxy object for use by the sdk.
    /// </summary>
    /// <param name="AdRequest">the AdRequest from the unity plugin.</param>
    public static AndroidJavaObject GetAdRequestJavaObject(AdRequest request, string adUnitId) {
      AndroidJavaObject adRequestBuilder =
          new AndroidJavaObject(AdRequestBuilderClassName, adUnitId);
      foreach (string keyword in request.Keywords) {
        adRequestBuilder.Call<AndroidJavaObject>("addKeyword", keyword);
      }

      foreach (KeyValuePair<string, string> entry in request.CustomTargeting) {
        adRequestBuilder.Call<AndroidJavaObject>("putCustomTargeting", entry.Key, entry.Value);
      }
      adRequestBuilder.Call<AndroidJavaObject>("setRequestAgent", AdRequest.BuildVersionString());
      ApplyAdManagerFields(adRequestBuilder, request);

      return adRequestBuilder.Call<AndroidJavaObject>("build");
    }

    /// <summary>
    /// Converts the plugin AdRequest object to a native java proxy object for use by the sdk.
    /// </summary>
    /// <param name="adUnitId">the ad unit ID.</param>
    /// <param name="request">the AdRequest from the unity plugin.</param>
    /// <param name="adSize">the ad size.</param>
    /// <param name="adSizes">the list of valid ad sizes.</param>
    public static AndroidJavaObject GetBannerAdRequestJavaObject(string adUnitId, AdRequest request,
                                                                 AdSize adSize = null,
                                                                 List<AdSize> adSizes = null) {
      AndroidJavaObject adSizeObject = null;
      if (adSizes != null && adSizes.Count > 0) {
        AndroidJavaObject adSizeArrayList = new AndroidJavaObject(ArrayListClassName);
        foreach (AdSize size in adSizes) {
          adSizeArrayList.Call<bool>("add", GetAdSizeJavaObject(size));
        }
        adSizeObject = adSizeArrayList;
      } else if (adSize != null) {
        adSizeObject = GetAdSizeJavaObject(adSize);
      }

      AndroidJavaObject bannerAdRequestBuilder = new AndroidJavaObject(
          BannerAdRequestBuilderClassName, adUnitId, adSizeObject);
      foreach (string keyword in request.Keywords) {
        bannerAdRequestBuilder.Call<AndroidJavaObject>("addKeyword", keyword);
      }

      foreach (KeyValuePair<string, string> entry in request.CustomTargeting) {
        bannerAdRequestBuilder.Call<AndroidJavaObject>("putCustomTargeting", entry.Key,
                                                       entry.Value);
      }
      bannerAdRequestBuilder.Call<AndroidJavaObject>("setRequestAgent",
                                                     AdRequest.BuildVersionString());

      ApplyAdManagerFields(bannerAdRequestBuilder, request);

      return bannerAdRequestBuilder.Call<AndroidJavaObject>("build");
    }

    /// <summary>
    /// Converts the plugin AdRequest object to a native java proxy object for use by the sdk.
    /// </summary>
    /// <param name="adUnitId">the ad unit ID.</param>
    /// <param name="request">the AdRequest from the unity plugin.</param>
    /// <param name="nativePluginVersion">the native plugin version.</param>
    public static AndroidJavaObject GetNativeAdRequestJavaObject(
        string adUnitId, AdRequest request, string nativePluginVersion = null) {
      return GetNativeAdRequestJavaObject(adUnitId, request, null, true, nativePluginVersion);
    }

    /// <summary>
    /// Converts the plugin AdRequest object to a native java proxy object for use by the sdk.
    /// </summary>
    /// <param name="adUnitId">the ad unit ID.</param>
    /// <param name="request">the AdRequest from the unity plugin.</param>
    /// <param name="nativeAdOptions">the C# NativeAdOptions.</param>
    /// <param name="disableImageDownloading">Whether image downloading is disabled.</param>
    /// <param name="nativePluginVersion">the native plugin version.</param>
    public static AndroidJavaObject GetNativeAdRequestJavaObject(
        string adUnitId, AdRequest request, NativeAdOptions nativeAdOptions,
        bool disableImageDownloading, string nativePluginVersion = null) {
      AndroidJavaClass nativeAdTypeClass = new AndroidJavaClass(NativeAdTypeClassName);
      AndroidJavaObject nativeTypeNative = nativeAdTypeClass.GetStatic<AndroidJavaObject>("NATIVE");

      AndroidJavaObject nativeAdTypesList = new AndroidJavaObject(ArrayListClassName);
      nativeAdTypesList.Call<bool>("add", nativeTypeNative);

      AndroidJavaObject nativeAdRequestBuilder =
          new AndroidJavaObject(NativeAdRequestBuilderClassName, adUnitId, nativeAdTypesList);

      if (disableImageDownloading) {
        nativeAdRequestBuilder.Call<AndroidJavaObject>("disableImageDownloading");
      }

      if (nativeAdOptions != null) {
        AndroidJavaClass mediaClass = new AndroidJavaClass(NativeMediaAspectRatioClassName);
        AndroidJavaObject mediaAspectRatio = null;
        switch (nativeAdOptions.MediaAspectRatio) {
          case MediaAspectRatio.Any:
            mediaAspectRatio = mediaClass.GetStatic<AndroidJavaObject>("ANY");
            break;
          case MediaAspectRatio.Landscape:
            mediaAspectRatio = mediaClass.GetStatic<AndroidJavaObject>("LANDSCAPE");
            break;
          case MediaAspectRatio.Portrait:
            mediaAspectRatio = mediaClass.GetStatic<AndroidJavaObject>("PORTRAIT");
            break;
          case MediaAspectRatio.Square:
            mediaAspectRatio = mediaClass.GetStatic<AndroidJavaObject>("SQUARE");
            break;
          case MediaAspectRatio.Unknown:
          default:
            mediaAspectRatio = mediaClass.GetStatic<AndroidJavaObject>("UNKNOWN");
            break;
        }
        if (mediaAspectRatio != null) {
          nativeAdRequestBuilder.Call<AndroidJavaObject>("setMediaAspectRatio", mediaAspectRatio);
        }

        AndroidJavaClass placementClass = new AndroidJavaClass(AdChoicesPlacementClassName);
        AndroidJavaObject adChoicesPlacement = null;
        switch (nativeAdOptions.AdChoicesPlacement) {
          case AdChoicesPlacement.TopLeftCorner:
            adChoicesPlacement = placementClass.GetStatic<AndroidJavaObject>("TOP_LEFT");
            break;
          case AdChoicesPlacement.BottomRightCorner:
            adChoicesPlacement = placementClass.GetStatic<AndroidJavaObject>("BOTTOM_RIGHT");
            break;
          case AdChoicesPlacement.BottomLeftCorner:
            adChoicesPlacement = placementClass.GetStatic<AndroidJavaObject>("BOTTOM_LEFT");
            break;
          case AdChoicesPlacement.TopRightCorner:
          default:
            adChoicesPlacement = placementClass.GetStatic<AndroidJavaObject>("TOP_RIGHT");
            break;
        }
        if (adChoicesPlacement != null) {
          nativeAdRequestBuilder.Call<AndroidJavaObject>("setAdChoicesPlacement", adChoicesPlacement);
        }

        if (nativeAdOptions.VideoOptions != null) {
          AndroidJavaObject videoOptionsBuilder = new AndroidJavaObject(VideoOptionsBuilderClassName);
          videoOptionsBuilder.Call<AndroidJavaObject>("setStartMuted", (bool)nativeAdOptions.VideoOptions.StartMuted);
          videoOptionsBuilder.Call<AndroidJavaObject>("setCustomControlsRequested", (bool)nativeAdOptions.VideoOptions.CustomControlsRequested);
          videoOptionsBuilder.Call<AndroidJavaObject>("setClickToExpandRequested", (bool)nativeAdOptions.VideoOptions.ClickToExpandRequested);
          AndroidJavaObject videoOptionsJava = videoOptionsBuilder.Call<AndroidJavaObject>("build");
          nativeAdRequestBuilder.Call<AndroidJavaObject>("setVideoOptions", videoOptionsJava);
        }
      }

      foreach (string keyword in request.Keywords) {
        nativeAdRequestBuilder.Call<AndroidJavaObject>("addKeyword", keyword);
      }

      // TODO(b/485327880): Extras are split into two different categories in the next-gen sdk. The
      // below implementation only maps to the GoogleExtras so the third-party extras should be
      // handled separately.
      if (request.Extras != null)
      {
          AndroidJavaObject bundle = new AndroidJavaObject(Utils.BundleClassName);
          foreach (KeyValuePair<string, string> entry in request.Extras)
          {
              bundle.Call("putString", entry.Key, entry.Value);
          }
          nativeAdRequestBuilder.Call<AndroidJavaObject>("setGoogleExtrasBundle", bundle);
      }

      foreach (KeyValuePair<string, string> entry in request.CustomTargeting) {
        nativeAdRequestBuilder.Call<AndroidJavaObject>("putCustomTargeting", entry.Key,
                                                       entry.Value);
      }
      nativeAdRequestBuilder.Call<AndroidJavaObject>(
          "setRequestAgent", AdRequest.BuildVersionString(nativePluginVersion));

      ApplyAdManagerFields(nativeAdRequestBuilder, request);

      return nativeAdRequestBuilder.Call<AndroidJavaObject>("build");
    }

    public static AndroidJavaObject GetAdSizeJavaObject(AdSize adSize) {
      AndroidJavaClass adSizeClass = new AndroidJavaClass(AdSizeClassName);
      AndroidJavaClass playerClass;
      AndroidJavaObject activity;
      switch (adSize.AdType) {
        case AdSize.Type.LargeAnchoredAdaptive:
          playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
          activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
          switch (adSize.Orientation) {
            case Orientation.Landscape:
              return adSizeClass.CallStatic<AndroidJavaObject>(
                  "getLargeLandscapeAnchoredAdaptiveBannerAdSize", activity, adSize.Width);
            case Orientation.Portrait:
              return adSizeClass.CallStatic<AndroidJavaObject>(
                  "getLargePortraitAnchoredAdaptiveBannerAdSize", activity, adSize.Width);
            case Orientation.Current:
              return adSizeClass.CallStatic<AndroidJavaObject>(
                  "getLargeAnchoredAdaptiveBannerAdSize", activity, adSize.Width);
            default:
              throw new ArgumentException("Invalid Orientation provided for ad size.");
          }
        case AdSize.Type.AnchoredAdaptive:
          playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
          activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
          switch (adSize.Orientation) {
            case Orientation.Landscape:
              return adSizeClass.CallStatic<AndroidJavaObject>(
                  "getLandscapeAnchoredAdaptiveBannerAdSize", activity, adSize.Width);
            case Orientation.Portrait:
              return adSizeClass.CallStatic<AndroidJavaObject>(
                  "getPortraitAnchoredAdaptiveBannerAdSize", activity, adSize.Width);
            case Orientation.Current:
              return adSizeClass.CallStatic<AndroidJavaObject>(
                  "getCurrentOrientationAnchoredAdaptiveBannerAdSize", activity, adSize.Width);
            default:
              throw new ArgumentException("Invalid Orientation provided for ad size.");
          }
        case AdSize.Type.Standard:
          return new AndroidJavaObject(AdSizeClassName, adSize.Width, adSize.Height);
        default:
          throw new ArgumentException("Invalid AdSize.Type provided for ad size.");
      }
    }

    public static AndroidJavaObject GetPreloadConfigurationJavaObject(
        PreloadConfiguration preloadConfiguration) {
      if (preloadConfiguration.AdUnitId == null) {
        throw new ArgumentNullException("PreloadConfiguration.AdUnitId");
      }
      AndroidJavaObject adRequest =
          GetAdRequestJavaObject(preloadConfiguration.Request, preloadConfiguration.AdUnitId);
      if (preloadConfiguration.BufferSize > 0) {
        return new AndroidJavaObject(NextGenUtils.PreloadConfigurationClassName, adRequest,
                                     preloadConfiguration.BufferSize);
      }
      return new AndroidJavaObject(NextGenUtils.PreloadConfigurationClassName, adRequest);
    }

    public static PreloadConfiguration GetPreloadConfiguration(
        AndroidJavaObject configurationJavaObject) {
      if (configurationJavaObject == null) {
        return null;
      }
      string adUnitId = configurationJavaObject.Call<string>("getAdUnitId");
      uint bufferSize = Convert.ToUInt32(configurationJavaObject.Call<int>("getBufferSize"));
      return new PreloadConfiguration() { AdUnitId = adUnitId, BufferSize = bufferSize };
    }

    public static AndroidJavaObject GetServerSideVerificationOptionsJavaObject(
        ServerSideVerificationOptions serverSideVerificationOptions) {
      if (serverSideVerificationOptions == null) {
        Debug.LogError("ServerSideVerificationOptions is null. Returning null.");
        return null;
      }
      return new AndroidJavaObject(NextGenUtils.ServerSideVerificationOptionsClassName,
                                     serverSideVerificationOptions.UserId ?? "",
                                     serverSideVerificationOptions.CustomData ?? "");
    }

    private static void ApplyAdManagerFields(AndroidJavaObject builder, AdRequest request) {
      AdManagerAdRequest adManagerAdRequest = request as AdManagerAdRequest;
      if (adManagerAdRequest != null) {
        if (!string.IsNullOrEmpty(adManagerAdRequest.PublisherProvidedId)) {
          builder.Call<AndroidJavaObject>("setPublisherProvidedId", adManagerAdRequest.PublisherProvidedId);
        }
        if (adManagerAdRequest.CategoryExclusions != null) {
          foreach (string exclusion in adManagerAdRequest.CategoryExclusions) {
            builder.Call<AndroidJavaObject>("addCategoryExclusion", exclusion);
          }
        }
      }
    }
  }
}
