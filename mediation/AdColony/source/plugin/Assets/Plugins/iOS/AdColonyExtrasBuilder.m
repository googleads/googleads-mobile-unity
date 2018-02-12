// Copyright 2018 Google Inc. All Rights Reserved.

#import "AdColonyExtrasBuilder.h"

#import <AdColonyAdapter/GADMAdapterAdColonyExtras.h>

@implementation AdColonyExtrasBuilder

NSString *const UserIdKey = @"user_id";

NSString *const ShowPrePopupKey = @"show_pre_popup";

NSString *const ShowPostPopupKey = @"show_post_popup";

NSString *const TestModeKey = @"test_mode";

- (id<GADAdNetworkExtras>)adNetworkExtrasWithDictionary:
    (NSDictionary<NSString *, NSString *> *)extras {

  GADMAdapterAdColonyExtras *adColonyExtras = [[GADMAdapterAdColonyExtras alloc] init];

  NSString *userId = extras[UserIdKey];
  if (userId) {
    adColonyExtras.userId = userId;
  }

  NSString *showPrePopup = extras[ShowPrePopupKey];
  if (showPrePopup) {
    adColonyExtras.showPrePopup = showPrePopup.boolValue;
  }

  NSString *showPostPopup = extras[ShowPostPopupKey];
  if (showPostPopup) {
    adColonyExtras.showPostPopup = showPostPopup.boolValue;
  }

  NSString *testMode = extras[TestModeKey];
  if (testMode) {
    adColonyExtras.testMode = testMode.boolValue;
  }

  return adColonyExtras;
}

@end
