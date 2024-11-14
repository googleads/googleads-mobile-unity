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

// Internal ivar that represents the GADAdFormat value.
@synthesize format = _format;

- (nonnull GADUPreloadConfiguration *)initWithConfig:(nonnull GADPreloadConfiguration *)config {
  self = [super init];
  if (self) {
    _adUnitID = [config.adUnitID copy];
    _request = [config.request copy];
    _bufferSize = config.bufferSize;
    _format = (int)config.format;
  }
  return self;
}

// Format is read only when returned to Unity, not in the GMA iOS SDK.
// Banner                0 -> 0
// Interstitial          1 -> 1
// Rewarded              2 -> 2
// Rewarded Interstitial 3 -> 4
// Native                4 -> 3
// AppOpen               6 -> 5
- (int)format {
  switch (_format) {
    case GADAdFormatBanner:
      return kGADUAdFormatBanner;
    case GADAdFormatInterstitial:
      return kGADUAdFormatInterstitial;
    case GADAdFormatRewarded:
      return kGADUAdFormatRewarded;
    case GADAdFormatRewardedInterstitial:
      return kGADUAdFormatRewardedInterstitial;
    case GADAdFormatNative:
      return kGADUAdFormatNative;
    case GADAdFormatAppOpen:
      return kGADUAdFormatAppOpen;
  }
  return -1;
}

// Format needs to be set only to pass to the GMA iOS SDK.
// Banner                0 -> 0
// Interstitial          1 -> 1
// Rewarded              2 -> 2
// Rewarded Interstitial 3 -> 4
// Native                4 -> 3
// AppOpen               5 -> 6
- (void)setFormat:(int)adFormat {
  switch (adFormat) {
    case kGADUAdFormatBanner:
      _format = GADAdFormatBanner;
      break;
    case kGADUAdFormatInterstitial:
      _format = GADAdFormatInterstitial;
      break;
    case kGADUAdFormatRewarded:
      _format = GADAdFormatRewarded;
      break;
    case kGADUAdFormatRewardedInterstitial:
      _format = GADAdFormatRewardedInterstitial;
      break;
    case kGADUAdFormatNative:
      _format = GADAdFormatNative;
      break;
    case kGADUAdFormatAppOpen:
      _format = GADAdFormatAppOpen;
      break;
  }
}

- (nonnull GADPreloadConfiguration *)preloadConfiguration {
  GADPreloadConfiguration *config = nil;
  if (!self.request) {
    config = [[GADPreloadConfiguration alloc] initWithAdUnitID:_adUnitID
                                                      adFormat:(GADAdFormat)_format];
  } else {
    config = [[GADPreloadConfiguration alloc] initWithAdUnitID:_adUnitID
                                                      adFormat:(GADAdFormat)_format
                                                       request:_request];
  }
  if (self.bufferSize > 0) {
    config.bufferSize = self.bufferSize;
  }
  return config;
}

@end
