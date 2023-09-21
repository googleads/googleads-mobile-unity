// Copyright 2022 Google LLC
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

#import "GADUNativeTemplateStyle.h"
#import "GADUPluginUtil.h"

@implementation GADUNativeTemplateStyle

- (nonnull instancetype)initWithTemplateName:(nonnull NSString *)templateName {
  self = [super init];
  if (self) {
    _templateName = templateName;
  }
  return self;
}

- (nullable GADTTemplateView *)templateView:(nonnull NSString *)templateName {
  NSString *xibName = @"GADTMediumTemplateView";
  if ([templateName isEqualToString:@"small"]) {
    xibName = @"GADTSmallTemplateView";
  }

  @try {
    NSBundle *bundle = [NSBundle bundleForClass:[self class]];
    GADTTemplateView *templView = [bundle loadNibNamed:xibName owner:nil options:nil].firstObject;
    return templView;
  } @catch (NSException *exception) {
    return nil;
  }
}

- (nullable GADUNativeTemplateViewWrapper *)getDisplayedView:(nullable GADNativeAd *)gadNativeAd {
  GADTTemplateView *templateView = [self templateView:_templateName];
  if (!templateView) {
    return nil;
  }
  templateView.nativeAd = gadNativeAd;

  NSMutableDictionary *styles = [[NSMutableDictionary alloc] init];
  if ([GADUPluginUtil isNotNull:_mainBackgroundColor]) {
    styles[GADTNativeTemplateStyleKeyMainBackgroundColor] = _mainBackgroundColor;
  }

  if ([GADUPluginUtil isNotNull:_callToActionStyle]) {
    if ([GADUPluginUtil isNotNull:_callToActionStyle.backgroundColor]) {
      styles[GADTNativeTemplateStyleKeyCallToActionBackgroundColor] =
          _callToActionStyle.backgroundColor;
    }
    if ([GADUPluginUtil isNotNull:_callToActionStyle.font]) {
      styles[GADTNativeTemplateStyleKeyCallToActionFont] = _callToActionStyle.font;
    }
    if ([GADUPluginUtil isNotNull:_callToActionStyle.textColor]) {
      styles[GADTNativeTemplateStyleKeyCallToActionFontColor] = _callToActionStyle.textColor;
    }
  }

  if ([GADUPluginUtil isNotNull:_primaryTextStyle]) {
    if ([GADUPluginUtil isNotNull:_primaryTextStyle.backgroundColor]) {
      styles[GADTNativeTemplateStyleKeyPrimaryBackgroundColor] = _primaryTextStyle.backgroundColor;
    }
    if ([GADUPluginUtil isNotNull:_primaryTextStyle.font]) {
      styles[GADTNativeTemplateStyleKeyPrimaryFont] = _primaryTextStyle.font;
    }
    if ([GADUPluginUtil isNotNull:_primaryTextStyle.textColor]) {
      styles[GADTNativeTemplateStyleKeyPrimaryFontColor] = _primaryTextStyle.textColor;
    }
  }

  if ([GADUPluginUtil isNotNull:_secondaryTextStyle]) {
    if ([GADUPluginUtil isNotNull:_secondaryTextStyle.backgroundColor]) {
      styles[GADTNativeTemplateStyleKeySecondaryBackgroundColor] =
          _secondaryTextStyle.backgroundColor;
    }
    if ([GADUPluginUtil isNotNull:_secondaryTextStyle.font]) {
      styles[GADTNativeTemplateStyleKeySecondaryFont] = _secondaryTextStyle.font;
    }
    if ([GADUPluginUtil isNotNull:_secondaryTextStyle.textColor]) {
      styles[GADTNativeTemplateStyleKeySecondaryFontColor] = _secondaryTextStyle.textColor;
    }
  }

  if ([GADUPluginUtil isNotNull:_tertiaryTextStyle]) {
    if ([GADUPluginUtil isNotNull:_tertiaryTextStyle.backgroundColor]) {
      styles[GADTNativeTemplateStyleKeyTertiaryBackgroundColor] =
          _tertiaryTextStyle.backgroundColor;
    }
    if ([GADUPluginUtil isNotNull:_tertiaryTextStyle.font]) {
      styles[GADTNativeTemplateStyleKeyTertiaryFont] = _tertiaryTextStyle.font;
    }
    if ([GADUPluginUtil isNotNull:_tertiaryTextStyle.textColor]) {
      styles[GADTNativeTemplateStyleKeyTertiaryFontColor] = _tertiaryTextStyle.textColor;
    }
  }
  templateView.styles = styles;

  GADUNativeTemplateViewWrapper *wrapper =
      [[GADUNativeTemplateViewWrapper alloc] initWithFrame:CGRectZero];
  wrapper.templateView = templateView;
  return wrapper;
}

@end

@implementation GADUNativeTemplateViewWrapper {
  /// Tracks whether constraints have been applied to the templateView.
  BOOL _hasConstraints;
}

- (void)layoutSubviews {
  [super layoutSubviews];
  if (_templateView) {
    [self addSubview:_templateView];

    if (_hasConstraints) {
      return;
    }

    // Constrain the top of the templateView to the top of this view. This top
    // aligns the template view
    if (_templateView.superview) {
      [_templateView.superview
          addConstraint:[NSLayoutConstraint
                            constraintWithItem:_templateView.superview
                                     attribute:NSLayoutAttributeTop
                                     relatedBy:NSLayoutRelationEqual
                                        toItem:_templateView
                                     attribute:NSLayoutAttributeTop
                                    multiplier:1
                                      constant:0]];
      [_templateView.superview
          addConstraint:
              [NSLayoutConstraint
                  constraintWithItem:_templateView.superview
                           attribute:NSLayoutAttributeBottom
                           relatedBy:NSLayoutRelationGreaterThanOrEqual
                              toItem:_templateView
                           attribute:NSLayoutAttributeBottom
                          multiplier:1
                            constant:0]];
    }
    [_templateView addHorizontalConstraintsToSuperviewWidth];
    [_templateView addVerticalCenterConstraintToSuperview];
    _hasConstraints = YES;
  }
}

@end
