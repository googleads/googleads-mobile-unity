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

#import <UIKit/UIKit.h>

/// Font Styles for Native Templates
typedef NS_ENUM(NSInteger, GADUNativeTemplateFontStyle) {
  GADUNativeTemplateFontNormal,
  GADUNativeTemplateFontBold,
  GADUNativeTemplateFontItalic,
  GADUNativeTemplateFontMonospace
};

/// Contains style options that can be applied to text in a native template.
@interface GADUNativeTemplateTextStyle : NSObject

@property(nonatomic, nullable) UIColor *textColor;
@property(nonatomic, nullable) UIColor *backgroundColor;
@property(nonatomic) GADUNativeTemplateFontStyle fontStyle;
@property(nonatomic, nullable) NSNumber *size;

/// UIFont that has the corresponding fontStyle and size.
@property(readonly, nullable) UIFont *font;
@end
