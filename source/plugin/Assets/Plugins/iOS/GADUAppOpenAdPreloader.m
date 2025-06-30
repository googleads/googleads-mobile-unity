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

#import "GADUAppOpenAd.h"

#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

#import "GADUAppOpenAdPreloader.h"
#import "UnityInterface.h"

@interface GADUAppOpenAd ()
- (void)setAppOpenAdAndConfigure:(GADAppOpenAd *)appOpenAd;
@end

@interface GADUAppOpenAdPreloader () <GADPreloadDelegate>
@end

@implementation GADUAppOpenAdPreloader {
  // Keep a reference to the error objects so references to Unity-level
  // ResponseInfo object are not released until the ad object is released.
  NSError *_lastPreloadAdError;
}

- (nullable instancetype)initWithAppOpenAdPreloaderClientReference:
    (_Nonnull GADUTypeAppOpenAdPreloaderClientRef *_Nonnull)appOpenAdPreloaderClient {
  self = [super init];
  if (!self) {
    return nil;
  }
  _appOpenAdPreloaderClient = appOpenAdPreloaderClient;
  return self;
}

- (BOOL)preloadForPreloadID:(nonnull NSString *)preloadID
              configuration:(nonnull GADPreloadConfigurationV2 *)configuration {
  return [GADAppOpenAdPreloader.sharedInstance preloadForPreloadID:preloadID
                                                     configuration:configuration
                                                          delegate:self];
}

- (BOOL)isAdAvailableWithPreloadID:(nonnull NSString *)preloadID {
  return [GADAppOpenAdPreloader.sharedInstance isAdAvailableWithPreloadID:preloadID];
}

- (nullable GADUAppOpenAd *)adWithPreloadID:(nonnull NSString *)preloadID
                            appOpenAdClient:
                                (_Nonnull GADUTypeAppOpenAdClientRef *_Nonnull)appOpenAdClient {
  GADAppOpenAd *nativeAppOpenAd = [GADAppOpenAdPreloader.sharedInstance adWithPreloadID:preloadID];
  if (nativeAppOpenAd) {
    GADUAppOpenAd *appOpenAd =
        [[GADUAppOpenAd alloc] initWithAppOpenAdClientReference:appOpenAdClient];
    [appOpenAd setAppOpenAdAndConfigure:nativeAppOpenAd];
    return appOpenAd;
  }
  return nil;
}

- (nullable GADUPreloadConfigurationV2 *)configurationWithPreloadID:(nonnull NSString *)preloadID {
  GADPreloadConfigurationV2 *config =
      [GADAppOpenAdPreloader.sharedInstance configurationWithPreloadID:preloadID];
  if (!config) {
    return nil;
  }
  return [[GADUPreloadConfigurationV2 alloc] initWithConfig:config];
}

- (nonnull NSDictionary<NSString *, GADUPreloadConfigurationV2 *> *)configurations {
  NSDictionary<NSString *, GADPreloadConfigurationV2 *> *configs =
      [GADAppOpenAdPreloader.sharedInstance configurations];
  NSMutableDictionary<NSString *, GADUPreloadConfigurationV2 *> *convertedConfigs =
      [NSMutableDictionary dictionaryWithCapacity:configs.count];
  for (NSString *key in configs) {
    convertedConfigs[key] = [[GADUPreloadConfigurationV2 alloc] initWithConfig:configs[key]];
  }
  return convertedConfigs;
}

- (NSUInteger)numberOfAdsAvailableWithPreloadID:(nonnull NSString *)preloadID {
  return [GADAppOpenAdPreloader.sharedInstance numberOfAdsAvailableWithPreloadID:preloadID];
}

- (void)stopPreloadingAndRemoveAdsForPreloadID:(nonnull NSString *)preloadID {
  [GADAppOpenAdPreloader.sharedInstance stopPreloadingAndRemoveAdsForPreloadID:preloadID];
}

- (void)stopPreloadingAndRemoveAllAds {
  [GADAppOpenAdPreloader.sharedInstance stopPreloadingAndRemoveAllAds];
}

#pragma mark GADPreloadDelegate implementation

- (void)adAvailableForPreloadID:(nonnull NSString *)preloadID
                   responseInfo:(nonnull GADResponseInfo *)responseInfo {
  if (self.adAvailableForPreloadIDCallback) {
    GADUObjectCache *cache = GADUObjectCache.sharedInstance;
    cache[responseInfo.gadu_referenceKey] = responseInfo;
    self.adAvailableForPreloadIDCallback(self.appOpenAdPreloaderClient, preloadID.UTF8String,
                                         (__bridge GADUTypeResponseInfoRef)(responseInfo));
  }
}

- (void)adsExhaustedForPreloadID:(nonnull NSString *)preloadID {
  if (self.adsExhaustedForPreloadIDCallback) {
    self.adsExhaustedForPreloadIDCallback(self.appOpenAdPreloaderClient, preloadID.UTF8String);
  }
}

- (void)adFailedToPreloadForPreloadID:(nonnull NSString *)preloadID error:(nonnull NSError *)error {
  if (self.adFailedToPreloadForPreloadIDCallback) {
    _lastPreloadAdError = error;
    self.adFailedToPreloadForPreloadIDCallback(self.appOpenAdPreloaderClient, preloadID.UTF8String,
                                               (__bridge GADUTypeErrorRef)error);
  }
}

@end
