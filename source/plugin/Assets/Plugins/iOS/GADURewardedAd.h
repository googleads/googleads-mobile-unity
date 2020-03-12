// Copyright 2018 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADUTypes.h"

@interface GADURewardedAd : NSObject

/// Initializes a GADURewardedAd.
- (instancetype)initWithRewardedAdClientReference:(GADUTypeRewardedAdClientRef *)rewardedAdClient
                                         adUnitID:(NSString *)adUnitID;

/// The rewarded ad.
@property(nonatomic, strong) GADRewardedAd *rewardedAd;

/// A reference to the Unity rewarded ad client.
@property(nonatomic, assign) GADUTypeRewardedAdClientRef *rewardedAdClient;

/// The ad received callback into Unity.
@property(nonatomic, assign) GADURewardedAdDidReceiveAdCallback adReceivedCallback;

/// The ad request failed callback into Unity.
@property(nonatomic, assign)
    GADURewardedAdDidFailToReceiveAdWithErrorCallback adFailedToLoadCallback;

/// The ad request failed callback into Unity.
@property(nonatomic, assign) GADURewardedAdDidFailToShowAdWithErrorCallback adFailedToShowCallback;

/// The as was opened callback into Unity.
@property(nonatomic, assign) GADURewardedAdDidOpenCallback didOpenCallback;

/// The ad was closed callback into Unity.
@property(nonatomic, assign) GADURewardedAdDidCloseCallback didCloseCallback;

/// The user was rewarded callback into Unity.
@property(nonatomic, assign) GADUUserEarnedRewardCallback didEarnRewardCallback;

/// The paid event callback into Unity.
@property(nonatomic, assign) GADURewardedAdPaidEventCallback paidEventCallback;

// Returns the mediation adapter class name.
@property(nonatomic, readonly, copy) NSString *mediationAdapterClassName;

/// Makes an ad request. Additional targeting options can be supplied with a request object.
- (void)loadRequest:(GADRequest *)request;

/// Returns YES if the rewarded ad is ready to be displayed.
- (BOOL)isReady;

/// Shows the rewarded ad.
- (void)show;

/// Options specified for server-to-server user reward verification.
- (void)setServerSideVerificationOptions:(GADServerSideVerificationOptions *)options;

@end
