// Copyright 2014 Google Inc. All Rights Reserved.

#import "GADUObjectCache.h"

@interface GADUObjectCache ()
/// References to objects Google Mobile ads objects created from Unity.
@property(nonatomic, strong) NSMutableDictionary *internalReferences;

@end

@implementation GADUObjectCache {
  dispatch_queue_t _lockQueue;
}

+ (instancetype)sharedInstance {
  static GADUObjectCache *sharedInstance;
  static dispatch_once_t onceToken;
  dispatch_once(&onceToken, ^{
    sharedInstance = [[self alloc] init];
  });
  return sharedInstance;
}

- (id)init {
  self = [super init];
  if (self) {
    _internalReferences = [[NSMutableDictionary alloc] init];
    _lockQueue = dispatch_queue_create("GADUObjectCache lock queue", DISPATCH_QUEUE_SERIAL);
  }
  return self;
}

- (GADUObjectCache *)references {
  return self;
}

- (void)setObject:(id)object forKey:(NSString *)key {
  dispatch_async(_lockQueue, ^{
    self->_internalReferences[key] = object;
  });
}

- (id)objectForKey:(NSString *)key {
  __block id object;
  dispatch_sync(_lockQueue, ^{
    object = self->_internalReferences[key];
  });
  return object;
}

- (void)setObject:(id)obj forKeyedSubscript:(NSString *)key {
  [self setObject:obj forKey:key];
}

- (id)objectForKeyedSubscript:(NSString *)key {
  return [self objectForKey:key];
}

- (void)removeObjectForKey:(NSString *)key {
  dispatch_async(_lockQueue, ^{
   [self->_internalReferences removeObjectForKey:key];
  });
}

@end

@implementation NSObject (GADUOwnershipAdditions)

- (NSString *)gadu_referenceKey {
  return [NSString stringWithFormat:@"%p", (void *)self];
}

@end
