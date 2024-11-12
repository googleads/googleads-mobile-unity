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

@synthesize format;

- (nonnull GADUPreloadConfiguration *)initWithConfig:(nonnull GADPreloadConfiguration *)config {
  self = [super init];
  if (self) {
    self.adUnitID = config.adUnitID;
    self.request = config.request;
    self.bufferSize = config.bufferSize;
    self.format = config.format;
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
  switch (format) {
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
    default:
      NSLog(@"Unknown format: %d", format);
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
      format = GADAdFormatBanner;
      break;
    case kGADUAdFormatInterstitial:
      format = GADAdFormatInterstitial;
      break;
    case kGADUAdFormatRewarded:
      format = GADAdFormatRewarded;
      break;
    case kGADUAdFormatRewardedInterstitial:
      format = GADAdFormatRewardedInterstitial;
      break;
    case kGADUAdFormatNative:
      format = GADAdFormatNative;
      break;
    case kGADUAdFormatAppOpen:
      format = GADAdFormatAppOpen;
      break;
    default:
      NSLog(@"Unknown format: %d", adFormat);
      break;
  }
}

- (nonnull GADPreloadConfiguration *)preloadConfiguration {
  GADPreloadConfiguration *config = nil;
  if (!self.request) {
    config = [[GADPreloadConfiguration alloc] initWithAdUnitID:self.adUnitID
                                                      adFormat:(GADAdFormat)self.format];
  } else {
    config = [[GADPreloadConfiguration alloc] initWithAdUnitID:self.adUnitID
                                                      adFormat:(GADAdFormat)self.format
                                                       request:self.request];
  }
  config.bufferSize = self.bufferSize;
  return config;
}

@end
