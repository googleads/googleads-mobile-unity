#if UNITY_IOS

// Copyright (C) 2022 Google LLC.
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

namespace GoogleMobileAds.Ump.iOS
{
    // Externs used by the iOS component.
    internal class Externs
    {
        #region Consent Form externs

        [DllImport("__Internal")]
        internal static extern int GADUGetFormErrorCode(IntPtr error);

        [DllImport("__Internal")]
        internal static extern string GADUGetFormErrorMessage(IntPtr error);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateRequestParameters();

        [DllImport("__Internal")]
        internal static extern void GADUSetRequestParametersTagForUnderAgeOfConsent(
                IntPtr requestParametersRef, bool tagForUnderAgeOfConsent);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateDebugSettings();

        [DllImport("__Internal")]
        internal static extern void GADUSetDebugSettingsDebugGeography(
            IntPtr debugSettingsRef, int debugGeography);

        [DllImport("__Internal")]
        internal static extern void GADUSetDebugSettingsTestDeviceIdentifiers(
            IntPtr debugSettingsRef, string[] testDeviceIDs, int testDeviceIDLength);

        [DllImport("__Internal")]
        internal static extern void GADUSetRequestParametersDebugSettings(
                IntPtr requestParametersRef, IntPtr debugSettingsRef);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateConsentInformation(IntPtr clientRef);

        [DllImport("__Internal")]
        internal static extern void GADUResetConsentInformation(IntPtr consentInfoRef);

        [DllImport("__Internal")]
        internal static extern int GADUGetConsentStatus(IntPtr consentInfoRef);

        [DllImport("__Internal")]
        internal static extern int GADUGetPrivacyOptionsRequirementStatus(IntPtr consentInfoRef);

        [DllImport("__Internal")]
        internal static extern bool GADUUMPCanRequestAds(IntPtr consentInfoRef);

        [DllImport("__Internal")]
        internal static extern bool GADUIsConsentFormAvailable(IntPtr consentInfoRef);

        [DllImport("__Internal")]
        internal static extern void GADURequestConsentInfoUpdate(
                IntPtr clientRef, IntPtr parameters,
                ConsentInformationClient.GADUConsentInfoUpdateCallback callback);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateConsentForm(IntPtr clientRef);

        [DllImport("__Internal")]
        internal static extern void GADULoadConsentForm(IntPtr formRef,
                ConsentFormClient.GADUConsentFormLoadCompletionHandler consentFormLoadCallback);

        [DllImport("__Internal")]
        internal static extern void GADUPresentConsentForm(IntPtr formRef,
                ConsentFormClient.GADUConsentFormPresentCompletionHandler
                consentFormPresentCallback);

        [DllImport("__Internal")]
        internal static extern void GADULoadAndPresentConsentForm(IntPtr formRef,
                ConsentFormClient.GADUConsentFormPresentCompletionHandler
                consentFormPresentCallback);

        [DllImport("__Internal")]
        internal static extern void GADUPresentPrivacyOptionsForm(IntPtr formRef,
                ConsentFormClient.GADUConsentFormPresentCompletionHandler
                consentFormPresentCallback);

        [DllImport("__Internal")]
        internal static extern void GADURelease(IntPtr obj);

        #endregion
    }
}
#endif
