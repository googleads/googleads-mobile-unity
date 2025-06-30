// Copyright 2025 Google LLC. All Rights Reserved.

#import "GADUInterstitial.h"

#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

#import "GADUInterstitialAdPreloader.h"
#import "UnityInterface.h"

@interface GADUInterstitial ()
- (void)setInterstitialAdAndConfigure:(GADInterstitialAd *)interstitialAd;
@end

@interface GADUInterstitialAdPreloader () <GADPreloadDelegate>
@end

@implementation GADUInterstitialAdPreloader {
  // Keep a reference to the error objects so references to Unity-level
  // ResponseInfo object are not released until the ad object is released.
  NSError *_lastPreloadAdError;
}

- (nonnull instancetype)initWithInterstitialAdPreloaderClientReference:
    (_Nonnull GADUTypeInterstitialAdPreloaderClientRef *_Nonnull)interstitialAdPreloaderClient {
  self = [super init];
  if (self) {
    _interstitialAdPreloaderClient = interstitialAdPreloaderClient;
  }
  return self;
}

- (BOOL)preloadForPreloadID:(nonnull NSString *)preloadID
              configuration:(nonnull GADPreloadConfigurationV2 *)configuration {
  return [GADInterstitialAdPreloader.sharedInstance preloadForPreloadID:preloadID
                                                          configuration:configuration
                                                               delegate:self];
}

- (BOOL)isAdAvailableWithPreloadID:(nonnull NSString *)preloadID {
  return [GADInterstitialAdPreloader.sharedInstance isAdAvailableWithPreloadID:preloadID];
}

- (nullable GADUInterstitial *)adWithPreloadID:(nonnull NSString *)preloadID
                          interstitialAdClient:(_Nonnull GADUTypeInterstitialClientRef *_Nonnull)
                                                   interstitialAdClient {
  GADInterstitialAd *nativeInterstitialAd =
      [GADInterstitialAdPreloader.sharedInstance adWithPreloadID:preloadID];
  if (nativeInterstitialAd) {
    GADUInterstitial *interstitialAd =
        [[GADUInterstitial alloc] initWithInterstitialClientReference:interstitialAdClient];
    [interstitialAd setInterstitialAdAndConfigure:nativeInterstitialAd];
    return interstitialAd;
  }
  return nil;
}

- (nullable GADUPreloadConfigurationV2 *)configurationWithPreloadID:(nonnull NSString *)preloadID {
  GADPreloadConfigurationV2 *config =
      [GADInterstitialAdPreloader.sharedInstance configurationWithPreloadID:preloadID];
  if (!config) {
    return nil;
  }
  return [[GADUPreloadConfigurationV2 alloc] initWithConfig:config];
}

- (nonnull NSDictionary<NSString *, GADUPreloadConfigurationV2 *> *)configurations {
  NSDictionary<NSString *, GADPreloadConfigurationV2 *> *configs =
      [GADInterstitialAdPreloader.sharedInstance configurations];
  NSMutableDictionary<NSString *, GADUPreloadConfigurationV2 *> *convertedConfigs =
      [NSMutableDictionary dictionaryWithCapacity:configs.count];
  for (NSString *key in configs) {
    convertedConfigs[key] = [[GADUPreloadConfigurationV2 alloc] initWithConfig:configs[key]];
  }
  return [convertedConfigs copy];
}

- (NSUInteger)numberOfAdsAvailableWithPreloadID:(nonnull NSString *)preloadID {
  return [GADInterstitialAdPreloader.sharedInstance numberOfAdsAvailableWithPreloadID:preloadID];
}

- (void)stopPreloadingAndRemoveAdsForPreloadID:(nonnull NSString *)preloadID {
  [GADInterstitialAdPreloader.sharedInstance stopPreloadingAndRemoveAdsForPreloadID:preloadID];
}

- (void)stopPreloadingAndRemoveAllAds {
  [GADInterstitialAdPreloader.sharedInstance stopPreloadingAndRemoveAllAds];
}

#pragma mark GADPreloadDelegate implementation

- (void)adAvailableForPreloadID:(nonnull NSString *)preloadID
                   responseInfo:(nonnull GADResponseInfo *)responseInfo {
  if (self.adAvailableForPreloadIDCallback) {
    GADUObjectCache *cache = GADUObjectCache.sharedInstance;
    cache[responseInfo.gadu_referenceKey] = responseInfo;
    self.adAvailableForPreloadIDCallback(self.interstitialAdPreloaderClient, preloadID.UTF8String,
                                         (__bridge GADUTypeResponseInfoRef)(responseInfo));
  }
}

- (void)adsExhaustedForPreloadID:(nonnull NSString *)preloadID {
  if (self.adsExhaustedForPreloadIDCallback) {
    self.adsExhaustedForPreloadIDCallback(self.interstitialAdPreloaderClient, preloadID.UTF8String);
  }
}

- (void)adFailedToPreloadForPreloadID:(nonnull NSString *)preloadID error:(nonnull NSError *)error {
  if (self.adFailedToPreloadForPreloadIDCallback) {
    _lastPreloadAdError = error;
    self.adFailedToPreloadForPreloadIDCallback(
        self.interstitialAdPreloaderClient, preloadID.UTF8String, (__bridge GADUTypeErrorRef)error);
  }
}

@end
