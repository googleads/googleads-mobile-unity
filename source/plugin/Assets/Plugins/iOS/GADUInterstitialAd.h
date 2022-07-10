// Copyright 2014 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>
#import <GoogleMobileAds/GoogleMobileAds.h>
#import "GADUBaseAd.h"

/// A wrapper around GADInterstitial. Includes the ability to create GADInterstitial objects, load
/// them with ads, show them, and listen for ad events.
@interface GADUInterstitialAd : GADUBaseAd

/// Initializes a GADUInterstitial.
- (id)initWithAdClientReference:(GADUTypeAdClientRef *)adClient;

/// The interstitial ad.
@property(nonatomic, strong) GADInterstitialAd *interstitialAd;

/// Makes an ad request. Additional targeting options can be supplied with a request object.
- (void)loadWithAdUnitID:(NSString *)adUnitID request:(GADRequest *)request;

/// Shows the interstitial ad.
- (void)show;

@end
