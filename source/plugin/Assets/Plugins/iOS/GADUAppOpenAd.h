// Copyright 2021 Google LLC. All Rights Reserved.

#import <Foundation/Foundation.h>
#import <GoogleMobileAds/GoogleMobileAds.h>
#import "GADUBaseAd.h"

@interface GADUAppOpenAd : GADUBaseAd

/// Initializes a GADUAppOpenAd.
- (id)initWithAdClientReference:(GADUTypeAdClientRef *)adClient;

/// The app open ad.
@property(nonatomic, strong) GADAppOpenAd *appOpenAd;

/// Makes an ad request. Additional targeting options can be supplied with a request object.
- (void)loadWithAdUnitID:(NSString *)adUnit
             orientation:(GADUScreenOrientation)orientation
                 request:(GADRequest *)request;

/// Shows the app open ad.
- (void)show;

@end
