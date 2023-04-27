// Copyright 2019 Google LLC
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

#import <IASDKCore/IASDKCore.h>

#pragma mark - GADUMVerizonMediaInterface Interface

void GADUMDTExchangeSetGDPRConsent(bool consent) {
  [IASDKCore.sharedInstance setGDPRConsent:consent];
}

void GADUMDTExchangeSetGDPRConsentString(const char *_Nonnull consentString) {
  [IASDKCore.sharedInstance setGDPRConsentString:@(consentString)];
}

void GADUMDTExchangeClearGDPRConsentData() { [IASDKCore.sharedInstance clearGDPRConsentData]; }

void GADUMDTExchangeSetCCPAString(const char *_Nonnull ccpaString) {
  IASDKCore.sharedInstance.CCPAString = @(ccpaString);
}

void GADUMDTExchangeClearCCPAString() { IASDKCore.sharedInstance.CCPAString = nil; }
