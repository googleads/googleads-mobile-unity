// Copyright 2023 Google LLC. All Rights Reserved.

#import "GAMUInterstitial.h"

#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

#import "GADUPluginUtil.h"
#import "GAMUInterstitial.h"
#import "UnityInterface.h"


@interface GAMUInterstitial () <GADFullScreenContentDelegate, GADAppEventDelegate>
@end

@implementation GAMUInterstitial {
  // Keep a reference to the error objects so references to Unity-level ResponseInfo object are not
  // released until the ad object is released.
  NSError *_lastLoadError;
  NSError *_lastPresentError;
}

- (id)initWithAdManagerInterstitialClientReference:(GAMUTypeInterstitialClientRef * _Nonnull)
    interstitialClient {
  self = [super initWithInterstitialClientReference:interstitialClient];
  _interstitialClientGAM = interstitialClient;
  return self;
}

- (void)loadWithAdManagerAdUnitID:(nonnull NSString *)adUnitID
                          request:(nonnull GAMRequest *)request {
  __weak GAMUInterstitial *weakSelf = self;

  [GAMInterstitialAd
       loadWithAdManagerAdUnitID:adUnitID
                         request:request
      completionHandler:^(GAMInterstitialAd *_Nullable interstitialAd, NSError *_Nullable error) {
        GAMUInterstitial *strongSelf = weakSelf;
        if (!strongSelf) {
          return;
        }
        if (error || !interstitialAd) {
          if (strongSelf.adFailedToLoadCallback) {
            _lastLoadError = error;
            strongSelf.adFailedToLoadCallback(strongSelf.interstitialClient,
                                              (__bridge GADUTypeErrorRef)error);
          }
          return;
        }
        strongSelf.interstitialAd = interstitialAd;
        strongSelf.interstitialAdGAM = interstitialAd;
        strongSelf.interstitialAd.fullScreenContentDelegate = strongSelf;
        strongSelf.interstitialAdGAM.appEventDelegate = strongSelf;
        strongSelf.interstitialAd.paidEventHandler = ^void(GADAdValue *_Nonnull adValue) {
          GAMUInterstitial *strongSecondSelf = weakSelf;
          if (!strongSecondSelf) {
            return;
          }
          if (strongSecondSelf.paidEventCallback) {
            int64_t valueInMicros =
                [adValue.value decimalNumberByMultiplyingByPowerOf10:6].longLongValue;
            strongSecondSelf.paidEventCallback(
                strongSecondSelf.interstitialClient, (int)adValue.precision, valueInMicros,
                [adValue.currencyCode cStringUsingEncoding:NSUTF8StringEncoding]);
          }
        };
        if (strongSelf.adLoadedCallback) {
          strongSelf.adLoadedCallback(self.interstitialClient);
        }
      }];
}

#pragma mark GADAppEventDelegate Implementation

/// Called when the interstitial receives an app event.
- (void)interstitialAd:(nonnull GADInterstitialAd *)interstitialAd
    didReceiveAppEvent:(nonnull NSString *)name
              withInfo:(nullable NSString *)info {
  if (self.appEventCallback) {
    self.appEventCallback(self.interstitialClient, [name cStringUsingEncoding:NSUTF8StringEncoding],
        [info cStringUsingEncoding:NSUTF8StringEncoding]);
  }
}

@end
