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

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADURequest.h"

/// Specifies optional parameters for ad requests.
@interface GAMURequest : GADURequest

/// Publisher provided user ID.
@property(nonatomic, copy, nullable) NSString *publisherProvidedID;

/// Array of strings used to exclude specified categories in ad results.
@property(nonatomic, copy, nullable) NSMutableArray *categoryExclusions;

/// Key-value pairs used for custom targeting.
@property(nonatomic, copy, nullable) NSMutableDictionary *customTargeting;

/// Convenience method for adding a category exclusion label.
- (void)addCategoryExclusion:(nonnull NSString *)category;

/// Convenience method for setting custom targeting parameters.
- (void)setCustomTargetingWithKey:(nonnull NSString *)key value:(nullable NSString *)value;

/// Constructs a GAMRequest with the defined targeting values.
- (nonnull GAMRequest *)request;

@end
