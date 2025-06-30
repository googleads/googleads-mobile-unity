// Copyright 2014 Google Inc. All Rights Reserved.

#import "GADUInterstitial.h"

#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

#import "GADUPluginUtil.h"
#import "UnityInterface.h"

@interface GADUInterstitial () <GADFullScreenContentDelegate>
@end

@implementation GADUInterstitial {
  // Keep a reference to the error objects so references to Unity-level
  // ResponseInfo object are not released until the ad object is released.
  NSError *_lastLoadError;
  NSError *_lastPresentError;
}

- (id)initWithInterstitialClientReference:(GADUTypeInterstitialClientRef *)interstitialClient {
  self = [super init];
  _interstitialClient = interstitialClient;
  return self;
}

- (void)setInterstitialAdAndConfigure:(GADInterstitialAd *)interstitialAd {
  if (self.interstitialAd == interstitialAd) {
    return;
  }
  self.interstitialAd = interstitialAd;
  self.interstitialAd.fullScreenContentDelegate = self;
  [self configurePaidEventHandler];
}

+ (BOOL)isPreloadedAdAvailable:(NSString *)adUnitID {
  return [GADInterstitialAd isPreloadedAdAvailable:adUnitID];
}

- (void)preloadedAdWithAdUnitID:(nonnull NSString *)adUnitID {
  GADInterstitialAd *interstitialAd = [GADInterstitialAd preloadedAdWithAdUnitID:adUnitID];
  if (!interstitialAd) {
    NSLog(@"Preloaded ad failed to load for ad unit ID: %@", adUnitID);
    return;
  }
  [self setInterstitialAdAndConfigure:interstitialAd];
}

- (void)loadWithAdUnitID:(nonnull NSString *)adUnitID request:(nonnull GADRequest *)request {
  __weak GADUInterstitial *weakSelf = self;
  [GADInterstitialAd
       loadWithAdUnitID:adUnitID
                request:request
      completionHandler:^(GADInterstitialAd *_Nullable interstitialAd, NSError *_Nullable error) {
        GADUInterstitial *strongSelf = weakSelf;
        if (!strongSelf) {
          return;
        }
        if (error || !interstitialAd) {
          if (strongSelf.adFailedToLoadCallback) {
            strongSelf->_lastLoadError = error;
            strongSelf.adFailedToLoadCallback(strongSelf.interstitialClient,
                                              (__bridge GADUTypeErrorRef)error);
          }
          return;
        }
        [strongSelf setInterstitialAdAndConfigure:interstitialAd];
        if (strongSelf.adLoadedCallback) {
          strongSelf.adLoadedCallback(strongSelf.interstitialClient);
        }
      }];
}

- (void)show {
  UIViewController *unityController = [GADUPluginUtil unityGLViewController];
  [self.interstitialAd presentFromRootViewController:unityController];
}

- (GADResponseInfo *)responseInfo {
  return self.interstitialAd.responseInfo;
}

#pragma mark GADFullScreenContentDelegate implementation

- (void)ad:(nonnull id<GADFullScreenPresentingAd>)ad
    didFailToPresentFullScreenContentWithError:(nonnull NSError *)error {
  if (self.adFailedToPresentFullScreenContentCallback) {
    _lastPresentError = error;
    self.adFailedToPresentFullScreenContentCallback(self.interstitialClient,
                                                    (__bridge GADUTypeErrorRef)error);
  }
}

- (void)adWillPresentFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad {
  if (GADUPluginUtil.pauseOnBackground) {
    UnityPause(YES);
  }
  if (self.adWillPresentFullScreenContentCallback) {
    self.adWillPresentFullScreenContentCallback(self.interstitialClient);
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
    self.adDidDismissFullScreenContentCallback(self.interstitialClient);
  }
}

- (void)adDidRecordImpression:(nonnull id<GADFullScreenPresentingAd>)ad {
  if (self.adDidRecordImpressionCallback) {
    self.adDidRecordImpressionCallback(self.interstitialClient);
  }
}

- (void)adDidRecordClick:(nonnull id<GADFullScreenPresentingAd>)ad {
  if (self.adDidRecordClickCallback) {
    self.adDidRecordClickCallback(self.interstitialClient);
  }
}

/// Helper method to configure the paid event handler for the interstitial ad.
- (void)configurePaidEventHandler {
  __weak GADUInterstitial *weakSelf = self;
  self.interstitialAd.paidEventHandler = ^void(GADAdValue *_Nonnull adValue) {
    GADUInterstitial *strongSelf = weakSelf;
    if (!strongSelf) {
      return;
    }
    if (strongSelf.paidEventCallback) {
      int64_t valueInMicros = [adValue.value decimalNumberByMultiplyingByPowerOf10:6].longLongValue;
      strongSelf.paidEventCallback(
          strongSelf.interstitialClient, (int)adValue.precision, valueInMicros,
          [adValue.currencyCode cStringUsingEncoding:NSUTF8StringEncoding]);
    }
  };
}

@end
