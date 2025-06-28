// Copyright 2025 Google LLC. All Rights Reserved.

#import "GADURewardedAd.h"

#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

#import "GADURewardedAdPreloader.h"
#import "UnityInterface.h"

@interface GADURewardedAd ()
-(void) setRewardedAdAndConfigure: (GADRewardedAd *) rewardedAd;
@end

@interface GADURewardedAdPreloader () <GADPreloadDelegate>
@end

@implementation GADURewardedAdPreloader {
  // Keep a reference to the error objects so references to Unity-level
  // ResponseInfo object are not released until the ad object is released.
  NSError *_lastPreloadAdError;
}

- (nonnull instancetype)initWithRewardedAdPreloaderClientReference:(_Nonnull GADUTypeRewardedAdPreloaderClientRef *_Nonnull)rewardedAdPreloaderClient {
  self = [super init];
  _rewardedAdPreloaderClient = rewardedAdPreloaderClient;
  return self;
}

- (BOOL)preloadForPreloadID:(nonnull NSString *)preloadID
              configuration:(nonnull GADPreloadConfigurationV2 *)configuration {
  return [GADRewardedAdPreloader.sharedInstance preloadForPreloadID:preloadID
                                                     configuration:configuration
                                                          delegate:self];
}

- (BOOL)isAdAvailableWithPreloadID:(nonnull NSString *)preloadID {
  return [GADRewardedAdPreloader.sharedInstance isAdAvailableWithPreloadID:preloadID];
}

- (nullable GADURewardedAd *)adWithPreloadID:(nonnull NSString *)preloadID rewardedAdClient:(_Nonnull GADUTypeRewardedAdClientRef *_Nonnull)rewardedAdClient{
    GADRewardedAd *nativeRewardedAd = [GADRewardedAdPreloader.sharedInstance adWithPreloadID:preloadID];
    if(nativeRewardedAd) {
        GADURewardedAd *rewardedAd = [[GADURewardedAd alloc] initWithRewardedAdClientReference:rewardedAdClient];
        [rewardedAd setRewardedAdAndConfigure:nativeRewardedAd];
        return rewardedAd;
    }
    return nil;
}

- (nullable GADUPreloadConfigurationV2 *)configurationWithPreloadID:(nonnull NSString *)preloadID {
  GADPreloadConfigurationV2 *config = [GADRewardedAdPreloader.sharedInstance configurationWithPreloadID:preloadID];
  if (!config) {
    return nil;
  }
  return [[GADUPreloadConfigurationV2 alloc] initWithConfig:config];
}

- (nonnull NSDictionary<NSString *, GADUPreloadConfigurationV2 *> *)configurations {
  NSDictionary<NSString *, GADPreloadConfigurationV2 *> *configs =
      [GADRewardedAdPreloader.sharedInstance configurations];
  NSMutableDictionary<NSString *, GADUPreloadConfigurationV2 *> *convertedConfigs = [NSMutableDictionary dictionaryWithCapacity:configs.count];
  for (NSString *key in configs) {
    convertedConfigs[key] = [[GADUPreloadConfigurationV2 alloc] initWithConfig:configs[key]];
  }
  return convertedConfigs;
}

- (NSUInteger)numberOfAdsAvailableWithPreloadID:(nonnull NSString *)preloadID {
  return [GADRewardedAdPreloader.sharedInstance numberOfAdsAvailableWithPreloadID:preloadID];
}

- (void)stopPreloadingAndRemoveAdsForPreloadID:(nonnull NSString *)preloadID {
  [GADRewardedAdPreloader.sharedInstance stopPreloadingAndRemoveAdsForPreloadID:preloadID];
}

- (void)stopPreloadingAndRemoveAllAds {
  [GADRewardedAdPreloader.sharedInstance stopPreloadingAndRemoveAllAds];
}

#pragma mark GADPreloadDelegate implementation

- (void)adAvailableForPreloadID:(nonnull NSString *)preloadID responseInfo:(nonnull GADResponseInfo *)responseInfo {
  if (self.adAvailableForPreloadIDCallback) {
    GADUObjectCache *cache = GADUObjectCache.sharedInstance;
    cache[responseInfo.gadu_referenceKey] = responseInfo;
    self.adAvailableForPreloadIDCallback(
        self.rewardedAdPreloaderClient, preloadID.UTF8String, (__bridge GADUTypeResponseInfoRef)(responseInfo));
  }
}

- (void)adsExhaustedForPreloadID:(nonnull NSString *)preloadID {
  if(self.adsExhaustedForPreloadIDCallback) {
    self.adsExhaustedForPreloadIDCallback(self.rewardedAdPreloaderClient, preloadID.UTF8String);
  }
}

- (void)adFailedToPreloadForPreloadID:(nonnull NSString *)preloadID error:(nonnull NSError *)error {
  if (self.adFailedToPreloadForPreloadIDCallback) {
    _lastPreloadAdError = error;
    self.adFailedToPreloadForPreloadIDCallback(self.rewardedAdPreloaderClient, preloadID.UTF8String, (__bridge GADUTypeErrorRef)error);
  }
}

@end
