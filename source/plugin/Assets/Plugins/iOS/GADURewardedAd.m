// Copyright 2018 Google Inc. All Rights Reserved.

#import "GADURewardedAd.h"

#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

#import "GADUPluginUtil.h"
#import "UnityInterface.h"

@interface GADURewardedAd () <GADFullScreenContentDelegate>
@end

@implementation GADURewardedAd {
  // Keep a reference to the error objects so references to Unity-level
  // ResponseInfo object are not released until the ad object is released.
  NSError *_lastLoadError;
  NSError *_lastPresentError;
}

- (instancetype)initWithRewardedAdClientReference:(GADUTypeRewardedAdClientRef *)rewardedAdClient {
  self = [super init];
  _rewardedAdClient = rewardedAdClient;
  return self;
}

- (void)setRewardedAdAndConfigure:(GADRewardedAd *)rewardedAd {
  if (self.rewardedAd == rewardedAd) {
    return;
  }
  self.rewardedAd = rewardedAd;
  self.rewardedAd.fullScreenContentDelegate = self;
  [self configurePaidEventHandler];
}

#if GMA_PREVIEW_FEATURES

+ (BOOL)isPreloadedAdAvailable:(NSString *)adUnitID {
  return [GADRewardedAd isPreloadedAdAvailable:adUnitID];
}

- (void)preloadedAdWithAdUnitID:(nonnull NSString *)adUnitID {
  GADRewardedAd *rewardedAd = [GADRewardedAd preloadedAdWithAdUnitID:adUnitID];
  if (!rewardedAd) {
    NSLog(@"Preloaded ad failed to load for ad unit ID: %@", adUnitID);
    return;
  }
  [self setRewardedAdAndConfigure:rewardedAd];
}

#endif  // GMA_PREVIEW_FEATURES

- (void)loadWithAdUnitID:(nonnull NSString *)adUnitID request:(nonnull GADRequest *)request {
  __weak GADURewardedAd *weakSelf = self;
  [GADRewardedAd loadWithAdUnitID:adUnitID
                          request:request
                completionHandler:^(GADRewardedAd *_Nullable rewardedAd, NSError *_Nullable error) {
                  GADURewardedAd *strongSelf = weakSelf;
                  if (!strongSelf) {
                    return;
                  }
                  if (error || !rewardedAd) {
                    if (strongSelf.adFailedToLoadCallback) {
                      strongSelf->_lastLoadError = error;
                      strongSelf.adFailedToLoadCallback(strongSelf.rewardedAdClient,
                                                        (__bridge GADUTypeErrorRef)error);
                    }
                    return;
                  }
                  [strongSelf setRewardedAdAndConfigure:rewardedAd];
                  if (strongSelf.adLoadedCallback) {
                    strongSelf.adLoadedCallback(strongSelf.rewardedAdClient);
                  }
                }];
}

- (void)show {
  UIViewController *unityController = [GADUPluginUtil unityGLViewController];
  __weak GADURewardedAd *weakSelf = self;

  [self.rewardedAd
      presentFromRootViewController:unityController
           userDidEarnRewardHandler:^void() {
             GADURewardedAd *strongSelf = weakSelf;
             if (!strongSelf) {
               return;
             }
             if (strongSelf.didEarnRewardCallback) {
               strongSelf.didEarnRewardCallback(
                   strongSelf.rewardedAdClient,
                   [strongSelf.rewardedAd.adReward.type cStringUsingEncoding:NSUTF8StringEncoding],
                   strongSelf.rewardedAd.adReward.amount.doubleValue);
             }
           }];
}

- (GADResponseInfo *)responseInfo {
  return self.rewardedAd.responseInfo;
}

- (int64_t)placementID {
  return _rewardedAd.placementID;
}

- (void)setPlacementID:(int64_t)placementID {
  _rewardedAd.placementID = placementID;
}

- (void)rewardedAd:(nonnull GADRewardedAd *)rewardedAd
    userDidEarnReward:(nonnull GADAdReward *)reward {
  if (self.didEarnRewardCallback) {
    // Double value used for didEarnRewardCallback callback to maintain consistency with Android
    // implementation.
    self.didEarnRewardCallback(self.rewardedAdClient,
                               [reward.type cStringUsingEncoding:NSUTF8StringEncoding],
                               reward.amount.doubleValue);
  }
}

- (void)setServerSideVerificationOptions:(GADServerSideVerificationOptions *)options {
  self.rewardedAd.serverSideVerificationOptions = options;
}

#pragma mark GADFullScreenContentDelegate implementation

- (void)ad:(nonnull id<GADFullScreenPresentingAd>)ad
    didFailToPresentFullScreenContentWithError:(nonnull NSError *)error {
  if (self.adFailedToPresentFullScreenContentCallback) {
    _lastPresentError = error;
    self.adFailedToPresentFullScreenContentCallback(self.rewardedAdClient,
                                                    (__bridge GADUTypeErrorRef)error);
  }
}

- (void)adWillPresentFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad {
  if (GADUPluginUtil.pauseOnBackground) {
    UnityPause(YES);
  }
  if (self.adWillPresentFullScreenContentCallback) {
    self.adWillPresentFullScreenContentCallback(self.rewardedAdClient);
  }
}

- (void)adDidDismissFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad {
  extern bool _didResignActive;
  if (_didResignActive) {
    // We are in the middle of the shutdown sequence, and at this point unity runtime is already
    // destroyed. We shall not call unity API, and definitely not script callbacks, so nothing to do
    // here
    return;
  }
  if (UnityIsPaused()) {
    UnityPause(NO);
  }

  if (self.adDidDismissFullScreenContentCallback) {
    self.adDidDismissFullScreenContentCallback(self.rewardedAdClient);
  }
}

- (void)adDidRecordImpression:(nonnull id<GADFullScreenPresentingAd>)ad {
  if (self.adDidRecordImpressionCallback) {
    self.adDidRecordImpressionCallback(self.rewardedAdClient);
  }
}

- (void)adDidRecordClick:(nonnull id<GADFullScreenPresentingAd>)ad {
  if (self.adDidRecordClickCallback) {
    self.adDidRecordClickCallback(self.rewardedAdClient);
  }
}

/// Helper method to configure the paid event handler for the rewarded ad.
- (void)configurePaidEventHandler {
  __weak GADURewardedAd *weakSelf = self;
  self.rewardedAd.paidEventHandler = ^void(GADAdValue *_Nonnull adValue) {
    GADURewardedAd *strongSelf = weakSelf;
    if (!strongSelf) {
      return;
    }
    if (strongSelf.paidEventCallback) {
      int64_t valueInMicros = [adValue.value decimalNumberByMultiplyingByPowerOf10:6].longLongValue;
      strongSelf.paidEventCallback(
          strongSelf.rewardedAdClient, (int)adValue.precision, valueInMicros,
          [adValue.currencyCode cStringUsingEncoding:NSUTF8StringEncoding]);
    }
  };
}

@end
