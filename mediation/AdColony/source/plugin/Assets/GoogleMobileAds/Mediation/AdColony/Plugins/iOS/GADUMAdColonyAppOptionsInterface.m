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

typedef NS_ENUM(NSUInteger, GADUMAdColonyPrivacyFramework) {
  GADUMAdColonyPrivacyFrameworkGDPR = 0,
  GADUMAdColonyPrivacyFrameworkCCPA = 1
};

static const char *NSStringToCString(NSString *_Nonnull string) {
  const char *utf8String = [string UTF8String];
  if (!utf8String) {
    utf8String = "";
  }

  // Unity recommends that string values returned from a native method should be UTF–8 encoded and
  // allocated on the heap.
  char *cString = (char *)malloc(strlen(utf8String) + 1);
  strcpy(cString, utf8String);
  return cString;
}

static NSString *GADUMAdColonyPrivacyFrameworkFromUnity(
    GADUMAdColonyPrivacyFramework privacyFramework) {
  switch (privacyFramework) {
    case GADUMAdColonyPrivacyFrameworkGDPR:
      return ADC_GDPR;
    case GADUMAdColonyPrivacyFrameworkCCPA:
      return ADC_CCPA;
    default:
      return @"";
  }
}

#pragma mark - GADUMAdColonyAppOptions Interface

void GADUMAdColonyAppOptionsSetPrivacyFrameworkRequired(
    GADUMAdColonyPrivacyFramework privacyFramework, BOOL isRequired) {
  NSString *adColonyPrivacyFramework = GADUMAdColonyPrivacyFrameworkFromUnity(privacyFramework);
  if (!adColonyPrivacyFramework.length) {
    NSLog(@"[AdColony Plugin] Error: Invalid AdColonyPrivacyFramework value provided to the "
          @"AdColony adapter: %ld",
          privacyFramework);
    return;
  }

  AdColonyAppOptions *appOptions = [GADMediationAdapterAdColony appOptions];
  [appOptions setPrivacyFrameworkOfType:adColonyPrivacyFramework isRequired:isRequired];
}

void GADUMAdColonyAppOptionsSetPrivacyConsentString(GADUMAdColonyPrivacyFramework privacyFramework,
                                                    const char *consentString) {
  NSString *adColonyPrivacyFramework = GADUMAdColonyPrivacyFrameworkFromUnity(privacyFramework);
  if (!adColonyPrivacyFramework.length) {
    NSLog(@"[AdColony Plugin] Error: Invalid AdColonyPrivacyFramework value provided to the "
          @"AdColony adapter: %ld",
          privacyFramework);
    return;
  }

  AdColonyAppOptions *appOptions = [GADMediationAdapterAdColony appOptions];
  [appOptions setPrivacyConsentString:@(consentString) forType:adColonyPrivacyFramework];
}

void GADUMAdColonyAppOptionsSetUserID(const char *userID) {
  NSString *customUserID = [NSString stringWithUTF8String:userID];
  AdColonyAppOptions *appOptions = [GADMediationAdapterAdColony appOptions];
  [appOptions setUserID:customUserID];
}

void GADUMAdColonyAppOptionsSetTestMode(BOOL isTestMode) {
  AdColonyAppOptions *appOptions = [GADMediationAdapterAdColony appOptions];
  [appOptions setTestMode:isTestMode];
}

BOOL GADUMAdColonyAppOptionsGetPrivacyFrameworkRequired(
    GADUMAdColonyPrivacyFramework privacyFramework) {
  NSString *adColonyPrivacyFramework = GADUMAdColonyPrivacyFrameworkFromUnity(privacyFramework);
  if (!adColonyPrivacyFramework.length) {
    NSLog(@"[AdColony Plugin] Error: Invalid AdColonyPrivacyFramework value provided to the "
          @"AdColony adapter: %ld",
          privacyFramework);
    return NO;
  }

  AdColonyAppOptions *appOptions = [GADMediationAdapterAdColony appOptions];
  return [appOptions getPrivacyFrameworkRequiredForType:adColonyPrivacyFramework];
}

const char *GADUMAdColonyAppOptionsGetPrivacyConsentString(
    GADUMAdColonyPrivacyFramework privacyFramework) {
  NSString *adColonyPrivacyFramework = GADUMAdColonyPrivacyFrameworkFromUnity(privacyFramework);
  if (!adColonyPrivacyFramework.length) {
    NSLog(@"[AdColony Plugin] Error: Invalid AdColonyPrivacyFramework value provided to the "
          @"AdColony adapter: %ld",
          privacyFramework);
    return "";
  }

  AdColonyAppOptions *appOptions = [GADMediationAdapterAdColony appOptions];
  return NSStringToCString([appOptions getPrivacyConsentStringForType:adColonyPrivacyFramework]);
}

const char *GADUMAdColonyAppOptionsGetUserID() {
  AdColonyAppOptions *appOptions = [GADMediationAdapterAdColony appOptions];
  return NSStringToCString(appOptions.userID);
}

BOOL GADUMAdColonyAppOptionsIsTestMode() {
  AdColonyAppOptions *appOptions = [GADMediationAdapterAdColony appOptions];
  return appOptions.testMode;
}
