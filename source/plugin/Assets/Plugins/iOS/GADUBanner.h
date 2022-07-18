// Copyright 2014 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADUBaseAd.h"

/// A wrapper around GADBannerView. Includes the ability to create GADBannerView objects, load them
/// with ads, and listen for ad events.
@interface GADUBanner : GADUBaseAd

/// Initializes a GADUBanner with specified width and height, positioned at either the top or
/// bottom of the screen.
- (id)initWithBannerClientReference:(GADUTypeAdClientRef *)adClient
                           adUnitID:(NSString *)adUnitID
                              width:(CGFloat)width
                             height:(CGFloat)height
                         adPosition:(GADAdPosition)adPosition;

/// Initializes a full-width GADUBanner, positioned at either the top or bottom of the screen.
- (id)initWithSmartBannerSizeAndBannerClientReference:(GADUTypeAdClientRef *)adClient
                                             adUnitID:(NSString *)adUnitID
                                           adPosition:(GADAdPosition)adPosition;

/// Initializes a GADUBanner with specified width and height at the specified point.
- (id)initWithBannerClientReference:(GADUTypeAdClientRef *)adClient
                           adUnitID:(NSString *)adUnitID
                              width:(CGFloat)width
                             height:(CGFloat)height
                   customAdPosition:(CGPoint)customAdPosition;

/// Initializes a full-width GADUBanner with specified width and height at the specified point.
- (id)initWithSmartBannerSizeAndBannerClientReference:(GADUTypeAdClientRef *)adClient
                                             adUnitID:(NSString *)adUnitID
                                     customAdPosition:(CGPoint)customAdPosition;

/// Initializes an adaptive GADUBanner, positioned at either the top or bottom of the screen.
- (id)initWithAdaptiveBannerSizeAndBannerClientReference:(GADUTypeAdClientRef *)adClient
                                               adUnitID:(NSString *)adUnitID
                                                  width:(NSInteger)width
                                            orientation:(GADUBannerOrientation)orientation
                                             adPosition:(GADAdPosition)adPosition;

/// Initializes an adaptive GADUBanner with a custom position at given point from top left.
- (id)initWithAdaptiveBannerSizeAndBannerClientReference:(GADUTypeAdClientRef *)adClient
                                                adUnitID:(NSString *)adUnitID
                                                   width:(NSInteger)width
                                             orientation:(GADUBannerOrientation)orientation
                                        customAdPosition:(CGPoint)customAdPosition;

/// A GADBannerView which contains the ad.
@property(nonatomic, strong) GADBannerView *bannerView;

// Returns the banner ad response info.
@property(nonatomic, readonly, copy) GADResponseInfo *responseInfo;

// Returns the height of the banner view in pixels.
@property(nonatomic, readonly) CGFloat heightInPixels;

// Returns the width of the banner view in pixels.
@property(nonatomic, readonly) CGFloat widthInPixels;

/// Makes an ad request. Additional targeting options can be supplied with a request object.
- (void)loadRequest:(GADRequest *)request;

/// Makes the GADBannerView hidden on the screen.
- (void)hideBannerView;

/// Makes the GADBannerView visible on the screen.
- (void)showBannerView;

/// Removes the GADBannerView from the view hierarchy.
- (void)removeBannerView;

/// Set the GADBannerView's position on screen using a standard position.
- (void)setAdPosition:(GADAdPosition)adPosition;

/// Set the GADBannerView's position on screen using a custom position.
- (void)setCustomAdPosition:(CGPoint)customPosition;

@end
