#if UNITY_IOS
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

using System;
using System.Runtime.InteropServices;
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System.Linq;
using System.Collections.Generic;

namespace GoogleMobileAds.iOS
{
    public class RequestConfigurationClient

    {
        private static IntPtr requestConfigurationPtr = Externs.GADUCreateRequestConfiguration();

        public static void SetRequestConfiguration(RequestConfiguration requestConfiguration)
        {

            if (requestConfiguration.MaxAdContentRating != null)
            {
                Externs.GADUSetRequestConfigurationMaxAdContentRating(requestConfigurationPtr, requestConfiguration.MaxAdContentRating.Value);
            }

            if (requestConfiguration.TestDeviceIds.Count > 0)
            {
                string[] testDeviceIdsArray = new string[requestConfiguration.TestDeviceIds.Count];
                requestConfiguration.TestDeviceIds.CopyTo(testDeviceIdsArray);
                Externs.GADUSetRequestConfigurationTestDeviceIdentifiers(requestConfigurationPtr, testDeviceIdsArray, requestConfiguration.TestDeviceIds.Count);
            }
            if (requestConfiguration.TagForChildDirectedTreatment.HasValue)
            {
                TagForChildDirectedTreatment? tagForChildDirectedTreatment = requestConfiguration.TagForChildDirectedTreatment;
                Externs.GADUSetRequestConfigurationTagForChildDirectedTreatment(requestConfigurationPtr, (int)tagForChildDirectedTreatment.GetValueOrDefault());
            }

            if (requestConfiguration.TagForUnderAgeOfConsent.HasValue)
            {
                TagForUnderAgeOfConsent? TagForUnderAgeOfConsent = requestConfiguration.TagForUnderAgeOfConsent;
                Externs.GADUSetRequestConfigurationTagForUnderAgeOfConsent(requestConfigurationPtr, (int)TagForUnderAgeOfConsent.GetValueOrDefault());
            }

            if (requestConfiguration.PublisherFirstPartyIdEnabled.HasValue) {
                Externs.GADUSetRequestConfigurationPublisherFirstPartyIDEnabled(
                    requestConfiguration.PublisherFirstPartyIdEnabled.Value);
            }

            if (requestConfiguration.PublisherPrivacyPersonalizationState.HasValue)
            {
              Externs.GADUSetRequestConfigurationPublisherPrivacyPersonalizationState(
                  (int)requestConfiguration.PublisherPrivacyPersonalizationState
                      .GetValueOrDefault());
            }

            Externs.GADUSetRequestConfiguration(requestConfigurationPtr);

        }

        public static RequestConfiguration GetRequestConfiguration()
        {
            MaxAdContentRating maxAdContentRating = MaxAdContentRating.ToMaxAdContentRating(Externs.GADUGetMaxAdContentRating(requestConfigurationPtr));
            IntPtr testDeviceIdsArray = Externs.GADUGetTestDeviceIdentifiers(requestConfigurationPtr);
            List<string> testDeviceIds = Utils.PtrArrayToManagedList(testDeviceIdsArray, Externs.GADUGetTestDeviceIdentifiersCount(requestConfigurationPtr));

            TagForChildDirectedTreatment tagForChildDirectedTreatment = (TagForChildDirectedTreatment)Externs.GADUGetRequestConfigurationTagForChildDirectedTreatment(requestConfigurationPtr);
            TagForUnderAgeOfConsent tagForUnderAgeOfConsent = (TagForUnderAgeOfConsent)Externs.GADUGetRequestConfigurationTagForUnderAgeOfConsent(requestConfigurationPtr);
            PublisherPrivacyPersonalizationState publisherPrivacyPersonalizationState =
                (PublisherPrivacyPersonalizationState)Externs.GADUGetRequestConfigurationPublisherPrivacyPersonalizationState();

            RequestConfiguration requestConfiguration = new RequestConfiguration()
            {
                MaxAdContentRating = maxAdContentRating,
                TagForChildDirectedTreatment = tagForChildDirectedTreatment,
                TagForUnderAgeOfConsent = tagForUnderAgeOfConsent,
                TestDeviceIds = testDeviceIds,
                PublisherPrivacyPersonalizationState = publisherPrivacyPersonalizationState
            };
            return requestConfiguration;
        }

    }

}
#endif
