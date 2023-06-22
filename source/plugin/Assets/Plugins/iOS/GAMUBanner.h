// Copyright (C) 2023 Google, Inc.
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

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADUBanner.h"
#import "GADUTypes.h"

/// A wrapper around GAMBannerView. Includes the ability to create GAMBannerView objects, load them
/// with ads, and listen for ad events.
@interface GAMUBanner : GADUBanner

/// Initializes a GAMUBanner with specified width and height, positioned at either the top or
/// bottom of the screen.
- (nonnull instancetype)initWithAdManagerBannerClientReference:
                            (_Nonnull GAMUTypeBannerClientRef *_Nonnull)bannerClient
                                                      adUnitID:(nonnull NSString *)adUnitID
                                                         width:(CGFloat)width
                                                        height:(CGFloat)height
                                                    adPosition:(GADAdPosition)adPosition;

/// Initializes a GAMUBanner with specified width and height at the specified point.
- (nonnull instancetype)initWithAdManagerBannerClientReference:
                            (_Nonnull GAMUTypeBannerClientRef *_Nonnull)bannerClient
                                                      adUnitID:(nonnull NSString *)adUnitID
                                                         width:(CGFloat)width
                                                        height:(CGFloat)height
                                              customAdPosition:(CGPoint)customAdPosition;

/// Initializes an adaptive GAMUBanner, positioned at either the top or bottom of the screen.
- (nonnull instancetype)
    initWithAdaptiveBannerSizeAndAdManagerBannerClientReference:
        (_Nonnull GAMUTypeBannerClientRef *_Nonnull)bannerClient
                                                       adUnitID:(nonnull NSString *)adUnitID
                                                          width:(NSInteger)width
                                                    orientation:(GADUBannerOrientation)orientation
                                                     adPosition:(GADAdPosition)adPosition;

/// Initializes an adaptive GAMUBanner with a custom position at given point from top left.
- (nonnull instancetype)
    initWithAdaptiveBannerSizeAndAdManagerBannerClientReference:
        (_Nonnull GAMUTypeBannerClientRef *_Nonnull)bannerClient
                                                       adUnitID:(nonnull NSString *)adUnitID
                                                          width:(NSInteger)width
                                                    orientation:(GADUBannerOrientation)orientation
                                               customAdPosition:(CGPoint)customAdPosition;

/// A reference to the Unity Ad Manager banner client.
@property(nonatomic, assign) _Nonnull GAMUTypeBannerClientRef *_Nonnull bannerClientGAM;

/// A GAMBannerView which contains the ad.
@property(nonatomic, strong, nullable) GAMBannerView *bannerViewGAM;

/// Optional array of NSValue encoded GADAdSize structs, specifying all valid sizes that are
/// appropriate for this slot.
@property(nonatomic, copy, nullable) NSArray<NSValue *> *validAdSizes;

/// The app event callback into Unity.
@property(nonatomic, assign, nullable) GAMUAdViewAppEventCallback appEventCallback;

@end
