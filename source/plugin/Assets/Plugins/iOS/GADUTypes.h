// Copyright 2014 Google Inc. All Rights Reserved.

/// Positions to place an ad.
typedef NS_ENUM(NSUInteger, GADAdPosition) {
  kGADAdPositionCustom = -1,              ///< Custom ad position.
  kGADAdPositionTopOfScreen = 0,          ///< Top of screen.
  kGADAdPositionBottomOfScreen = 1,       ///< Bottom of screen.
  kGADAdPositionTopLeftOfScreen = 2,      ///< Top left of screen.
  kGADAdPositionTopRightOfScreen = 3,     ///< Top right of screen.
  kGADAdPositionBottomLeftOfScreen = 4,   ///< Bottom left of screen.
  kGADAdPositionBottomRightOfScreen = 5,  ///< Bottom right of screen.
  kGADAdPositionCenterOfScreen = 6        ///< Bottom right of screen.
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

/// Type representing a Unity banner client.
typedef const void *GADUTypeBannerClientRef;

/// Type representing a Unity interstitial client.
typedef const void *GADUTypeInterstitialClientRef;

/// Type representing a Unity reward based video client.
typedef const void *GADUTypeRewardBasedVideoAdClientRef;

/// Type representing a Unity rewarded ad client.
typedef const void *GADUTypeRewardedAdClientRef;

/// Type representing a GADURewardedInterstitialAd.
typedef const void *GADUTypeRewardedInterstitialAdRef;

/// Type representing a Unity rewarded interstitial ad client.
typedef const void *GADUTypeRewardedInterstitialAdClientRef;

/// Type representing a Unity ad loader.
typedef const void *GADUTypeAdLoaderClientRef;

/// Type representing a Unity native custom template ad.
typedef const void *GADUTypeNativeCustomTemplateAdClientRef;

/// Type representing a GADUBanner.
typedef const void *GADUTypeBannerRef;

/// Type representing a GADUInterstitial.
typedef const void *GADUTypeInterstitialRef;

/// Type representing a GADURewardBasedVideoAd.
typedef const void *GADUTypeRewardBasedVideoAdRef;

/// Type representing a GADURewardedAd.
typedef const void *GADUTypeRewardedAdRef;

/// Type representing a GADUAdLoader.
typedef const void *GADUTypeAdLoaderRef;

/// Type representing a GADUNativeCustomTemplateAd.
typedef const void *GADUTypeNativeCustomTemplateAdRef;

/// Type representing a GADURequest.
typedef const void *GADUTypeRequestRef;

/// Type representing a GADUTypeRequestConfigurationRef
typedef const void *GADUTypeRequestConfigurationRef;

/// Type representing a GADUTypeResponseInfoRef
typedef const void *GADUTypeResponseInfoRef;

/// Type representing a NSMutableDictionary of extras.
typedef const void *GADUTypeMutableDictionaryRef;

/// Type representing a GADUAdNetworkExtras.
typedef const void *GADUTypeAdNetworkExtrasRef;

/// Type representing a GADUServerSideVerificationOptions.
typedef const void *GADUTypeServerSideVerificationOptionsRef;

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

/// Callback for when a interstitial ad request was successfully loaded.
typedef void (*GADUInterstitialDidReceiveAdCallback)(
    GADUTypeInterstitialClientRef *interstitialClient);

/// Callback for when an interstitial ad request failed.
typedef void (*GADUInterstitialDidFailToReceiveAdWithErrorCallback)(
    GADUTypeInterstitialClientRef *interstitialClient, const char *error);

/// Callback for when an interstitial is about to be presented.
typedef void (*GADUInterstitialWillPresentScreenCallback)(
    GADUTypeInterstitialClientRef *interstitialClient);

/// Callback for when an interstitial is about to be dismissed.
typedef void (*GADUInterstitialWillDismissScreenCallback)(
    GADUTypeInterstitialClientRef *interstitialClient);

/// Callback for when an interstitial has just been dismissed.
typedef void (*GADUInterstitialDidDismissScreenCallback)(
    GADUTypeInterstitialClientRef *interstitialClient);

/// Callback for when an application will background or terminate because of an interstitial click.
typedef void (*GADUInterstitialWillLeaveApplicationCallback)(
    GADUTypeInterstitialClientRef *interstitialClient);

/// Callback when an interstitial ad is estimated to have earned money.
typedef void (*GADUInterstitialPaidEventCallback)(GADUTypeInterstitialClientRef *interstitialClient,
                                                  int precision, int64_t value,
                                                  const char *currencyCode);

/// Callback for when a reward based video ad request was successfully loaded.
typedef void (*GADURewardBasedVideoAdDidReceiveAdCallback)(
    GADUTypeRewardBasedVideoAdClientRef *rewardBasedVideoClient);

/// Callback for when a reward based video ad request failed.
typedef void (*GADURewardBasedVideoAdDidFailToReceiveAdWithErrorCallback)(
    GADUTypeRewardBasedVideoAdClientRef *rewardBasedVideoClient, const char *error);

/// Callback for when a reward based video is opened.
typedef void (*GADURewardBasedVideoAdDidOpenCallback)(
    GADUTypeRewardBasedVideoAdClientRef *rewardBasedVideoClient);

/// Callback for when a reward based video has started to play.
typedef void (*GADURewardBasedVideoAdDidStartPlayingCallback)(
    GADUTypeRewardBasedVideoAdClientRef *rewardBasedVideoClient);

/// Callback for when a reward based video is closed.
typedef void (*GADURewardBasedVideoAdDidCloseCallback)(
    GADUTypeRewardBasedVideoAdClientRef *rewardBasedVideoClient);

/// Callback for when a user is rewarded by a reward based video.
typedef void (*GADURewardBasedVideoAdDidRewardCallback)(
    GADUTypeRewardBasedVideoAdClientRef *rewardBasedVideoClient, const char *rewardType,
    double rewardAmount);

/// Callback for when an application will background or terminate because of an reward based video
/// click.
typedef void (*GADURewardBasedVideoAdWillLeaveApplicationCallback)(
    GADUTypeRewardBasedVideoAdClientRef *rewardBasedVideoClient);

/// Callback for when a reward based video ad completes playing.
typedef void (*GADURewardBasedVideoAdDidCompleteCallback)(
    GADUTypeRewardBasedVideoAdClientRef *rewardBasedVideoClient);

/// Callback for when a native custom template ad request was successfully loaded.
typedef void (*GADUAdLoaderDidReceiveNativeCustomTemplateAdCallback)(
    GADUTypeAdLoaderClientRef *adLoader, GADUTypeNativeCustomTemplateAdRef nativeCustomTemplateAd,
    const char *templateID);

/// Callback for when a rewarded ad request was successfully loaded.
typedef void (*GADURewardedAdDidReceiveAdCallback)(GADUTypeRewardedAdClientRef *rewardedAdClient);

/// Callback for when a rewarded ad request failed.
typedef void (*GADURewardedAdDidFailToReceiveAdWithErrorCallback)(
    GADUTypeRewardedAdClientRef *rewardedAdClient, const char *error);

/// Callback for when a rewarded ad failed to show.
typedef void (*GADURewardedAdDidFailToShowAdWithErrorCallback)(
    GADUTypeRewardedAdClientRef *rewardedAdClient, const char *error);

/// Callback for when a rewarded ad is opened.
typedef void (*GADURewardedAdDidOpenCallback)(GADUTypeRewardedAdClientRef *rewardedAdClient);

/// Callback for when a rewarded ad is closed.
typedef void (*GADURewardedAdDidCloseCallback)(GADUTypeRewardedAdClientRef *rewardedAdClient);

/// Callback for when a user earned a reward.
typedef void (*GADUUserEarnedRewardCallback)(GADUTypeRewardedAdClientRef *rewardBasedVideoClient,
                                             const char *rewardType, double rewardAmount);

/// Callback for when a rewarded ad is estimated to have earned money.
typedef void (*GADURewardedAdPaidEventCallback)(GADUTypeRewardedAdClientRef *rewardedAdClient,
                                                int precision, int64_t value,
                                                const char *currencyCode);

/// Callback for when a rewarded interstitial ad is loaded.
typedef void (*GADURewardedInterstitialAdLoadedCallback)(
    GADUTypeRewardedInterstitialAdClientRef *rewardedInterstitialAdClient);

/// Callback for when a rewarded interstitial ad request failed to load.
typedef void (*GADURewardedInterstitialAdFailedToLoadCallback)(
    GADUTypeRewardedInterstitialAdClientRef *rewardedAdClient, const char *error);

/// Callback for when a user earned a reward.
typedef void (*GADUUserEarnedRewardCallback)(
    GADUTypeRewardedInterstitialAdClientRef *rewardedInterstitialAdClient, const char *rewardType,
    double rewardAmount);

/// Callback for when a rewarded interstitial ad is estimated to have earned money.
typedef void (*GADURewardedInterstitialAdPaidEventCallback)(
    GADUTypeRewardedInterstitialAdClientRef *rewardedInterstitialAdClient, int precision,
    int64_t value, const char *currencyCode);

/// Callback when an ad failed to present full screen content.
typedef void (*GADUFailedToPresentFullScreenContentCallback)(
    GADUTypeRewardedInterstitialAdRef *rewardedInterstitialAdClient, const char *error);

/// Callback when an ad presented full screen content.
typedef void (*GADUDidPresentFullScreenContentCallback)(
    GADUTypeRewardedInterstitialAdRef *rewardedInterstitialAdClient);

/// Callback when an ad dismissed full screen content.
typedef void (*GADUDidDismissFullScreenContentCallback)(
    GADUTypeRewardedInterstitialAdRef *rewardedInterstitialAdClient);

/// Callback for when a native ad request failed.
typedef void (*GADUAdLoaderDidFailToReceiveAdWithErrorCallback)(GADUTypeAdLoaderClientRef *adLoader,
                                                                const char *error);

/// Callback for when a native custom template ad is clicked.
typedef void (*GADUNativeCustomTemplateDidReceiveClickCallback)(
    GADUTypeNativeCustomTemplateAdClientRef *nativeCustomTemplateAd, const char *assetName);
