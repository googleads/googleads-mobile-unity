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

#import "GADUPreloadConfigurationV2.h"

/// Configuration for preloading ads.
@implementation GADUPreloadConfigurationV2

- (nonnull GADUPreloadConfigurationV2 *)initWithConfig:(nonnull GADPreloadConfigurationV2 *)config {
  self = [super init];
  if (self) {
    self.adUnitID = [config.adUnitID copy];
    self.request = [config.request copy];
    self.bufferSize = config.bufferSize;
  }
  return self;
}

- (nonnull GADPreloadConfigurationV2 *)preloadConfiguration {
  GADPreloadConfigurationV2 *config = nil;
  if (!self.request) {
    config = [[GADPreloadConfigurationV2 alloc] initWithAdUnitID:self.adUnitID];
  } else {
    config = [[GADPreloadConfigurationV2 alloc] initWithAdUnitID:self.adUnitID
                                                         request:self.request];
  }
  if (self.bufferSize > 0) {
    config.bufferSize = self.bufferSize;
  }
  return config;
}

@end
