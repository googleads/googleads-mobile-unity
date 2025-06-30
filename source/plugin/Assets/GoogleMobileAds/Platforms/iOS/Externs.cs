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
        internal static extern void GADUPreloadWithCallback(
            IntPtr mobileAdsClient,
            IntPtr[] configurations,
            int configurationsCount,
            MobileAdsClient.GADUAdAvailableCallback adAvailable,
            MobileAdsClient.GADUAdsExhaustedCallback adsExhausted);

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
        internal static extern void GADUDisableSDKCrashReporting();

        [DllImport("__Internal")]
        internal static extern float GADUDeviceScale();

        [DllImport("__Internal")]
        internal static extern int GADUDeviceSafeWidth();

        [DllImport("__Internal")]
        internal static extern void GADUSetIntegerPreference(string key, int value);

        [DllImport("__Internal")]
        internal static extern void GADUSetStringPreference(string key, string value);

        [DllImport("__Internal")]
        internal static extern int GADUGetIntegerPreference(string key);

        [DllImport("__Internal")]
        internal static extern string GADUGetStringPreference(string key);

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
        internal static extern int GADUNSDictionaryCount(IntPtr dictPtr);

        [DllImport("__Internal")]
        internal static extern string GADUNSDictionaryKeyAtIndex(IntPtr dictPtr, int index);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUNSDictionaryValueForKey(IntPtr dictPtr, string key);

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
        internal static extern void GADUSetCustomTargeting(IntPtr request, string key, string value);

        [DllImport("__Internal")]
        internal static extern void GAMUSetCustomTargeting(IntPtr request, string key, string value);

        [DllImport("__Internal")]
        internal static extern void GADUSetRequestAgent(IntPtr request, string requestAgent);

        [DllImport("__Internal")]
        internal static extern void GADURelease(IntPtr obj);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreatePreloadConfiguration();

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreatePreloadConfigurationV2();

        [DllImport("__Internal")]
        internal static extern string GADUGetPreloadConfigurationAdUnitID(IntPtr preloadConfiguration);

        [DllImport("__Internal")]
        internal static extern void GADUSetPreloadConfigurationAdUnitID(IntPtr preloadConfiguration, string adUnitId);

        [DllImport("__Internal")]
        internal static extern int GADUGetPreloadConfigurationAdFormat(IntPtr preloadConfiguration);

        [DllImport("__Internal")]
        internal static extern void GADUSetPreloadConfigurationAdFormat(IntPtr preloadConfiguration, int adFormat);

        [DllImport("__Internal")]
        internal static extern void GADUSetPreloadConfigurationAdRequest(IntPtr preloadConfiguration, IntPtr adRequest);

        [DllImport("__Internal")]
        internal static extern uint GADUGetPreloadConfigurationBufferSize(IntPtr preloadConfiguration);

        [DllImport("__Internal")]
        internal static extern void GADUSetPreloadConfigurationBufferSize(IntPtr preloadConfiguration, uint bufferSize);

        [DllImport("__Internal")]
        internal static extern string GADUGetPreloadConfigurationV2AdUnitID(
            IntPtr preloadConfiguration);

        [DllImport("__Internal")]
        internal static extern void GADUSetPreloadConfigurationV2AdUnitID(
            IntPtr preloadConfiguration, string adUnitId);

        [DllImport("__Internal")]
        internal static extern void GADUSetPreloadConfigurationV2AdRequest(
            IntPtr preloadConfiguration, IntPtr adRequest);

        [DllImport("__Internal")]
        internal static extern uint GADUGetPreloadConfigurationV2BufferSize(
            IntPtr preloadConfiguration);

        [DllImport("__Internal")]
        internal static extern void GADUSetPreloadConfigurationV2BufferSize(
            IntPtr preloadConfiguration, uint bufferSize);

        [DllImport("__Internal")]
        internal static extern string GADUMobileAdsVersion();

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
        internal static extern void GADUSetRequestConfigurationPublisherFirstPartyIDEnabled(
            bool enabled);

        [DllImport("__Internal")]
        internal static extern void GADUSetRequestConfigurationPublisherPrivacyPersonalizationState(
            int state);

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
        internal static extern int GADUGetRequestConfigurationPublisherPrivacyPersonalizationState();

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
        internal static extern bool GADUAppOpenIsPreloadedAdAvailable(string adUnitId);

        [DllImport("__Internal")]
        internal static extern void GADUAppOpenPreloadedAdWithAdUnitID(IntPtr appOpenAd,
                                                                       string adUnitId);

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

        [DllImport("__Internal")]
        internal static extern string GADUGetAppOpenAdUnitID(IntPtr appOpenAd);

        #endregion

        #region AppOpenAdPreloader externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateAppOpenAdPreloader(IntPtr appOpenAdPreloaderClient);

        [DllImport("__Internal")]
        internal static extern bool GADUAppOpenAdPreloaderPreload(IntPtr appOpenAdPreloaderClient,
            string preloadId, IntPtr preloadConfiguration);

        [DllImport("__Internal")]
        internal static extern bool GADUAppOpenAdPreloaderIsAdAvailable(
            IntPtr appOpenAdPreloaderClient, string preloadId);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUAppOpenAdPreloaderGetPreloadedAd(
            IntPtr appOpenAdPreloaderClient, string preloadId, IntPtr appOpenAdClientPtr);

        [DllImport("__Internal")]
        internal static extern int GADUAppOpenAdPreloaderGetNumAdsAvailable(
            IntPtr appOpenAdPreloaderClient, string preloadId);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUAppOpenAdPreloaderGetConfiguration(
            IntPtr appOpenAdPreloaderClient, string preloadId);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUAppOpenAdPreloaderGetConfigurations(
            IntPtr appOpenAdPreloaderClient);

        [DllImport("__Internal")]
        internal static extern void GADUAppOpenAdPreloaderDestroy(IntPtr appOpenAdPreloaderClient,
                                                                  string preloadId);

        [DllImport("__Internal")]
        internal static extern void GADUAppOpenAdPreloaderDestroyAll(
            IntPtr appOpenAdPreloaderClient);

        [DllImport("__Internal")]
        internal static extern void GADUSetAppOpenAdPreloaderCallbacks(
            IntPtr appOpenAdPreloader,
            AppOpenAdPreloaderClient.GADUAdAvailableForPreloadIdCallback adPreloadedCallback,
            AppOpenAdPreloaderClient.GADUAdFailedToPreloadForPreloadIdCallback
                adFailedToPreloadCallback,
            AppOpenAdPreloaderClient.GADUAdsExhaustedForPreloadIdCallback adsExhaustedCallback);

        #endregion

        #region RewardedAdPreloader externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateRewardedAdPreloader(
            IntPtr rewardedAdPreloaderClient);

        [DllImport("__Internal")]
        internal static extern bool GADURewardedAdPreloaderPreload(IntPtr rewardedAdPreloaderClient,
            string preloadId, IntPtr preloadConfiguration);

        [DllImport("__Internal")]
        internal static extern bool GADURewardedAdPreloaderIsAdAvailable(
            IntPtr rewardedAdPreloaderClient, string preloadId);

        [DllImport("__Internal")]
        internal static extern IntPtr GADURewardedAdPreloaderGetPreloadedAd(
            IntPtr rewardedAdPreloaderClient, string preloadId, IntPtr appOpenAdClientPtr);

        [DllImport("__Internal")]
        internal static extern int GADURewardedAdPreloaderGetNumAdsAvailable(
            IntPtr rewardedAdPreloaderClient, string preloadId);

        [DllImport("__Internal")]
        internal static extern IntPtr GADURewardedAdPreloaderGetConfiguration(
            IntPtr rewardedAdPreloaderClient, string preloadId);

        [DllImport("__Internal")]
        internal static extern IntPtr GADURewardedAdPreloaderGetConfigurations(
            IntPtr rewardedAdPreloaderClient);

        [DllImport("__Internal")]
        internal static extern bool GADURewardedAdPreloaderDestroy(IntPtr rewardedAdPreloaderClient,
                                                                  string preloadId);

        [DllImport("__Internal")]
        internal static extern void GADURewardedAdPreloaderDestroyAll(
            IntPtr rewardedAdPreloaderClient);

        [DllImport("__Internal")]
        internal static extern void GADUSetRewardedAdPreloaderCallbacks(
            IntPtr appOpenAdPreloader,
            RewardedAdPreloaderClient.GADUAdAvailableForPreloadIdCallback adPreloadedCallback,
            RewardedAdPreloaderClient.GADUAdFailedToPreloadForPreloadIdCallback
                adFailedToPreloadCallback,
            RewardedAdPreloaderClient.GADUAdsExhaustedForPreloadIdCallback adsExhaustedCallback);

        #endregion

        #region InterstitialAdPreloader externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateInterstitialAdPreloader(
            IntPtr interstitialAdPreloaderClient);

        [DllImport("__Internal")]
        internal static extern bool GADUInterstitialAdPreloaderPreload(
            IntPtr interstitialAdPreloaderClient,
            string preloadId, IntPtr preloadConfiguration);

        [DllImport("__Internal")]
        internal static extern bool GADUInterstitialAdPreloaderIsAdAvailable(
            IntPtr interstitialAdPreloaderClient, string preloadId);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUInterstitialAdPreloaderGetPreloadedAd(
            IntPtr interstitialAdPreloaderClient, string preloadId, IntPtr appOpenAdClientPtr);

        [DllImport("__Internal")]
        internal static extern int GADUInterstitialAdPreloaderGetNumAdsAvailable(
            IntPtr interstitialAdPreloaderClient, string preloadId);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUInterstitialAdPreloaderGetConfiguration(
            IntPtr interstitialAdPreloaderClient, string preloadId);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUInterstitialAdPreloaderGetConfigurations(
            IntPtr interstitialAdPreloaderClient);

        [DllImport("__Internal")]
        internal static extern bool GADUInterstitialAdPreloaderDestroy(
            IntPtr interstitialAdPreloaderClient,
            string preloadId);

        [DllImport("__Internal")]
        internal static extern void GADUInterstitialAdPreloaderDestroyAll(
            IntPtr interstitialAdPreloaderClient);

        [DllImport("__Internal")]
        internal static extern void GADUSetInterstitialAdPreloaderCallbacks(
            IntPtr interstitialAdPreloader,
            InterstitialAdPreloaderClient.GADUAdAvailableForPreloadIdCallback adPreloadedCallback,
            InterstitialAdPreloaderClient.GADUAdFailedToPreloadForPreloadIdCallback
                adFailedToPreloadCallback,
            InterstitialAdPreloaderClient.GADUAdsExhaustedForPreloadIdCallback
                adsExhaustedCallback);

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
        internal static extern string GADUGetBannerViewAdUnitID(IntPtr bannerView);

        [DllImport("__Internal")]
        internal static extern float GADUGetBannerViewHeightInPixels(IntPtr bannerView);

        [DllImport("__Internal")]
        internal static extern float GADUGetBannerViewWidthInPixels(IntPtr bannerView);

        [DllImport("__Internal")]
        internal static extern void GADUSetBannerViewAdPosition(IntPtr bannerView, int position);

        [DllImport("__Internal")]
        internal static extern void GADUSetBannerViewCustomPosition(IntPtr bannerView, int x, int y);

        [DllImport("__Internal")]
        internal static extern bool GADUIsBannerViewCollapsible(IntPtr bannerView);

        [DllImport("__Internal")]
        internal static extern bool GADUIsBannerViewHidden(IntPtr bannerView);

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
        internal static extern string GAMUGetBannerViewAdUnitID(IntPtr bannerView);

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
        internal static extern bool GADUInterstitialIsPreloadedAdAvailable(string adUnitId);

        [DllImport("__Internal")]
        internal static extern void GADUInterstitialPreloadedAdWithAdUnitID(IntPtr interstitialAd,
                                                                            string adUnitId);

        [DllImport("__Internal")]
        internal static extern IntPtr GADULoadInterstitialAd(IntPtr interstitialAd, string adUnitID,
            IntPtr request);

        [DllImport("__Internal")]
        internal static extern string GADUGetInterstitialAdUnitID(IntPtr interstitialAd);

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
        internal static extern bool GAMUInterstitialIsPreloadedAdAvailable(string adUnitId);

        [DllImport("__Internal")]
        internal static extern void GAMUInterstitialPreloadedAdWithAdUnitID(IntPtr interstitialAd,
                                                                            string adUnitId);

        [DllImport("__Internal")]
        internal static extern IntPtr GAMULoadInterstitialAd(IntPtr interstitialAd, string adUnitID,
            IntPtr request);

        [DllImport("__Internal")]
        internal static extern string GAMUGetInterstitialAdUnitID(IntPtr interstitialAd);

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
        internal static extern bool GADURewardedIsPreloadedAdAvailable(string adUnitId);

        [DllImport("__Internal")]
        internal static extern void GADURewardedPreloadedAdWithAdUnitID(IntPtr rewardedAd,
                                                                        string adUnitId);

        [DllImport("__Internal")]
        internal static extern IntPtr GADULoadRewardedAd(IntPtr interstitialAd, string adUnitID,
            IntPtr request);

        [DllImport("__Internal")]
        internal static extern void GADUShowRewardedAd(IntPtr rewardedAd);

        [DllImport("__Internal")]
        internal static extern string GADUGetRewardedAdUnitID(IntPtr rewardedAd);

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
        internal static extern string GADUGetRewardedInterstitialAdUnitID(
            IntPtr rewardedInterstitialAd);

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

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateNativeTemplateAd(IntPtr nativeAdClient);

        [DllImport("__Internal")]
        internal static extern void GADUSetNativeTemplateAdCallbacks(
            IntPtr nativeTemplateAd,
            NativeOverlayAdClient.GADUNativeAdLoadedCallback adLoadedCallback,
            NativeOverlayAdClient.GADUNativeAdFailedToLoadCallback adFailedCallback,
            NativeOverlayAdClient.GADUNativeAdDidRecordImpressionCallback
                adDidRecordImpressionCallback,
            NativeOverlayAdClient.GADUNativeAdDidRecordClickCallback
                adDidRecordClickCallback,
            NativeOverlayAdClient.GADUNativePaidEventCallback paidEventCallback,
            NativeOverlayAdClient.GADUNativeAdWillPresentScreenCallback willPresentCallback,
            NativeOverlayAdClient.GADUNativeAdDidDismissScreenCallback didDismissCallback);

        [DllImport("__Internal")]
        internal static extern void GADULoadNativeTemplateAd(IntPtr nativeTemplateAd,
                                                             string adUnitID,
                                                             IntPtr nativeAdOptions,
                                                             IntPtr request);

        [DllImport("__Internal")]
        internal static extern void GADUShowNativeTemplateAd(IntPtr nativeTemplateAd,
                                                             IntPtr templateStyle, int height,
                                                             int width);

        [DllImport("__Internal")]
        internal static extern void GADUShowDefaultNativeTemplateAd(IntPtr nativeTemplateAd,
                                                                    IntPtr templateStyle);

        [DllImport("__Internal")]
        internal static extern void GADUSetNativeTemplateAdPosition(IntPtr nativeTemplateAd,
                                                                    int position);

        [DllImport("__Internal")]
        internal static extern void GADUSetNativeTemplateAdCustomPosition(IntPtr nativeTemplateAd,
                                                                          int x, int y);

        [DllImport("__Internal")]
        internal static extern void GADUHideNativeTemplateAd(IntPtr nativeTemplateAd);

        [DllImport("__Internal")]
        internal static extern void GADUDisplayNativeTemplateAd(IntPtr nativeTemplateAd);

        [DllImport("__Internal")]
        internal static extern void GADUDestroyNativeTemplateAd(IntPtr nativeTemplateAd);

        [DllImport("__Internal")]
        internal static extern float GADUGetNativeTemplateAdHeightInPixels(IntPtr nativeTemplateAd);

        [DllImport("__Internal")]
        internal static extern float GADUGetNativeTemplateAdWidthInPixels(IntPtr nativeTemplateAd);
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
