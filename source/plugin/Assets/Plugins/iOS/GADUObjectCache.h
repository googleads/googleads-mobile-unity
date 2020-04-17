// Copyright 2014 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>

/// A thread-safe cache to hold onto objects while Unity is still referencing them.
@interface GADUObjectCache : NSObject

+ (nonnull instancetype)sharedInstance;

/// Reference to self for backwards API compatibility.
@property(nonatomic, readonly, nonnull) GADUObjectCache *references;

/// NSMutableDictionary methods for setting and retrieving objects.
- (void)setObject:(nullable id)object forKey:(nonnull NSString *)key;
- (nullable id)objectForKey:(nonnull NSString *)key;
- (void)setObject:(nullable id)obj forKeyedSubscript:(nonnull NSString *)key;
- (nullable id)objectForKeyedSubscript:(nonnull NSString *)key;
- (void)removeObjectForKey:(nonnull NSString *)key;

@end

@interface NSObject (GADUOwnershipAdditions)

/// Returns a key used to lookup a Google Mobile Ads object. This method is intended to only be used
/// by Google Mobile Ads objects.
- (nonnull NSString *)gadu_referenceKey;

@end
