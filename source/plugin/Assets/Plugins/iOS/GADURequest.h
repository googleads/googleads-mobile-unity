// Copyright 2014 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

// Specifies optional parameters for ad requests.
@interface GADURequest : NSObject

/// Returns an initialized GADURequest object.
- (id)init;

/// Words or phrase describing the current activity of the user.
@property(nonatomic, strong) NSMutableArray *keywords;

/// String that identifies the ad request's origin.
@property(nonatomic, strong) NSString *requestAgent;

/// GADMediationExtras to be sent up in the ad request.
@property(nonatomic, strong) NSMutableArray<id<GADAdNetworkExtras>> *mediationExtras;

/// Extra parameters to be sent up in the ad request.
@property(nonatomic, strong) NSMutableDictionary *extras;

/// A long integer provided by the AdMob UI for the configured placement.
@property(nonatomic) int64_t placementID;

/// Convenience method for adding a single keyword.
- (void)addKeyword:(NSString *)keyword;

/// Convenience method for setting an extra parameters.
- (void)setExtraWithKey:(NSString *)key value:(NSString *)value;

/// Convenience method for setting custom targeting parameters.
- (void)setCustomTargetingWithKey:(nonnull NSString *)key value:(nullable NSString *)value;

/// Constructs a GADRequest with the defined targeting values.
- (nonnull GADRequest *)request;

@end
