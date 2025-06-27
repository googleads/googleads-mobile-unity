// Copyright 2025 Google LLC. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADInterstitialAdPreloader_Preview.h"
#import "GADPreloadDelegate_Preview.h"
#import "GADUObjectCache.h"
#import "GADUPreloadConfigurationV2.h"
#import "GADUTypes.h"

@interface GADUInterstitialAdPreloader : NSObject

/// Initializes a GADUInterstitialAdPreloader.
- (nonnull instancetype)initWithInterstitialAdPreloaderClientReference:
    (_Nonnull GADUTypeInterstitialAdPreloaderClientRef *_Nonnull)interstitialAdPreloaderClient;

/// A reference to the Unity interstitial ad preloader client.
@property(nonatomic, assign)
    _Nonnull GADUTypeInterstitialAdPreloaderClientRef *_Nonnull interstitialAdPreloaderClient;

/// Called when the ad preloaded callback preload ID.
@property(nonatomic, assign, nullable)
    GADUAdAvailableForPreloadIDCallback adAvailableForPreloadIDCallback;

/// Called when the ad failed to preload callback preload ID.
@property(nonatomic, assign, nullable)
    GADUAdFailedToPreloadForPreloadIDCallback adFailedToPreloadForPreloadIDCallback;

/// Called when the last available ad is exhausted for the preload ID.
@property(nonatomic, assign, nullable)
    GADUAdsExhaustedForPreloadIDCallback adsExhaustedForPreloadIDCallback;

/// The interstitial ad response info.
@property(nonatomic, readonly, copy, nullable) GADResponseInfo *responseInfo;

/// Starts preloading interstitial ads from the configuration for the given preload ID.
/// If a delegate is provided, ad events will be forwarded to the delegate.
/// Returns false if preload failed to start. Check console for error log.
- (BOOL)preloadForPreloadID:(nonnull NSString *)preloadID
              configuration:(nonnull GADPreloadConfigurationV2 *)configuration;

/// Returns whether an interstitial ad is preloaded for the given preload ID.
- (BOOL)isAdAvailableWithPreloadID:(nonnull NSString *)preloadID;

/// Returns a preloaded interstitial ad for the given preload ID. Returns nil if an ad is not
/// available.
- (nullable GADInterstitialAd *)adWithPreloadID:(nonnull NSString *)preloadID
                           interstitialAdClient:(_Nonnull GADUTypeInterstitialClientRef *_Nonnull)
                                                    interstitialAdClient;

/// Returns the corresponding configuration for the given preload ID.
- (nullable GADUPreloadConfigurationV2 *)configurationWithPreloadID:(nonnull NSString *)preloadID;

/// Returns a map of preload IDs to their corresponding configurations.
- (nonnull NSDictionary<NSString *, GADUPreloadConfigurationV2 *> *)configurations;

/// Returns the number of preloaded interstitial ads available for the given preload ID.
- (NSUInteger)numberOfAdsAvailableWithPreloadID:(nonnull NSString *)preloadID;

/// Stops preloading interstitial ads for the given preload ID.
/// Removes preloaded interstitial ads for the given preload ID.
- (void)stopPreloadingAndRemoveAdsForPreloadID:(nonnull NSString *)preloadID;

/// Stops preloading all interstitial ads.
/// Removes all preloaded interstitial ads.
- (void)stopPreloadingAndRemoveAllAds;

@end
