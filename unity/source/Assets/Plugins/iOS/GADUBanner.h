// Copyright 2014 Google Inc. All Rights Reserved.

#import <CoreGraphics/CoreGraphics.h>
#import <Foundation/Foundation.h>

#import "GADUTypes.h"

@class GADBannerView;
@class GADRequest;

/// Positions to place a banner.
typedef NS_ENUM(NSUInteger, GADAdPosition) {
  kGADAdPositionTopOfScreen = 0,    ///< Ad positioned at top of screen.
  kGADAdPositionBottomOfScreen = 1  ///< Ad positioned at bottom of screen.
};

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

/// A reference to the Unity banner client.
@property(nonatomic, assign) GADUTypeBannerClientRef *bannerClient;

/// A GADBannerView which contains the ad.
@property(nonatomic, retain) GADBannerView *bannerView;

/// The ad received callback into Unity.
@property(nonatomic, assign) GADUAdViewDidReceiveAdCallback adReceivedCallback;

/// The ad failed callback into Unity.
@property(nonatomic, assign) GADUAdViewDidFailToReceiveAdWithErrorCallback adFailedCallback;

/// The will present screen callback into Unity.
@property(nonatomic, assign) GADUAdViewWillPresentScreenCallback willPresentCallback;

/// The will dismiss screen callback into Unity.
@property(nonatomic, assign) GADUAdViewWillDismissScreenCallback willDismissCallback;

/// The did dismiss screen callback into Unity.
@property(nonatomic, assign) GADUAdViewDidDismissScreenCallback didDismissCallback;

/// The will leave application callback into Unity.
@property(nonatomic, assign) GADUAdViewWillLeaveApplicationCallback willLeaveCallback;

/// Makes an ad request. Additional targeting options can be supplied with a request object.
- (void)loadRequest:(GADRequest *)request;

/// Makes the GADBannerView hidden on the screen.
- (void)hideBannerView;

/// Makes the GADBannerView visible on the screen.
- (void)showBannerView;

/// Removes the GADBannerView from the view hierarchy.
- (void)removeBannerView;

@end
