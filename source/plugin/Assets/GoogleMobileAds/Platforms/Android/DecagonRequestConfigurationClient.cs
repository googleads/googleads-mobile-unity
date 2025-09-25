// Copyright (C) 2025 Google LLC
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

using GoogleMobileAds.Api;
using System.Collections.Generic;

namespace GoogleMobileAds.Android {
  public class DecagonRequestConfigurationClient {
    public static AndroidJavaObject BuildRequestConfiguration(
        RequestConfiguration requestConfiguration) {
      AndroidJavaObject requestConfigurationBuilder =
          new AndroidJavaObject(DecagonUtils.RequestConfigurationBuilderClassName);
      if (requestConfiguration.MaxAdContentRating != null) {
        AndroidJavaObject maxAdContentRating = null;
        string ratingValue = requestConfiguration.MaxAdContentRating.Value;
        if (ratingValue == Api.MaxAdContentRating.G.Value) {
          maxAdContentRating =
              new AndroidJavaClass(DecagonUtils.MaxAdContentRatingClassName)
                  .GetStatic<AndroidJavaObject>("MAX_AD_CONTENT_RATING_G");
        } else if (ratingValue == Api.MaxAdContentRating.PG.Value) {
          maxAdContentRating =
              new AndroidJavaClass(DecagonUtils.MaxAdContentRatingClassName)
                  .GetStatic<AndroidJavaObject>("MAX_AD_CONTENT_RATING_PG");
        } else if (ratingValue == Api.MaxAdContentRating.T.Value) {
          maxAdContentRating =
              new AndroidJavaClass(DecagonUtils.MaxAdContentRatingClassName)
                  .GetStatic<AndroidJavaObject>("MAX_AD_CONTENT_RATING_T");
        } else if (ratingValue == Api.MaxAdContentRating.MA.Value) {
          maxAdContentRating =
              new AndroidJavaClass(DecagonUtils.MaxAdContentRatingClassName)
                  .GetStatic<AndroidJavaObject>("MAX_AD_CONTENT_RATING_MA");
        } else {
          maxAdContentRating =
              new AndroidJavaClass(DecagonUtils.MaxAdContentRatingClassName)
                  .GetStatic<AndroidJavaObject>("MAX_AD_CONTENT_RATING_UNSPECIFIED");
        }

        requestConfigurationBuilder.Call<AndroidJavaObject>("setMaxAdContentRating",
                                                            maxAdContentRating);
      }

      if (requestConfiguration.TestDeviceIds.Count > 0) {
        AndroidJavaObject testDeviceIdsJavaObject =
            Utils.GetJavaListObject(requestConfiguration.TestDeviceIds);
        requestConfigurationBuilder = requestConfigurationBuilder.Call<AndroidJavaObject>(
            "setTestDeviceIds", testDeviceIdsJavaObject);
      }

      if (requestConfiguration.TagForUnderAgeOfConsent.HasValue) {
        AndroidJavaObject tagForUnderAgeOfConsentCode = null;
        switch (requestConfiguration.TagForUnderAgeOfConsent.GetValueOrDefault()) {
          case Api.TagForUnderAgeOfConsent.False:
            tagForUnderAgeOfConsentCode =
                new AndroidJavaClass(
                    DecagonUtils.TagForUnderAgeOfConsentClassName)
                    .GetStatic<AndroidJavaObject>("TAG_FOR_UNDER_AGE_OF_CONSENT_FALSE");
            break;
          case Api.TagForUnderAgeOfConsent.True:
            tagForUnderAgeOfConsentCode =
                new AndroidJavaClass(
                    DecagonUtils.TagForUnderAgeOfConsentClassName)
                    .GetStatic<AndroidJavaObject>("TAG_FOR_UNDER_AGE_OF_CONSENT_TRUE");
            break;
          case Api.TagForUnderAgeOfConsent.Unspecified:
            tagForUnderAgeOfConsentCode =
                new AndroidJavaClass(
                    DecagonUtils.TagForUnderAgeOfConsentClassName)
                    .GetStatic<AndroidJavaObject>("TAG_FOR_UNDER_AGE_OF_CONSENT_UNSPECIFIED");
            break;
        }

        if (tagForUnderAgeOfConsentCode != null) {
          requestConfigurationBuilder.Call<AndroidJavaObject>("setTagForUnderAgeOfConsent",
                                                              tagForUnderAgeOfConsentCode);
        }
      }
      if (requestConfiguration.TagForChildDirectedTreatment.HasValue) {
        AndroidJavaObject tagForChildDirectedTreatmentCode = null;
        switch (requestConfiguration.TagForChildDirectedTreatment.GetValueOrDefault()) {
          case Api.TagForChildDirectedTreatment.False:
            tagForChildDirectedTreatmentCode =
                new AndroidJavaClass(
                    DecagonUtils.TagForChildDirectedTreatmentClassName)
                    .GetStatic<AndroidJavaObject>("TAG_FOR_CHILD_DIRECTED_TREATMENT_FALSE");
            break;
          case Api.TagForChildDirectedTreatment.True:
            tagForChildDirectedTreatmentCode =
                new AndroidJavaClass(
                    DecagonUtils.TagForChildDirectedTreatmentClassName)
                    .GetStatic<AndroidJavaObject>("TAG_FOR_CHILD_DIRECTED_TREATMENT_TRUE");
            break;
          case Api.TagForChildDirectedTreatment.Unspecified:
            tagForChildDirectedTreatmentCode =
                new AndroidJavaClass(
                    DecagonUtils.TagForChildDirectedTreatmentClassName)
                    .GetStatic<AndroidJavaObject>("TAG_FOR_CHILD_DIRECTED_TREATMENT_UNSPECIFIED");
            break;
        }

        if (tagForChildDirectedTreatmentCode != null) {
          requestConfigurationBuilder.Call<AndroidJavaObject>("setTagForChildDirectedTreatment",
                                                              tagForChildDirectedTreatmentCode);
        }
      }
      if (requestConfiguration.PublisherPrivacyPersonalizationState.HasValue) {
        AndroidJavaObject personalizationState = null;
        switch (requestConfiguration.PublisherPrivacyPersonalizationState.GetValueOrDefault()) {
          case Api.PublisherPrivacyPersonalizationState.Default:
            personalizationState =
                new AndroidJavaClass(DecagonUtils.PublisherPrivacyPersonalizationStateEnumName)
                    .GetStatic<AndroidJavaObject>("DEFAULT");
            break;
          case Api.PublisherPrivacyPersonalizationState.Disabled:
            personalizationState =
                new AndroidJavaClass(DecagonUtils.PublisherPrivacyPersonalizationStateEnumName)
                    .GetStatic<AndroidJavaObject>("DISABLED");
            break;
          case Api.PublisherPrivacyPersonalizationState.Enabled:
            personalizationState =
                new AndroidJavaClass(DecagonUtils.PublisherPrivacyPersonalizationStateEnumName)
                    .GetStatic<AndroidJavaObject>("ENABLED");
            break;
        }

        if (personalizationState != null) {
          requestConfigurationBuilder.Call<AndroidJavaObject>(
              "setPublisherPrivacyPersonalizationState", personalizationState);
        }
      }
      return requestConfigurationBuilder.Call<AndroidJavaObject>("build");
    }

    public static RequestConfiguration GetRequestConfiguration(
        AndroidJavaObject androidRequestConfiguration) {
      TagForChildDirectedTreatment tagForChildDirectedTreatment =
          (TagForChildDirectedTreatment)androidRequestConfiguration.Call<int>(
              "getTagForChildDirectedTreatment");

      TagForUnderAgeOfConsent tagForUnderAgeOfConsent =
          (TagForUnderAgeOfConsent)androidRequestConfiguration.Call<int>(
              "getTagForUnderAgeOfConsent");

      MaxAdContentRating maxAdContentRating = MaxAdContentRating.ToMaxAdContentRating(
          androidRequestConfiguration.Call<string>("getMaxAdContentRating"));
      List<string> testDeviceIds = GoogleMobileAds.Android.Utils.GetCsTypeList(
          androidRequestConfiguration.Call<AndroidJavaObject>("getTestDeviceIds"));

      AndroidJavaObject publisherPrivacyPersonalizationStateEnum =
          androidRequestConfiguration.Call<AndroidJavaObject>(
              "getPublisherPrivacyPersonalizationState");
      PublisherPrivacyPersonalizationState publisherPrivacyPersonalizationState =
          (PublisherPrivacyPersonalizationState)publisherPrivacyPersonalizationStateEnum.Call<int>(
              "ordinal");

      RequestConfiguration requestConfiguration = new RequestConfiguration() {
        MaxAdContentRating = maxAdContentRating,
        TagForChildDirectedTreatment = tagForChildDirectedTreatment,
        TagForUnderAgeOfConsent = tagForUnderAgeOfConsent, TestDeviceIds = testDeviceIds,
        PublisherPrivacyPersonalizationState = publisherPrivacyPersonalizationState
      };

      return requestConfiguration;
    }
  }

}
