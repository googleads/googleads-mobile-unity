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

#import <VungleSDK/VungleSDK.h>

#pragma mark - Utility

typedef NS_ENUM(NSUInteger, GADUMLiftoffMonetizeConsentStatus) {
  GADUMLiftoffMonetizeConsentStatusAccepted = 0,
  GADUMLiftoffMonetizeConsentStatusDenied = 1
};

typedef NS_ENUM(NSUInteger, GADUMLiftoffMonetizeCCPAStatus) {
  GADUMLiftoffMonetizeCCPAStatusAccepted = 0,
  GADUMLiftoffMonetizeCCPAStatusDenied = 1
};

#pragma mark - GADUMVungleInterface implementation

void GADUMLiftoffMonetizeUpdateConsentStatus(int consentStatus, const char *consentMessageVersion) {
  switch (consentStatus) {
    case GADUMLiftoffMonetizeConsentStatusAccepted:
      [[VungleSDK sharedSDK] updateConsentStatus:VungleConsentAccepted
                           consentMessageVersion:@(consentMessageVersion)];
      break;
    case GADUMLiftoffMonetizeConsentStatusDenied:
      [[VungleSDK sharedSDK] updateConsentStatus:VungleConsentDenied
                           consentMessageVersion:@(consentMessageVersion)];
      break;
    default:
      break;
  }
}

void GADUMLiftoffMonetizeUpdateCCPAStatus(int ccpaStatus) {
  switch (ccpaStatus) {
    case GADUMLiftoffMonetizeCCPAStatusAccepted:
      [[VungleSDK sharedSDK] updateCCPAStatus:VungleCCPAAccepted];
      break;
    case GADUMLiftoffMonetizeCCPAStatusDenied:
      [[VungleSDK sharedSDK] updateCCPAStatus:VungleCCPADenied];
      break;
    default:
      break;
  }
}
