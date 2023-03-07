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

typedef NS_ENUM(NSUInteger, GADUMVungleConsentStatus) {
  GADUMVungleConsentStatusAccepted = 0,
  GADUMVungleConsentStatusDenied = 1
};

typedef NS_ENUM(NSUInteger, GADUMVungleCCPAStatus) {
  GADUMVungleCCPAStatusAccepted = 0,
  GADUMVungleCCPAStatusDenied = 1
};

#pragma mark - GADUMVungleInterface implementation

void GADUMVungleUpdateConsentStatus(int consentStatus, const char *consentMessageVersion) {
  switch (consentStatus) {
    case GADUMVungleConsentStatusAccepted:
      [[VungleSDK sharedSDK] updateConsentStatus:VungleConsentAccepted
                           consentMessageVersion:@(consentMessageVersion)];
      break;
    case GADUMVungleConsentStatusDenied:
      [[VungleSDK sharedSDK] updateConsentStatus:VungleConsentDenied
                           consentMessageVersion:@(consentMessageVersion)];
      break;
    default:
      break;
  }
}

void GADUMVungleUpdateCCPAStatus(int ccpaStatus) {
  switch (ccpaStatus) {
    case GADUMVungleCCPAStatusAccepted:
      [[VungleSDK sharedSDK] updateCCPAStatus:VungleCCPAAccepted];
      break;
    case GADUMVungleCCPAStatusDenied:
      [[VungleSDK sharedSDK] updateCCPAStatus:VungleCCPADenied];
      break;
    default:
      break;
  }
}
