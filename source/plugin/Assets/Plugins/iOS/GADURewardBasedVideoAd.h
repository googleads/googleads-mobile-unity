// Copyright 2014 Google Inc. All Rights Reserved.

@import Foundation;
@import GoogleMobileAds;

#import "GADUTypes.h"

@interface GADURewardBasedVideoAd : NSObject

/// Initializes a GADURewardBasedVideoAd.
- (instancetype)initWithRewardBasedVideoClientReference:
        (GADUTypeRewardBasedVideoAdClientRef *)rewardBasedVideoAdClient;

/// The reward based video ad.
@property(nonatomic, strong) GADRewardBasedVideoAd *rewardBasedVideo;

/// A reference to the Unity reward based video ad client.
@property(nonatomic, assign) GADUTypeRewardBasedVideoAdClientRef *rewardBasedVideoAdClient;

/// The ad received callback into Unity.
@property(nonatomic, assign) GADURewardBasedVideoAdDidReceiveAdCallback adReceivedCallback;

/// The ad failed callback into Unity.
@property(nonatomic, assign)
    GADURewardBasedVideoAdDidFailToReceiveAdWithErrorCallback adFailedCallback;

/// The as was opened callback into Unity.
@property(nonatomic, assign) GADURewardBasedVideoAdDidOpenCallback didOpenCallback;

/// The ad started playing callback into Unity.
@property(nonatomic, assign) GADURewardBasedVideoAdDidStartPlayingCallback didStartPlayingCallback;

/// The ad was closed callback into Unity.
@property(nonatomic, assign) GADURewardBasedVideoAdDidCloseCallback didCloseCallback;

/// The user was rewarded callback into Unity.
@property(nonatomic, assign) GADURewardBasedVideoAdDidRewardCallback didRewardCallback;

/// The will leave application callback into Unity.
@property(nonatomic, assign) GADURewardBasedVideoAdWillLeaveApplicationCallback willLeaveCallback;

/// Makes an ad request. Additional targeting options can be supplied with a request object.
- (void)loadRequest:(GADRequest *)request withAdUnitID:(NSString *)adUnitID;

/// Returns YES if the reward based video is ready to be displayed.
- (BOOL)isReady;

/// Shows the reward based video ad.
- (void)show;

@end
