// Copyright 2023 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADUInterstitial.h"
#import "GADUTypes.h"

/// A wrapper around GAMInterstitial. Includes the ability to create GAMInterstitial objects, load
/// them with ads, show them, and listen for ad events.
@interface GAMUInterstitial : GADUInterstitial

/// Initializes a GAMUInterstitial.
- (nonnull id)initWithAdManagerInterstitialClientReference:
    (_Nonnull GAMUTypeInterstitialClientRef * _Nonnull)interstitialClient;

/// A reference to the Unity Ad Manager interstitial client.
@property(nonatomic, assign) _Nonnull GAMUTypeInterstitialClientRef *
    _Nonnull interstitialClientGAM;

/// The Ad Manager interstitial ad.
@property(nonatomic, strong, nonnull) GAMInterstitialAd *interstitialAdGAM;

/// The app event callback into Unity.
@property(nonatomic, assign, nullable) GAMUInterstitialAppEventCallback appEventCallback;

#if GMA_PREVIEW_FEATURES

/// Returns whether a GAM interstitial ad is preloaded for the given ad unit ID.
+ (BOOL)isPreloadedAdAvailable:(nonnull NSString *)adUnitID;

/// Loads a preloaded GAM interstitial ad corresponding to the given ad unit ID if available.
- (void)preloadedAdWithAdUnitID:(nonnull NSString *)adUnitID;

#endif  // GMA_PREVIEW_FEATURES

/// Makes an ad request. Additional targeting options can be supplied with a request object.
- (void)loadWithAdManagerAdUnitID:(nonnull NSString *)adUnitID
                          request:(nonnull GAMRequest *)request;

@end
