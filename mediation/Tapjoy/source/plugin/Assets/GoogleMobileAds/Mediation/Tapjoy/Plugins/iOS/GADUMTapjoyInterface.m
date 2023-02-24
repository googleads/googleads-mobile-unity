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

#import <Tapjoy/Tapjoy.h>

void GADUMTapjoySetUserConsent(const char *consentString) {
  TJPrivacyPolicy *privacyPolicy = [Tapjoy getPrivacyPolicy];
  [privacyPolicy setUserConsent:@(consentString)];
}

void GADUMTapjoySubjectToGDPR(BOOL gdprApplicability) {
  TJPrivacyPolicy *privacyPolicy = [Tapjoy getPrivacyPolicy];
  [privacyPolicy setSubjectToGDPR:gdprApplicability];
}

void GADUMTapjoySetUSPrivacy(const char *privacyString) {
  TJPrivacyPolicy *privacyPolicy = [Tapjoy getPrivacyPolicy];
  [privacyPolicy setUSPrivacy:@(privacyString)];
}
