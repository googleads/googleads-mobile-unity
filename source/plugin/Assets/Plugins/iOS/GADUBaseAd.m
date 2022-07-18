// Copyright 2014 Google Inc. All Rights Reserved.

#import "GADUBaseAd.h"

#import <Foundation/Foundation.h>

#import <CoreGraphics/CoreGraphics.h>
#import <GoogleMobileAds/GoogleMobileAds.h>
#import <UIKit/UIKit.h>
#import "UnityInterface.h"

#import "GADUBaseAd.h"
#import "GADUPluginUtil.h"
#import "GADUTypes.h"
#import "GADUObjectCache.h"

@implementation GADUBaseAd {
  // Keep a reference to the error objects so references to Unity-level
  // ResponseInfo object are not released until the ad object is released.
  NSError *_lastPresentError;
  GADResponseInfo * _responseInfo;
}

#pragma mark GADFullScreenContentDelegate implementation

- (void)adDidRecordImpression:(nonnull id<GADFullScreenPresentingAd>)ad {
  if (self.adImpressionCallback) {
    self.adImpressionCallback(self.adClient);
  }
}

- (void)adDidRecordClick:(nonnull id<GADFullScreenPresentingAd>)ad {
  if (self.adClickedCallback) {
    self.adClickedCallback(self.adClient);
  }
}

- (void)ad:(nonnull id<GADFullScreenPresentingAd>)ad
    didFailToPresentFullScreenContentWithError:(nonnull NSError *)error {
  if (self.adFullScreenFailedCallback) {
    _lastPresentError = error;
    self.adFullScreenFailedCallback(self.adClient, (__bridge GADUTypeErrorRef)error);
  }
}

- (void)adWillPresentFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad {
  if (GADUPluginUtil.pauseOnBackground) {
    UnityPause(YES);
  }
  if (self.adFullScreenOpenedCallback) {
    self.adFullScreenOpenedCallback(self.adClient);
  }
}

- (void)adWillDismissFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad {
  // Not calling into Unity.
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

  if (self.adFullScreenClosedCallback) {
    self.adFullScreenClosedCallback(self.adClient);
  }
}

@end

#pragma mark Unity Native Interop Methods

/// Sets the callback methods to be invoked during ad events.
void GADUBaseAdSetCallbacks(GADUTypeAdBridgeRef adBridgeRef, GADUBaseAdCallback adLoadCallback,
                            GADUBaseAdErrorCallback adLoadFailedCallback,
                            GADUBaseAdCallback adFullScreenOpenedCallback,
                            GADUBaseAdErrorCallback adFullScreenFailedCallback,
                            GADUBaseAdCallback adFullScreenClosedCallback,
                            GADUBaseAdCallback adImpressionCallback,
                            GADUBaseAdCallback adClickedCallback,
                            GADUBaseAdPaidCallback adPaidCallback,
                            GADUBaseAdRewardedCallback adUserEarnedRewardCallback) {
  GADUBaseAd *adBridge = (__bridge GADUBaseAd *)adBridgeRef;
  adBridge.adLoadCallback = adLoadCallback;
  adBridge.adLoadFailedCallback = adLoadFailedCallback;
  adBridge.adFullScreenOpenedCallback = adFullScreenOpenedCallback;
  adBridge.adFullScreenFailedCallback = adFullScreenFailedCallback;
  adBridge.adFullScreenClosedCallback = adFullScreenClosedCallback;
  adBridge.adImpressionCallback = adImpressionCallback;
  adBridge.adClickedCallback = adClickedCallback;
  adBridge.adPaidCallback = adPaidCallback;
  adBridge.adUserEarnedRewardCallback = adUserEarnedRewardCallback;
}

GADUTypeResponseInfoRef GADUBaseAdGetResponseInfo(GADUTypeAdBridgeRef adBridgeRef) {
  GADUBaseAd *adBridge = (__bridge GADUBaseAd *)adBridgeRef;
  GADResponseInfo *responseInfo = adBridge.responseInfo;

  // cache the response because it may live beyond the lifetime of the bridge.
  // this should be removed once ResponseInfo change is complete.
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  cache[responseInfo.gadu_referenceKey] = responseInfo;

  return (__bridge GADUTypeResponseInfoRef)responseInfo;
}
