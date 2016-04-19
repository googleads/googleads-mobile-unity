// Copyright 2014 Google Inc. All Rights Reserved.

/// Base type representing a GADU* pointer.
typedef const void *GADUTypeRef;

/// Type representing a Unity banner client.
typedef const void *GADUTypeBannerClientRef;

/// Type representing a Unity interstitial client.
typedef const void *GADUTypeInterstitialClientRef;

/// Type representing a Unity reward based video client.
typedef const void *GADUTypeRewardBasedVideoAdClientRef;

/// Type representing a GADUBanner.
typedef const void *GADUTypeBannerRef;

/// Type representing a GADUInterstitial.
typedef const void *GADUTypeInterstitialRef;

/// Type representing a GADURewardBasedVideoAd.
typedef const void *GADUTypeRewardBasedVideoAdRef;

/// Type representing a GADURequest.
typedef const void *GADUTypeRequestRef;

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
