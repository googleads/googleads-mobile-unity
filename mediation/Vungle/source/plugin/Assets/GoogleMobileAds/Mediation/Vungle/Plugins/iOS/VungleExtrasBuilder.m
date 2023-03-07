// Copyright 2017 Google Inc. All Rights Reserved.

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
