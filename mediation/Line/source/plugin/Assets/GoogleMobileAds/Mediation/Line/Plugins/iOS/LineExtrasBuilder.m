// Copyright 2024 Google LLC
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

#import "LineExtrasBuilder.h"

#import <LineAdapter/GADMediationAdapterLineExtras.h>

@implementation LineExtrasBuilder

NSString *const EnableAdSoundKey = @"enable_ad_sound";

- (id<GADAdNetworkExtras>)adNetworkExtrasWithDictionary:
    (NSDictionary<NSString *, NSString *> *)extras {
  GADMediationAdapterLineExtras *lineExtras = [[GADMediationAdapterLineExtras alloc] init];

  NSString *enableAdSound = extras[EnableAdSoundKey];
  lineExtras.adAudio = GADMediationAdapterLineAdAudioUnset;
  if (!enableAdSound) {
    return lineExtras;
  }
  if (enableAdSound.boolValue) {
    lineExtras.adAudio = GADMediationAdapterLineAdAudioUnmuted;
  } else {
    lineExtras.adAudio = GADMediationAdapterLineAdAudioMuted;
  }
  return lineExtras;
}

@end
