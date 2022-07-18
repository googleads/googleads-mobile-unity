// Copyright 2020 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>
#import <GoogleMobileAds/GoogleMobileAds.h>
#import "GADUBaseAd.h"

@interface GADURewardedInterstitialAd : GADUBaseAd

/// Initializes a GADURewardedInterstitialAd.
- (id)initWithAdClientReference:(GADUTypeAdClientRef *)adClient;

/// The rewarded interstitial ad.
@property(nonatomic, strong) GADRewardedInterstitialAd *rewardedInterstitialAd;

/// Returns the rewarded interstitial ad response info.
@property(nonatomic, readonly, copy) GADResponseInfo *responseInfo;

/// Makes an ad request. Additional targeting options can be supplied with a request object.
- (void)loadWithAdUnitID:(NSString *)adUnit request:(GADRequest *)request;

/// Shows the rewarded interstitial ad.
- (void)show;

/// Options specified for server-to-server user reward verification.
- (void)setServerSideVerificationOptions:(GADServerSideVerificationOptions *)options;

@end
