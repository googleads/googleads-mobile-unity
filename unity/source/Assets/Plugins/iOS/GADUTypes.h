// Copyright 2014 Google Inc. All Rights Reserved.

/// Base type representing a GADU* pointer.
typedef const void *GADUTypeRef;

/// Type representing a Unity banner client.
typedef const void *GADUTypeBannerClientRef;

/// Type representing a Unity interstitial client.
typedef const void *GADUTypeInterstitialClientRef;

/// Type representing a GADUBanner.
typedef const void *GADUTypeBannerRef;

/// Type representing a GADUInterstitial.
typedef const void *GADUTypeInterstitialRef;

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
