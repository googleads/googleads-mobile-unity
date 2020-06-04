// Copyright 2014 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADUTypes.h"

/// A wrapper around GADBannerView. Includes the ability to create GADBannerView objects, load them
/// with ads, and listen for ad events.
@interface GADUBanner : NSObject

/// Initializes a GADUBanner with specified width and height, positioned at either the top or
/// bottom of the screen.
- (id)initWithBannerClientReference:(GADUTypeBannerClientRef *)bannerClient
                           adUnitID:(NSString *)adUnitID
                              width:(CGFloat)width
                             height:(CGFloat)height
                         adPosition:(GADAdPosition)adPosition;

/// Initializes a full-width GADUBanner, positioned at either the top or bottom of the screen.
- (id)initWithSmartBannerSizeAndBannerClientReference:(GADUTypeBannerClientRef *)bannerClient
                                             adUnitID:(NSString *)adUnitID
                                           adPosition:(GADAdPosition)adPosition;

/// Initializes a GADUBanner with specified width and height at the specified point.
- (id)initWithBannerClientReference:(GADUTypeBannerClientRef *)bannerClient
                           adUnitID:(NSString *)adUnitID
                              width:(CGFloat)width
                             height:(CGFloat)height
                   customAdPosition:(CGPoint)customAdPosition;

/// Initializes a full-width GADUBanner with specified width and height at the specified point.
- (id)initWithSmartBannerSizeAndBannerClientReference:(GADUTypeBannerClientRef *)bannerClient
                                             adUnitID:(NSString *)adUnitID
                                     customAdPosition:(CGPoint)customAdPosition;

/// Initializes an adaptive GADUBanner, positioned at either the top or bottom of the screen.
- (id)initWithAdaptiveBannerSizeAndBannerClientReference:(GADUTypeBannerClientRef *)bannerClient
                                               adUnitID:(NSString *)adUnitID
                                                  width:(NSInteger)width
                                            orientation:(GADUBannerOrientation)orientation
                                             adPosition:(GADAdPosition)adPosition;

/// Initializes an adaptive GADUBanner with a custom position at given point from top left.
- (id)initWithAdaptiveBannerSizeAndBannerClientReference:(GADUTypeBannerClientRef *)bannerClient
                                                adUnitID:(NSString *)adUnitID
                                                   width:(NSInteger)width
                                             orientation:(GADUBannerOrientation)orientation
                                        customAdPosition:(CGPoint)customAdPosition;

/// A reference to the Unity banner client.
@property(nonatomic, assign) GADUTypeBannerClientRef *bannerClient;

/// A GADBannerView which contains the ad.
@property(nonatomic, strong) GADBannerView *bannerView;

/// The ad received callback into Unity.
@property(nonatomic, assign) GADUAdViewDidReceiveAdCallback adReceivedCallback;

/// The ad failed callback into Unity.
@property(nonatomic, assign) GADUAdViewDidFailToReceiveAdWithErrorCallback adFailedCallback;

/// The will present screen callback into Unity.
@property(nonatomic, assign) GADUAdViewWillPresentScreenCallback willPresentCallback;

/// The did dismiss screen callback into Unity.
@property(nonatomic, assign) GADUAdViewDidDismissScreenCallback didDismissCallback;

/// The will leave application callback into Unity.
@property(nonatomic, assign) GADUAdViewWillLeaveApplicationCallback willLeaveCallback;

/// The paid event callback into Unity.
@property(nonatomic, assign) GADUAdViewPaidEventCallback paidEventCallback;

// Returns the mediation adapter class name.
@property(nonatomic, readonly, copy) NSString *mediationAdapterClassName;

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
