// Copyright 2024 Google Inc. All Rights Reserved.

#import "GADUMobileAds.h"

@implementation GADUMobileAds

- (id)initWithMobileAdsClientReference:(GADUTypeMobileAdsClientRef *)mobileAdsClient {
  self = [super init];
  _mobileAdsClient = mobileAdsClient;
  return self;
}

#pragma mark GADPreloadEventDelegate implementation

/// Called when an ad becomes available for the configuration.
- (void)adAvailableForPreloadConfiguration:(nonnull GADPreloadConfiguration *)configuration {
  if (self.adAvailableForPreloadConfigurationCallback) {
    GADUObjectCache *cache = GADUObjectCache.sharedInstance;
    cache[configuration.gadu_referenceKey] = configuration;
    GADUPreloadConfiguration *internalPreloadConfiguration =
        [GADUPreloadConfiguration internalPreloadConfiguration:configuration];
    self.adAvailableForPreloadConfigurationCallback(
        self.mobileAdsClient,
        (__bridge GADUTypePreloadConfigurationRef)internalPreloadConfiguration);
  }
}

/// Called when the last available ad is exhausted for the configuration.
- (void)adExhaustedForPreloadConfiguration:(nonnull GADPreloadConfiguration *)configuration {
  if (self.adsExhaustedForPreloadConfigurationCallback) {
    GADUObjectCache *cache = GADUObjectCache.sharedInstance;
    cache[configuration.gadu_referenceKey] = configuration;
    self.adsExhaustedForPreloadConfigurationCallback(
        self.mobileAdsClient, (__bridge GADUTypePreloadConfigurationRef)configuration);
  }
}

@end
