// Copyright 2014 Google Inc. All Rights Reserved.

@import CoreGraphics;
@import Foundation;
@import GoogleMobileAds;
@import UIKit;

#import "UnityAppController.h"

#import "GADURewardBasedVideoAd.h"

@interface GADURewardBasedVideoAd () <GADRewardBasedVideoAdDelegate>
@end

@implementation GADURewardBasedVideoAd

+ (UIViewController *)unityGLViewController {
  UnityAppController *applicationDelegate = [UIApplication sharedApplication].delegate;
  return applicationDelegate.rootViewController;
}

- (instancetype)initWithRewardBasedVideoClientReference:
        (GADUTypeRewardBasedVideoAdClientRef *)rewardBasedVideoAdClient {
  self = [super init];
  if (self) {
    _rewardBasedVideoAdClient = rewardBasedVideoAdClient;
    _rewardBasedVideo = [GADRewardBasedVideoAd sharedInstance];
    _rewardBasedVideo.delegate = self;
  }
  return self;
}

- (void)dealloc {
  _rewardBasedVideo.delegate = nil;
}

- (void)loadRequest:(GADRequest *)request withAdUnitID:(NSString *)adUnitID {
  [self.rewardBasedVideo loadRequest:request withAdUnitID:adUnitID];
}

- (BOOL)isReady {
  return [self.rewardBasedVideo isReady];
}

- (void)show {
  if ([self.rewardBasedVideo isReady]) {
    UIViewController *unityController = [GADURewardBasedVideoAd unityGLViewController];
    [self.rewardBasedVideo presentFromRootViewController:unityController];
  } else {
    NSLog(@"GoogleMobileAdsPlugin: Reward based video ad is not ready to be shown.");
  }
}

#pragma mark GADRewardBasedVideoAdDelegate implementation

- (void)rewardBasedVideoAdDidReceiveAd:(GADRewardBasedVideoAd *)rewardBasedVideoAd {
  if (self.adReceivedCallback) {
    self.adReceivedCallback(self.rewardBasedVideoAdClient);
  }
}

- (void)rewardBasedVideoAd:(GADRewardBasedVideoAd *)rewardBasedVideoAd
    didFailToLoadWithError:(NSError *)error {
  NSString *errorMsg = [NSString
      stringWithFormat:@"Failed to receive ad with error: %@", [error localizedFailureReason]];
  self.adFailedCallback(self.rewardBasedVideoAdClient,
                        [errorMsg cStringUsingEncoding:NSUTF8StringEncoding]);
}

- (void)rewardBasedVideoAdDidOpen:(GADRewardBasedVideoAd *)rewardBasedVideoAd {
  if (self.didOpenCallback) {
    self.didOpenCallback(self.rewardBasedVideoAdClient);
  }
}

- (void)rewardBasedVideoAdDidStartPlaying:(GADRewardBasedVideoAd *)rewardBasedVideoAd {
  if (self.didStartPlayingCallback) {
    self.didStartPlayingCallback(self.rewardBasedVideoAdClient);
  }
}

- (void)rewardBasedVideoAdDidClose:(GADRewardBasedVideoAd *)rewardBasedVideoAd {
  if (self.didCloseCallback) {
    self.didCloseCallback(self.rewardBasedVideoAdClient);
  }
}

- (void)rewardBasedVideoAd:(GADRewardBasedVideoAd *)rewardBasedVideoAd
    didRewardUserWithReward:(GADAdReward *)reward {
  if (self.didRewardCallback) {
    // Integer value used for didRewardCallback callback to maintain consistency with Android
    // implementation.
    self.didRewardCallback(self.rewardBasedVideoAdClient,
                           [reward.type cStringUsingEncoding:NSUTF8StringEncoding],
                           reward.amount.doubleValue);
  }
}

- (void)rewardBasedVideoAdWillLeaveApplication:(GADRewardBasedVideoAd *)rewardBasedVideoAd {
  if (self.willLeaveCallback) {
    self.willLeaveCallback(self.rewardBasedVideoAdClient);
  }
}

@end
