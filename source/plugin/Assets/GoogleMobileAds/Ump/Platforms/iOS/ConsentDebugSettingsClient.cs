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
    public class ConsentDebugSettingsClient

    {
        private static IntPtr consentDebugSettingsPtr = Externs.GADUCreateConsentDebugSettings();

        public static void SetConsentDebugSettings(ConsentDebugSettings consentDebugSettings)
        {

            if (consentDebugSettings.MaxAdContentRating != null)
            {
                Externs.GADUSetConsentDebugSettingsMaxAdContentRating(consentDebugSettingsPtr, consentDebugSettings.MaxAdContentRating.Value);
            }

            if (consentDebugSettings.TestDeviceIds.Count > 0)
            {
                string[] testDeviceIdsArray = new string[consentDebugSettings.TestDeviceIds.Count];
                consentDebugSettings.TestDeviceIds.CopyTo(testDeviceIdsArray);
                Externs.GADUSetConsentDebugSettingsTestDeviceIdentifiers(consentDebugSettingsPtr, testDeviceIdsArray, consentDebugSettings.TestDeviceIds.Count);
            }
            if (consentDebugSettings.TagForChildDirectedTreatment.HasValue)
            {

                TagForChildDirectedTreatment? tagForChildDirectedTreatment = consentDebugSettings.TagForChildDirectedTreatment;
                Externs.GADUSetConsentDebugSettingsTagForChildDirectedTreatment(consentDebugSettingsPtr, (int)tagForChildDirectedTreatment.GetValueOrDefault());
            }

            if (consentDebugSettings.TagForUnderAgeOfConsent.HasValue)
            {
                TagForUnderAgeOfConsent? TagForUnderAgeOfConsent = consentDebugSettings.TagForUnderAgeOfConsent;
                Externs.GADUSetConsentDebugSettingsTagForUnderAgeOfConsent(consentDebugSettingsPtr, (int)TagForUnderAgeOfConsent.GetValueOrDefault());
            }

            if (consentDebugSettings.SameAppKeyEnabled.HasValue) {
              Externs.GADUSetConsentDebugSettingsSameAppKeyEnabled(
                  consentDebugSettingsPtr, consentDebugSettings.SameAppKeyEnabled.Value);
            }

            Externs.GADUSetConsentDebugSettings(consentDebugSettingsPtr);

        }

        public static ConsentDebugSettings GetConsentDebugSettings()
        {
            ConsentDebugSettings.Builder consentDebugSettingsBuilder = new ConsentDebugSettings.Builder();
            MaxAdContentRating maxAdContentRating = MaxAdContentRating.ToMaxAdContentRating(Externs.GADUGetMaxAdContentRating(consentDebugSettingsPtr));
            IntPtr testDeviceIdsArray = Externs.GADUGetTestDeviceIdentifiers(consentDebugSettingsPtr);
            List<string> testDeviceIds = Utils.PtrArrayToManagedList(testDeviceIdsArray, Externs.GADUGetTestDeviceIdentifiersCount(consentDebugSettingsPtr));

            TagForChildDirectedTreatment TagForChildDirectedTreatment = (TagForChildDirectedTreatment)Externs.GADUGetConsentDebugSettingsTagForChildDirectedTreatment(consentDebugSettingsPtr);
            TagForUnderAgeOfConsent TagForUnderAgeOfConsent = (TagForUnderAgeOfConsent)Externs.GADUGetConsentDebugSettingsTagForUnderAgeOfConsent(consentDebugSettingsPtr);

            bool sameAppKeyEnabled =
                Externs.GADUGetConsentDebugSettingsSameAppKeyEnabled(consentDebugSettingsPtr);

            consentDebugSettingsBuilder.SetMaxAdContentRating(maxAdContentRating);
            consentDebugSettingsBuilder.SetTestDeviceIds(testDeviceIds);
            consentDebugSettingsBuilder.SetTagForChildDirectedTreatment(TagForChildDirectedTreatment);
            consentDebugSettingsBuilder.SetTagForUnderAgeOfConsent(TagForUnderAgeOfConsent);
            consentDebugSettingsBuilder.SetSameAppKeyEnabled(sameAppKeyEnabled);

            return consentDebugSettingsBuilder.build();
        }

    }

}
#endif
