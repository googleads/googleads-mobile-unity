// Copyright 2016 Google Inc. All Rights Reserved.

#import <GoogleMobileAds/GoogleMobileAds.h>

@protocol GADUAdNetworkExtras<NSObject>

- (id<GADAdNetworkExtras>)adNetworkExtrasWithDictionary:
        (NSDictionary<NSString *, NSString *> *)extras;

@end
