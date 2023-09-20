// Copyright 2023 Google LLC. All Rights Reserved.

#import "GADUNativeAdOptions.h"

@implementation GADUNativeAdOptions {
  /// Holds the location of the ad choices icon. Default is top right corner.
  GADAdChoicesPosition _adChoicesPlacement;
  /// Holds the preference of the aspect ratio for the media in the native ad.
  GADMediaAspectRatio _mediaAspectRatio;
  /// Indicates how native video assets should be displayed.
  GADVideoOptions *_videoOptions;
}

- (nonnull instancetype)initWithAdChoicesPlacement:(GADAdChoicesPosition)adChoicesPlacement
                                  mediaAspectRatio:(GADMediaAspectRatio)mediaAspectRatio
                                      videoOptions:(nullable GADVideoOptions *)videoOptions {
  self = [super init];
  if (self) {
    _adChoicesPlacement = adChoicesPlacement;
    _mediaAspectRatio = mediaAspectRatio;
    _videoOptions = videoOptions;
  }
  return self;
}

- (nonnull NSArray<GADAdLoaderOptions *> *)asGADAdLoaderOptions {
  NSMutableArray<GADAdLoaderOptions *> *options = [NSMutableArray array];

  GADNativeAdViewAdOptions *adViewOptions = [[GADNativeAdViewAdOptions alloc] init];
  adViewOptions.preferredAdChoicesPosition = _adChoicesPlacement;
  [options addObject:adViewOptions];

  GADNativeAdMediaAdLoaderOptions *mediaOptions = [[GADNativeAdMediaAdLoaderOptions alloc] init];
  mediaOptions.mediaAspectRatio = _mediaAspectRatio;
  [options addObject:mediaOptions];

  [options addObject:_videoOptions];
  return options;
}

@end
