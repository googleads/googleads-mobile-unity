// Copyright 2014 Google Inc. All Rights Reserved.

#import "GADUNativeCustomTemplateAd.h"

@implementation GADUNativeCustomTemplateAd

- (instancetype)initWithAd:(GADNativeCustomTemplateAd *)nativeCustomTemplateAd {
  self = [super init];
  if (self) {
    _nativeCustomTemplateAd = nativeCustomTemplateAd;
  }
  return self;
}

- (NSString *)templateID {
  return [self.nativeCustomTemplateAd templateID];
}

- (NSString *)stringForKey:(NSString *)key {
  return [self.nativeCustomTemplateAd stringForKey:key];
}

- (UIImage *)imageForKey:(NSString *)key {
  return [self.nativeCustomTemplateAd imageForKey:key].image;
}

- (void)performClickOnAssetWithKey:(NSString *)key withCustomClickAction:(bool)customClickAction {
  dispatch_block_t clickHandler = nil;
  if (customClickAction) {
    clickHandler = ^{
      [self didReceiveClickForAsset:key];
    };
  }
  [self.nativeCustomTemplateAd performClickOnAssetWithKey:key customClickHandler:clickHandler];
}

- (void)didReceiveClickForAsset:(NSString *)key {
  if (self.didReceiveClickCallback) {
    self.didReceiveClickCallback(self.nativeCustomTemplateClient,
                                 [key cStringUsingEncoding:NSUTF8StringEncoding]);
  }
}

- (void)recordImpression {
  [self.nativeCustomTemplateAd recordImpression];
}

- (NSArray *)availableAssetKeys {
  return [self.nativeCustomTemplateAd availableAssetKeys];
}

@end
