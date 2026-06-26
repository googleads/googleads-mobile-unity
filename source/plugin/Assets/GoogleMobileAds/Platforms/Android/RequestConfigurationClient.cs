// Copyright (C) 2020 Google LLC
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

namespace GoogleMobileAds.Android
{
    public class RequestConfigurationClient
    {

        public static AndroidJavaObject BuildRequestConfiguration(RequestConfiguration requestConfiguration)
        {

            AndroidJavaObject requestConfigurationBuilder = new AndroidJavaObject(Utils.RequestConfigurationBuilderClassName);
            if (requestConfiguration.MaxAdContentRating != null)
            {
                requestConfigurationBuilder =
                      requestConfigurationBuilder.Call<AndroidJavaObject>("setMaxAdContentRating",
                      requestConfiguration.MaxAdContentRating.Value);
            }
            if (requestConfiguration.TestDeviceIds.Count > 0)
            {
                AndroidJavaObject testDeviceIdsJavaObject = Utils.GetJavaListObject(requestConfiguration.TestDeviceIds);
                requestConfigurationBuilder =
                    requestConfigurationBuilder.Call<AndroidJavaObject>("setTestDeviceIds",
                    testDeviceIdsJavaObject);
            }
#pragma warning disable 618
            if (requestConfiguration.TagForUnderAgeOfConsent.HasValue)
            {
                int? tagForUnderAgeOfConsentCode = null;
                switch (requestConfiguration.TagForUnderAgeOfConsent.GetValueOrDefault())
                {
                    case Api.TagForUnderAgeOfConsent.False:
                        tagForUnderAgeOfConsentCode = new AndroidJavaClass(Utils.RequestConfigurationClassName)
                                .GetStatic<int>("TAG_FOR_UNDER_AGE_OF_CONSENT_FALSE");
                        break;
                    case Api.TagForUnderAgeOfConsent.True:
                        tagForUnderAgeOfConsentCode = new AndroidJavaClass(Utils.RequestConfigurationClassName)
                                .GetStatic<int>("TAG_FOR_UNDER_AGE_OF_CONSENT_TRUE");
                        break;
                    case Api.TagForUnderAgeOfConsent.Unspecified:
                        tagForUnderAgeOfConsentCode = new AndroidJavaClass(Utils.RequestConfigurationClassName)
                                .GetStatic<int>("TAG_FOR_UNDER_AGE_OF_CONSENT_UNSPECIFIED");
                        break;
                }
                // Unity 2019.2 has a bug where calling AndroidJavaClass.GetStatic
                // returns null, hence we assign the value directly (they are the same).
#if UNITY_2019_2
                tagForUnderAgeOfConsentCode = (int)requestConfiguration.TagForUnderAgeOfConsent.GetValueOrDefault();
#endif

                if (tagForUnderAgeOfConsentCode.HasValue)
                {
                    requestConfigurationBuilder.Call<AndroidJavaObject>("setTagForUnderAgeOfConsent",
                         tagForUnderAgeOfConsentCode);
                }
            }
            if (requestConfiguration.TagForChildDirectedTreatment.HasValue)
            {
                int? tagForChildDirectedTreatmentCode = null;
                switch (requestConfiguration.TagForChildDirectedTreatment.GetValueOrDefault())
                {
                    case Api.TagForChildDirectedTreatment.False:
                        tagForChildDirectedTreatmentCode = new AndroidJavaClass(Utils.RequestConfigurationClassName)
                                .GetStatic<int>("TAG_FOR_CHILD_DIRECTED_TREATMENT_FALSE");
                        break;
                    case Api.TagForChildDirectedTreatment.True:
                        tagForChildDirectedTreatmentCode = new AndroidJavaClass(Utils.RequestConfigurationClassName)
                                .GetStatic<int>("TAG_FOR_UNDER_AGE_OF_CONSENT_TRUE");
                        break;
                    case Api.TagForChildDirectedTreatment.Unspecified:
                        tagForChildDirectedTreatmentCode = new AndroidJavaClass(Utils.RequestConfigurationClassName)
                                .GetStatic<int>("TAG_FOR_UNDER_AGE_OF_CONSENT_UNSPECIFIED");
                        break;
                }
                // Unity 2019.2 has a bug where calling AndroidJavaClass.GetStatic
                // returns null, hence we assign the value directly (they are the same).
#if UNITY_2019_2
                tagForChildDirectedTreatmentCode = (int)requestConfiguration.TagForChildDirectedTreatment.GetValueOrDefault();
#endif

                if (tagForChildDirectedTreatmentCode.HasValue)
                {
                    requestConfigurationBuilder.Call<AndroidJavaObject>("setTagForChildDirectedTreatment",
                         tagForChildDirectedTreatmentCode);
                }
            }
#pragma warning restore 618
            if (requestConfiguration.AgeRestrictedTreatment.HasValue)
            {
                AndroidJavaObject ageRestrictedTreatmentCode = null;
                switch (requestConfiguration.AgeRestrictedTreatment.GetValueOrDefault())
                {
                    case Api.AgeRestrictedTreatment.Child:
                        ageRestrictedTreatmentCode = new AndroidJavaClass("com.google.android.gms.ads.AgeRestrictedTreatment")
                                .GetStatic<AndroidJavaObject>("CHILD");
                        break;
                    case Api.AgeRestrictedTreatment.Teen:
                        ageRestrictedTreatmentCode = new AndroidJavaClass("com.google.android.gms.ads.AgeRestrictedTreatment")
                                .GetStatic<AndroidJavaObject>("TEEN");
                        break;
                    case Api.AgeRestrictedTreatment.Unspecified:
                        ageRestrictedTreatmentCode = new AndroidJavaClass("com.google.android.gms.ads.AgeRestrictedTreatment")
                                .GetStatic<AndroidJavaObject>("UNSPECIFIED");
                        break;
                }
                if (ageRestrictedTreatmentCode != null)
                {
                    requestConfigurationBuilder = requestConfigurationBuilder.Call<AndroidJavaObject>("setAgeRestrictedTreatment",
                         ageRestrictedTreatmentCode);
                }
            }
            if (requestConfiguration.PublisherPrivacyPersonalizationState.HasValue)
            {
                AndroidJavaObject personalizationState = null;
                switch (requestConfiguration.PublisherPrivacyPersonalizationState.GetValueOrDefault())
                {
                    case Api.PublisherPrivacyPersonalizationState.Default:
                      personalizationState =
                          new AndroidJavaClass(Utils.PublisherPrivacyPersonalizationStateEnumName)
                              .GetStatic<AndroidJavaObject>("DEFAULT");
                      break;
                    case Api.PublisherPrivacyPersonalizationState.Disabled:
                        personalizationState =
                          new AndroidJavaClass(Utils.PublisherPrivacyPersonalizationStateEnumName)
                              .GetStatic<AndroidJavaObject>("DISABLED");
                        break;
                    case Api.PublisherPrivacyPersonalizationState.Enabled:
                        personalizationState =
                          new AndroidJavaClass(Utils.PublisherPrivacyPersonalizationStateEnumName)
                              .GetStatic<AndroidJavaObject>("ENABLED");
                        break;
                }

                if (personalizationState != null)
                {
                    requestConfigurationBuilder.Call<AndroidJavaObject>(
                      "setPublisherPrivacyPersonalizationState", personalizationState);
                }
            }
            return requestConfigurationBuilder.Call<AndroidJavaObject>("build");
        }

        public static RequestConfiguration GetRequestConfiguration(AndroidJavaObject androidRequestConfiguration)
        {

#pragma warning disable 618
            TagForChildDirectedTreatment tagForChildDirectedTreatment = (TagForChildDirectedTreatment)androidRequestConfiguration.Call<int>("getTagForChildDirectedTreatment");

            TagForUnderAgeOfConsent tagForUnderAgeOfConsent = (TagForUnderAgeOfConsent)androidRequestConfiguration.Call<int>("getTagForUnderAgeOfConsent");
#pragma warning restore 618

            AndroidJavaObject ageRestrictedTreatmentEnum = androidRequestConfiguration.Call<AndroidJavaObject>("getAgeRestrictedTreatment");
            AgeRestrictedTreatment ageRestrictedTreatment = AgeRestrictedTreatment.Unspecified;
            if (ageRestrictedTreatmentEnum != null)
            {
                int value = ageRestrictedTreatmentEnum.Call<int>("getValue");
                ageRestrictedTreatment = (AgeRestrictedTreatment)value;
            }

            MaxAdContentRating maxAdContentRating = MaxAdContentRating.ToMaxAdContentRating(androidRequestConfiguration.Call<string>("getMaxAdContentRating"));
            List<string> testDeviceIds = Utils.GetCsTypeList(androidRequestConfiguration.Call<AndroidJavaObject>("getTestDeviceIds"));

            // TODO(@vkini): We should ideally read this value from Unity Java Bridge code.
            // Currently we expect the integer values to stay the same but this might not always be
            // the case. Other option is to compare enums with returned value to get correct value.
            AndroidJavaObject publisherPrivacyPersonalizationStateEnum =
                androidRequestConfiguration.Call<AndroidJavaObject>(
                    "getPublisherPrivacyPersonalizationState");
            PublisherPrivacyPersonalizationState publisherPrivacyPersonalizationState =
                (PublisherPrivacyPersonalizationState)
                    publisherPrivacyPersonalizationStateEnum.Call<int>("ordinal");

            RequestConfiguration requestConfiguration = new RequestConfiguration()
            {
                MaxAdContentRating = maxAdContentRating,
#pragma warning disable 618
                TagForChildDirectedTreatment = tagForChildDirectedTreatment,
                TagForUnderAgeOfConsent = tagForUnderAgeOfConsent,
#pragma warning restore 618
                AgeRestrictedTreatment = ageRestrictedTreatment,
                TestDeviceIds = testDeviceIds,
                PublisherPrivacyPersonalizationState = publisherPrivacyPersonalizationState
            };

            return requestConfiguration;
        }

    }

}
