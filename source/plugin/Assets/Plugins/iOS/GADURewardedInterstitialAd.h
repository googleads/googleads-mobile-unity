// Copyright 2020 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADUTypes.h"

@interface GADURewardedInterstitialAd : NSObject

/// Initializes a GADURewardedInterstitialAd.
- (instancetype)initWithRewardedInterstitialAdClientReference:
    (GADUTypeRewardedInterstitialAdClientRef *)rewardedInterstitialAdClient;

/// The rewarded interstitial ad.
@property(nonatomic, strong) GADRewardedInterstitialAd *rewardedInterstitialAd;

/// A reference to the Unity rewarded interstitial ad client.
@property(nonatomic, assign) GADUTypeRewardedInterstitialAdClientRef *rewardedInterstitialAdClient;

/// The ad received callback into Unity.
@property(nonatomic, assign) GADURewardedInterstitialAdLoadedCallback adLoadedCallback;

/// The ad request failed callback into Unity.
@property(nonatomic, assign) GADURewardedInterstitialAdFailedToLoadCallback adFailedToLoadCallback;

/// The ad failed to present full screen content callback into Unity.
@property(nonatomic, assign)
    GADUFailedToPresentFullScreenContentCallback adFailedToPresentFullScreenContentCallback;

/// The ad presented full screen content callback into Unity.
@property(nonatomic, assign)
    GADUDidPresentFullScreenContentCallback adDidPresentFullScreenContentCallback;

/// The ad dismissed full screen content callback into Unity.
@property(nonatomic, assign)
    GADUDidDismissFullScreenContentCallback adDidDismissFullScreenContentCallback;

/// The user was rewarded callback into Unity.
@property(nonatomic, assign) GADUUserEarnedRewardCallback didEarnRewardCallback;

/// The paid event callback into Unity.
@property(nonatomic, assign) GADURewardedInterstitialAdPaidEventCallback paidEventCallback;

/// Returns the rewarded interstitial ad response info.
@property(nonatomic, readonly, copy) GADResponseInfo *responseInfo;

/// Makes an ad request. Additional targeting options can be supplied with a request object.
- (void)loadWithAdUnitID:(NSString *)adUnit request:(GADRequest *)request;

/// Shows the rewarded interstitial ad.
- (void)show;

/// Options specified for server-to-server user reward verification.
- (void)setServerSideVerificationOptions:(GADServerSideVerificationOptions *)options;

@end
