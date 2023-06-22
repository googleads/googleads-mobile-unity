// Copyright 2014 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADUTypes.h"

/// A wrapper around GADBannerView. Includes the ability to create GADBannerView objects, load them
/// with ads, and listen for ad events.
@interface GADUBanner : NSObject

/// Initializes a GADUBanner with specified width and height, positioned at either the top or
/// bottom of the screen.
- (nonnull instancetype)initWithBannerClientReference:
                            (_Nonnull GADUTypeBannerClientRef *_Nonnull)bannerClient
                                             adUnitID:(nonnull NSString *)adUnitID
                                                width:(CGFloat)width
                                               height:(CGFloat)height
                                           adPosition:(GADAdPosition)adPosition;

/// Initializes a full-width GADUBanner, positioned at either the top or bottom of the screen.
- (nonnull instancetype)initWithSmartBannerSizeAndBannerClientReference:
                            (_Nonnull GADUTypeBannerClientRef *_Nonnull)bannerClient
                                                               adUnitID:(nonnull NSString *)adUnitID
                                                             adPosition:(GADAdPosition)adPosition;

/// Initializes a GADUBanner with specified width and height at the specified point.
- (nonnull instancetype)initWithBannerClientReference:
                            (_Nonnull GADUTypeBannerClientRef *_Nonnull)bannerClient
                                             adUnitID:(nonnull NSString *)adUnitID
                                                width:(CGFloat)width
                                               height:(CGFloat)height
                                     customAdPosition:(CGPoint)customAdPosition;

/// Initializes a full-width GADUBanner with specified width and height at the specified point.
- (nonnull instancetype)initWithSmartBannerSizeAndBannerClientReference:
                            (_Nonnull GADUTypeBannerClientRef *_Nonnull)bannerClient
                                                               adUnitID:(nonnull NSString *)adUnitID
                                                       customAdPosition:(CGPoint)customAdPosition;

/// Initializes an adaptive GADUBanner, positioned at either the top or bottom of the screen.
- (nonnull instancetype)
    initWithAdaptiveBannerSizeAndBannerClientReference:
        (_Nonnull GADUTypeBannerClientRef *_Nonnull)bannerClient
                                              adUnitID:(nonnull NSString *)adUnitID
                                                 width:(NSInteger)width
                                           orientation:(GADUBannerOrientation)orientation
                                            adPosition:(GADAdPosition)adPosition;

/// Initializes an adaptive GADUBanner with a custom position at given point from top left.
- (nonnull instancetype)
    initWithAdaptiveBannerSizeAndBannerClientReference:
        (_Nonnull GADUTypeBannerClientRef *_Nonnull)bannerClient
                                              adUnitID:(nonnull NSString *)adUnitID
                                                 width:(NSInteger)width
                                           orientation:(GADUBannerOrientation)orientation
                                      customAdPosition:(CGPoint)customAdPosition;

/// Initializes a GADUBanner with a preset ad position.
- (nonnull instancetype)initWithBannerClientReference:
                            (_Nonnull GADUTypeBannerClientRef *_Nonnull)bannerClient
                                             adUnitID:(nonnull NSString *)adUnitID
                                               adSize:(GADAdSize)size
                                           adPosition:(GADAdPosition)adPosition;

/// Initializes a GADUBanner with a custom ad position.
- (nonnull instancetype)initWithBannerClientReference:
                            (_Nonnull GADUTypeBannerClientRef *_Nonnull)bannerClient
                                             adUnitID:(nonnull NSString *)adUnitID
                                               adSize:(GADAdSize)size
                                     customAdPosition:(CGPoint)customAdPosition;

/// A reference to the Unity banner client.
@property(nonatomic, assign) _Nonnull GADUTypeBannerClientRef *_Nonnull bannerClient;

/// A GADBannerView which contains the ad.
@property(nonatomic, strong, nullable) GADBannerView *bannerView;

/// The ad received callback into Unity.
@property(nonatomic, assign, nullable) GADUAdViewDidReceiveAdCallback adReceivedCallback;

/// The ad failed callback into Unity.
@property(nonatomic, assign, nullable)
    GADUAdViewDidFailToReceiveAdWithErrorCallback adFailedCallback;

/// The will present screen callback into Unity.
@property(nonatomic, assign, nullable) GADUAdViewWillPresentScreenCallback willPresentCallback;

/// The did dismiss screen callback into Unity.
@property(nonatomic, assign, nullable) GADUAdViewDidDismissScreenCallback didDismissCallback;

/// The paid event callback into Unity.
@property(nonatomic, assign, nullable) GADUAdViewPaidEventCallback paidEventCallback;

/// The ad impression event callback into Unity.
@property(nonatomic, assign, nullable) GADUAdViewImpressionCallback adImpressionCallback;

/// The ad clicked event callback into Unity.
@property(nonatomic, assign, nullable) GADUAdViewClickCallback adClickedCallback;

// Returns the banner ad response info.
@property(nonatomic, readonly, copy, nullable) GADResponseInfo *responseInfo;

// Returns the height of the banner view in pixels.
@property(nonatomic, readonly) CGFloat heightInPixels;

// Returns the width of the banner view in pixels.
@property(nonatomic, readonly) CGFloat widthInPixels;

/// Makes an ad request. Additional targeting options can be supplied with a request object.
- (void)loadRequest:(nonnull GADRequest *)request;

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
