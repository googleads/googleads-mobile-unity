#if UNITY_IOS
// Copyright (C) 2015 Google, Inc.
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

namespace GoogleMobileAds.iOS
{
    // Externs used by the iOS component.
    internal class Externs
    {
        #region Common externs

        [DllImport("__Internal")]
        internal static extern void GADUInitializeWithCallback(
            IntPtr mobileAdsClient, MobileAdsClient.GADUInitializationCompleteCallback callback);

        [DllImport("__Internal")]
        internal static extern void GADUDisableMediationInitialization();

        [DllImport("__Internal")]
        internal static extern IntPtr GADUGetInitDescription(IntPtr status, string className);

        [DllImport("__Internal")]
        internal static extern int GADUGetInitLatency(IntPtr status, string className);

        [DllImport("__Internal")]
        internal static extern int GADUGetInitState(IntPtr status, string className);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUGetInitAdapterClasses(IntPtr status);

        [DllImport("__Internal")]
        internal static extern int GADUGetInitNumberOfAdapterClasses(IntPtr status);


        [DllImport("__Internal")]
        internal static extern void GADUSetApplicationVolume(float volume);

        [DllImport("__Internal")]
        internal static extern void GADUSetApplicationMuted(bool muted);

        [DllImport("__Internal")]
        internal static extern void GADUSetiOSAppPauseOnBackground(bool pause);

        [DllImport("__Internal")]
        internal static extern float GADUDeviceScale();

        [DllImport("__Internal")]
        internal static extern int GADUDeviceSafeWidth();

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateRequest();

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateMutableDictionary();

        [DllImport("__Internal")]
        internal static extern void GADUMutableDictionarySetValue(
            IntPtr mutableDictionaryPtr,
            string key,
            string value);

        [DllImport("__Internal")]
        internal static extern void GADUSetMediationExtras(
            IntPtr request,
            IntPtr mutableDictionaryPtr,
            string adNetworkExtrasClassName);

        [DllImport("__Internal")]
        internal static extern void GADUAddKeyword(IntPtr request, string keyword);

        [DllImport("__Internal")]
        internal static extern void GADUSetExtra(IntPtr request, string key, string value);

        [DllImport("__Internal")]
        internal static extern void GADUSetRequestAgent(IntPtr request, string requestAgent);

        [DllImport("__Internal")]
        internal static extern void GADURelease(IntPtr obj);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateRequestConfiguration();

        [DllImport("__Internal")]
        internal static extern void GADUSetRequestConfiguration(IntPtr requestConfiguration);

        [DllImport("__Internal")]
        internal static extern void GADUSetRequestConfigurationTestDeviceIdentifiers(
            IntPtr requestConfiguration, string[] testDeviceIDs, int testDeviceIDLength);

        [DllImport("__Internal")]
        internal static extern void GADUSetRequestConfigurationMaxAdContentRating(
            IntPtr requestConfiguration, string maxAdContentRating);

        [DllImport("__Internal")]
        internal static extern void GADUSetRequestConfigurationTagForUnderAgeOfConsent(
            IntPtr requestConfiguration, int tagForUnderAgeOfConsent);

        [DllImport("__Internal")]
        internal static extern void GADUSetRequestConfigurationTagForChildDirectedTreatment(
            IntPtr requestConfiguration, int tagForChildDirectedTreatment);

        [DllImport("__Internal")]
        internal static extern void GADUSetRequestConfigurationSameAppKeyEnabled(
            IntPtr requestConfiguration, bool enabled);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUGetTestDeviceIdentifiers(IntPtr request);

        [DllImport("__Internal")]
        internal static extern int GADUGetTestDeviceIdentifiersCount(IntPtr request);

        [DllImport("__Internal")]
        internal static extern string GADUGetMaxAdContentRating(IntPtr requestConfiguration);

        [DllImport("__Internal")]
        internal static extern int GADUGetRequestConfigurationTagForUnderAgeOfConsent(
            IntPtr requestConfiguration);

        [DllImport("__Internal")]
        internal static extern int GADUGetRequestConfigurationTagForChildDirectedTreatment(
            IntPtr requestConfiguration);

        [DllImport("__Internal")]
        internal static extern bool GADUGetRequestConfigurationSameAppKeyEnabled(
            IntPtr requestConfiguration);

#endregion

        #region ResponseInfo externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUGetResponseInfo(IntPtr adFormat);

        [DllImport("__Internal")]
        internal static extern string GADUResponseInfoMediationAdapterClassName(IntPtr responseInfo);

        [DllImport("__Internal")]
        internal static extern string GADUResponseInfoResponseId(IntPtr responseInfo);

        [DllImport("__Internal")]
        internal static extern string GADUGetResponseInfoDescription(IntPtr responseInfo);

        [DllImport("__Internal")]
        internal static extern int GADUGetAdErrorCode(IntPtr error);

        [DllImport("__Internal")]
        internal static extern string GADUGetAdErrorDomain(IntPtr error);

        [DllImport("__Internal")]
        internal static extern string GADUGetAdErrorMessage(IntPtr error);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUGetAdErrorUnderLyingError(IntPtr error);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUGetAdErrorResponseInfo(IntPtr error);

        [DllImport("__Internal")]
        internal static extern string GADUGetAdErrorDescription(IntPtr error);

        #endregion

        #region RewardedAd externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateServerSideVerificationOptions();

        [DllImport("__Internal")]
        internal static extern void GADUServerSideVerificationOptionsSetUserId(
            IntPtr options, string userId);

        [DllImport("__Internal")]
        internal static extern void GADUServerSideVerificationOptionsSetCustomRewardString(
            IntPtr options, string customRewardString);

        [DllImport("__Internal")]
        internal static extern void GADURewardedAdSetServerSideVerificationOptions(
            IntPtr rewardedAd, IntPtr options);

        [DllImport("__Internal")]
        internal static extern string GADURewardedAdGetRewardType(IntPtr rewardedAd);

        [DllImport("__Internal")]
        internal static extern double GADURewardedAdGetRewardAmount(IntPtr rewardedAd);

        #endregion

        #region AdInspector externs


        [DllImport("__Internal")]
        internal static extern void GADUPresentAdInspector(
            IntPtr mobileAdsClient, MobileAdsClient.GADUAdInspectorClosedCallback callback);

        #endregion
    }
}
#endif
