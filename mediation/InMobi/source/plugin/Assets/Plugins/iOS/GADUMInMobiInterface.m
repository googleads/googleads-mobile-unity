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

#import <InMobiAdapter/InMobiAdapter.h>

void GADMInMobiUpdateGDPRConsent(const char* consentObjectRawString) {
  NSString *consentObjectString = [NSString stringWithUTF8String:consentObjectRawString];
  NSMutableDictionary *consentObject = [[NSMutableDictionary alloc] init];

  NSArray *entries = [consentObjectString componentsSeparatedByString:@";"];
  for (int i = 0; i < entries.count-1; i++) {
    NSArray *keyValue = [entries[i] componentsSeparatedByString:@"="];
    [consentObject setValue:keyValue[1] forKey:keyValue[0]];
  }

  [GADMInMobiConsent updateGDPRConsent:consentObject];
}
