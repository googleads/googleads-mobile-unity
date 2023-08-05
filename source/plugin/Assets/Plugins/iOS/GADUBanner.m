// Copyright 2014 Google Inc. All Rights Reserved.

#import "GADUBanner.h"

#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

#import "GADUPluginUtil.h"

@interface GADUBanner () <GADBannerViewDelegate>

/// Defines where the ad should be positioned on the screen with a GADAdPosition.
@property(nonatomic, assign) GADAdPosition adPosition;

/// Defines where the ad should be positioned on the screen with a CGPoint.
@property(nonatomic, assign) CGPoint customAdPosition;

@end

@implementation GADUBanner {
  // Keep a reference to the error objects so references to Unity-level
  // ResponseInfo object are not released until the ad object is released.
  NSError *_lastLoadError;
}

- (id)initWithBannerClientReference:(GADUTypeBannerClientRef *)bannerClient
                           adUnitID:(NSString *)adUnitID
                              width:(CGFloat)width
                             height:(CGFloat)height
                         adPosition:(GADAdPosition)adPosition {
  return [self initWithBannerClientReference:bannerClient
                                    adUnitID:adUnitID
                                      adSize:[GADUPluginUtil adSizeForWidth:width height:height]
                                  adPosition:adPosition];
}

- (id)initWithBannerClientReference:(GADUTypeBannerClientRef *)bannerClient
                           adUnitID:(NSString *)adUnitID
                              width:(CGFloat)width
                             height:(CGFloat)height
                   customAdPosition:(CGPoint)customAdPosition {
  return [self initWithBannerClientReference:bannerClient
                                    adUnitID:adUnitID
                                      adSize:[GADUPluginUtil adSizeForWidth:width height:height]
                            customAdPosition:customAdPosition];
}

- (id)initWithSmartBannerSizeAndBannerClientReference:(GADUTypeBannerClientRef *)bannerClient
                                             adUnitID:(NSString *)adUnitID
                                           adPosition:(GADAdPosition)adPosition {
  // Choose the correct Smart Banner constant according to orientation.
  UIInterfaceOrientation currentOrientation =
      [UIApplication sharedApplication].statusBarOrientation;
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

- (id)initWithSmartBannerSizeAndBannerClientReference:(GADUTypeBannerClientRef *)bannerClient
                                             adUnitID:(NSString *)adUnitID
                                     customAdPosition:(CGPoint)customAdPosition {
  // Choose the correct Smart Banner constant according to orientation.
  UIInterfaceOrientation currentOrientation =
      [UIApplication sharedApplication].statusBarOrientation;
  GADAdSize adSize;
  if (UIInterfaceOrientationIsPortrait(currentOrientation)) {
    adSize = kGADAdSizeSmartBannerPortrait;
  } else {
    adSize = kGADAdSizeSmartBannerLandscape;
  }
  return [self initWithBannerClientReference:bannerClient
                                    adUnitID:adUnitID
                                      adSize:adSize
                            customAdPosition:customAdPosition];
}

- (id)initWithAdaptiveBannerSizeAndBannerClientReference:(GADUTypeBannerClientRef *)bannerClient
                                                adUnitID:(NSString *)adUnitID
                                                   width:(NSInteger)width
                                             orientation:(GADUBannerOrientation)orientation
                                              adPosition:(GADAdPosition)adPosition {
  return [self initWithBannerClientReference:bannerClient
                                    adUnitID:adUnitID
                                      adSize:[GADUPluginUtil adaptiveAdSizeForWidth:(CGFloat)width
                                                                        orientation:orientation]
                                  adPosition:adPosition];
}

- (id)initWithAdaptiveBannerSizeAndBannerClientReference:(GADUTypeBannerClientRef *)bannerClient
                                                adUnitID:(NSString *)adUnitID
                                                   width:(NSInteger)width
                                             orientation:(GADUBannerOrientation)orientation
                                        customAdPosition:(CGPoint)customAdPosition {
  return [self initWithBannerClientReference:bannerClient
                                    adUnitID:adUnitID
                                      adSize:[GADUPluginUtil adaptiveAdSizeForWidth:(CGFloat)width
                                                                        orientation:orientation]
                            customAdPosition:customAdPosition];
}

- (id)initWithBannerClientReference:(GADUTypeBannerClientRef *)bannerClient
                           adUnitID:(NSString *)adUnitID
                             adSize:(GADAdSize)size
                         adPosition:(GADAdPosition)adPosition {
  self = [super init];
  if (self) {
    _bannerClient = bannerClient;
    _adPosition = adPosition;
    _bannerView = [[GADBannerView alloc] initWithAdSize:[GADUPluginUtil safeAdSizeForAdSize:size]];
    _bannerView.adUnitID = adUnitID;
    _bannerView.delegate = self;
    _bannerView.rootViewController = [GADUPluginUtil unityGLViewController];

    [self addPaidEventHandler];
  }
  return self;
}

- (id)initWithBannerClientReference:(GADUTypeBannerClientRef *)bannerClient
                           adUnitID:(NSString *)adUnitID
                             adSize:(GADAdSize)size
                   customAdPosition:(CGPoint)customAdPosition {
  self = [super init];
  if (self) {
    _bannerClient = bannerClient;
    _customAdPosition = customAdPosition;
    _adPosition = kGADAdPositionCustom;
    _bannerView = [[GADBannerView alloc] initWithAdSize:[GADUPluginUtil safeAdSizeForAdSize:size]];
    _bannerView.adUnitID = adUnitID;
    _bannerView.delegate = self;
    _bannerView.rootViewController = [GADUPluginUtil unityGLViewController];

    [self addPaidEventHandler];
  }
  return self;
}

- (void)dealloc {
  _bannerView.delegate = nil;
}

- (void)addPaidEventHandler {
  __weak GADUBanner *weakSelf = self;
  _bannerView.paidEventHandler = ^void(GADAdValue *_Nonnull adValue) {
    GADUBanner *strongSelf = weakSelf;
    if (!strongSelf) {
      return;
    }
    if (strongSelf.paidEventCallback) {
      int64_t valueInMicros = [adValue.value decimalNumberByMultiplyingByPowerOf10:6].longLongValue;
      strongSelf.paidEventCallback(
          strongSelf.bannerClient, (int)adValue.precision, valueInMicros,
          [adValue.currencyCode cStringUsingEncoding:NSUTF8StringEncoding]);
    }
  };
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

- (GADResponseInfo *)responseInfo {
  return self.bannerView.responseInfo;
}

- (CGFloat)heightInPixels {
  return CGRectGetHeight(CGRectStandardize(self.bannerView.frame)) * [UIScreen mainScreen].scale;
}

- (CGFloat)widthInPixels {
  return CGRectGetWidth(CGRectStandardize(self.bannerView.frame)) * [UIScreen mainScreen].scale;
}

- (void)setAdPosition:(GADAdPosition)adPosition {
  _adPosition = adPosition;
  [self positionBannerView];
}

- (void)setCustomAdPosition:(CGPoint)customPosition {
  _customAdPosition = customPosition;
  _adPosition = kGADAdPositionCustom;
  [self positionBannerView];
}

- (void)positionBannerView {
  /// Align the bannerView in the Unity view bounds.
  UIView *unityView = [GADUPluginUtil unityGLViewController].view;

  if (self.adPosition != kGADAdPositionCustom) {
    [GADUPluginUtil positionView:self.bannerView inParentView:unityView adPosition:self.adPosition];
  } else {
    [GADUPluginUtil positionView:self.bannerView
                    inParentView:unityView
                  customPosition:self.customAdPosition];
  }
}

#pragma mark GADBannerViewDelegate implementation

- (void)bannerViewDidReceiveAd:(GADBannerView *)bannerView {
  // Remove existing banner view from superview.
  [self.bannerView removeFromSuperview];

  // Add the new banner view.
  self.bannerView = bannerView;

  /// Align the bannerView in the Unity view bounds.
  UIView *unityView = [GADUPluginUtil unityGLViewController].view;

  [self positionBannerView];

  [unityView addSubview:self.bannerView];

  if (self.adReceivedCallback) {
    self.adReceivedCallback(self.bannerClient);
  }
}

- (void)bannerView:(GADBannerView *)view didFailToReceiveAdWithError:(NSError *)error {
  if (self.adFailedCallback) {
    _lastLoadError = error;
    self.adFailedCallback(self.bannerClient, (__bridge GADUTypeErrorRef)error);
  }
}

- (void)bannerViewWillPresentScreen:(GADBannerView *)bannerView {
  if (self.willPresentCallback) {
    self.willPresentCallback(self.bannerClient);
  }
}

- (void)bannerViewWillDismissScreen:(GADBannerView *)bannerView {
  // Callback is not forwarded to Unity.
}

- (void)bannerViewDidDismissScreen:(GADBannerView *)bannerView {
  if (self.didDismissCallback) {
    self.didDismissCallback(self.bannerClient);
  }
}

- (void)bannerViewDidRecordImpression:(nonnull GADBannerView *)bannerView {
  if (self.adImpressionCallback) {
    self.adImpressionCallback(self.bannerClient);
  }
}

- (void)bannerViewDidRecordClick:(nonnull GADBannerView *)bannerView {
  if (self.adClickedCallback) {
    self.adClickedCallback(self.bannerClient);
  }
}
@end
