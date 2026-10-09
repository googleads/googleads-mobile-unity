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
  BOOL _adDidDismissCallbackDeferred;
}

- (nonnull instancetype)initWithPictureInPictureAdClientReference:
    (_Nonnull GADUTypePictureInPictureAdClientRef *_Nonnull)pipAdClient {
  self = [super init];
  if (self) {
    _pipAdClient = pipAdClient;
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
}

- (void)handleDidBecomeActive:(NSNotification *)notification {
#if GMA_PREVIEW_FEATURES
  if (_adDidDismissCallbackDeferred) {
    [self pictureInPictureAdDidDismissScreen:self.pictureInPictureAd];
  }
#endif
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
#else
  if (self.adFailedToLoadCallback) {
    NSError *error =
        [NSError errorWithDomain:@"com.google.admob.unity"
                            code:1
                        userInfo:@{
                          NSLocalizedDescriptionKey :
                              @"Picture-in-Picture ads require GMA_PREVIEW_FEATURES to be enabled."
                        }];
    _lastLoadError = error;
    self.adFailedToLoadCallback(self.pipAdClient,
                                (__bridge GADUTypeErrorRef)error);
  }
#endif
}

- (void)showWithPosition:(GADPictureInPictureAdPosition)position {
#if GMA_PREVIEW_FEATURES
  if (!self.pictureInPictureAd) {
    return;
  }
  GADPictureInPictureAdOptions *options =
      [[GADPictureInPictureAdOptions alloc] init];
  options.position = position;
  [self.pictureInPictureAd showWithOptions:options];
#else
  if (self.adDidFailToShowCallback) {
    NSError *error =
        [NSError errorWithDomain:@"com.google.admob.unity"
                            code:1
                        userInfo:@{
                          NSLocalizedDescriptionKey :
                              @"Picture-in-Picture ads require GMA_PREVIEW_FEATURES to be enabled."
                        }];
    _lastPresentError = error;
    self.adDidFailToShowCallback(
        self.pipAdClient, (__bridge GADUTypeErrorRef)error);
  }
#endif
}

- (void)hide {
#if GMA_PREVIEW_FEATURES
  [self.pictureInPictureAd hide];
#endif
}

- (void)destroy {
#if GMA_PREVIEW_FEATURES
  [self.pictureInPictureAd hide];
  self.pictureInPictureAd = nil;
#endif
}

- (GADPictureInPictureAdPosition)position {
#if GMA_PREVIEW_FEATURES
  return self.pictureInPictureAd ?
      self.pictureInPictureAd.position : GADPictureInPictureAdPositionDefault;
#else
  return GADPictureInPictureAdPositionDefault;
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
  if (self.adDidFailToShowCallback) {
    _lastPresentError = error;
    self.adDidFailToShowCallback(
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
  extern bool _didResignActive;
  if (_didResignActive) {
    // We are in the middle of the shutdown sequence, and at this point unity runtime is already
    // destroyed. We shall not call unity API, and definitely not script callbacks, so nothing to do
    // here.
    _adDidDismissCallbackDeferred = YES;
    return;
  }
  _adDidDismissCallbackDeferred = NO;
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
