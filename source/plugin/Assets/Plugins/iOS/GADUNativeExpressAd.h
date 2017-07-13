// Copyright 2016 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADUTypes.h"

/// A wrapper around GADNativeExpressAdView. Handles creating GADNativeExpressAdView
/// objects, loading them with ads, and listening for ad events.
@interface GADUNativeExpressAd : NSObject

/// Returns an initialized GADNativeExpressAdView object with specified width and height, positioned
/// at either the top or bottom of the screen.
- (instancetype)initWithNativeExpressAdClientReference:
                    (GADUTypeNativeExpressAdClientRef *)nativeExpressAdClient
                                              adUnitID:(NSString *)adUnitID
                                                 width:(CGFloat)width
                                                height:(CGFloat)height
                                            adPosition:(GADAdPosition)adPosition;

/// Returns an initialized GADNativeExpressAdView object with specified width and height, positioned
/// at the specified CGPoint.
- (instancetype)initWithNativeExpressAdClientReference:
                    (GADUTypeNativeExpressAdClientRef *)nativeExpressAdClient
                                              adUnitID:(NSString *)adUnitID
                                                 width:(CGFloat)width
                                                height:(CGFloat)height
                                      customAdPosition:(CGPoint)customAdPosition;

/// A reference to the Unity native express ad client.
@property(nonatomic, assign) GADUTypeNativeExpressAdClientRef *nativeExpressAdClient;

/// A GADNativeExpressAdView which contains the ad.
@property(nonatomic, strong) GADNativeExpressAdView *nativeExpressAdView;

/// The ad received callback into Unity.
@property(nonatomic, assign) GADUAdViewDidReceiveAdCallback adReceivedCallback;

/// The ad failed callback into Unity.
@property(nonatomic, assign) GADUAdViewDidFailToReceiveAdWithErrorCallback adFailedCallback;

/// The will present screen callback into Unity.
@property(nonatomic, assign) GADUAdViewWillPresentScreenCallback willPresentScreenCallback;

/// The did dismiss screen callback into Unity.
@property(nonatomic, assign) GADUAdViewDidDismissScreenCallback didDismissScreenCallback;

/// The will leave application callback into Unity.
@property(nonatomic, assign) GADUAdViewWillLeaveApplicationCallback willLeaveAppCallback;

/// Requests a native express ad. Additional targeting options can be supplied with a request
/// object.
- (void)loadRequest:(GADRequest *)request;

/// Hides the GADNativeExpressAdView.
- (void)hideNativeExpressAdView;

/// Shows the GADNativeExpressAdView.
- (void)showNativeExpressAdView;

/// Removes the GADNativeExpressAdView from the view hierarchy.
- (void)removeNativeExpressAdView;

@end
