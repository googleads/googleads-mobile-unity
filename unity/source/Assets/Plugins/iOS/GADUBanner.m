// Copyright 2014 Google Inc. All Rights Reserved.

#import <CoreGraphics/CoreGraphics.h>
#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#import "GADUBanner.h"

#import "GADAdMobExtras.h"
#import "GADAdSize.h"
#import "GADBannerView.h"
#import "GADBannerViewDelegate.h"
#import "UnityAppController.h"

@interface GADUBanner ()<GADBannerViewDelegate>

/// Defines where the ad should be positioned on the screen.
@property(nonatomic, assign) GADAdPosition adPosition;

@end

@implementation GADUBanner

/// Returns the Unity view controller.
+ (UIViewController *)unityGLViewController {
  return ((UnityAppController *)[UIApplication sharedApplication].delegate).rootViewController;
}

- (id)initWithBannerClientReference:(GADUTypeBannerClientRef *)bannerClient
                           adUnitID:(NSString *)adUnitID
                              width:(CGFloat)width
                             height:(CGFloat)height
                         adPosition:(GADAdPosition)adPosition {
  GADAdSize adSize = GADAdSizeFromCGSize(CGSizeMake(width, height));
  return [self initWithBannerClientReference:bannerClient
                                    adUnitID:adUnitID
                                      adSize:adSize
                                  adPosition:adPosition];
}

- (id)initWithSmartBannerSizeAndBannerClientReference:(GADUTypeBannerClientRef *)bannerClient
                                             adUnitID:(NSString *)adUnitID
                                           adPosition:(GADAdPosition)adPosition {
  // Choose the correct Smart Banner constant according to orientation.
  UIDeviceOrientation currentOrientation = [[UIDevice currentDevice] orientation];
  GADAdSize adSize;
  if (UIInterfaceOrientationIsPortrait(currentOrientation)) {
    adSize = kGADAdSizeSmartBannerPortrait;
  } else {
    adSize = kGADAdSizeSmartBannerLandscape;
  }
  return [self initWithBannerClientReference:bannerClient
                                    adUnitID:adUnitID
                                      adSize:adSize
                                  adPosition:adPosition];
}

- (id)initWithBannerClientReference:(GADUTypeBannerClientRef *)bannerClient
                           adUnitID:(NSString *)adUnitID
                             adSize:(GADAdSize)size
                         adPosition:(GADAdPosition)adPosition {
  self = [super init];
  if (self) {
    _bannerClient = bannerClient;
    _adPosition = adPosition;
    _bannerView = [[GADBannerView alloc] initWithAdSize:size];
    _bannerView.adUnitID = adUnitID;
    _bannerView.delegate = self;
    _bannerView.rootViewController = [GADUBanner unityGLViewController];
  }
  return self;
}

- (void)loadRequest:(GADRequest *)request {
  if (!self.bannerView) {
    NSLog(@"GoogleMobileAdsPlugin: BannerView is nil. Ignoring ad request.");
    return;
  }
  [self.bannerView loadRequest:request];
}

- (void)hideBannerView {
  if (!self.bannerView) {
    NSLog(@"GoogleMobileAdsPlugin: BannerView is nil. Ignoring call to hideBannerView");
    return;
  }
  self.bannerView.hidden = YES;
}

- (void)showBannerView {
  if (!self.bannerView) {
    NSLog(@"GoogleMobileAdsPlugin: BannerView is nil. Ignoring call to showBannerView");
    return;
  }
  self.bannerView.hidden = NO;
}

- (void)removeBannerView {
  if (!self.bannerView) {
    NSLog(@"GoogleMobileAdsPlugin: BannerView is nil. Ignoring call to removeBannerView");
    return;
  }
  [self.bannerView removeFromSuperview];
}

#pragma mark GADBannerViewDelegate implementation

- (void)adViewDidReceiveAd:(GADBannerView *)adView {
  UIView *unityView = [[GADUBanner unityGLViewController] view];
  CGPoint center = CGPointMake(CGRectGetMidX(unityView.bounds), CGRectGetMidY(_bannerView.bounds));
  // Position the GADBannerView.
  switch (self.adPosition) {
    case kGADAdPositionTopOfScreen:
      center = CGPointMake(CGRectGetMidX(unityView.bounds), CGRectGetMidY(_bannerView.bounds));
      break;
    case kGADAdPositionBottomOfScreen:
      center = CGPointMake(CGRectGetMidX(unityView.bounds),
                           CGRectGetMaxY(unityView.bounds) - CGRectGetMidY(_bannerView.bounds));
      break;
  }

  // Remove existing banner view from superview.
  [self.bannerView removeFromSuperview];

  // Add the new banner view.
  self.bannerView = adView;
  self.bannerView.center = center;
  [unityView addSubview:self.bannerView];

  if (self.adReceivedCallback) {
    self.adReceivedCallback(self.bannerClient);
  }
}

- (void)adView:(GADBannerView *)view didFailToReceiveAdWithError:(GADRequestError *)error {
  if (self.adFailedCallback) {
    NSString *errorMsg = [NSString
        stringWithFormat:@"Failed to receive ad with error: %@", [error localizedFailureReason]];
    self.adFailedCallback(self.bannerClient, [errorMsg cStringUsingEncoding:NSUTF8StringEncoding]);
  }
}

- (void)adViewWillPresentScreen:(GADBannerView *)adView {
  if (self.willPresentCallback) {
    self.willPresentCallback(self.bannerClient);
  }
}

- (void)adViewWillDismissScreen:(GADBannerView *)adView {
  if (self.willDismissCallback) {
    self.willDismissCallback(self.bannerClient);
  }
}

- (void)adViewDidDismissScreen:(GADBannerView *)adView {
  if (self.didDismissCallback) {
    self.didDismissCallback(self.bannerClient);
  }
}

- (void)adViewWillLeaveApplication:(GADBannerView *)adView {
  if (self.willLeaveCallback) {
    self.willLeaveCallback(self.bannerClient);
  }
}

#pragma mark Cleanup

- (void)dealloc {
  _bannerView.delegate = nil;
  [_bannerView release];
  [super dealloc];
}

@end
