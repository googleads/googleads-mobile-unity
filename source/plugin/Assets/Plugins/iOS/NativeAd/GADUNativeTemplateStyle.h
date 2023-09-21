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
#import <Foundation/Foundation.h>
#import <GoogleMobileAds/GADNativeAd.h>

#import "GADUNativeTemplateTextStyle.h"
#import "NativeTemplates/GADTTemplateView.h"

/// A wrapper around the GADTTemplateView that adds view constraints.
@interface GADUNativeTemplateViewWrapper : UIView

/// The GADTTemplateView that is being wrapped.
@property(nonatomic, nullable) GADTTemplateView *templateView;

@end

@interface GADUNativeTemplateStyle : NSObject

/// Returns the template name of the Native Template being used.
@property(readonly, nonnull) NSString *templateName;

/// The background color for the bulk of the ad.
@property(nonatomic, nullable) UIColor *mainBackgroundColor;

/// The native template text style for call to action
@property(nonatomic, nullable) GADUNativeTemplateTextStyle *callToActionStyle;

/// The native template text style for the first row of text in the template.
@property(nonatomic, nullable) GADUNativeTemplateTextStyle *primaryTextStyle;

/// The native template text style for the second row of text in the template.
@property(nonatomic, nullable) GADUNativeTemplateTextStyle *secondaryTextStyle;

/// The native template text style for the third row of text in the template.
@property(nonatomic, nullable) GADUNativeTemplateTextStyle *tertiaryTextStyle;

/// Returns an instance containing the template name to be used.
- (nonnull instancetype)initWithTemplateName:(nonnull NSString *)templateName;

/// The actual view to be displayed.
- (nullable GADUNativeTemplateViewWrapper *)getDisplayedView:(nullable GADNativeAd *)gadNativeAd;

@end
