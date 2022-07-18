// Copyright 2014 Google Inc. All Rights Reserved.

#import "GADUInterstitialAd.h"

#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

#import "GADUObjectCache.h"
#import "GADUPluginUtil.h"
#import "GADURequest.h"
#import "UnityInterface.h"

@implementation GADUInterstitialAd {
  // Keep a reference to the error objects so references to Unity-level
  // ResponseInfo object are not released until the ad object is released.
  NSError *_lastLoadError;
}

- (id)initWithAdClientReference:(GADUTypeAdClientRef *)adClient {
  self = [super init];
  super.adClient = adClient;
  return self;
}

- (void)loadWithAdUnitID:(NSString *)adUnitID request:(GADRequest *)request {
  __weak GADUInterstitialAd *weakSelf = self;

  [GADInterstitialAd
       loadWithAdUnitID:adUnitID
                request:request
      completionHandler:^(GADInterstitialAd *_Nullable interstitialAd, NSError *_Nullable error) {
        GADUInterstitialAd *strongSelf = weakSelf;
        if (error || !interstitialAd) {
          if (strongSelf.adLoadFailedCallback) {
            _lastLoadError = error;
            strongSelf.adLoadFailedCallback(strongSelf.adClient, (__bridge GADUTypeErrorRef)error);
          }
          return;
        }
        strongSelf.interstitialAd = interstitialAd;
        strongSelf.interstitialAd.fullScreenContentDelegate = strongSelf;
        strongSelf.interstitialAd.paidEventHandler = ^void(GADAdValue *_Nonnull adValue) {
          GADUInterstitialAd *strongSecondSelf = weakSelf;
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
  return self.interstitialAd.responseInfo;
}

- (void)show {
  UIViewController *unityController = [GADUPluginUtil unityGLViewController];
  [self.interstitialAd presentFromRootViewController:unityController];
}

@end

#pragma mark - Interopt Methods

static NSString *GADUStringFromUTF8String(const char *bytes) { return bytes ? @(bytes) : nil; }

GADUTypeAdBridgeRef GADUInterstitialAdCreate(GADUTypeAdClientRef *adClientRef) {
  GADUInterstitialAd *adBridge = [[GADUInterstitialAd alloc] initWithAdClientReference:adClientRef];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  cache[adBridge.gadu_referenceKey] = adBridge;
  return (__bridge GADUTypeAdBridgeRef)adBridge;
}

void GADUInterstitialAdLoad(GADUTypeAdBridgeRef adBridgeRef, const char *adUnitID,
                            GADUTypeRequestRef adRequestRef) {
  GADUInterstitialAd *adBridge = (__bridge GADUInterstitialAd *)adBridgeRef;
  GADURequest *adRequest = (__bridge GADURequest *)adRequestRef;
  [adBridge loadWithAdUnitID:GADUStringFromUTF8String(adUnitID) request:[adRequest request]];
}

void GADUInterstitialAdShow(GADUTypeAdBridgeRef adBridgeRef) {
  GADUInterstitialAd *adBridge = (__bridge GADUInterstitialAd *)adBridgeRef;
  [adBridge show];
}
