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
        internal static extern void GADUSetPlugin(string plugin);

        [DllImport("__Internal")]
        internal static extern void GADUSetiOSAppPauseOnBackground(bool pause);

        [DllImport("__Internal")]
        internal static extern float GADUDeviceScale();

        [DllImport("__Internal")]
        internal static extern int GADUDeviceSafeWidth();

        [DllImport("__Internal")]
        internal static extern void GADUSetUserDefaultsInteger(string key, int value);

        [DllImport("__Internal")]
        internal static extern void GADUSetUserDefaultsString(string key, string value);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateRequest();

        [DllImport("__Internal")]
        internal static extern IntPtr GAMUCreateRequest();

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
        internal static extern void GAMUSetPublisherProvidedID(IntPtr request,
                                                               string publisherProvidedID);

        [DllImport("__Internal")]
        internal static extern void GAMUAddCategoryExclusion(IntPtr request, string category);

        [DllImport("__Internal")]
        internal static extern void GAMUSetCustomTargeting(IntPtr request, string key, string value);

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

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateUIColor(float alpha, float red, float green,
                                                        float blue);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateVideoOptions(bool startMuted,
                                                             bool clickToExpandRequested,
                                                             bool customControlsRequested);

#endregion

        #region AppOpenAd externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateAppOpenAd(IntPtr appOpenAdClient);

        [DllImport("__Internal")]
        internal static extern void GADULoadAppOpenAdWithAdUnitID(
            IntPtr appOpenAd, string adUnitID, IntPtr request);

        [DllImport("__Internal")]
        internal static extern void GADULoadAppOpenAd(
            IntPtr appOpenAd, string adUnitID, int orientation, IntPtr request);

        [DllImport("__Internal")]
        internal static extern void GADUShowAppOpenAd(IntPtr appOpenAd);

        [DllImport("__Internal")]
        internal static extern void GADUSetAppOpenAdCallbacks(
            IntPtr appOpenAd,
            AppOpenAdClient.GADUAppOpenAdLoadedCallback adLoadedCallback,
            AppOpenAdClient.GADUAppOpenAdFailToLoadCallback adFailedToLoadCallback,
            AppOpenAdClient.GADUAppOpenAdPaidEventCallback paidEventCallback,
            AppOpenAdClient.GADUAppOpenAdFailedToPresentFullScreenContentCallback
                    adFailToPresentFullScreenContentCallback,
            AppOpenAdClient.GADUAppOpenAdWillPresentFullScreenContentCallback
                    adWillPresentFullScreenContentCallback,
            AppOpenAdClient.GADUAppOpenAdDidDismissFullScreenContentCallback
                    adDidDismissFullScreenContentCallback,
            AppOpenAdClient.GADUAppOpenAdDidRecordImpressionCallback
                    adDidRecordImpressionCallback,
            AppOpenAdClient.GADUAppOpenAdDidRecordClickCallback
                    adDidRecordClickCallback
        );

        #endregion

        #region Banner externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateBannerView(
            IntPtr bannerClient, string adUnitId, int width, int height, int positionAtTop);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateBannerViewWithCustomPosition(
            IntPtr bannerClient,
            string adUnitId,
            int width,
            int height,
            int x,
            int y);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateSmartBannerView(
            IntPtr bannerClient, string adUnitId, int positionAtTop);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateSmartBannerViewWithCustomPosition(
            IntPtr bannerClient, string adUnitId, int x, int y);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateAnchoredAdaptiveBannerView(
                    IntPtr bannerClient,
                    string adUnitId,
                    int width,
                    int orientation,
                    int positionAtTop);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateAnchoredAdaptiveBannerViewWithCustomPosition(
                    IntPtr bannerClient,
                    string adUnitId,
                    int width,
                    int orientation,
                    int x,
                    int y);

        [DllImport("__Internal")]
        internal static extern void GADUSetBannerCallbacks(
            IntPtr bannerView,
            BannerClient.GADUAdViewDidReceiveAdCallback adReceivedCallback,
            BannerClient.GADUAdViewDidFailToReceiveAdWithErrorCallback adFailedCallback,
            BannerClient.GADUAdViewWillPresentScreenCallback willPresentCallback,
            BannerClient.GADUAdViewDidDismissScreenCallback didDismissCallback,
            BannerClient.GADUAdViewPaidEventCallback paidEventCallback,
            BannerClient.GADUAdViewImpressionCallback adImpressionCallback,
            BannerClient.GADUAdViewClickCallback adClickCallback
        );

        [DllImport("__Internal")]
        internal static extern void GADUHideBannerView(IntPtr bannerView);

        [DllImport("__Internal")]
        internal static extern void GADUShowBannerView(IntPtr bannerView);

        [DllImport("__Internal")]
        internal static extern void GADURemoveBannerView(IntPtr bannerView);

        [DllImport("__Internal")]
        internal static extern void GADURequestBannerAd(IntPtr bannerView, IntPtr request);

        [DllImport("__Internal")]
        internal static extern float GADUGetBannerViewHeightInPixels(IntPtr bannerView);

        [DllImport("__Internal")]
        internal static extern float GADUGetBannerViewWidthInPixels(IntPtr bannerView);

        [DllImport("__Internal")]
        internal static extern void GADUSetBannerViewAdPosition(IntPtr bannerView, int position);

        [DllImport("__Internal")]
        internal static extern void GADUSetBannerViewCustomPosition(IntPtr bannerView, int x, int y);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUGetResponseInfo(IntPtr adFormat);

        [DllImport("__Internal")]
        internal static extern string GADUResponseInfoMediationAdapterClassName(IntPtr responseInfo);

        [DllImport("__Internal")]
        internal static extern string GADUResponseInfoResponseId(IntPtr responseInfo);

        [DllImport("__Internal")]
        internal static extern string GADUGetResponseInfoDescription(IntPtr responseInfo);

        [DllImport("__Internal")]
        internal static extern int GADUResponseInfoAdNetworkCount(IntPtr responseInfo);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUResponseInfoAdNetworkAtIndex(IntPtr responseInfo,
                                                                       int index);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUResponseInfoLoadedAdNetworkResponseInfo(
            IntPtr responseInfo);

        [DllImport("__Internal")]
        internal static extern int GADUResponseInfoExtrasCount(IntPtr responseInfo);

        [DllImport("__Internal")]
        internal static extern string GADUResponseInfoExtrasKey(IntPtr responseInfo, int index);

        [DllImport("__Internal")]
        internal static extern string GADUResponseInfoExtrasValue(IntPtr responseInfo, string key);

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

        #region
        [DllImport("__Internal")]
        internal static extern IntPtr GAMUCreateBannerView(
            IntPtr bannerClient, string adUnitId, int width, int height, int adPosition);

        [DllImport("__Internal")]
        internal static extern IntPtr GAMUCreateBannerViewWithCustomPosition(
            IntPtr bannerClient,
            string adUnitId,
            int width,
            int height,
            int x,
            int y);

        [DllImport("__Internal")]
        internal static extern IntPtr GAMUCreateAnchoredAdaptiveBannerView(
            IntPtr bannerClient,
            string adUnitId,
            int width,
            int orientation,
            int adPosition);

        [DllImport("__Internal")]
        internal static extern IntPtr GAMUCreateAnchoredAdaptiveBannerViewWithCustomPosition(
                IntPtr bannerClient,
                string adUnitId,
                int width,
                int orientation,
                int x,
                int y);

        [DllImport("__Internal")]
        internal static extern void GAMUSetBannerCallbacks(
            IntPtr bannerView,
            AdManagerBannerClient.GADUAdViewDidReceiveAdCallback adReceivedCallback,
            AdManagerBannerClient.GADUAdViewDidFailToReceiveAdWithErrorCallback adFailedCallback,
            AdManagerBannerClient.GADUAdViewWillPresentScreenCallback willPresentCallback,
            AdManagerBannerClient.GADUAdViewDidDismissScreenCallback didDismissCallback,
            AdManagerBannerClient.GADUAdViewPaidEventCallback paidEventCallback,
            AdManagerBannerClient.GADUAdViewImpressionCallback adImpressionCallback,
            AdManagerBannerClient.GADUAdViewClickCallback adClickCallback,
            AdManagerBannerClient.GAMUAdViewAppEventCallback appEventCallback
        );

        [DllImport("__Internal")]
        internal static extern void GAMUBannerViewSetValidAdSizes(
                IntPtr bannerView, int[] validAdSizesArray, int validAdSizesLength);

        #endregion

        #region Interstitial externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateInterstitial(IntPtr interstitialClient);

        [DllImport("__Internal")]
        internal static extern IntPtr GADULoadInterstitialAd(IntPtr interstitialAd, string adUnitID,
            IntPtr request);

        [DllImport("__Internal")]
        internal static extern void GADUSetInterstitialCallbacks(
            IntPtr interstitial,
            InterstitialClient.GADUInterstitialAdLoadedCallback adReceivedCallback,
            InterstitialClient.GADUInterstitialAdFailedToLoadCallback adFailedCallback,
            InterstitialClient.GADUInterstitialAdWillPresentFullScreenContentCallback
                adWillPresentFullScreenContentCallback,
            InterstitialClient.GADUInterstitialAdFailedToPresentFullScreenContentCallback
                adFailToPresentFullScreenContentCallback,
            InterstitialClient.GADUInterstitialAdDidDismissFullScreenContentCallback
                adDidDismissFullScreenContentCallback,
            InterstitialClient.GADUInterstitialAdDidRecordImpressionCallback
                adDidRecordImpressionCallback,
            InterstitialClient.GADUInterstitialAdDidRecordClickCallback
                adDidRecordClickCallback,
            InterstitialClient.GADUInterstitialPaidEventCallback paidEventCallback
        );

        [DllImport("__Internal")]
        internal static extern void GADUShowInterstitial(IntPtr interstitial);

        #endregion

        #region AdManager Interstitial externs

        [DllImport("__Internal")]
        internal static extern IntPtr GAMUCreateInterstitial(IntPtr interstitialClient);

        [DllImport("__Internal")]
        internal static extern IntPtr GAMULoadInterstitialAd(IntPtr interstitialAd, string adUnitID,
            IntPtr request);

        [DllImport("__Internal")]
        internal static extern void GAMUSetInterstitialCallbacks(
            IntPtr interstitial,
            AdManagerInterstitialClient.GADUInterstitialAdLoadedCallback adLoadedCallback,
            AdManagerInterstitialClient.GADUInterstitialAdFailedToLoadCallback
                adFailedToLoadCallback,
            AdManagerInterstitialClient.GADUInterstitialAdWillPresentFullScreenContentCallback
                adWillPresentFullScreenContentCallback,
            AdManagerInterstitialClient.GADUInterstitialAdFailedToPresentFullScreenContentCallback
                adFailToPresentFullScreenContentCallback,
            AdManagerInterstitialClient.GADUInterstitialAdDidDismissFullScreenContentCallback
                adDidDismissFullScreenContentCallback,
            AdManagerInterstitialClient.GADUInterstitialAdDidRecordImpressionCallback
                adDidRecordImpressionCallback,
            AdManagerInterstitialClient.GADUInterstitialAdDidRecordClickCallback
                adDidRecordClickCallback,
            AdManagerInterstitialClient.GADUInterstitialPaidEventCallback paidEventCallback,
            AdManagerInterstitialClient.GAMUInterstitialAppEventCallback appEventCallback
        );

        [DllImport("__Internal")]
        internal static extern void GAMUShowInterstitial(IntPtr interstitial);

        #endregion

        #region RewardedAd externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateRewardedAd(IntPtr rewardedAd);

        [DllImport("__Internal")]
        internal static extern IntPtr GADULoadRewardedAd(IntPtr interstitialAd, string adUnitID,
            IntPtr request);

        [DllImport("__Internal")]
        internal static extern void GADUShowRewardedAd(IntPtr rewardedAd);

        [DllImport("__Internal")]
        internal static extern void GADUSetRewardedAdCallbacks(
            IntPtr rewardedAd,
            RewardedAdClient.GADURewardedAdLoadedCallback adLoadedCallback,
            RewardedAdClient.GADURewardedAdFailedToLoadCallback adFailedToLoadCallback,
            RewardedAdClient.GADURewardedAdWillPresentFullScreenContentCallback
                adWillPresentFullScreenContentCallback,
            RewardedAdClient.GADURewardedAdFailedToPresentFullScreenContentCallback
                adFailToPresentFullScreenContentCallback,
            RewardedAdClient.GADURewardedAdDidDismissFullScreenContentCallback
                adDidDismissFullScreenContentCallback,
            RewardedAdClient.GADURewardedAdDidRecordImpressionCallback
                adDidRecordImpressionCallback,
            RewardedAdClient.GADURewardedAdDidRecordClickCallback
                adDidRecordClickCallback,
            RewardedAdClient.GADURewardedAdUserEarnedRewardCallback adDidEarnRewardCallback,
            RewardedAdClient.GADURewardedAdPaidEventCallback
                paidEventCallback
        );

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

        #region RewardedInterstitialAd externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateRewardedInterstitialAd(IntPtr rewardedInterstitialAd);

        [DllImport("__Internal")]
        internal static extern IntPtr GADULoadRewardedInterstitialAd(IntPtr rewardedInterstitialAd,
            string adUnitID, IntPtr request);

        [DllImport("__Internal")]
        internal static extern void GADUShowRewardedInterstitialAd(IntPtr rewardedInterstitialAd);

        [DllImport("__Internal")]
        internal static extern void GADUSetRewardedInterstitialAdCallbacks(
            IntPtr rewardedInterstitialAd,
            RewardedInterstitialAdClient.GADURewardedInterstitialAdLoadedCallback
                adLoadedCallback,
            RewardedInterstitialAdClient.GADURewardedInterstitialAdFailedToLoadCallback
                adFailedToLoadCallback,
            RewardedInterstitialAdClient.GADURewardedInterstitialAdUserEarnedRewardCallback adDidEarnRewardCallback,
            RewardedInterstitialAdClient.GADURewardedInterstitialAdPaidEventCallback
                paidEventCallback,
            RewardedInterstitialAdClient.GADURewardedInterstitialAdFailedToPresentFullScreenContentCallback
                adFailToPresentFullScreenContentCallback,
            RewardedInterstitialAdClient.GADURewardedInterstitialAdWillPresentFullScreenContentCallback
                adWillPresentFullScreenContentCallback,
            RewardedInterstitialAdClient.GADURewardedInterstitialAdDidDismissFullScreenContentCallback
                adDidDismissFullScreenContentCallback,
            RewardedInterstitialAdClient.GADURewardedInterstitialAdDidRecordImpressionCallback
                adDidRecordImpressionCallback,
            RewardedInterstitialAdClient.GADURewardedInterstitialAdDidRecordClickCallback
                adDidRecordClickCallback
        );

        [DllImport("__Internal")]
        internal static extern void GADURewardedInterstitialAdSetServerSideVerificationOptions(
            IntPtr rewardedAd, IntPtr options);

        [DllImport("__Internal")]
        internal static extern string GADURewardedInterstitialAdGetRewardType(
            IntPtr rewardedInterstitialAd);

        [DllImport("__Internal")]
        internal static extern double GADURewardedInterstitialAdGetRewardAmount(
            IntPtr rewardedInterstitialAd);

        #endregion

#region NativeOverlay externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateNativeAdOptions(int adChoicesPlacement,
                                                                int mediaAspectRatio,
                                                                IntPtr videoOptions);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateNativeTemplateTextStyle();

        [DllImport("__Internal")]
        internal static extern IntPtr GADUSetNativeTemplateTextColor(IntPtr templateTextStyle,
                                                                     IntPtr color);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUSetNativeTemplateTextBackgroundColor(
            IntPtr templateTextStyle, IntPtr color);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUSetNativeTemplateTextFontStyle(IntPtr templateTextStyle,
                                                                         int fontType);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUSetNativeTemplateTextFontSize(IntPtr templateTextStyle,
                                                                        int size);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateNativeTemplateStyle(string templateName);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUSetNativeTemplateStyleBackgroundColor(
            IntPtr templateStyle, IntPtr color);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUSetNativeTemplateStyleText(IntPtr templateStyle,
                                                                     string textType, IntPtr color);
#endregion

#region AdInspector externs


        [DllImport("__Internal")]
        internal static extern void GADUPresentAdInspector(
            IntPtr mobileAdsClient, MobileAdsClient.GADUAdInspectorClosedCallback callback);

        #endregion

        #region AdapterResponseInfo externs

        [DllImport("__Internal")]
        internal static extern string GADUAdapterResponseInfoAdNetworkClassName(
            IntPtr adapterResponseInfoRef);

        [DllImport("__Internal")]
        internal static extern string GADUAdapterResponseInfoAdSourceID(
            IntPtr adapterResponseInfoRef);

        [DllImport("__Internal")]
        internal static extern string GADUAdapterResponseInfoAdSourceName(
            IntPtr adapterResponseInfoRef);

        [DllImport("__Internal")]
        internal static extern string GADUAdapterResponseInfoAdSourceInstanceID(
            IntPtr adapterResponseInfoRef);

        [DllImport("__Internal")]
        internal static extern string GADUAdapterResponseInfoAdSourceInstanceName(
            IntPtr adapterResponseInfoRef);

        [DllImport("__Internal")]
        internal static extern long GADUAdapterResponseInfoLatency(
            IntPtr adapterResponseInfoRef);

        [DllImport("__Internal")]
        internal static extern int GADUAdapterResponseInfoAdUnitMappingCount(
            IntPtr adapterResponseInfoRef);

        [DllImport("__Internal")]
        internal static extern string GADUAdapterResponseInfoAdUnitMappingKey(
            IntPtr adapterResponseInfoRef,  int index);

        [DllImport("__Internal")]
        internal static extern string GADUAdapterResponseInfoAdUnitMappingValue(
            IntPtr adapterResponseInfoRef, string key);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUAdapterResponseInfoAdError(
            IntPtr adapterResponseInfoRef);

        [DllImport("__Internal")]
        internal static extern string GADUAdapterResponseInfoDescription(IntPtr error);

        #endregion
    }
}
#endif
