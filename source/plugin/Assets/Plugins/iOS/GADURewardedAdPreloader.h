// Copyright 2025 Google LLC. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADRewardedAdPreloader_Preview.h"
#import "GADUPreloadConfigurationV2.h"
#import "GADPreloadDelegate_Preview.h"
#import "GADUObjectCache.h"
#import "GADUTypes.h"

@interface GADURewardedAdPreloader : NSObject

/// Initializes a GADURewardedAdPreloader.
- (nonnull instancetype)initWithRewardedAdPreloaderClientReference:
        (_Nonnull GADUTypeRewardedAdPreloaderClientRef *_Nonnull)rewardedAdPreloaderClient;

/// A reference to the Unity rewarded ad preloader client.
@property(nonatomic, assign) _Nonnull GADUTypeRewardedAdPreloaderClientRef *_Nonnull
        rewardedAdPreloaderClient;

/// Called when the ad preloaded callback preload ID.
@property(nonatomic, assign, nullable) GADUAdAvailableForPreloadIDCallback
        adAvailableForPreloadIDCallback;

/// Called when the ad failed to preload callback preload ID.
@property(nonatomic, assign, nullable) GADUAdFailedToPreloadForPreloadIDCallback
        adFailedToPreloadForPreloadIDCallback;

/// Called when the last available ad is exhausted for the preload ID.
@property(nonatomic, assign, nullable) GADUAdsExhaustedForPreloadIDCallback
        adsExhaustedForPreloadIDCallback;

/// The rewarded ad response info.
@property(nonatomic, readonly, copy, nullable) GADResponseInfo *responseInfo;

/// Starts preloading rewarded ads from the configuration for the given preload ID.
/// If a delegate is provided, ad events will be forwarded to the delegate.
/// Returns false if preload failed to start. Check console for error log.
- (BOOL)preloadForPreloadID:(nonnull NSString *)preloadID
              configuration:(nonnull GADPreloadConfigurationV2 *)configuration;

/// Returns whether an rewarded ad is preloaded for the given preload ID.
- (BOOL)isAdAvailableWithPreloadID:(nonnull NSString *)preloadID;

/// Returns a preloaded rewarded ad for the given preload ID. Returns nil if an ad is not
/// available.
- (nullable GADRewardedAd *)adWithPreloadID:(nonnull NSString *)preloadID rewardedAdClient:(_Nonnull GADUTypeRewardedAdClientRef *_Nonnull)rewardedAdClient;

/// Returns the corresponding configuration for the given preload ID.
- (nullable GADUPreloadConfigurationV2 *)configurationWithPreloadID:(nonnull NSString *)preloadID;

/// Returns a map of preload IDs to their corresponding configurations.
- (nonnull NSDictionary<NSString *, GADUPreloadConfigurationV2 *> *)configurations;

/// Returns the number of preloaded rewarded ads available for the given preload ID.
- (NSUInteger)numberOfAdsAvailableWithPreloadID:(nonnull NSString *)preloadID;

/// Stops preloading rewarded ads for the given preload ID.
/// Removes preloaded rewarded ads for the given preload ID.
- (void)stopPreloadingAndRemoveAdsForPreloadID:(nonnull NSString *)preloadID;

/// Stops preloading all rewarded ads.
/// Removes all preloaded rewarded ads.
- (void)stopPreloadingAndRemoveAllAds;

@end
