// Copyright 2018 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>
#import <GoogleMobileAds/GoogleMobileAds.h>
#import "GADUBaseAd.h"

@interface GADURewardedAd : GADUBaseAd

/// Initializes a GADURewardedAd.
- (id)initWithAdClientReference:(GADUTypeAdClientRef *)adClient;

/// The rewarded ad.
@property(nonatomic, strong) GADRewardedAd *rewardedAd;

// Returns the rewarded ad response info.
@property(nonatomic, readonly, copy) GADResponseInfo *responseInfo;

/// Makes an ad request. Additional targeting options can be supplied with a request object.
- (void)loadWithAdUnitID:(NSString *)adUnitID request:(GADRequest *)request;

/// Shows the rewarded ad.
- (void)show;

/// Options specified for server-to-server user reward verification.
- (void)setServerSideVerificationOptions:(GADServerSideVerificationOptions *)options;

@end
