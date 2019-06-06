// Copyright 2018 Google Inc. All Rights Reserved.

#import "AdColonyExtrasBuilder.h"

#import <AdColonyAdapter/GADMAdapterAdColonyExtras.h>

@implementation AdColonyExtrasBuilder

NSString *const ShowPrePopupKey = @"show_pre_popup";

NSString *const ShowPostPopupKey = @"show_post_popup";

- (id<GADAdNetworkExtras>)adNetworkExtrasWithDictionary:
    (NSDictionary<NSString *, NSString *> *)extras {

  GADMAdapterAdColonyExtras *adColonyExtras = [[GADMAdapterAdColonyExtras alloc] init];

  NSString *showPrePopup = extras[ShowPrePopupKey];
  if (showPrePopup) {
    adColonyExtras.showPrePopup = showPrePopup.boolValue;
  }

  NSString *showPostPopup = extras[ShowPostPopupKey];
  if (showPostPopup) {
    adColonyExtras.showPostPopup = showPostPopup.boolValue;
  }

  return adColonyExtras;
}

@end
