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

#import "LiftoffMonetizeExtrasBuilder.h"

#import <LiftoffMonetizeAdapter/VungleAdNetworkExtras.h>

@implementation LiftoffMonetizeExtrasBuilder

NSString *const UserIdKey = @"user_id";

- (id<GADAdNetworkExtras>)adNetworkExtrasWithDictionary:
    (NSDictionary<NSString *, NSString *> *)extras {
  VungleAdNetworkExtras *vungleExtras = [[VungleAdNetworkExtras alloc] init];

  NSString *userId = extras[UserIdKey];
  if (userId) {
    vungleExtras.userId = userId;
  }

  return vungleExtras;
}

@end
