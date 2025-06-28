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
#import "GADPreloadConfigurationV2_Preview.h"

#import "GADUTypes.h"

/// Configuration for preloading ads.
@interface GADUPreloadConfigurationV2 : NSObject

/// The ad unit ID.
@property(nonatomic, nonnull) NSString *adUnitID;

/// The GADRequest object.
@property(nonatomic, nullable) GADRequest *request;

/// The maximum amount of ads buffered for this configuration.
@property(nonatomic, readwrite) NSUInteger bufferSize;

/// The preload configuration object compatible with the GMA iOS SDK.
@property(nonatomic, readonly, nonnull) GADPreloadConfigurationV2 *preloadConfiguration;

/// Initializes the GADUPreloadConfigurationV2 with the GADPreloadConfigurationV2.
- (nonnull id)initWithConfig:(nonnull GADPreloadConfigurationV2 *)config;

@end
