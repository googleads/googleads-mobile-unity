// Copyright 2014 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADUTypes.h"

/// A wrapper around GADInterstitial. Includes the ability to create GADInterstitial objects, load
/// them with ads, show them, and listen for ad events.
@interface GADUInterstitial : NSObject

/// Initializes a GADUInterstitial.
- (id)initWithInterstitialClientReference:(GADUTypeInterstitialClientRef *)interstitialClient;

/// The interstitial ad.
@property(nonatomic, strong) GADInterstitialAd *interstitialAd;

/// A reference to the Unity interstitial client.
@property(nonatomic, assign) GADUTypeInterstitialClientRef *interstitialClient;

/// The ad received callback into Unity.
@property(nonatomic, assign) GADUInterstitialAdLoadedCallback adLoadedCallback;

/// The ad failed callback into Unity.
@property(nonatomic, assign) GADUInterstitialAdFailedToLoadCallback adFailedToLoadCallback;

/// The ad failed to present full screen content callback into Unity.
@property(nonatomic, assign) GADUInterstitialAdFailedToPresentFullScreenContentCallback
    adFailedToPresentFullScreenContentCallback;

/// The ad will present full screen content callback into Unity.
@property(nonatomic, assign)
    GADUInterstitialAdWillPresentFullScreenContentCallback adWillPresentFullScreenContentCallback;

/// The ad dismissed full screen content callback into Unity.
@property(nonatomic, assign)
    GADUInterstitialAdDidDismissFullScreenContentCallback adDidDismissFullScreenContentCallback;

/// The ad impression callback into Unity.
@property(nonatomic, assign)
    GADUInterstitialAdDidRecordImpressionCallback adDidRecordImpressionCallback;

/// The ad click callback into Unity.
@property(nonatomic, assign)
    GADUInterstitialAdDidRecordClickCallback adDidRecordClickCallback;

/// The paid event callback into Unity.
@property(nonatomic, assign) GADUInterstitialPaidEventCallback paidEventCallback;

// Returns the interstitial ad response info.
@property(nonatomic, readonly, copy) GADResponseInfo *responseInfo;

/// Makes an ad request. Additional targeting options can be supplied with a request object.
- (void)loadWithAdUnitID:(NSString *)adUnitID request:(GADRequest *)request;

/// Shows the interstitial ad.
- (void)show;

@end
