// Copyright 2014 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>
#import <GoogleMobileAds/GoogleMobileAds.h>
#import "GADUTypes.h"

/// Abstraction with common interface members for Unity Bridge clients
@interface GADUBaseAd : NSObject <GADFullScreenContentDelegate>

/// A reference to the Unity ad load callback
@property(nonatomic, assign) GADUBaseAdCallback adLoadCallback;

/// A reference to the Unity ad load failed callback
@property(nonatomic, assign) GADUBaseAdErrorCallback adLoadFailedCallback;

/// A reference to the Unity ad impression callback
@property(nonatomic, assign) GADUBaseAdCallback adImpressionCallback;

/// A reference to the Unity ad click callback
@property(nonatomic, assign) GADUBaseAdCallback adClickedCallback;

/// A reference to the Unity ad click callback paid event callback.
@property(nonatomic, assign) GADUBaseAdPaidCallback adPaidCallback;

/// A reference to the Unity full screen ad opened callback
@property(nonatomic, assign) GADUBaseAdCallback adFullScreenOpenedCallback;

/// A reference to the Unity full screen ad opene failed callback
@property(nonatomic, assign) GADUBaseAdErrorCallback adFullScreenFailedCallback;

/// A reference to the Unity full screen ad dismissed callback
@property(nonatomic, assign) GADUBaseAdCallback adFullScreenClosedCallback;

/// The user was rewarded callback into Unity.
@property(nonatomic, assign) GADUBaseAdRewardedCallback adUserEarnedRewardCallback;

/// A reference to the Unity ad load callback
@property(nonatomic, assign) GADUTypeAdClientRef *adClient;

// The ad response info.
@property(nonatomic, readonly, copy) GADResponseInfo *responseInfo;

@end
