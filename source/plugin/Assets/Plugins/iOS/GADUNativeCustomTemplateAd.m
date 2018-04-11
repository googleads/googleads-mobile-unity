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
  if (customClickAction) {
    __weak GADUNativeCustomTemplateAd *weakSelf = self;
    [self.nativeCustomTemplateAd setCustomClickHandler:^(NSString *assetID) {
      [weakSelf didReceiveClickForAsset:key];
    }];
  }
  [self.nativeCustomTemplateAd performClickOnAssetWithKey:key];
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
