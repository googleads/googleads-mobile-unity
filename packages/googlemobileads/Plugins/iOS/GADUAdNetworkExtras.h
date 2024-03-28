// Copyright 2016 Google Inc. All Rights Reserved.

#import <GoogleMobileAds/GoogleMobileAds.h>

@protocol GADUAdNetworkExtras <NSObject>

- (nonnull id<GADAdNetworkExtras>)adNetworkExtrasWithDictionary:
    (nonnull NSDictionary<NSString *, NSString *> *)extras;

@end
