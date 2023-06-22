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

#import "GAMUBanner.h"

#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

#import "GADUPluginUtil.h"

@interface GAMUBanner () <GADBannerViewDelegate, GADAppEventDelegate>

/// Defines where the ad should be positioned on the screen with a GADAdPosition.
@property(nonatomic, assign) GADAdPosition adPosition;

/// Defines where the ad should be positioned on the screen with a CGPoint.
@property(nonatomic, assign) CGPoint customAdPosition;

@end

@implementation GAMUBanner {
  // Keep a reference to the error objects so references to Unity-level
  // ResponseInfo object are not released until the ad object is released.
  NSError *_lastLoadError;
}

- (nonnull instancetype)initWithAdManagerBannerClientReference:
                            (_Nonnull GAMUTypeBannerClientRef *_Nonnull)bannerClient
                                                      adUnitID:(nonnull NSString *)adUnitID
                                                         width:(CGFloat)width
                                                        height:(CGFloat)height
                                                    adPosition:(GADAdPosition)adPosition {
  return [self initWithAdManagerBannerClientReference:bannerClient
                                             adUnitID:adUnitID
                                               adSize:[GADUPluginUtil adSizeForWidth:width
                                                                              height:height]
                                           adPosition:adPosition];
}

- (nonnull instancetype)initWithAdManagerBannerClientReference:
                            (_Nonnull GAMUTypeBannerClientRef *_Nonnull)bannerClient
                                                      adUnitID:(nonnull NSString *)adUnitID
                                                         width:(CGFloat)width
                                                        height:(CGFloat)height
                                              customAdPosition:(CGPoint)customAdPosition {
  return [self initWithAdManagerBannerClientReference:bannerClient
                                             adUnitID:adUnitID
                                               adSize:[GADUPluginUtil adSizeForWidth:width
                                                                              height:height]
                                     customAdPosition:customAdPosition];
}

- (nonnull instancetype)
    initWithAdaptiveBannerSizeAndAdManagerBannerClientReference:
        (_Nonnull GAMUTypeBannerClientRef *_Nonnull)bannerClient
                                                       adUnitID:(nonnull NSString *)adUnitID
                                                          width:(NSInteger)width
                                                    orientation:(GADUBannerOrientation)orientation
                                                     adPosition:(GADAdPosition)adPosition {
  return [self
      initWithAdManagerBannerClientReference:bannerClient
                                    adUnitID:adUnitID
                                      adSize:[GADUPluginUtil adaptiveAdSizeForWidth:(CGFloat)width
                                                                        orientation:orientation]
                                  adPosition:adPosition];
}

- (nonnull instancetype)
    initWithAdaptiveBannerSizeAndAdManagerBannerClientReference:
        (_Nonnull GAMUTypeBannerClientRef *_Nonnull)bannerClient
                                                       adUnitID:(nonnull NSString *)adUnitID
                                                          width:(NSInteger)width
                                                    orientation:(GADUBannerOrientation)orientation
                                               customAdPosition:(CGPoint)customAdPosition {
  return [self
      initWithAdManagerBannerClientReference:bannerClient
                                    adUnitID:adUnitID
                                      adSize:[GADUPluginUtil adaptiveAdSizeForWidth:(CGFloat)width
                                                                        orientation:orientation]
                            customAdPosition:customAdPosition];
}

- (nonnull instancetype)initWithAdManagerBannerClientReference:
                            (_Nonnull GAMUTypeBannerClientRef *_Nonnull)bannerClient
                                                      adUnitID:(nonnull NSString *)adUnitID
                                                        adSize:(GADAdSize)size
                                                    adPosition:(GADAdPosition)adPosition {
  self = [super initWithBannerClientReference:bannerClient
                                     adUnitID:adUnitID
                                       adSize:size
                                   adPosition:adPosition];
  if (self) {
    _bannerClientGAM = bannerClient;
    _bannerViewGAM =
        [[GAMBannerView alloc] initWithAdSize:[GADUPluginUtil safeAdSizeForAdSize:size]];
    self.bannerView = _bannerViewGAM;
    _bannerViewGAM.adUnitID = adUnitID;
    _bannerViewGAM.delegate = self;
    _bannerViewGAM.appEventDelegate = self;
    _bannerViewGAM.rootViewController = [GADUPluginUtil unityGLViewController];
  }
  return self;
}

- (nonnull instancetype)initWithAdManagerBannerClientReference:
                            (_Nonnull GAMUTypeBannerClientRef *_Nonnull)bannerClient
                                                      adUnitID:(nonnull NSString *)adUnitID
                                                        adSize:(GADAdSize)size
                                              customAdPosition:(CGPoint)customAdPosition {
  self = [super initWithBannerClientReference:bannerClient
                                     adUnitID:adUnitID
                                       adSize:size
                             customAdPosition:customAdPosition];
  if (self) {
    _bannerClientGAM = bannerClient;
    _bannerViewGAM =
        [[GAMBannerView alloc] initWithAdSize:[GADUPluginUtil safeAdSizeForAdSize:size]];
    self.bannerView = _bannerViewGAM;
    _bannerViewGAM.adUnitID = adUnitID;
    _bannerViewGAM.delegate = self;
    _bannerViewGAM.appEventDelegate = self;
    _bannerViewGAM.rootViewController = [GADUPluginUtil unityGLViewController];
  }
  return self;
}

- (nullable NSArray<NSValue *> *)validAdSizes {
  return self.bannerViewGAM.validAdSizes;
}

- (void)setValidAdSizes:(nullable NSArray<NSValue *> *)validAdSizes {
  self.bannerViewGAM.validAdSizes = validAdSizes;
}

#pragma mark GADAppEventDelegate Implementation

/// Called when the banner receives an app event.
- (void)adView:(nonnull GADBannerView *)banner
    didReceiveAppEvent:(nonnull NSString *)name
              withInfo:(nullable NSString *)info {
  if (self.appEventCallback) {
    self.appEventCallback(self.bannerClientGAM, [name cStringUsingEncoding:NSUTF8StringEncoding],
                          [info cStringUsingEncoding:NSUTF8StringEncoding]);
  }
}

@end
