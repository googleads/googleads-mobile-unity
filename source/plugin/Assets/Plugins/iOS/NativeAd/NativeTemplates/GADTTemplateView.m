// Copyright 2018-2021 Google LLC
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

#import "GADTTemplateView.h"
#import <QuartzCore/QuartzCore.h>

GADTNativeTemplateStyleKey _Nonnull const GADTNativeTemplateStyleKeyCallToActionFont =
    @"call_to_action_font";

GADTNativeTemplateStyleKey _Nonnull const GADTNativeTemplateStyleKeyCallToActionFontColor =
    @"call_to_action_font_color";

GADTNativeTemplateStyleKey _Nonnull const GADTNativeTemplateStyleKeyCallToActionBackgroundColor =
    @"call_to_action_background_color";

GADTNativeTemplateStyleKey _Nonnull const GADTNativeTemplateStyleKeySecondaryFont = @"secondary_font";

GADTNativeTemplateStyleKey _Nonnull const GADTNativeTemplateStyleKeySecondaryFontColor =
    @"secondary_font_color";

GADTNativeTemplateStyleKey _Nonnull const GADTNativeTemplateStyleKeySecondaryBackgroundColor =
    @"secondary_background_color";

GADTNativeTemplateStyleKey _Nonnull const GADTNativeTemplateStyleKeyPrimaryFont = @"primary_font";

GADTNativeTemplateStyleKey _Nonnull const GADTNativeTemplateStyleKeyPrimaryFontColor = @"primary_font_color";

GADTNativeTemplateStyleKey _Nonnull const GADTNativeTemplateStyleKeyPrimaryBackgroundColor =
    @"primary_background_color";

GADTNativeTemplateStyleKey _Nonnull const GADTNativeTemplateStyleKeyTertiaryFont = @"tertiary_font";

GADTNativeTemplateStyleKey _Nonnull const GADTNativeTemplateStyleKeyTertiaryFontColor =
    @"tertiary_font_color";

GADTNativeTemplateStyleKey _Nonnull const GADTNativeTemplateStyleKeyTertiaryBackgroundColor =
    @"tertiary_background_color";

GADTNativeTemplateStyleKey _Nonnull const GADTNativeTemplateStyleKeyMainBackgroundColor =
    @"main_background_color";

GADTNativeTemplateStyleKey _Nonnull const GADTNativeTemplateStyleKeyCornerRadius = @"corner_radius";

static NSString *_Nonnull const GADTBlue = @"#5C84F0";

@implementation GADTTemplateView {
  NSDictionary<GADTNativeTemplateStyleKey, NSObject*>* _defaultStyles;
}

- (instancetype)initWithFrame:(CGRect)frame {
  if (self = [super initWithFrame:frame]) {
    _rootView = [NSBundle.mainBundle loadNibNamed:NSStringFromClass([self class])
                                            owner:self
                                          options:nil]
                    .firstObject;

    [self addSubview:_rootView];

    [self
        addConstraints:[NSLayoutConstraint
                           constraintsWithVisualFormat:@"H:|[_rootView]|"
                                               options:0
                                               metrics:nil
                                                 views:NSDictionaryOfVariableBindings(_rootView)]];
    [self
        addConstraints:[NSLayoutConstraint
                           constraintsWithVisualFormat:@"V:|[_rootView]|"
                                               options:0
                                               metrics:nil
                                                 views:NSDictionaryOfVariableBindings(_rootView)]];
    [self applyStyles];
    [self styleAdBadge];
  }
  return self;
}

- (nonnull NSString *)getTemplateTypeName {
  return @"root";
}

/// Returns the style value for the provided key or the default style if no styles dictionary
/// was set.
- (nullable id)styleForKey:(nonnull GADTNativeTemplateStyleKey)key {
  return _styles[key];
}

// Goes through all recognized style keys and updates the views accordingly, overwriting the
// defaults.
- (void)applyStyles {
  self.layer.borderColor = [GADTTemplateView colorFromHexString:@"E0E0E0"].CGColor;
  self.layer.borderWidth = 1.0f;
  [self.mediaView sizeToFit];
  if ([self styleForKey:GADTNativeTemplateStyleKeyCornerRadius]) {
    float roundedCornerRadius =
        ((NSNumber *)[self styleForKey:GADTNativeTemplateStyleKeyCornerRadius]).floatValue;

    // Rounded corners
    self.iconView.layer.cornerRadius = roundedCornerRadius;
    self.iconView.clipsToBounds = YES;
    ((UIButton*)self.callToActionView).layer.cornerRadius = roundedCornerRadius;
    ((UIButton*)self.callToActionView).clipsToBounds = YES;
  }

  // Fonts
  if ([self styleForKey:GADTNativeTemplateStyleKeyPrimaryFont]) {
    ((UILabel *)self.headlineView).font =
        (UIFont *)[self styleForKey:GADTNativeTemplateStyleKeyPrimaryFont];
  }

  if ([self styleForKey:GADTNativeTemplateStyleKeySecondaryFont]) {
    ((UILabel *)self.bodyView).font =
        (UIFont *)[self styleForKey:GADTNativeTemplateStyleKeySecondaryFont];
  }

  if ([self styleForKey:GADTNativeTemplateStyleKeyTertiaryFont]) {
    ((UILabel *)self.advertiserView).font =
        (UIFont *)[self styleForKey:GADTNativeTemplateStyleKeyTertiaryFont];
  }

  if ([self styleForKey:GADTNativeTemplateStyleKeyCallToActionFont]) {
    ((UIButton *)self.callToActionView).titleLabel.font =
        (UIFont *)[self styleForKey:GADTNativeTemplateStyleKeyCallToActionFont];
  }

  // Font colors
  if ([self styleForKey:GADTNativeTemplateStyleKeyPrimaryFontColor])
    ((UILabel *)self.headlineView).textColor =
        (UIColor *)[self styleForKey:GADTNativeTemplateStyleKeyPrimaryFontColor];

  if ([self styleForKey:GADTNativeTemplateStyleKeySecondaryFontColor]) {
    ((UILabel *)self.bodyView).textColor =
        (UIColor *)[self styleForKey:GADTNativeTemplateStyleKeySecondaryFontColor];
  }

  if ([self styleForKey:GADTNativeTemplateStyleKeyTertiaryFontColor]) {
    ((UILabel *)self.advertiserView).textColor =
        (UIColor *)[self styleForKey:GADTNativeTemplateStyleKeyTertiaryFontColor];
  }

  if ([self styleForKey:GADTNativeTemplateStyleKeyCallToActionFontColor]) {
    [((UIButton *)self.callToActionView)
        setTitleColor:(UIColor *)[self styleForKey:GADTNativeTemplateStyleKeyCallToActionFontColor]
             forState:UIControlStateNormal];
  }

  // Background colors
  if ([self styleForKey:GADTNativeTemplateStyleKeyPrimaryBackgroundColor]) {
    ((UILabel *)self.headlineView).backgroundColor =
        (UIColor *)[self styleForKey:GADTNativeTemplateStyleKeyPrimaryBackgroundColor];
  }

  if ([self styleForKey:GADTNativeTemplateStyleKeySecondaryBackgroundColor]) {
    ((UILabel *)self.bodyView).backgroundColor =
        (UIColor *)[self styleForKey:GADTNativeTemplateStyleKeySecondaryBackgroundColor];
  }

  if ([self styleForKey:GADTNativeTemplateStyleKeyTertiaryBackgroundColor]) {
    ((UILabel *)self.advertiserView).backgroundColor =
        (UIColor *)[self styleForKey:GADTNativeTemplateStyleKeyTertiaryBackgroundColor];
  }

  if ([self styleForKey:GADTNativeTemplateStyleKeyCallToActionBackgroundColor]) {
    ((UIButton *)self.callToActionView).backgroundColor =
        (UIColor *)[self styleForKey:GADTNativeTemplateStyleKeyCallToActionBackgroundColor];
  }

  if ([self styleForKey:GADTNativeTemplateStyleKeyMainBackgroundColor]) {
    self.backgroundColor =
        (UIColor *)[self styleForKey:GADTNativeTemplateStyleKeyMainBackgroundColor];
  }
}
- (void)styleAdBadge {
  UILabel *adBadge = self.adBadge;
  adBadge.layer.borderColor = adBadge.textColor.CGColor;
  adBadge.layer.borderWidth = 1.0;
  adBadge.layer.cornerRadius = 3.0;
}

- (void)setStyles:(nullable NSDictionary<GADTNativeTemplateStyleKey, NSObject *> *)styles {
  _styles = [styles copy];
  [self applyStyles];
}

- (void)setNativeAd:(nullable GADNativeAd *)nativeAd {
  ((UILabel *)self.headlineView).text = nativeAd.headline;

  // Some of the assets are not guaranteed to be present. This is to check
  // that they are before showing or hiding them.
  ((UIImageView *)self.iconView).image = nativeAd.icon.image;
  self.iconView.hidden = nativeAd.icon ? NO : YES;

  [((UIButton *)self.callToActionView) setTitle:nativeAd.callToAction
                                       forState:UIControlStateNormal];

  // Either show the advertiser an app has, or show the store of the ad.
  if (nativeAd.advertiser && !nativeAd.store) {
    // Ad has advertiser but not store
    self.storeView.hidden = YES;
    ((UILabel *)self.advertiserView).text = nativeAd.advertiser;
    self.advertiserView.hidden = NO;
  } else if (nativeAd.store && !nativeAd.advertiser) {
    // Ad has store but not advertiser
    self.advertiserView.hidden = YES;
    ((UILabel *)self.storeView).text = nativeAd.store;
    self.storeView.hidden = NO;
  } else if (nativeAd.advertiser && nativeAd.store) {
    // Ad has both store and advertiser, default to showing advertiser.
    self.storeView.hidden = YES;
    ((UILabel *)self.advertiserView).text = nativeAd.advertiser;
    self.advertiserView.hidden = NO;
  }

  // Either show the number of stars an app has, or show the body of the ad.
  // If there is a starRating then starRatingView is shown and bodyView is hidden
  // otherwise, starRatingView is hidden and bodyView is filled.
  // Use the unicode characters for filled in or empty stars.
  if (nativeAd.starRating.floatValue > 0) {
    NSMutableString* stars = [[NSMutableString alloc] initWithString:@""];
    int count = 0;
    for (; count < nativeAd.starRating.intValue; count++) {
      NSString* filledStar = [NSString stringWithUTF8String:"\u2605"];
      [stars appendString:filledStar];
    }
    for (; count < 5; count++) {
      NSString* emptyStar = [NSString stringWithUTF8String:"\u2606"];
      [stars appendString:emptyStar];
    }
    ((UILabel *)self.starRatingView).text = stars;
    self.bodyView.hidden = YES;
    self.starRatingView.hidden = NO;
  } else {
    self.starRatingView.hidden = YES;
    ((UILabel *)self.bodyView).text = nativeAd.body;
    self.bodyView.hidden = NO;
  }

  [self.mediaView setMediaContent:nativeAd.mediaContent];
  [super setNativeAd:nativeAd];
}

- (void)addHorizontalConstraintsToSuperviewWidth {
  // Add an autolayout constraint to make sure our template view stretches to fill the
  // width of its parent.
  if (self.superview) {
    UIView* child = self;
    [self.superview
        addConstraints:[NSLayoutConstraint
                           constraintsWithVisualFormat:@"H:|[child]|"
                                               options:0
                                               metrics:nil
                                                 views:NSDictionaryOfVariableBindings(child)]];
  }
}

- (void)addVerticalCenterConstraintToSuperview {
  if (self.superview) {
    UIView* child = self;
    [self.superview addConstraint:[NSLayoutConstraint constraintWithItem:self.superview
                                                               attribute:NSLayoutAttributeCenterY
                                                               relatedBy:NSLayoutRelationEqual
                                                                  toItem:child
                                                               attribute:NSLayoutAttributeCenterY
                                                              multiplier:1
                                                                constant:0]];
  }
}

/// Creates an opaque UIColor object from a byte-value color definition.
+ (nullable UIColor*)colorFromHexString:(nullable NSString*)hexString {
  if (!hexString) {
    return nil;
  }
  NSRange range = [hexString rangeOfString:@"^#[0-9a-fA-F]{6}$" options:NSRegularExpressionSearch];
  if (range.location == NSNotFound) {
    return nil;
  }
  unsigned rgbValue = 0;
  NSScanner* scanner = [NSScanner scannerWithString:hexString];
  [scanner setScanLocation:1];  // Bypass '#' character.
  [scanner scanHexInt:&rgbValue];

  return [UIColor colorWithRed:((rgbValue & 0xff0000) >> 16) / 255.0f
                         green:((rgbValue & 0xff00) >> 8) / 255.0f
                          blue:(rgbValue & 0xff) / 255.0f
                         alpha:1];
}
@end
