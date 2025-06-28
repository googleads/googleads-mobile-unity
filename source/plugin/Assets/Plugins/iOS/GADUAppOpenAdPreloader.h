/**
 * Copyright (C) 2025 Google, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADAppOpenAdPreloader_Preview.h"
#import "GADPreloadDelegate_Preview.h"
#import "GADUObjectCache.h"
#import "GADUPreloadConfigurationV2.h"
#import "GADUTypes.h"

@interface GADUAppOpenAdPreloader : NSObject

/// Initializes a GADUAppOpenAdPreloader.
- (nullable instancetype)initWithAppOpenAdPreloaderClientReference:
    (_Nonnull GADUTypeAppOpenAdPreloaderClientRef *_Nonnull)appOpenAdPreloaderClient;

/// A reference to the Unity app open ad preloader client.
@property(nonatomic, assign)
    _Nonnull GADUTypeAppOpenAdPreloaderClientRef *_Nonnull appOpenAdPreloaderClient;

/// Called when the ad preloaded callback preload ID.
@property(nonatomic, assign, nullable)
    GADUAdAvailableForPreloadIDCallback adAvailableForPreloadIDCallback;

/// Called when the ad failed to preload callback preload ID.
@property(nonatomic, assign, nullable)
    GADUAdFailedToPreloadForPreloadIDCallback adFailedToPreloadForPreloadIDCallback;

/// Called when the last available ad is exhausted for the preload ID.
@property(nonatomic, assign, nullable)
    GADUAdsExhaustedForPreloadIDCallback adsExhaustedForPreloadIDCallback;

/// The app open ad response info.
@property(nonatomic, readonly, copy, nullable) GADResponseInfo *responseInfo;

/// Starts preloading app open ads from the configuration for the given preload ID.
/// If a delegate is provided, ad events will be forwarded to the delegate.
/// Returns false if preload failed to start. Check console for error log.
- (BOOL)preloadForPreloadID:(nonnull NSString *)preloadID
              configuration:(nonnull GADPreloadConfigurationV2 *)configuration;

/// Returns whether an app open ad is preloaded for the given preload ID.
- (BOOL)isAdAvailableWithPreloadID:(nonnull NSString *)preloadID;

/// Returns a preloaded app open ad for the given preload ID. Returns nil if an ad is not
/// available.
- (nullable GADAppOpenAd *)adWithPreloadID:(nonnull NSString *)preloadID
                           appOpenAdClient:
                               (_Nonnull GADUTypeAppOpenAdClientRef *_Nonnull)appOpenAdClient;

/// Returns the corresponding configuration for the given preload ID.
- (nullable GADUPreloadConfigurationV2 *)configurationWithPreloadID:(nonnull NSString *)preloadID;

/// Returns a map of preload IDs to their corresponding configurations.
- (nonnull NSDictionary<NSString *, GADUPreloadConfigurationV2 *> *)configurations;

/// Returns the number of preloaded app open ads available for the given preload ID.
- (NSUInteger)numberOfAdsAvailableWithPreloadID:(nonnull NSString *)preloadID;

/// Stops preloading app open ads for the given preload ID.
/// Removes preloaded app open ads for the given preload ID.
- (void)stopPreloadingAndRemoveAdsForPreloadID:(nonnull NSString *)preloadID;

/// Stops preloading all app open ads.
/// Removes all preloaded app open ads.
- (void)stopPreloadingAndRemoveAllAds;

@end
