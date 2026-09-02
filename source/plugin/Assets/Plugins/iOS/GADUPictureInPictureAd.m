// Copyright 2026 Google LLC. All Rights Reserved.

#import "GADUPictureInPictureAd.h"

#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

#import "GADUPluginUtil.h"
#import "UnityInterface.h"

#if GMA_PREVIEW_FEATURES
@interface GADUPictureInPictureAd () <GADPictureInPictureAdDelegate>
@end
#endif

@implementation GADUPictureInPictureAd {
  NSError *_lastLoadError;
  NSError *_lastPresentError;
}

- (nonnull instancetype)initWithPictureInPictureAdClientReference:
    (_Nonnull GADUTypePictureInPictureAdClientRef *_Nonnull)pipAdClient {
  self = [super init];
  if (self) {
    _pipAdClient = pipAdClient;
  }
  return self;
}

- (void)loadWithAdUnitID:(nonnull NSString *)adUnitID
                 request:(nonnull GADRequest *)request {
#if GMA_PREVIEW_FEATURES
  __weak GADUPictureInPictureAd *weakSelf = self;
  [GADPictureInPictureAd
      loadWithAdUnitID:adUnitID
               request:request
     completionHandler:^(GADPictureInPictureAd *_Nullable ad,
                         NSError *_Nullable error) {
       GADUPictureInPictureAd *strongSelf = weakSelf;
       if (!strongSelf) {
         return;
       }
       if (error) {
         if (strongSelf.adFailedToLoadCallback) {
           strongSelf->_lastLoadError = error;
           strongSelf.adFailedToLoadCallback(strongSelf.pipAdClient,
                                             (__bridge GADUTypeErrorRef)error);
         }
         return;
       }
       strongSelf.pictureInPictureAd = ad;
       strongSelf.pictureInPictureAd.delegate = strongSelf;
       [strongSelf configurePaidEventHandler];
       if (strongSelf.adLoadedCallback) {
         strongSelf.adLoadedCallback(strongSelf.pipAdClient);
       }
     }];
#endif
}

- (void)showWithPosition:(NSInteger)position {
#if GMA_PREVIEW_FEATURES
  if (!self.pictureInPictureAd) {
    return;
  }
  GADPictureInPictureAdOptions *options =
      [[GADPictureInPictureAdOptions alloc] init];
  options.position = (GADPictureInPictureAdPosition)position;
  [self.pictureInPictureAd showWithOptions:options];
#endif
}

- (void)hide {
#if GMA_PREVIEW_FEATURES
  [self.pictureInPictureAd hide];
#endif
}

- (NSInteger)position {
#if GMA_PREVIEW_FEATURES
  return self.pictureInPictureAd ?
      (NSInteger)self.pictureInPictureAd.position : 0;
#else
  return 0;
#endif
}

- (GADResponseInfo *)responseInfo {
#if GMA_PREVIEW_FEATURES
  return self.pictureInPictureAd.responseInfo;
#else
  return nil;
#endif
}

#if GMA_PREVIEW_FEATURES

#pragma mark - GADPictureInPictureAdDelegate implementation

- (void)pictureInPictureAdDidShow:
    (nonnull GADPictureInPictureAd *)pictureInPictureAd {
  if (self.adShownCallback) {
    self.adShownCallback(self.pipAdClient);
  }
}

- (void)pictureInPictureAdDidHide:
    (nonnull GADPictureInPictureAd *)pictureInPictureAd {
  if (self.adHiddenCallback) {
    self.adHiddenCallback(self.pipAdClient);
  }
}

- (void)pictureInPictureAdDidFailToShow:
    (nonnull GADPictureInPictureAd *)pictureInPictureAd
                              withError:(nonnull NSError *)error {
  if (self.adFailedToPresentFullScreenContentCallback) {
    _lastPresentError = error;
    self.adFailedToPresentFullScreenContentCallback(
        self.pipAdClient, (__bridge GADUTypeErrorRef)error);
  }
}

- (void)pictureInPictureAdDidRecordClick:
    (nonnull GADPictureInPictureAd *)pictureInPictureAd {
  if (self.adDidRecordClickCallback) {
    self.adDidRecordClickCallback(self.pipAdClient);
  }
}

- (void)pictureInPictureAdDidRecordImpression:
    (nonnull GADPictureInPictureAd *)pictureInPictureAd {
  if (self.adDidRecordImpressionCallback) {
    self.adDidRecordImpressionCallback(self.pipAdClient);
  }
}

- (void)pictureInPictureAdWillPresentScreen:
    (nonnull GADPictureInPictureAd *)pictureInPictureAd {
  if (GADUPluginUtil.pauseOnBackground) {
    UnityPause(YES);
  }

  if (self.adWillPresentFullScreenContentCallback) {
    self.adWillPresentFullScreenContentCallback(self.pipAdClient);
  }
}

- (void)pictureInPictureAdDidDismissScreen:
    (nonnull GADPictureInPictureAd *)pictureInPictureAd {
  if (UnityIsPaused()) {
    UnityPause(NO);
  }

  if (self.adDidDismissFullScreenContentCallback) {
    self.adDidDismissFullScreenContentCallback(self.pipAdClient);
  }
}

// Configures the paid event handler for the Picture-in-Picture ad.
- (void)configurePaidEventHandler {
  __weak GADUPictureInPictureAd *weakSelf = self;
  self.pictureInPictureAd.paidEventHandler =
      ^void(GADAdValue *_Nonnull adValue) {
        GADUPictureInPictureAd *strongSelf = weakSelf;
        if (!strongSelf) {
          return;
        }
        if (strongSelf.paidEventCallback) {
          NSDecimalNumber *microValue =
              [adValue.value decimalNumberByMultiplyingByPowerOf10:6];
          int64_t valueInMicros = microValue.longLongValue;
          strongSelf.paidEventCallback(
              strongSelf.pipAdClient, (int)adValue.precision, valueInMicros,
              [adValue.currencyCode
                  cStringUsingEncoding:NSUTF8StringEncoding]);
        }
      };
}

#endif  // GMA_PREVIEW_FEATURES

@end
