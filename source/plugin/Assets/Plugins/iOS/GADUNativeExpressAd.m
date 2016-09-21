// Copyright 2016 Google Inc. All Rights Reserved.

#import "GADUNativeExpressAd.h"

@import CoreGraphics;
@import UIKit;

#import "GADUPluginUtil.h"
#import "UnityAppController.h"

@interface GADUNativeExpressAd () <GADNativeExpressAdViewDelegate>

/// Defines where the ad should be positioned on the screen.
@property(nonatomic, assign) GADAdPosition adPosition;

@end

@implementation GADUNativeExpressAd

- (instancetype)initWithNativeExpressAdClientReference:
                    (GADUTypeNativeExpressAdClientRef *)nativeExpressAdClient
                                              adUnitID:(NSString *)adUnitID
                                                 width:(CGFloat)width
                                                height:(CGFloat)height
                                            adPosition:(GADAdPosition)adPosition {
  GADAdSize adSize = GADAdSizeFromCGSize(CGSizeMake(width, height));
  return [self initWithNativeExpressAdClientReference:nativeExpressAdClient
                                             adUnitID:adUnitID
                                               adSize:adSize
                                           adPosition:adPosition];
}

- (instancetype)initWithNativeExpressAdClientReference:
                    (GADUTypeNativeExpressAdClientRef *)nativeExpressAdClient
                                              adUnitID:(NSString *)adUnitID
                                                adSize:(GADAdSize)size
                                            adPosition:(GADAdPosition)adPosition {
  self = [super init];
  if (self) {
    _nativeExpressAdClient = nativeExpressAdClient;
    _adPosition = adPosition;
    _nativeExpressAdView = [[GADNativeExpressAdView alloc] initWithAdSize:size];
    _nativeExpressAdView.adUnitID = adUnitID;
    _nativeExpressAdView.delegate = self;
    _nativeExpressAdView.rootViewController = [GADUPluginUtil unityGLViewController];
  }
  return self;
}

- (void)dealloc {
  _nativeExpressAdView.delegate = nil;
}

- (void)loadRequest:(GADRequest *)request {
  if (!self.nativeExpressAdView) {
    NSLog(@"GoogleMobileAdsPlugin: NativeExpressAdView is nil. Ignoring ad request.");
    return;
  }
  [self.nativeExpressAdView loadRequest:request];
}

- (void)hideNativeExpressAdView {
  if (!self.nativeExpressAdView) {
    NSLog(@"GoogleMobileAdsPlugin: NativeExpressAdView is nil. Ignoring call to "
          @"hideNativeExpressAdView");
    return;
  }
  self.nativeExpressAdView.hidden = YES;
}

- (void)showNativeExpressAdView {
  if (!self.nativeExpressAdView) {
    NSLog(@"GoogleMobileAdsPlugin: NativeExpressAdView is nil. Ignoring call to "
          @"showNativeExpressAdView");
    return;
  }
  self.nativeExpressAdView.hidden = NO;
}

- (void)removeNativeExpressAdView {
  if (!self.nativeExpressAdView) {
    NSLog(@"GoogleMobileAdsPlugin: NativeExpressAdView is nil. Ignoring call to "
          @"removeNativeExpressAdView");
    return;
  }
  [self.nativeExpressAdView removeFromSuperview];
}

#pragma mark GADNativeExpressAdViewDelegate implementation

- (void)nativeExpressAdViewDidReceiveAd:(GADNativeExpressAdView *)nativeExpressAdView {
  // Remove existing native express ad view from superview.
  [self.nativeExpressAdView removeFromSuperview];

  // Add the new native express ad view.
  self.nativeExpressAdView = nativeExpressAdView;

  /// Align the nativeExpressAdView in the Unity view bounds.
  UIView *unityView = [GADUPluginUtil unityGLViewController].view;
  [GADUPluginUtil positionView:self.nativeExpressAdView
                inParentBounds:unityView.bounds
                withAdPosition:self.adPosition];
  [unityView addSubview:self.nativeExpressAdView];

  if (self.adReceivedCallback) {
    self.adReceivedCallback(self.nativeExpressAdClient);
  }
}

- (void)nativeExpressAdView:(GADNativeExpressAdView *)nativeExpressAdView
    didFailToReceiveAdWithError:(GADRequestError *)error {
  if (self.adFailedCallback) {
    NSString *errorMsg = [[NSString alloc]
        initWithFormat:@"Failed to receive ad with error: %@", [error localizedFailureReason]];
    self.adFailedCallback(self.nativeExpressAdClient,
                          [errorMsg cStringUsingEncoding:NSUTF8StringEncoding]);
  }
}

- (void)nativeExpressAdViewWillPresentScreen:(GADNativeExpressAdView *)nativeExpressAdView {
  if (self.willPresentScreenCallback) {
    self.willPresentScreenCallback(self.nativeExpressAdClient);
  }
}

- (void)nativeExpressAdViewWillDismissScreen:(GADNativeExpressAdView *)nativeExpressAdView {
  // Callback is not forwarded to Unity.
}

- (void)nativeExpressAdViewDidDismissScreen:(GADNativeExpressAdView *)nativeExpressAdView {
  if (self.didDismissScreenCallback) {
    self.didDismissScreenCallback(self.nativeExpressAdClient);
  }
}

- (void)nativeExpressAdViewWillLeaveApplication:(GADNativeExpressAdView *)nativeExpressAdView {
  if (self.willLeaveAppCallback) {
    self.willLeaveAppCallback(self.nativeExpressAdClient);
  }
}

@end
