// Copyright (C) 2020 Google, Inc.
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
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
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

            return requestConfigurationBuilder.Call<AndroidJavaObject>("build");

        }
        public static RequestConfiguration GetRequestConfiguration(AndroidJavaObject androidRequestConfiguration)
        {

            TagForChildDirectedTreatment TagForChildDirectedTreatment = (TagForChildDirectedTreatment)androidRequestConfiguration.Call<int>("getTagForChildDirectedTreatment");

            TagForUnderAgeOfConsent TagForUnderAgeOfConsent = (TagForUnderAgeOfConsent)androidRequestConfiguration.Call<int>("getTagForUnderAgeOfConsent");

            MaxAdContentRating maxAdContentRating = MaxAdContentRating.ToMaxAdContentRating(androidRequestConfiguration.Call<string>("getMaxAdContentRating"));
            List<string> TestDeviceIds = Utils.GetCsTypeList(androidRequestConfiguration.Call<AndroidJavaObject>("getTestDeviceIds"));

            RequestConfiguration.Builder builder = new RequestConfiguration.Builder();
            builder = builder.SetTagForChildDirectedTreatment(TagForChildDirectedTreatment);
            builder = builder.SetTagForUnderAgeOfConsent(TagForUnderAgeOfConsent);

            builder = builder.SetMaxAdContentRating(maxAdContentRating);
            builder = builder.SetTestDeviceIds(TestDeviceIds);

            return builder.build();
        }

    }

}
