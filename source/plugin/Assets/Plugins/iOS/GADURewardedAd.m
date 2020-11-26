// Copyright 2018 Google Inc. All Rights Reserved.

#import "GADURewardedAd.h"

#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

#import "GADUPluginUtil.h"
#import "UnityAppController.h"
#import "UnityInterface.h"

@interface GADURewardedAd () <GADRewardedAdDelegate>
@end

@implementation GADURewardedAd

+ (UIViewController *)unityGLViewController {
  UnityAppController *applicationDelegate = [UIApplication sharedApplication].delegate;
  return applicationDelegate.rootViewController;
}

- (instancetype)initWithRewardedAdClientReference:(GADUTypeRewardedAdClientRef *)rewardedAdClient
                                         adUnitID:(NSString *)adUnitID {
  self = [super init];
  if (self) {
    _rewardedAdClient = rewardedAdClient;
    _rewardedAd = [[GADRewardedAd alloc] initWithAdUnitID:adUnitID];

    __weak GADURewardedAd *weakSelf = self;
    _rewardedAd.paidEventHandler = ^void(GADAdValue *_Nonnull adValue) {
      GADURewardedAd *strongSelf = weakSelf;
      if (strongSelf.paidEventCallback) {
        int64_t valueInMicros =
            [adValue.value decimalNumberByMultiplyingByPowerOf10:6].longLongValue;
        strongSelf.paidEventCallback(
            strongSelf.rewardedAdClient, (int)adValue.precision, valueInMicros,
            [adValue.currencyCode cStringUsingEncoding:NSUTF8StringEncoding]);
      }
    };
  }
  return self;
}

- (void)loadRequest:(GADRequest *)request {
  [self.rewardedAd loadRequest:request
             completionHandler:^(GADRequestError *_Nullable error) {
               if (error) {
                 if (self.adFailedToLoadCallback) {

                   self.adFailedToLoadCallback(self.rewardedAdClient,
                        (__bridge GADUTypeErrorRef)error);
                 }
               } else {
                 if (self.adReceivedCallback) {
                   self.adReceivedCallback(self.rewardedAdClient);
                 }
               }
             }];
}

- (BOOL)isReady {
  return [self.rewardedAd isReady];
}

- (void)show {
  if ([self.rewardedAd isReady]) {
    UIViewController *unityController = [GADURewardedAd unityGLViewController];
    [self.rewardedAd presentFromRootViewController:unityController delegate:self];
  } else {
    NSLog(@"GoogleMobileAdsPlugin: Rewarded ad is not ready to be shown.");
  }
}

- (NSString *)mediationAdapterClassName {
  return self.rewardedAd.responseInfo.adNetworkClassName;
}

- (GADResponseInfo *)responseInfo {
  return self.rewardedAd.responseInfo;
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

- (void)rewardedAd:(nonnull GADRewardedAd *)rewardedAd
    didFailToPresentWithError:(nonnull GADRequestError *)error {
  if (self.adFailedToShowCallback) {
    self.adFailedToShowCallback(self.rewardedAdClient, (__bridge GADUTypeErrorRef)error);
  }
}

- (void)rewardedAdDidPresent:(nonnull GADRewardedAd *)rewardedAd {
  if ([GADUPluginUtil pauseOnBackground]) {
    UnityPause(YES);
  }

  if (self.didOpenCallback) {
    self.didOpenCallback(self.rewardedAdClient);
  }
}

- (void)rewardedAdDidDismiss:(nonnull GADRewardedAd *)rewardedAd {
  extern bool _didResignActive;
  if(_didResignActive) {
       // We are in the middle of the shutdown sequence, and at this point unity runtime is already destroyed.
       // We shall not call unity API, and definitely not script callbacks, so nothing to do here
    return;
  }
  if (UnityIsPaused()) {
    UnityPause(NO);
  }

  if (self.didCloseCallback) {
    self.didCloseCallback(self.rewardedAdClient);
  }
}

- (void)setServerSideVerificationOptions:(GADServerSideVerificationOptions *)options {
  self.rewardedAd.serverSideVerificationOptions = options;
}

@end
