// Copyright 2022 Google LLC. All Rights Reserved.

#import "GADUUMPRequestParameters.h"
@implementation GADUUMPRequestParameters
- (instancetype)init {
  self = [super init];
  if (self) {
    _tagForUnderAgeOfConsent = false;
  }
  return self;
}
@end
