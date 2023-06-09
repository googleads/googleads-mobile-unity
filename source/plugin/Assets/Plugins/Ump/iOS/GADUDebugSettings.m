// Copyright 2022 Google LLC. All Rights Reserved.

#import "GADUDebugSettings.h"

@implementation GADUDebugSettings
- (instancetype)init {
  self = [super init];
  if (self) {
    _geography = kGADUDebugGeographyDisabled;
  }
  return self;
}
@end
