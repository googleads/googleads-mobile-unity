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

typedef const void *GADUTypeMobileAdsClientRef;

typedef const void *GADUTypeInitializationStatusRef;

typedef void (*GADUInitializationCompleteCallback)(GADUTypeMobileAdsClientRef *clientRef,
                                                   GADUTypeInitializationStatusRef statusRef);

/// Type representing a Unity app open client.
typedef const void *GADUTypeAppOpenAdClientRef;

/// Type representing a Unity banner client.
typedef const void *GADUTypeBannerClientRef;

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

/// Type representing a GADURewardedAd.
typedef const void *GADUTypeRewardedAdRef;

/// Type representing a GADURequest.
typedef const void *GADUTypeRequestRef;

/// Type representing a GADUTypeRequestConfigurationRef
typedef const void *GADUTypeRequestConfigurationRef;

/// Type representing a GADUTypeResponseInfoRef
typedef const void *GADUTypeResponseInfoRef;

/// Type representing a AdError type
typedef const void *GADUTypeErrorRef;

/// Type representing a NSMutableDictionary of extras.
typedef const void *GADUTypeMutableDictionaryRef;

/// Type representing a GADUAdNetworkExtras.
typedef const void *GADUTypeAdNetworkExtrasRef;

/// Type representing a GADUServerSideVerificationOptions.
typedef const void *GADUTypeServerSideVerificationOptionsRef;

// MARK: - GADUBaseAdType

typedef const void *GADUTypeAdClientRef;

typedef const void *GADUTypeAdBridgeRef;

typedef void (*GADUBaseAdCallback)(GADUTypeAdClientRef *adClient);

typedef void (*GADUBaseAdErrorCallback)(GADUTypeAdClientRef *adClient, const char *error);

typedef void (*GADUBaseAdPaidCallback)(GADUTypeAdClientRef *adClient, int precision, int64_t value,
                                       const char *currencyCode);

typedef void (*GADUBaseAdRewardedCallback)(GADUTypeAdClientRef *adClient, const char *rewardType,
                                           double rewardA);

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

// MARK: - GADUAdInspector

/// Callback when ad inspector UI closes.
typedef void (*GADUAdInspectorCompleteCallback)(GADUTypeMobileAdsClientRef *clientRef,
                                                const char *error);
