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

#import <VerizonAdsCore/VerizonAdsCore.h>
#import <VerizonMediaAdapter/VerizonMediaAdapter.h>
#import "GADUMVerizonMediaObjectCache.h"
#import "GADUMVerizonMediaUnityUtils.h"

#pragma mark - GADUMVerizonMediaInterface Interface

void GADUMVerizonMediaSetPrivacyData(GADUMVerizonMediaTypeMutableDictionaryRef dictionary,
                                     BOOL restricted) {
  NSMutableDictionary<NSString *, NSString *> *internalDictionary =
      (__bridge NSMutableDictionary *)dictionary;
  NSString *referenceKey = GADUMVerizonMediaReferenceKeyForObject(internalDictionary);

  GADUMVerizonMediaObjectCache *cache = GADUMVerizonMediaObjectCache.sharedInstance;
  [GADMVerizonPrivacy.sharedInstance setPrivacyData:[cache objectForKey:referenceKey]];
  [cache removeObjectForKey:referenceKey];
}

/// Returns a pointer to the IAB Consent Key string that will be freed by Unity.
const char *_Nullable GADUMVerizonMediaGetVerizonIABConsentKey(void) {
  return strdup(kVASConfigIABConsentKey.UTF8String);
}

GADUMVerizonMediaTypeMutableDictionaryRef GADUMVerizonMediaCreateMutableDictionary(void) {
  NSMutableDictionary<NSString *, NSString *> *dictionary = [[NSMutableDictionary alloc] init];
  NSString *referenceKey = GADUMVerizonMediaReferenceKeyForObject(dictionary);

  GADUMVerizonMediaObjectCache *cache = GADUMVerizonMediaObjectCache.sharedInstance;
  [cache setObject:dictionary forKey:referenceKey];
  return (__bridge void *)(dictionary);
}

void GADUMVerizonMediaDictionaryAddObject(GADUMVerizonMediaTypeMutableDictionaryRef dictionary,
                                          const char *key, const char *value) {
  NSMutableDictionary<NSString *, NSString *> *internalDictionary =
      (__bridge NSMutableDictionary *)dictionary;
  GADUMVerizonMediaMutableDictionaryAddObject(internalDictionary, @(key), @(value));
}
