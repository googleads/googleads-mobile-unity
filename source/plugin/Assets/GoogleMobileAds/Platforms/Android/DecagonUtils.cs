
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

namespace GoogleMobileAds.Android {
  internal class DecagonUtils {

#region Fully - qualified Decagon Mobile Ads SDK class names

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
#endregion

#endregion

#region Fully - qualified Unity Decagon Bridge class names

    public const string UnityMobileAdsClassName = "com.google.unity.ads.decagon.UnityMobileAds";
    public const string UnityAdInspectorClassName = "com.google.unity.ads.decagon.UnityAdInspector";
    public const string UnityAdInspectorListenerClassName =
        "com.google.unity.ads.decagon.UnityAdInspectorListener";

    public const string UnityAppOpenAdClassName = "com.google.unity.ads.decagon.UnityAppOpenAd";
    public const string UnityAppOpenAdCallbackClassName =
        "com.google.unity.ads.decagon.UnityAppOpenAdCallback";

    public const string UnityInterstitialAdClassName =
        "com.google.unity.ads.decagon.UnityInterstitialAd";
    public const string UnityInterstitialAdCallbackClassName =
        "com.google.unity.ads.decagon.UnityInterstitialAdCallback";

    public const string UnityRewardedAdClassName = "com.google.unity.ads.decagon.UnityRewardedAd";
    public const string UnityRewardedAdCallbackClassName =
        "com.google.unity.ads.decagon.UnityRewardedAdCallback";

    public const string UnityRewardedInterstitialAdClassName =
        "com.google.unity.ads.decagon.UnityRewardedInterstitialAd";
    public const string UnityRewardedInterstitialAdCallbackClassName =
        "com.google.unity.ads.decagon.UnityRewardedInterstitialAdCallback";
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

      return adRequestBuilder.Call<AndroidJavaObject>("build");
    }
  }
}
