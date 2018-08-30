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
#import <VungleAdapter/VungleAdapter.h>

void GADUMUpdateConsentStatus(int consentStatus,
                              const char *consentMessageVersion) {
  NSString *consentMessage = @"";
  if (consentMessageVersion != nil && strlen(consentMessageVersion) > 0) {
    consentMessage = [NSString stringWithUTF8String:consentMessageVersion];
  }

  if (consentStatus == (int)VungleConsentAccepted) {
    [VungleRouterConsent updateConsentStatus:VungleConsentAccepted
                       consentMessageVersion:consentMessage];
  } else if (consentStatus == (int)VungleConsentDenied) {
    [VungleRouterConsent updateConsentStatus:VungleConsentDenied
                       consentMessageVersion:consentMessage];
  }
}

int GADUMGetCurrentConsentStatus() {
  return (int)[VungleRouterConsent getConsentStatus];
}

const char* GADUMGetCurrentConsentMessageVersion() {
  NSString *message = [VungleRouterConsent getConsentMessageVersion];
  if (message == nil || message.length == 0) {
    return nil;
  }

  char* consentMessage = (char*) malloc(strlen(message.UTF8String) + 1);
  strcpy(consentMessage, message.UTF8String);
  return consentMessage;
}
