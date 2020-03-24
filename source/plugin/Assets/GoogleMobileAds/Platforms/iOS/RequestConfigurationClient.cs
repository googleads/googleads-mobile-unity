// Copyright (C) 2017 Google, Inc.
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
        public static void SetRequestConfiguration(RequestConfiguration requestConfiguration)
        {
            IntPtr requestConfigurationPtr = Externs.GADUCreateRequestConfiguration();
            bool? tagForChildDirectedTreatmentValue = null;
            bool? tagForUnderAgeOfConsentValue = null;
            if (requestConfiguration.MaxAdContentRating != null)
            {
                Externs.GADUSetRequestConfigurationMaxAdContentRating(requestConfigurationPtr, requestConfiguration.MaxAdContentRating.Value);
            }
            if (requestConfiguration.TestDeviceIds.Count > 0)
            {
                Externs.GADUSetRequestConfigurationTestDeviceIdentifiers(requestConfigurationPtr, requestConfiguration.TestDeviceIds.ToArray(), requestConfiguration.TestDeviceIds.Count);
            }
            if (requestConfiguration.TagForChildDirectedTreatment.HasValue)
            {
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
            }

            if (requestConfiguration.TagForUnderAgeOfConsent.HasValue)
            {
            
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
            }
            Externs.GADUSetRequestConfiguration(requestConfigurationPtr, tagForChildDirectedTreatmentValue.GetValueOrDefault(), tagForUnderAgeOfConsentValue.GetValueOrDefault());

        }
    public static RequestConfiguration GetRequestConfiguration(){
        IntPtr requestConfigurationPtr = Externs.GADUGetRequestConfiguration();
        RequestConfiguration.Builder requestConfigurationBuilder = new RequestConfiguration.Builder();
        MaxAdContentRating maxAdContentRating = MaxAdContentRating.ToMaxAdContentRating(Externs.GADUGetMaxAdContentRating(requestConfigurationPtr));
        string [] testDeviceIdsArray = Externs.GADUGetTestDeviceIdentifiers(requestConfigurationPtr);
        List<String> testDeviceIds = testDeviceIdsArray.OfType<string>().ToList();
        requestConfigurationBuilder.SetMaxAdContentRating(maxAdContentRating);
        requestConfigurationBuilder.SetTestDeviceIds(testDeviceIds);
        return requestConfigurationBuilder.build();
    }
    

  
 }

}