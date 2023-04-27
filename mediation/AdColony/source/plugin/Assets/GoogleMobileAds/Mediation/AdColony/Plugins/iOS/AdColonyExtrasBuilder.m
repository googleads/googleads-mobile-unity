// Copyright 2018 Google LLC
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

#import "AdColonyExtrasBuilder.h"

#import <AdColonyAdapter/GADMAdapterAdColonyExtras.h>

@implementation AdColonyExtrasBuilder

NSString *const ShowPrePopupKey = @"show_pre_popup";

NSString *const ShowPostPopupKey = @"show_post_popup";

- (id<GADAdNetworkExtras>)adNetworkExtrasWithDictionary:
    (NSDictionary<NSString *, NSString *> *)extras {

  GADMAdapterAdColonyExtras *adColonyExtras = [[GADMAdapterAdColonyExtras alloc] init];

  NSString *showPrePopup = extras[ShowPrePopupKey];
  if (showPrePopup) {
    adColonyExtras.showPrePopup = showPrePopup.boolValue;
  }

  NSString *showPostPopup = extras[ShowPostPopupKey];
  if (showPostPopup) {
    adColonyExtras.showPostPopup = showPostPopup.boolValue;
  }

  return adColonyExtras;
}

@end
