// Copyright 2021 Google LLC. All Rights Reserved.

#import "GADUAppOpenAd.h"

#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

#import "GADUObjectCache.h"
#import "GADUPluginUtil.h"
#import "GADURequest.h"
#import "GADUTypes.h"
#import "UnityInterface.h"

@interface GADUAppOpenAd () <GADFullScreenContentDelegate>
@end

@implementation GADUAppOpenAd {
  // Keep a reference to the error objects so references to Unity-level
  // ResponseInfo object are not released until the ad object is released.
  NSError *_lastLoadError;
}

- (id)initWithAdClientReference:(GADUTypeAdClientRef *)adClient {
  self = [super init];
  super.adClient = adClient;
  return self;
}

- (void)loadWithAdUnitID:(NSString *)adUnit
             orientation:(GADUScreenOrientation)orientation
                 request:(GADRequest *)request {
  __weak GADUAppOpenAd *weakSelf = self;

  UIInterfaceOrientation uiOrientation =
      GADUUIInterfaceOrientationForGADUScreenOrientation(orientation);

  [GADAppOpenAd
       loadWithAdUnitID:adUnit
                request:request
            orientation:uiOrientation
      completionHandler:^(GADAppOpenAd *_Nullable appOpenAd, NSError *_Nullable error) {
        GADUAppOpenAd *strongSelf = weakSelf;
        if (error) {
          if (strongSelf.adLoadFailedCallback) {
            _lastLoadError = error;
            strongSelf.adLoadFailedCallback(strongSelf.adClient, (__bridge GADUTypeErrorRef)error);
          }
          return;
        }
        strongSelf.appOpenAd = appOpenAd;
        strongSelf.appOpenAd.fullScreenContentDelegate = strongSelf;
        strongSelf.appOpenAd.paidEventHandler = ^void(GADAdValue *_Nonnull adValue) {
          GADUAppOpenAd *strongSecondSelf = weakSelf;
          if (strongSecondSelf.adPaidCallback) {
            int64_t valueInMicros =
                [adValue.value decimalNumberByMultiplyingByPowerOf10:6].longLongValue;
            strongSecondSelf.adPaidCallback(
                strongSecondSelf.adClient, (int)adValue.precision, valueInMicros,
                [adValue.currencyCode cStringUsingEncoding:NSUTF8StringEncoding]);
          }
        };
        if (strongSelf.adLoadCallback) {
          strongSelf.adLoadCallback(strongSelf.adClient);
        }
      }];
}

- (GADResponseInfo *)responseInfo {
  return self.appOpenAd.responseInfo;
}

- (void)show {
  UIViewController *unityController = [GADUPluginUtil unityGLViewController];
  [self.appOpenAd presentFromRootViewController:unityController];
}

@end

#pragma mark Unity Native Interop Methods

static NSString *GADUStringFromUTF8String(const char *bytes) { return bytes ? @(bytes) : nil; }

GADUTypeAdBridgeRef GADUAppOpenAdCreate(GADUTypeAdClientRef *adClientRef) {
  GADUAppOpenAd *adBridge = [[GADUAppOpenAd alloc] initWithAdClientReference:adClientRef];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  cache[adBridge.gadu_referenceKey] = adBridge;
  return (__bridge GADUTypeAdBridgeRef)adBridge;
}

void GADUAppOpenAdLoad(GADUTypeAdBridgeRef adBridgeRef, const char *adUnitID, int orientation,
                       GADUTypeRequestRef requestRef) {
  GADUAppOpenAd *adBridge = (__bridge GADUAppOpenAd *)adBridgeRef;
  GADURequest *adRequest = (__bridge GADURequest *)requestRef;

  [adBridge loadWithAdUnitID:GADUStringFromUTF8String(adUnitID)
                 orientation:(GADUScreenOrientation)orientation
                     request:[adRequest request]];
}

void GADUAppOpenAdShow(GADUTypeAdBridgeRef adBridgeRef) {
  GADUAppOpenAd *adBridge = (__bridge GADUAppOpenAd *)adBridgeRef;
  [adBridge show];
}
