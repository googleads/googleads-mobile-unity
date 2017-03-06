// Copyright 2016 Google Inc. All Rights Reserved.

@import GoogleMobileAds;

@protocol GADUAdNetworkExtras<NSObject>

- (id<GADAdNetworkExtras>)adNetworkExtrasWithDictionary:
        (NSDictionary<NSString *, NSString *> *)extras;

@end
