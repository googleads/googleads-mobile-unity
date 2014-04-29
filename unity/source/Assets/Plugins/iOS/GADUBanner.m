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
    self.bannerClient = bannerClient;
    self.adPosition = adPosition;
    self.bannerView = [[[GADBannerView alloc] initWithAdSize:size] autorelease];
    self.bannerView.adUnitID = adUnitID;
    self.bannerView.delegate = self;
    UIViewController *unityController = [GADUBanner unityGLViewController];
    self.bannerView.rootViewController = unityController;
    [unityController.view addSubview:self.bannerView];
  }
  return self;
}

- (void)loadRequest:(GADRequest *)request {
  if (!self.bannerView) {
    NSLog(@"GoogleMobileAdsPlugin: BannerView is nil. Aborting ad request. Call CreateBannerView()"
          @"before RequestBannerAd().");
    return;
  }
  [self.bannerView loadRequest:request];
}

- (void)hideBannerView {
  if (!self.bannerView) {
    NSLog(@"GoogleMobileAdsPlugin: BannerView is nil. Call CreateBannerView() before"
          @"HideBannerView().");
    return;
  }
  self.bannerView.hidden = YES;
}

- (void)showBannerView {
  if (!self.bannerView) {
    NSLog(@"GoogleMobileAdsPlugin: BannerView is nil. Call CreateBannerView() before"
          @"ShowBannerView().");
    return;
  }
  self.bannerView.hidden = NO;
}

#pragma mark GADBannerViewDelegate implementation

- (void)adViewDidReceiveAd:(GADBannerView *)adView {
  UIView *unityView = [[GADUBanner unityGLViewController] view];
  CGRect frame = adView.frame;
  CGPoint center;
  // Position the GADBannerView.
  switch (self.adPosition) {
    case kGADAdPositionTopLeftOfScreen:
      frame.origin.x = 0;
      frame.origin.y = 0;
      adView.frame = frame;
      break;
    case kGADAdPositionTopOfScreen:
      center = CGPointMake(CGRectGetMidX(unityView.bounds), CGRectGetMidY(adView.bounds));
      adView.center = center;
      break;
    case kGADAdPositionTopRightOfScreen:
      frame.origin.x = CGRectGetMaxX(unityView.bounds) - CGRectGetWidth(adView.bounds);
      frame.origin.y = 0;
      adView.frame = frame;
      break;
    case kGADAdPositionBottomLeftOfScreen:
      frame.origin.x = 0;
      frame.origin.y = CGRectGetMaxY(unityView.bounds) - CGRectGetHeight(adView.bounds);
      adView.frame = frame;
    case kGADAdPositionBottomOfScreen:
      center = CGPointMake(CGRectGetMidX(unityView.bounds),
                           CGRectGetMaxY(unityView.bounds) - CGRectGetMidY(adView.bounds));
      adView.center = center;
      break;
    case kGADAdPositionBottomRightOfScreen:
      frame.origin.x = CGRectGetMaxX(unityView.bounds) - CGRectGetWidth(adView.bounds);
      frame.origin.y = CGRectGetMaxY(unityView.bounds) - CGRectGetHeight(adView.bounds);
      adView.frame = frame;
      break;
  }
  if (self.adReceivedCallback) {
    self.adReceivedCallback(self.bannerClient);
  }
}

- (void)adView:(GADBannerView *)view didFailToReceiveAdWithError:(GADRequestError *)error {
  NSString *errorMsg = [NSString
      stringWithFormat:@"Failed to receive ad with error: %@", [error localizedFailureReason]];
  self.adFailedCallback(self.bannerClient, [errorMsg cStringUsingEncoding:NSUTF8StringEncoding]);
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
