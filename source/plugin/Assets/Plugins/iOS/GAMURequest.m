// Copyright 2023 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#import "GAMURequest.h"

@implementation GAMURequest

- (id)init {
  self = [super init];
  if (self) {
    _categoryExclusions = [[NSMutableArray alloc] init];
    _customTargeting = [[NSMutableDictionary alloc] init];
  }
  return self;
}

- (void)addCategoryExclusion:(nonnull NSString *)category {
  [self.categoryExclusions addObject:category];
}

- (void)setCustomTargetingWithKey:(nonnull NSString *)key value:(NSString *)value {
  [self.customTargeting setValue:value forKey:key];
}

- (GAMRequest *)request {
  GAMRequest *request = [GAMRequest request];
  request.keywords = self.keywords;
  request.requestAgent = self.requestAgent;
  GADExtras *extras = [[GADExtras alloc] init];
  extras.additionalParameters = self.extras;
  [request registerAdNetworkExtras:extras];
  request.publisherProvidedID = self.publisherProvidedID;
  request.categoryExclusions = self.categoryExclusions;
  request.customTargeting = self.customTargeting;

  for (id<GADAdNetworkExtras> mediationExtras in self.mediationExtras) {
    [request registerAdNetworkExtras:mediationExtras];
  }
  return request;
}

@end
