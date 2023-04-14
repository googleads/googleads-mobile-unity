// Copyright 2017 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#import "VungleExtrasBuilder.h"

#import <VungleAdapter/VungleAdNetworkExtras.h>

@implementation VungleExtrasBuilder

NSString *const AllPlacementsKey = @"all_placements";

NSString *const UserIdKey = @"user_id";

NSString *const SoundEnabledKey = @"sound_enabled";

- (id<GADAdNetworkExtras>)adNetworkExtrasWithDictionary:
    (NSDictionary<NSString *, NSString *> *)extras {
  NSString *placements = extras[AllPlacementsKey];
  if (!placements) {
    return nil;
  }
  VungleAdNetworkExtras *vungleExtras = [[VungleAdNetworkExtras alloc] init];
  vungleExtras.allPlacements = [placements componentsSeparatedByString:@","];

  NSString *soundEnabled = extras[SoundEnabledKey];
  if (soundEnabled) {
    vungleExtras.muted = ![soundEnabled boolValue];
  }

  NSString *userId = extras[UserIdKey];
  if (userId) {
    vungleExtras.userId = userId;
  }

  return vungleExtras;
}

@end
