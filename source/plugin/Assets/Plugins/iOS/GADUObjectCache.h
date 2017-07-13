// Copyright 2014 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>

/// A cache to hold onto objects while Unity is still referencing them.
@interface GADUObjectCache : NSObject

+ (instancetype)sharedInstance;

/// References to objects Google Mobile ads objects created from Unity.
@property(nonatomic, strong) NSMutableDictionary *references;

@end

@interface NSObject (GADUOwnershipAdditions)

/// Returns a key used to lookup a Google Mobile Ads object. This method is intended to only be used
/// by Google Mobile Ads objects.
- (NSString *)gadu_referenceKey;

@end
