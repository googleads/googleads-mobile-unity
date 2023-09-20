// Copyright 2023 Google LLC. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

/// Contains Media Options that can be applied to Native Ad Requests.
@interface GADUNativeAdOptions : NSObject

/// Returns an initialized native ad options object for the given configuration.
- (nonnull instancetype)initWithAdChoicesPlacement:(GADAdChoicesPosition)adChoicesPlacement
                                  mediaAspectRatio:(GADMediaAspectRatio)mediaAspectRatio
                                      videoOptions:(nullable GADVideoOptions *)videoOptions;

/// Returns array of GADAdLoaderOptions from the initialized GADUNativeAdOptions object.
- (nonnull NSArray<GADAdLoaderOptions *> *)asGADAdLoaderOptions;

@end
