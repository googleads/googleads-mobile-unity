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
                bool? tagForChildDirectedTreatmentValue = null;
                switch (requestConfiguration.TagForChildDirectedTreatment.GetValueOrDefault())
                {
                    case Api.TagForChildDirectedTreatment.False:
                        tagForChildDirectedTreatmentValue = false;
                        break;
                    case Api.TagForChildDirectedTreatment.True:
                        tagForChildDirectedTreatmentValue = true;
                        break;
                    case Api.TagForChildDirectedTreatment.Unspecified:
                        tagForChildDirectedTreatmentValue = null;
                        break;
                }
                Externs.GADUSetRequestConfigurationTagForChildDirectedTreatment(requestConfigurationPtr, tagForChildDirectedTreatmentValue.GetValueOrDefault());
            }


            if (requestConfiguration.TagForUnderAgeOfConsent.HasValue)
            {
                bool? tagForUnderAgeOfConsentValue = null;
                switch (requestConfiguration.TagForUnderAgeOfConsent.GetValueOrDefault())
                {
                    case Api.TagForUnderAgeOfConsent.False:
                        tagForUnderAgeOfConsentValue = false;
                        break;
                    case Api.TagForUnderAgeOfConsent.True:
                        tagForUnderAgeOfConsentValue = true;
                        break;
                    case Api.TagForUnderAgeOfConsent.Unspecified:
                        tagForUnderAgeOfConsentValue = null;
                        break;
                }
                Externs.GADUSetRequestConfigurationTagForUnderAgeOfConsent(requestConfigurationPtr, tagForUnderAgeOfConsentValue.GetValueOrDefault());
            }
            Externs.GADUSetRequestConfiguration(requestConfigurationPtr);

        }
        public static RequestConfiguration GetRequestConfiguration()
        {
            RequestConfiguration.Builder requestConfigurationBuilder = new RequestConfiguration.Builder();
            MaxAdContentRating maxAdContentRating = MaxAdContentRating.ToMaxAdContentRating(Externs.GADUGetMaxAdContentRating(requestConfigurationPtr));
            IntPtr testDeviceIdsArray = Externs.GADUGetTestDeviceIdentifiers(requestConfigurationPtr);
            List<string> testDeviceIds = Utils.PtrArrayToManagedList(testDeviceIdsArray, Externs.GADUGetTestDeviceIdentifiersCount(requestConfigurationPtr));

            TagForChildDirectedTreatment? TagForChildDirectedTreatment = null;
            bool? tagForChildDirectedTreatmentValue = Externs.GADUGetRequestConfigurationtagTagForChildDirectedTreatment(requestConfigurationPtr);
            switch (tagForChildDirectedTreatmentValue)
            {
                case true:
                    TagForChildDirectedTreatment = Api.TagForChildDirectedTreatment.True;
                    break;
                case false:
                    TagForChildDirectedTreatment = Api.TagForChildDirectedTreatment.False;
                    break;
                case null:
                    TagForChildDirectedTreatment = Api.TagForChildDirectedTreatment.Unspecified;
                    break;

            }
            TagForUnderAgeOfConsent? TagForUnderAgeOfConsent = null;
            bool? tagForUnderAgeOfConsentValue = Externs.GADUGetRequestConfigurationTagForUnderAgeOfConsent(requestConfigurationPtr);
            switch (tagForUnderAgeOfConsentValue)
            {
                case true:
                    TagForUnderAgeOfConsent = Api.TagForUnderAgeOfConsent.True;
                    break;
                case false:
                    TagForUnderAgeOfConsent = Api.TagForUnderAgeOfConsent.False;
                    break;
                case null:
                    TagForUnderAgeOfConsent = Api.TagForUnderAgeOfConsent.Unspecified;
                    break;

            }

            requestConfigurationBuilder.SetMaxAdContentRating(maxAdContentRating);
            requestConfigurationBuilder.SetTestDeviceIds(testDeviceIds);
            requestConfigurationBuilder.SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.GetValueOrDefault());
            requestConfigurationBuilder.SetTagForUnderAgeOfConsent(TagForUnderAgeOfConsent.GetValueOrDefault());
            return requestConfigurationBuilder.build();
        }

    }

}
