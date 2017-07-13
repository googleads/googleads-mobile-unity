// Copyright 2014 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADUTypes.h"

/// A wrapper around GADInterstitial. Includes the ability to create GADInterstitial objects, load
/// them with ads, show them, and listen for ad events.
@interface GADUInterstitial : NSObject

/// Initializes a GADUInterstitial.
- (id)initWithInterstitialClientReference:(GADUTypeInterstitialClientRef *)interstitialClient
                                 adUnitID:(NSString *)adUnitID;

/// The interstitial ad.
@property(nonatomic, strong) GADInterstitial *interstitial;

/// A reference to the Unity interstitial client.
@property(nonatomic, assign) GADUTypeInterstitialClientRef *interstitialClient;

/// The ad received callback into Unity.
@property(nonatomic, assign) GADUInterstitialDidReceiveAdCallback adReceivedCallback;

/// The ad failed callback into Unity.
@property(nonatomic, assign) GADUInterstitialDidFailToReceiveAdWithErrorCallback adFailedCallback;

/// The will present screen callback into Unity.
@property(nonatomic, assign) GADUInterstitialWillPresentScreenCallback willPresentCallback;

/// The did dismiss screen callback into Unity.
@property(nonatomic, assign) GADUInterstitialDidDismissScreenCallback didDismissCallback;

/// The will leave application callback into Unity.
@property(nonatomic, assign) GADUInterstitialWillLeaveApplicationCallback willLeaveCallback;

/// Makes an ad request. Additional targeting options can be supplied with a request object.
- (void)loadRequest:(GADRequest *)request;

/// Returns YES if the interstitial is ready to be displayed.
- (BOOL)isReady;

/// Shows the interstitial ad.
- (void)show;

@end
