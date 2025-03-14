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

- (nonnull GADUPreloadConfiguration *)initWithConfig:(nonnull GADPreloadConfiguration *)config {
  self = [super init];
  if (self) {
    _adUnitID = [config.adUnitID copy];
    _request = [config.request copy];
    _bufferSize = config.bufferSize;
  }
  return self;
}

- (nonnull GADPreloadConfiguration *)preloadConfiguration {
  GADPreloadConfiguration *config = nil;
  if (!self.request) {
    config = [[GADPreloadConfiguration alloc] initWithAdUnitID:_adUnitID];
  } else {
    config = [[GADPreloadConfiguration alloc] initWithAdUnitID:_adUnitID
                                                       request:_request];
  }
  if (self.bufferSize > 0) {
    config.bufferSize = self.bufferSize;
  }
  return config;
}

@end
