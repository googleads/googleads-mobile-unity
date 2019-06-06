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

#import <AdColony/AdColony.h>
#import <AdColonyAdapter/AdColonyAdapter.h>

#pragma mark - Utility

static const char* NSStringToCString(NSString* whichString) {
  const char* utf8String = [whichString UTF8String];

  if (utf8String) {
    char *cString = (char *)malloc(strlen(utf8String) + 1);
    strcpy(cString, utf8String);

    return cString;
  }
  return "";
}

#pragma mark - GADUMAdColonyAppOptions Interface

void GADUMAdColonyAppOptionsSetGDPRConsentString(const char *consentString) {
  NSString *consent = @"";
  if (consentString) {
    consent = [NSString stringWithUTF8String:consentString];
  }
  AdColonyAppOptions *appOptions = [GADMediationAdapterAdColony appOptions];
  [appOptions setGdprConsentString:consent];
}

void GADUMAdColonyAppOptionsSetGDPRRequired(BOOL gdprRequired) {
  AdColonyAppOptions *appOptions = [GADMediationAdapterAdColony appOptions];
  [appOptions setGdprRequired:gdprRequired];
}

void GADUMAdColonyAppOptionsSetUserId(const char *userId) {
  NSString *customUserId = @"";
  if (customUserId) {
    customUserId = [NSString stringWithUTF8String:userId];
  }
  AdColonyAppOptions *appOptions = [GADMediationAdapterAdColony appOptions];
  [appOptions setUserID:customUserId];
}

void GADUMAdColonyAppOptionsSetTestMode(BOOL isTestMode) {
  AdColonyAppOptions *appOptions = [GADMediationAdapterAdColony appOptions];
  [appOptions setTestMode:isTestMode];
}

const char *GADUMAdColonyAppOptionsGetGDPRConsentString() {
  AdColonyAppOptions *appOptions = [GADMediationAdapterAdColony appOptions];
  return NSStringToCString(appOptions.gdprConsentString);
}

BOOL GADUMAdColonyAppOptionsIsGDPRRequired() {
  AdColonyAppOptions *appOptions = [GADMediationAdapterAdColony appOptions];
  return appOptions.gdprRequired;
}

const char *GADUMAdColonyAppOptionsGetUserId() {
  AdColonyAppOptions *appOptions = [GADMediationAdapterAdColony appOptions];
  return NSStringToCString(appOptions.userID);
}

BOOL GADUMAdColonyAppOptionsIsTestMode() {
  AdColonyAppOptions *appOptions = [GADMediationAdapterAdColony appOptions];
  return appOptions.testMode;
}
