// Copyright 2014 Google Inc. All Rights Reserved.

#import "GADUBanner.h"

#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

#import "GADUObjectCache.h"
#import "GADUPluginUtil.h"
#import "GADURequest.h"

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

- (id)initWithBannerClientReference:(GADUTypeAdClientRef *)bannerClient
                           adUnitID:(NSString *)adUnitID
                              width:(CGFloat)width
                             height:(CGFloat)height
                         adPosition:(GADAdPosition)adPosition {
  return [self initWithBannerClientReference:bannerClient
                                    adUnitID:adUnitID
                                      adSize:[GADUPluginUtil adSizeForWidth:width height:height]
                                  adPosition:adPosition];
}

- (id)initWithBannerClientReference:(GADUTypeAdClientRef *)bannerClient
                           adUnitID:(NSString *)adUnitID
                              width:(CGFloat)width
                             height:(CGFloat)height
                   customAdPosition:(CGPoint)customAdPosition {
  return [self initWithBannerClientReference:bannerClient
                                    adUnitID:adUnitID
                                      adSize:[GADUPluginUtil adSizeForWidth:width height:height]
                            customAdPosition:customAdPosition];
}

- (id)initWithSmartBannerSizeAndBannerClientReference:(GADUTypeAdClientRef *)bannerClient
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

- (id)initWithSmartBannerSizeAndBannerClientReference:(GADUTypeAdClientRef *)bannerClient
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

- (id)initWithAdaptiveBannerSizeAndBannerClientReference:(GADUTypeAdClientRef *)bannerClient
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

- (id)initWithAdaptiveBannerSizeAndBannerClientReference:(GADUTypeAdClientRef *)bannerClient
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

- (id)initWithBannerClientReference:(GADUTypeAdClientRef *)bannerClient
                           adUnitID:(NSString *)adUnitID
                             adSize:(GADAdSize)size
                         adPosition:(GADAdPosition)adPosition {
  self = [super init];
  if (self) {
    super.adClient = bannerClient;
    _adPosition = adPosition;
    _bannerView = [[GADBannerView alloc] initWithAdSize:[GADUPluginUtil safeAdSizeForAdSize:size]];
    _bannerView.adUnitID = adUnitID;
    _bannerView.delegate = self;
    _bannerView.rootViewController = [GADUPluginUtil unityGLViewController];

    [self addPaidEventHandler];
  }
  return self;
}

- (id)initWithBannerClientReference:(GADUTypeAdClientRef *)bannerClient
                           adUnitID:(NSString *)adUnitID
                             adSize:(GADAdSize)size
                   customAdPosition:(CGPoint)customAdPosition {
  self = [super init];
  if (self) {
    super.adClient = bannerClient;
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
    if (strongSelf.adPaidCallback) {
      int64_t valueInMicros = [adValue.value decimalNumberByMultiplyingByPowerOf10:6].longLongValue;
      strongSelf.adPaidCallback(strongSelf.adClient, (int)adValue.precision, valueInMicros,
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

  if (self.adLoadCallback) {
    self.adLoadCallback(self.adClient);
  }
}

- (void)bannerView:(GADBannerView *)view didFailToReceiveAdWithError:(NSError *)error {
  if (self.adLoadFailedCallback) {
    _lastLoadError = error;
    self.adLoadFailedCallback(self.adClient, (__bridge GADUTypeErrorRef)error);
  }
}

- (void)bannerViewWillPresentScreen:(GADBannerView *)bannerView {
  if (self.adFullScreenOpenedCallback) {
    self.adFullScreenOpenedCallback(self.adClient);
  }
}

- (void)bannerViewWillDismissScreen:(GADBannerView *)bannerView {
  // Callback is not forwarded to Unity.
}

- (void)bannerViewDidDismissScreen:(GADBannerView *)bannerView {
  if (self.adFullScreenClosedCallback) {
    self.adFullScreenClosedCallback(self.adClient);
  }
}

- (void)bannerViewDidRecordImpression:(nonnull GADBannerView *)bannerView {
  if (self.adImpressionCallback) {
    self.adImpressionCallback(self.adClient);
  }
}

- (void)bannerViewDidRecordClick:(nonnull GADBannerView *)bannerView {
  if (self.adClickedCallback) {
    self.adClickedCallback(self.adClient);
  }
}

@end

#pragma mark Unity Native Interop Methods

static NSString *GADUStringFromUTF8String(const char *bytes) { return bytes ? @(bytes) : nil; }

/// Creates a GADBannerView with the specified width, height, and position. Returns a reference to
/// the GADUBannerAd.
GADUTypeAdBridgeRef GADUBannerAdCreate(GADUTypeAdClientRef *adClient, const char *adUnitID,
                                       NSInteger width, NSInteger height,
                                       GADAdPosition adPosition) {
  GADUBanner *banner =
      [[GADUBanner alloc] initWithBannerClientReference:adClient
                                               adUnitID:GADUStringFromUTF8String(adUnitID)
                                                  width:(int)width
                                                 height:(int)height
                                             adPosition:adPosition];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  cache[banner.gadu_referenceKey] = banner;
  return (__bridge GADUTypeAdBridgeRef)banner;
}

/// Creates a GADBannerView with the specified width, height, and custom position. Returns
/// a reference to the GADUBannerAd.
GADUTypeAdBridgeRef GADUBannerAdCreateWithCustomPosition(GADUTypeAdClientRef *adClient,
                                                         const char *adUnitID, NSInteger width,
                                                         NSInteger height, NSInteger x,
                                                         NSInteger y) {
  CGPoint adPosition = CGPointMake(x, y);
  GADUBanner *banner =
      [[GADUBanner alloc] initWithBannerClientReference:adClient
                                               adUnitID:GADUStringFromUTF8String(adUnitID)
                                                  width:(int)width
                                                 height:(int)height
                                       customAdPosition:adPosition];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  cache[banner.gadu_referenceKey] = banner;
  return (__bridge GADUTypeAdBridgeRef)banner;
}

/// Creates a full-width GADBannerView in the current orientation. Returns a reference to the
/// GADUBannerAd.
GADUTypeAdBridgeRef GADUBannerAdCreateSmart(GADUTypeAdClientRef *adClient, const char *adUnitID,
                                              GADAdPosition adPosition) {
  GADUBanner *banner = [[GADUBanner alloc]
      initWithSmartBannerSizeAndBannerClientReference:adClient
                                             adUnitID:GADUStringFromUTF8String(adUnitID)
                                           adPosition:adPosition];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  cache[banner.gadu_referenceKey] = banner;
  return (__bridge GADUTypeBannerRef)banner;
}

/// Creates a full-width GADBannerView in the current orientation with a custom position. Returns a
/// reference to the GADUBannerAd.
GADUTypeAdBridgeRef GADUBannerAdCreateSmartWithCustomPosition(GADUTypeAdClientRef *adClient,
                                                                  const char *adUnitID, NSInteger x,
                                                                  NSInteger y) {
  CGPoint adPosition = CGPointMake(x, y);
  GADUBanner *banner = [[GADUBanner alloc]
      initWithSmartBannerSizeAndBannerClientReference:adClient
                                             adUnitID:GADUStringFromUTF8String(adUnitID)
                                     customAdPosition:adPosition];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  cache[banner.gadu_referenceKey] = banner;
  return (__bridge GADUTypeBannerRef)banner;
}

/// Creates a an adaptive sized GADBannerView with the specified width, orientation, and position.
/// Returns a reference to the GADUBannerAd.
GADUTypeAdBridgeRef GADUBannerAdCreateAnchoredAdaptive(GADUTypeAdClientRef *adClient,
                                                       const char *adUnitID, NSInteger width,
                                                       GADUBannerOrientation orientation,
                                                       GADAdPosition adPosition) {
  GADUBanner *banner = [[GADUBanner alloc]
      initWithAdaptiveBannerSizeAndBannerClientReference:adClient
                                                adUnitID:GADUStringFromUTF8String(adUnitID)
                                                   width:(int)width
                                             orientation:orientation
                                              adPosition:adPosition];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  cache[banner.gadu_referenceKey] = banner;
  return (__bridge GADUTypeBannerRef)banner;
}

/// Creates a an adaptive sized GADBannerView with the specified width, orientation, and position.
/// Returns a reference to the GADUBannerAd.
GADUTypeAdBridgeRef GADUBannerAdCreateAnchoredAdaptiveWithCustomPosition(
    GADUTypeAdClientRef *adClient, const char *adUnitID, NSInteger width,
    GADUBannerOrientation orientation, NSInteger x, NSInteger y) {
  CGPoint adPosition = CGPointMake(x, y);
  GADUBanner *banner = [[GADUBanner alloc]
      initWithAdaptiveBannerSizeAndBannerClientReference:adClient
                                                adUnitID:GADUStringFromUTF8String(adUnitID)
                                                   width:(int)width
                                             orientation:orientation
                                        customAdPosition:adPosition];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  cache[banner.gadu_referenceKey] = banner;
  return (__bridge GADUTypeBannerRef)banner;
}

void GADUBannerAdRequest(GADUTypeAdBridgeRef adBridgeRef, GADUTypeRequestRef requestRef) {
  GADUBanner *adBridge = (__bridge GADUBanner *)adBridgeRef;
  GADURequest *adRequest = (__bridge GADURequest *)requestRef;
  [adBridge loadRequest:[adRequest request]];
}

void GADUBannerAdHideView(GADUTypeAdBridgeRef adBridgeRef) {
  GADUBanner *adBridge = (__bridge GADUBanner *)adBridgeRef;
  [adBridge hideBannerView];
}

void GADUBannerAdShow(GADUTypeAdBridgeRef adBridgeRef) {
  GADUBanner *adBridge = (__bridge GADUBanner *)adBridgeRef;
  [adBridge showBannerView];
}

void GADUBannerAdRemove(GADUTypeAdBridgeRef adBridgeRef) {
  GADUBanner *adBridge = (__bridge GADUBanner *)adBridgeRef;
  [adBridge removeBannerView];
}

float GADUBannerAdGetHeightInPixels(GADUTypeAdBridgeRef adBridgeRef) {
  GADUBanner *adBridge = (__bridge GADUBanner *)adBridgeRef;
  return adBridge.heightInPixels;
}

float GADUBannerAdGetWidthInPixels(GADUTypeAdBridgeRef adBridgeRef) {
  GADUBanner *adBridge = (__bridge GADUBanner *)adBridgeRef;
  return adBridge.widthInPixels;
}

void GADUBannerAdSetPosition(GADUTypeAdBridgeRef adBridgeRef, int position) {
  GADUBanner *adBridge = (__bridge GADUBanner *)adBridgeRef;
  [adBridge setAdPosition:(GADAdPosition)position];
}

void GADUBannerAdSetCustomPosition(GADUTypeAdBridgeRef adBridgeRef, int x, int y) {
  GADUBanner *adBridge = (__bridge GADUBanner *)adBridgeRef;
  [adBridge setCustomAdPosition:CGPointMake(x, y)];
}
