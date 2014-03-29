// Copyright 2014 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>

#import "GADUObjectCache.h"

@implementation GADUObjectCache

+ (instancetype)sharedInstance {
  static GADUObjectCache *sharedInstance;
  static dispatch_once_t onceToken;
  dispatch_once(&onceToken, ^{ sharedInstance = [[self alloc] init]; });
  return sharedInstance;
}

- (id)init {
  self = [super init];
  if (self) {
    self.references = [[[NSMutableDictionary alloc] init] autorelease];
  }
  return self;
}

- (void)dealloc {
  [_references release];
  [super dealloc];
}

@end

@implementation NSObject (GADUOwnershipAdditions)

- (NSString *)gadu_referenceKey {
  return [NSString stringWithFormat:@"%p", (void *)self];
}

@end
