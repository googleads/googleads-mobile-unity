// Copyright 2026 Google LLC. All Rights Reserved.

#import "GADUSqueezebackAd.h"

#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

#import "GADUPluginUtil.h"
#import "UnityInterface.h"

@interface GADUSqueezebackAd () <GADSqueezebackAdDelegate>
@end

@implementation GADUSqueezebackAd {
  NSError *_lastLoadError;
  NSError *_lastPresentError;
  BOOL _adDidDismissCallbackDeferred;
}

- (nonnull instancetype)initWithSqueezebackAdClientReference:
    (_Nonnull GADUTypeSqueezebackAdClientRef *_Nonnull)squeezebackAdClient {
  self = [super init];
  if (self) {
    _squeezebackAdClient = squeezebackAdClient;
    [[NSNotificationCenter defaultCenter]
        addObserver:self
           selector:@selector(handleDidBecomeActive:)
               name:UIApplicationDidBecomeActiveNotification
             object:nil];
  }
  return self;
}

- (void)dealloc {
  [[NSNotificationCenter defaultCenter] removeObserver:self];
  [self destroy];
}

- (void)handleDidBecomeActive:(NSNotification *)notification {
  if (_adDidDismissCallbackDeferred) {
    [self squeezebackAdDidDismissScreen:self.squeezebackAd];
  }
}

- (void)loadWithAdUnitID:(nonnull NSString *)adUnitID
                 request:(nonnull GADRequest *)request {
  __weak GADUSqueezebackAd *weakSelf = self;
  [GADSqueezebackAd
      loadWithAdUnitID:adUnitID
               request:request
     completionHandler:^(GADSqueezebackAd *_Nullable ad,
                         NSError *_Nullable error) {
       GADUSqueezebackAd *strongSelf = weakSelf;
       if (!strongSelf) {
         return;
       }
       if (error) {
         if (strongSelf.adFailedToLoadCallback) {
           strongSelf->_lastLoadError = error;
           strongSelf.adFailedToLoadCallback(strongSelf.squeezebackAdClient,
                                             (__bridge GADUTypeErrorRef)error);
         }
         return;
       }
       strongSelf.squeezebackAd = ad;
       strongSelf.squeezebackAd.delegate = strongSelf;
       if (strongSelf.adLoadedCallback) {
         strongSelf.adLoadedCallback(strongSelf.squeezebackAdClient);
       }
     }];
}

- (void)show {
  if (!self.squeezebackAd) {
    return;
  }
  GADSqueezebackAdOptions *options =
      [[GADSqueezebackAdOptions alloc] init];
  [self.squeezebackAd showWithOptions:options];
}

- (void)hide {
  [self.squeezebackAd hide];
}

- (void)destroy {
  if (self.squeezebackAd) {
    self.squeezebackAd.delegate = nil;
    [self.squeezebackAd hide];
    self.squeezebackAd = nil;
  }
}

- (GADResponseInfo *)responseInfo {
  return self.squeezebackAd.responseInfo;
}

#pragma mark - GADSqueezebackAdDelegate implementation

- (void)squeezebackAdDidShow:
    (nonnull GADSqueezebackAd *)squeezebackAd {
  if (self.adShownCallback) {
    self.adShownCallback(self.squeezebackAdClient);
  }
}

- (void)squeezebackAdDidHide:
    (nonnull GADSqueezebackAd *)squeezebackAd {
  if (self.adHiddenCallback) {
    self.adHiddenCallback(self.squeezebackAdClient);
  }
}

- (void)squeezebackAdDidFailToShow:
    (nonnull GADSqueezebackAd *)squeezebackAd
                         withError:(nonnull NSError *)error {
  if (self.adDidFailToShowCallback) {
    _lastPresentError = error;
    self.adDidFailToShowCallback(
        self.squeezebackAdClient, (__bridge GADUTypeErrorRef)error);
  }
}

- (void)squeezebackAdDidRecordClick:
    (nonnull GADSqueezebackAd *)squeezebackAd {
  if (self.adDidRecordClickCallback) {
    self.adDidRecordClickCallback(self.squeezebackAdClient);
  }
}

- (void)squeezebackAdDidRecordImpression:
    (nonnull GADSqueezebackAd *)squeezebackAd {
  if (self.adDidRecordImpressionCallback) {
    self.adDidRecordImpressionCallback(self.squeezebackAdClient);
  }
}

- (void)squeezebackAd:(nonnull GADSqueezebackAd *)squeezebackAd
    didRecordPaidEventWithAdValue:(nonnull GADAdValue *)value {
  if (self.paidEventCallback) {
    NSDecimalNumber *microValue =
        [value.value decimalNumberByMultiplyingByPowerOf10:6];
    int64_t valueInMicros = microValue.longLongValue;
    self.paidEventCallback(
        self.squeezebackAdClient, (int)value.precision, valueInMicros,
        [value.currencyCode cStringUsingEncoding:NSUTF8StringEncoding]);
  }
}

- (void)squeezebackAdWillPresentScreen:
    (nonnull GADSqueezebackAd *)squeezebackAd {
  if (GADUPluginUtil.pauseOnBackground) {
    UnityPause(YES);
  }

  if (self.adWillPresentFullScreenContentCallback) {
    self.adWillPresentFullScreenContentCallback(self.squeezebackAdClient);
  }
}

- (void)squeezebackAdDidDismissScreen:
    (nonnull GADSqueezebackAd *)squeezebackAd {
  extern bool _didResignActive;
  if (_didResignActive) {
    _adDidDismissCallbackDeferred = YES;
    return;
  }
  _adDidDismissCallbackDeferred = NO;
  if (UnityIsPaused()) {
    UnityPause(NO);
  }

  if (self.adDidDismissFullScreenContentCallback) {
    self.adDidDismissFullScreenContentCallback(self.squeezebackAdClient);
  }
}

@end
