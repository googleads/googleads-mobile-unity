// Copyright 2023 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#import "GADUNativeTemplateColor.h"

@implementation GADUNativeTemplateColor {
  float _alpha;
  float _red;
  float _green;
  float _blue;
}

- (nonnull instancetype)initWithAlpha:(float)alpha
                                  red:(float)red
                                green:(float)green
                                 blue:(float)blue {
  self = [super init];
  if (self) {
    _alpha = alpha;
    _red = red;
    _green = green;
    _blue = blue;
  }
  return self;
}

- (nonnull UIColor *)uiColor {
  return [UIColor colorWithRed:_red
                         green:_green
                          blue:_blue
                         alpha:_alpha];
}

@end
