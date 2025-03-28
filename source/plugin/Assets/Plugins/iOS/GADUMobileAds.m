// Copyright 2024 Google Inc. All Rights Reserved.

#import "GADUMobileAds.h"

@implementation GADUMobileAds

- (nonnull instancetype)initWithMobileAdsClientReference:
    (GADUTypeMobileAdsClientRef *)mobileAdsClient {
  self = [super init];
  if(self) {
    _mobileAdsClient = mobileAdsClient;
  }
  return self;
}

#pragma mark GADPreloadEventDelegate implementation

/// Called when an ad becomes available for the configuration.
- (void)adAvailableForPreloadConfiguration:(nonnull GADPreloadConfiguration *)configuration {
  if (self.adAvailableForPreloadConfigurationCallback) {
    GADUObjectCache *cache = GADUObjectCache.sharedInstance;
    cache[configuration.gadu_referenceKey] = configuration;
    GADUPreloadConfiguration *preloadConfiguration =
        [[GADUPreloadConfiguration alloc] initWithConfig:configuration];
    self.adAvailableForPreloadConfigurationCallback(
        self.mobileAdsClient, (__bridge GADUTypePreloadConfigurationRef)preloadConfiguration);
  }
}

/// Called when the last available ad is exhausted for the configuration.
- (void)adsExhaustedForPreloadConfiguration:(nonnull GADPreloadConfiguration *)configuration {
  if (self.adsExhaustedForPreloadConfigurationCallback) {
    GADUObjectCache *cache = GADUObjectCache.sharedInstance;
    cache[configuration.gadu_referenceKey] = configuration;
    self.adsExhaustedForPreloadConfigurationCallback(
        self.mobileAdsClient, (__bridge GADUTypePreloadConfigurationRef)configuration);
  }
}

@end
