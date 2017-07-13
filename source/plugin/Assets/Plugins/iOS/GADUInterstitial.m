// Copyright 2014 Google Inc. All Rights Reserved.

#import "GADUInterstitial.h"

#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

#import "GADUPluginUtil.h"
#import "UnityAppController.h"

@interface GADUInterstitial ()<GADInterstitialDelegate>
@end

@implementation GADUInterstitial

- (id)initWithInterstitialClientReference:(GADUTypeInterstitialClientRef *)interstitialClient
                                 adUnitID:(NSString *)adUnitID {
  self = [super init];
  if (self) {
    _interstitialClient = interstitialClient;
    _interstitial = [[GADInterstitial alloc] initWithAdUnitID:adUnitID];
    _interstitial.delegate = self;
  }
  return self;
}

- (void)dealloc {
  _interstitial.delegate = nil;
}

- (void)loadRequest:(GADRequest *)request {
  [self.interstitial loadRequest:request];
}

- (BOOL)isReady {
  return self.interstitial.isReady;
}

- (void)show {
  if (self.interstitial.isReady) {
    UIViewController *unityController = [GADUPluginUtil unityGLViewController];
    [self.interstitial presentFromRootViewController:unityController];
  } else {
    NSLog(@"GoogleMobileAdsPlugin: Interstitial is not ready to be shown.");
  }
}

#pragma mark GADInterstitialDelegate implementation

- (void)interstitialDidReceiveAd:(GADInterstitial *)ad {
  if (self.adReceivedCallback) {
    self.adReceivedCallback(self.interstitialClient);
  }
}
- (void)interstitial:(GADInterstitial *)ad didFailToReceiveAdWithError:(GADRequestError *)error {
  if (self.adFailedCallback) {
    NSString *errorMsg = [NSString
        stringWithFormat:@"Failed to receive ad with error: %@", [error localizedFailureReason]];
    self.adFailedCallback(self.interstitialClient,
                          [errorMsg cStringUsingEncoding:NSUTF8StringEncoding]);
  }
}

- (void)interstitialWillPresentScreen:(GADInterstitial *)ad {
  if (self.willPresentCallback) {
    self.willPresentCallback(self.interstitialClient);
  }
}

- (void)interstitialWillDismissScreen:(GADInterstitial *)ad {
  // Callback is not forwarded to Unity.
}

- (void)interstitialDidDismissScreen:(GADInterstitial *)ad {
  if (self.didDismissCallback) {
    self.didDismissCallback(self.interstitialClient);
  }
}

- (void)interstitialWillLeaveApplication:(GADInterstitial *)ad {
  if (self.willLeaveCallback) {
    self.willLeaveCallback(self.interstitialClient);
  }
}

@end
