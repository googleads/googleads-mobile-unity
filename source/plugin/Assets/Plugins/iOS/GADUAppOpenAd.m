// Copyright 2021 Google LLC. All Rights Reserved.

#import "GADUAppOpenAd.h"

#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

#import "GADUPluginUtil.h"
#import "UnityInterface.h"

@interface GADUAppOpenAd () <GADFullScreenContentDelegate>
@end

@implementation GADUAppOpenAd {
  // Keep a reference to the error objects so references to Unity-level
  // ResponseInfo object are not released until the ad object is released.
  NSError *_lastLoadError;
  NSError *_lastPresentError;
}

- (nonnull instancetype)initWithAppOpenAdClientReference:(_Nonnull GADUTypeAppOpenAdClientRef *_Nonnull)appOpenAdClient {
  self = [super init];
  _appOpenAdClient = appOpenAdClient;
  return self;
}

+ (BOOL)isPreloadedAdAvailable:(NSString *)adUnitID {
  return [GADAppOpenAd isPreloadedAdAvailable:adUnitID];
}

- (void)preloadedAdWithAdUnitID:(nonnull NSString *)adUnitID {
  self.appOpenAd = [GADAppOpenAd preloadedAdWithAdUnitID:adUnitID];
  if (!self.appOpenAd) {
    NSLog(@"Preloaded ad failed to load for ad unit ID: %@", adUnitID);
    return;
  }
  self.appOpenAd.fullScreenContentDelegate = self;

  __weak GADUAppOpenAd *weakSelf = self;
  self.appOpenAd.paidEventHandler = ^void(GADAdValue *_Nonnull adValue) {
    GADUAppOpenAd *strongSelf = weakSelf;
    if (!strongSelf) {
      return;
    }
    if (strongSelf.paidEventCallback) {
      int64_t valueInMicros = [adValue.value decimalNumberByMultiplyingByPowerOf10:6].longLongValue;
      strongSelf.paidEventCallback(
          strongSelf.appOpenAdClient, (int)adValue.precision, valueInMicros,
          [adValue.currencyCode cStringUsingEncoding:NSUTF8StringEncoding]);
    }
  };
}

- (void)loadWithAdUnitID:(nonnull NSString *)adUnit request:(nonnull GADRequest *)request {
  __weak GADUAppOpenAd *weakSelf = self;

  [GADAppOpenAd loadWithAdUnitID:adUnit
                         request:request
               completionHandler:^(GADAppOpenAd *_Nullable appOpenAd, NSError *_Nullable error) {
                 GADUAppOpenAd *strongSelf = weakSelf;
                 if (!strongSelf) {
                   return;
                 }
                 if (error) {
                   if (strongSelf.adFailedToLoadCallback) {
                     self->_lastLoadError = error;
                     strongSelf.adFailedToLoadCallback(strongSelf.appOpenAdClient,
                                                       (__bridge GADUTypeErrorRef)error);
                   }
                   return;
                 }
                 strongSelf.appOpenAd = appOpenAd;
                 strongSelf.appOpenAd.fullScreenContentDelegate = strongSelf;
                 strongSelf.appOpenAd.paidEventHandler = ^void(GADAdValue *_Nonnull adValue) {
                   GADUAppOpenAd *strongSecondSelf = weakSelf;
                   if (!strongSecondSelf) {
                     return;
                   }
                   if (strongSecondSelf.paidEventCallback) {
                     int64_t valueInMicros =
                         [adValue.value decimalNumberByMultiplyingByPowerOf10:6].longLongValue;
                     strongSecondSelf.paidEventCallback(
                         strongSecondSelf.appOpenAdClient, (int)adValue.precision, valueInMicros,
                         [adValue.currencyCode cStringUsingEncoding:NSUTF8StringEncoding]);
                   }
                 };
                 if (strongSelf.adLoadedCallback) {
                   strongSelf.adLoadedCallback(self.appOpenAdClient);
                 }
               }];
}

- (void)show {
  UIViewController *unityController = [GADUPluginUtil unityGLViewController];
  [self.appOpenAd presentFromRootViewController:unityController];
}

- (GADResponseInfo *)responseInfo {
  return self.appOpenAd.responseInfo;
}

#pragma mark GADFullScreenContentDelegate implementation

- (void)ad:(nonnull id<GADFullScreenPresentingAd>)ad
    didFailToPresentFullScreenContentWithError:(nonnull NSError *)error {
  if (self.adFailedToPresentFullScreenContentCallback) {
    _lastPresentError = error;
    self.adFailedToPresentFullScreenContentCallback(self.appOpenAdClient,
                                                    (__bridge GADUTypeErrorRef)error);
  }
}

- (void)adWillPresentFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad {
  if (GADUPluginUtil.pauseOnBackground) {
    UnityPause(YES);
  }

  if (self.adWillPresentFullScreenContentCallback) {
    self.adWillPresentFullScreenContentCallback(self.appOpenAdClient);
  }
}

- (void)adDidRecordImpression:(nonnull id<GADFullScreenPresentingAd>)ad {
  if (self.adDidRecordImpressionCallback) {
    self.adDidRecordImpressionCallback(self.appOpenAdClient);
  }
}

- (void)adDidRecordClick:(nonnull id<GADFullScreenPresentingAd>)ad {
  if (self.adDidRecordClickCallback) {
    self.adDidRecordClickCallback(self.appOpenAdClient);
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
    self.adDidDismissFullScreenContentCallback(self.appOpenAdClient);
  }
}

@end
