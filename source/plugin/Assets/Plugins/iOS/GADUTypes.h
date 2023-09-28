// Copyright 2014 Google Inc. All Rights Reserved.
#import <Foundation/Foundation.h>

/// Positions to place an ad.
typedef NS_ENUM(NSInteger, GADAdPosition) {
  kGADAdPositionCustom = -1,              ///< Custom ad position.
  kGADAdPositionTopOfScreen = 0,          ///< Top of screen.
  kGADAdPositionBottomOfScreen = 1,       ///< Bottom of screen.
  kGADAdPositionTopLeftOfScreen = 2,      ///< Top left of screen.
  kGADAdPositionTopRightOfScreen = 3,     ///< Top right of screen.
  kGADAdPositionBottomLeftOfScreen = 4,   ///< Bottom left of screen.
  kGADAdPositionBottomRightOfScreen = 5,  ///< Bottom right of screen.
  kGADAdPositionCenterOfScreen = 6        ///< Bottom right of screen.
};

/// Screen orientation that matches to Unity's ScreenOrientation enum values.
/// (See https://docs.unity3d.com/ScriptReference/ScreenOrientation.html)
typedef NS_ENUM(NSUInteger, GADUScreenOrientation) {
  kGADUScreenOrientationUnknown = 0,
  kGADUScreenOrientationPortrait = 1,            ///< Portrait.
  kGADUScreenOrientationPortraitUpsideDown = 2,  ///< Portrait, upside down.
  kGADUScreenOrientationLandscapeLeft = 3,       ///< Landscape, CCW from the portrait.
  kGADUScreenOrientationLandscapeRight = 4,      ///< Landscape, CW from the portrait.
  kGADUScreenOrientationAutoRotation = 5,        ///< Auto rotate mode.
};

/// Orientation for an adaptive banner.
typedef NS_ENUM(NSUInteger, GADUBannerOrientation) {
  kGADUBannerOrientationCurrent = 0,    ///< Current Orientation.
  kGADUBannerOrientationLandscape = 1,  ///< Landscape.
  kGADUBannerOrientationPortrait = 2,   ///< Portrait.
};

typedef NS_ENUM(NSInteger, GADUAdSize) { kGADUAdSizeUseFullWidth = -1 };

/// Base type representing a GADU* pointer.
typedef const void *GADUTypeRef;

typedef const void *GADUTypeMobileAdsClientRef;

typedef const void *GADUTypeInitializationStatusRef;

typedef void (*GADUInitializationCompleteCallback)(GADUTypeMobileAdsClientRef *clientRef,
                                                   GADUTypeInitializationStatusRef statusRef);

/// Type representing a Unity app open client.
typedef const void *GADUTypeAppOpenAdClientRef;

/// Type representing a Unity banner client.
typedef const void *GADUTypeBannerClientRef;

/// Type representing a Unity Ad Manager banner client.
typedef const void *GAMUTypeBannerClientRef;

/// Type representing a Unity interstitial client.
typedef const void *GADUTypeInterstitialClientRef;

/// Type representing a Unity rewarded ad client.
typedef const void *GADUTypeRewardedAdClientRef;

/// Type representing a GADURewardedInterstitialAd.
typedef const void *GADUTypeRewardedInterstitialAdRef;

/// Type representing a Unity rewarded interstitial ad client.
typedef const void *GADUTypeRewardedInterstitialAdClientRef;

/// Type representing a GADUAppOpenAd.
typedef const void *GADUTypeAppOpenAdRef;

/// Type representing a GADUBanner.
typedef const void *GADUTypeBannerRef;

/// Type representing a GADUInterstitial.
typedef const void *GADUTypeInterstitialRef;

/// Type representing a GAMUInterstitial.
typedef const void *GAMUTypeInterstitialRef;

/// Type representing an Ad Manager interstitial client.
typedef const void *GAMUTypeInterstitialClientRef;

/// Type representing a GADUNativeAdOptions.
typedef const void *GADUTypeNativeAdOptionsRef;

/// Type representing a GADUNativeTemplateTextStyle.
typedef const void *GADUTypeNativeTemplateTextStyleRef;

/// Type representing a GADUNativeTemplateStyle.
typedef const void *GADUTypeNativeTemplateStyleRef;

/// Type representing a GADUNativeTemplateAd.
typedef const void *GADUTypeNativeTemplateAdRef;

/// Type representing a Native Template Ad Client.
typedef const void *GADUTypeNativeTemplateAdClientRef;

/// Type representing a GADURewardedAd.
typedef const void *GADUTypeRewardedAdRef;

/// Type representing a GADURequest.
typedef const void *GADUTypeRequestRef;

/// Type representing a GAMURequest.
typedef const void *GAMUTypeRequestRef;

/// Type representing a GADUTypeRequestConfigurationRef
typedef const void *GADUTypeRequestConfigurationRef;

/// Type representing a GADUTypeResponseInfoRef
typedef const void *GADUTypeResponseInfoRef;

/// Type representing a GADUTypeAdapterResponseInfoRef type
typedef const void *GADUTypeAdapterResponseInfoRef;

/// Type representing a AdError type
typedef const void *GADUTypeErrorRef;

/// Type representing a NSMutableDictionary of extras.
typedef const void *GADUTypeMutableDictionaryRef;

/// Type representing a GADUAdNetworkExtras.
typedef const void *GADUTypeAdNetworkExtrasRef;

/// Type representing a GADUServerSideVerificationOptions.
typedef const void *GADUTypeServerSideVerificationOptionsRef;

/// Type representing UIColor.
typedef const void *GADUTypeUIColorRef;

/// Type representing a GADVideoOptions.
typedef const void *GADUTypeVideoOptionsRef;

// MARK: - GADUAppOpenAd

/// Callback for when an app open ad is loaded.
typedef void (*GADUAppOpenAdLoadedCallback)(GADUTypeAppOpenAdClientRef *appOpenAdClient);

/// Callback for when an app open ad request failed to load.
typedef void (*GADUAppOpenAdFailedToLoadCallback)(GADUTypeAppOpenAdClientRef *appOpenAdClient,
                                                  const char *error);

/// Callback when an app open ad failed to present full screen content.
typedef void (*GADUAppOpenAdFailedToPresentFullScreenContentCallback)(
    GADUTypeAppOpenAdClientRef *appOpenAdClient, const char *error);

/// Callback when an app open ad will present full screen content.
typedef void (*GADUAppOpenAdWillPresentFullScreenContentCallback)(
    GADUTypeAppOpenAdClientRef *appOpenAdClient);

/// Callback when an ad impression has been recorded for the app open ad.
typedef void (*GADUAppOpenAdDidRecordImpressionCallback)(
    GADUTypeAppOpenAdClientRef *appOpenAdClient);

/// Callback when an ad click has been recorded for the app open ad.
typedef void (*GADUAppOpenAdDidRecordClickCallback)(
    GADUTypeAppOpenAdClientRef *appOpenAdClient);


/// Callback when an app open ad dismissed full screen content.
typedef void (*GADUAppOpenAdDidDismissFullScreenContentCallback)(
    GADUTypeAppOpenAdClientRef *appOpenAdClient);

/// Callback when an app open ad is estimated to have earned money.
typedef void (*GADUAppOpenAdPaidEventCallback)(GADUTypeAppOpenAdClientRef *appOpenAdClient,
                                               int precision, int64_t value,
                                               const char *currencyCode);

// MARK: - GADUAdView

/// Callback for when a banner ad request was successfully loaded.
typedef void (*GADUAdViewDidReceiveAdCallback)(GADUTypeBannerClientRef *bannerClient);

/// Callback for when a banner ad request failed.
typedef void (*GADUAdViewDidFailToReceiveAdWithErrorCallback)(GADUTypeBannerClientRef *bannerClient,
                                                              const char *error);

/// Callback for when a full screen view is about to be presented as a result of a banner click.
typedef void (*GADUAdViewWillPresentScreenCallback)(GADUTypeBannerClientRef *bannerClient);

/// Callback for when a full screen view is about to be dismissed.
typedef void (*GADUAdViewWillDismissScreenCallback)(GADUTypeBannerClientRef *bannerClient);

/// Callback for when a full screen view has just been dismissed.
typedef void (*GADUAdViewDidDismissScreenCallback)(GADUTypeBannerClientRef *bannerClient);

/// Callback for when an application will background or terminate as a result of a banner click.
typedef void (*GADUAdViewWillLeaveApplicationCallback)(GADUTypeBannerClientRef *bannerClient);

/// Callback for when an ad is estimated to have earned money.
typedef void (*GADUAdViewPaidEventCallback)(GADUTypeBannerClientRef *bannerClient, int precision,
                                            int64_t value, const char *currencyCode);

/// Callback for when an ad reports an impression.
typedef void (*GADUAdViewImpressionCallback)(GADUTypeBannerClientRef *bannerClient);

/// Callback for when an ad reports a click.
typedef void (*GADUAdViewClickCallback)(GADUTypeBannerClientRef *bannerClient);

// MARK: - GAMUAdView

/// Callback when an Ad Manager ad view sends an app event.
typedef void (*GAMUAdViewAppEventCallback)(GAMUTypeBannerClientRef *bannerClient, const char *name,
                                           const char *info);

// MARK: - GADUInterstitial

/// Callback for when a interstitial ad request was successfully loaded.
typedef void (*GADUInterstitialAdLoadedCallback)(GADUTypeInterstitialClientRef *interstitialClient);

/// Callback for when an interstitial ad request failed.
typedef void (*GADUInterstitialAdFailedToLoadCallback)(
    GADUTypeInterstitialClientRef *interstitialClient, const char *error);

/// Callback when an interstitial ad failed to present full screen content.
typedef void (*GADUInterstitialAdFailedToPresentFullScreenContentCallback)(
    GADUTypeInterstitialRef *interstitialClient, const char *error);

/// Callback when an interstitial ad will present full screen content.
typedef void (*GADUInterstitialAdWillPresentFullScreenContentCallback)(
    GADUTypeInterstitialRef *interstitialClient);

/// Callback when an interstitial ad dismissed full screen content.
typedef void (*GADUInterstitialAdDidDismissFullScreenContentCallback)(
    GADUTypeInterstitialRef *interstitialClient);

/// Callback when an interstitial ad has made an impression.
typedef void (*GADUInterstitialAdDidRecordImpressionCallback)(
    GADUTypeInterstitialRef *interstitialClient);

/// Callback when an interstitial ad has made a click.
typedef void (*GADUInterstitialAdDidRecordClickCallback)(
    GADUTypeInterstitialRef *interstitialClient);

/// Callback when an interstitial ad is estimated to have earned money.
typedef void (*GADUInterstitialPaidEventCallback)(GADUTypeInterstitialClientRef *interstitialClient,
                                                  int precision, int64_t value,
                                                  const char *currencyCode);

// MARK: - GAMUInterstitial

/// Callback when an Ad Manager interstitial ad sends an app event.
typedef void (*GAMUInterstitialAppEventCallback)(GAMUTypeInterstitialClientRef *interstitialClient,
                                                 const char *name, const char *info);

// MARK: - GADUNativeTemplateAd

/// Callback for when a native ad request was successfully loaded.
typedef void (*GADUNativeTemplateAdLoadedCallback)(
    GADUTypeNativeTemplateAdClientRef *nativeTemplateAdClient);

/// Callback for when an native ad request failed.
typedef void (*GADUNativeTemplateAdFailedToLoadCallback)(
    GADUTypeNativeTemplateAdClientRef *nativeTemplateAdClient, const char *error);

/// Callback when an native ad has made an impression.
typedef void (*GADUNativeTemplateAdDidRecordImpressionCallback)(
    GADUTypeNativeTemplateAdClientRef *nativeTemplateAdClient);

/// Callback when an native ad has made a click.
typedef void (*GADUNativeTemplateAdDidRecordClickCallback)(
    GADUTypeNativeTemplateAdClientRef *nativeTemplateAdClient);

/// Callback when an native ad is estimated to have earned money.
typedef void (*GADUNativeTemplateAdPaidEventCallback)(
    GADUTypeNativeTemplateAdClientRef *nativeTemplateAdClient, int precision, int64_t value,
    const char *currencyCode);

/// Callback when a full screen view will be presented to the user.
typedef void (*GADUNativeTemplateAdWillPresentScreenCallback)(
    GADUTypeNativeTemplateAdClientRef *nativeTemplateAdClient);

/// Callback when a full screen view will be dismissed.
typedef void (*GADUNativeTemplateAdDidDismissScreenCallback)(
    GADUTypeNativeTemplateAdClientRef *nativeTemplateAdClient);

// MARK: - GADURewarded

/// Callback for when a rewarded ad request was successfully loaded.
typedef void (*GADURewardedAdLoadedCallback)(GADUTypeRewardedAdClientRef *rewardedAdClient);

/// Callback for when a rewarded ad request failed.
typedef void (*GADURewardedAdFailedToLoadCallback)(GADUTypeRewardedAdClientRef *rewardedAdClient,
                                                   const char *error);

/// Callback when a rewarded ad failed to present full screen content.
typedef void (*GADURewardedAdFailedToPresentFullScreenContentCallback)(
    GADUTypeRewardedAdRef *rewardedAdClient, const char *error);

/// Callback when a rewarded ad will present full screen content.
typedef void (*GADURewardedAdWillPresentFullScreenContentCallback)(
    GADUTypeRewardedAdRef *rewardedAdClient);

/// Callback when a rewarded ad dismissed full screen content.
typedef void (*GADURewardedAdDidDismissFullScreenContentCallback)(
    GADUTypeRewardedAdRef *rewardedAdClient);

/// Callback when a rewarded ad has made an impression.
typedef void (*GADURewardedAdDidRecordImpressionCallback)(GADUTypeRewardedAdRef *rewardedAdClient);

/// Callback when a rewarded ad has made a click.
typedef void (*GADURewardedAdDidRecordClickCallback)(GADUTypeRewardedAdRef *rewardedAdClient);

/// Callback for when a user earned a reward.
typedef void (*GADURewardedAdUserEarnedRewardCallback)(
    GADUTypeRewardedAdClientRef *rewardBasedVideoClient, const char *rewardType,
    double rewardAmount);

/// Callback for when a rewarded ad is estimated to have earned money.
typedef void (*GADURewardedAdPaidEventCallback)(GADUTypeRewardedAdClientRef *rewardedAdClient,
                                                int precision, int64_t value,
                                                const char *currencyCode);
// MARK: - GADRewardedInterstitial

/// Callback for when a rewarded interstitial ad is loaded.
typedef void (*GADURewardedInterstitialAdLoadedCallback)(
    GADUTypeRewardedInterstitialAdClientRef *rewardedInterstitialAdClient);

/// Callback for when a rewarded interstitial ad request failed to load.
typedef void (*GADURewardedInterstitialAdFailedToLoadCallback)(
    GADUTypeRewardedInterstitialAdClientRef *rewardedAdClient, const char *error);

/// Callback for when a user earned a reward.
typedef void (*GADURewardedInterstitialAdUserEarnedRewardCallback)(
    GADUTypeRewardedInterstitialAdClientRef *rewardedInterstitialAdClient, const char *rewardType,
    double rewardAmount);

/// Callback for when a rewarded interstitial ad is estimated to have earned money.
typedef void (*GADURewardedInterstitialAdPaidEventCallback)(
    GADUTypeRewardedInterstitialAdClientRef *rewardedInterstitialAdClient, int precision,
    int64_t value, const char *currencyCode);

/// Callback when a rewarded interstitial ad failed to present full screen content.
typedef void (*GADURewardedInterstitialAdFailedToPresentFullScreenContentCallback)(
    GADUTypeRewardedInterstitialAdRef *rewardedInterstitialAdClient, const char *error);

/// Callback when a rewarded interstitial ad will present full screen content.
typedef void (*GADURewardedInterstitialAdWillPresentFullScreenContentCallback)(
    GADUTypeRewardedInterstitialAdRef *rewardedInterstitialAdClient);

/// Callback when a rewarded interstitial ad dismissed full screen content.
typedef void (*GADURewardedInterstitialAdDidDismissFullScreenContentCallback)(
    GADUTypeRewardedInterstitialAdRef *rewardedInterstitialAdClient);

/// Callback when a rewarded interstitial ad has made an impression.
typedef void (*GADURewardedInterstitialAdDidRecordImpressionCallback)(
    GADUTypeRewardedInterstitialAdRef *rewardedInterstitialAdClient);

/// Callback when a rewarded interstitial ad has made a click.
typedef void (*GADURewardedInterstitialAdDidRecordClickCallback)(
    GADUTypeRewardedInterstitialAdRef *rewardedInterstitialAdClient);

// MARK: - GADUAdInspector

/// Callback when ad inspector UI closes.
typedef void (*GADUAdInspectorCompleteCallback)(GADUTypeMobileAdsClientRef *clientRef,
                                                const char *error);
