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

#import "GADUMVerizonMediaObjectCache.h"
#import "GADUMVerizonMediaUnityUtils.h"

@implementation GADUMVerizonMediaObjectCache {
  /// References to objects Verizon Media mediation objects created from Unity.
  NSMutableDictionary<NSString *, id> *_references;

  /// Lock queue for instance variables.
  dispatch_queue_t _lockQueue;
}

+ (nonnull GADUMVerizonMediaObjectCache *)sharedInstance {
  static GADUMVerizonMediaObjectCache *sharedInstance;
  static dispatch_once_t onceToken;
  dispatch_once(&onceToken, ^{
    sharedInstance = [[GADUMVerizonMediaObjectCache alloc] init];
  });
  return sharedInstance;
}

- (nonnull GADUMVerizonMediaObjectCache *)init {
  self = [super init];
  if (self) {
    _references = [[NSMutableDictionary alloc] init];
    _lockQueue = dispatch_queue_create("verizonMedia-unityObjectCache", DISPATCH_QUEUE_SERIAL);
  }
  return self;
}

- (void)setObject:(nonnull id)object forKey:(nonnull NSString *)key {
  dispatch_async(_lockQueue, ^{
    GADUMVerizonMediaMutableDictionaryAddObject(_references, key, object);
  });
}

- (nullable id)objectForKey:(nonnull NSString *)key {
  __block id referenceObject = nil;
  dispatch_sync(_lockQueue, ^{
    referenceObject = _references[key];
  });
  return referenceObject;
}

- (void)removeObjectForKey:(nonnull NSString *)key {
  dispatch_async(_lockQueue, ^{
    GADUMVerizonMediaMutableDictionaryRemoveObjectForKey(_references, key);
  });
}

@end

NSString *_Nonnull GADUMVerizonMediaReferenceKeyForObject(_Nonnull id object) {
  return [NSString stringWithFormat:@"%p", object];
}
