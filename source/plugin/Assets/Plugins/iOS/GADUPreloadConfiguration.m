/**
 * Copyright (C) 2024 Google, Inc.
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

#import "GADUPreloadConfiguration.h"

/// Configuration for preloading ads.
@implementation GADUPreloadConfiguration

+(GADPreloadConfiguration *_Nonnull) preloadConfiguration: (GADUPreloadConfiguration *_Nonnull) config {
    GADPreloadConfiguration *preloadConfiguration = [[GADPreloadConfiguration alloc] init];
    if(config.request == nil) {
        preloadConfiguration = [preloadConfiguration initWithAdUnitID:config.adUnitId adFormat:(GADAdFormat)config.format];
    } else {
        preloadConfiguration = [preloadConfiguration initWithAdUnitID:config.adUnitId
                                                              request: config.request
                                                             adFormat:(GADAdFormat)config.format];
    }
    return preloadConfiguration;
}

+(GADUPreloadConfiguration *_Nonnull) internalPreloadConfiguration: (GADPreloadConfiguration *_Nonnull) config {
    GADUPreloadConfiguration *internalConfig = [[GADUPreloadConfiguration alloc] init];
    [internalConfig setAdUnitId:config.adUnitID];
    [internalConfig setFormat:config.format];
    [internalConfig setRequest:config.request];
    return  internalConfig;
}

@end
