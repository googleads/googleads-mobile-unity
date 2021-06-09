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

#import <Foundation/Foundation.h>

typedef const void *GADUMVerizonMediaTypeMutableDictionaryRef;

/// A cache to hold onto Verizon Media mediation objects while Unity is still referencing them.
@interface GADUMVerizonMediaObjectCache : NSObject

/// Shared instance.
@property(class, nonatomic, readonly, nonnull) GADUMVerizonMediaObjectCache *sharedInstance;

/// Stores reference to an object.
- (void)setObject:(nonnull id)object forKey:(nonnull NSString *)key;

/// Retrieves a reference to an object.
- (nullable id)objectForKey:(nonnull NSString *)key;

/// Removes reference to an object.
- (void)removeObjectForKey:(nonnull NSString *)key;

@end

/// Returns a key used to lookup a Google Mobile Ads object for Verizon Media mediation.
/// This function is intended to only be used by the Mediation Plugin for Verizon Media.
NSString *_Nonnull GADUMVerizonMediaReferenceKeyForObject(_Nonnull id object);
