// Copyright 2020 Google Inc. All Rights Reserved.

#import "GADURewardedInterstitialAd.h"

#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

#import "GADUObjectCache.h"
#import "GADUPluginUtil.h"
#import "GADURequest.h"
#import "UnityInterface.h"

@interface GADURewardedInterstitialAd () <GADFullScreenContentDelegate>
@end

@implementation GADURewardedInterstitialAd {
  // Keep a reference to the error objects so references to Unity-level
  // ResponseInfo object are not released until the ad object is released.
  NSError *_lastLoadError;
}

- (id)initWithAdClientReference:(GADUTypeAdClientRef *)adClient {
  self = [super init];
  super.adClient = adClient;
  return self;
}

- (void)loadWithAdUnitID:(NSString *)adUnit request:(GADRequest *)request {
  __weak GADURewardedInterstitialAd *weakSelf = self;

  [GADRewardedInterstitialAd
       loadWithAdUnitID:adUnit
                request:request
      completionHandler:^(GADRewardedInterstitialAd *_Nullable rewardedInterstitialAd,
                          NSError *_Nullable error) {
        GADURewardedInterstitialAd *strongSelf = weakSelf;
        if (error || !rewardedInterstitialAd) {
          if (strongSelf.adLoadFailedCallback) {
            _lastLoadError = error;
            strongSelf.adLoadFailedCallback(strongSelf.adClient, (__bridge GADUTypeErrorRef)error);
          }
          return;
        }
        strongSelf.rewardedInterstitialAd = rewardedInterstitialAd;
        strongSelf.rewardedInterstitialAd.fullScreenContentDelegate = strongSelf;
        strongSelf.rewardedInterstitialAd.paidEventHandler = ^void(GADAdValue *_Nonnull adValue) {
          GADURewardedInterstitialAd *strongSecondSelf = weakSelf;
          if (strongSecondSelf.adPaidCallback) {
            int64_t valueInMicros =
                [adValue.value decimalNumberByMultiplyingByPowerOf10:6].longLongValue;
            strongSecondSelf.adPaidCallback(
                strongSecondSelf.adClient, (int)adValue.precision, valueInMicros,
                [adValue.currencyCode cStringUsingEncoding:NSUTF8StringEncoding]);
          }
        };
        if (strongSelf.adLoadCallback) {
          strongSelf.adLoadCallback(self.adClient);
        }
      }];
}
- (GADResponseInfo *)responseInfo {
  return self.rewardedInterstitialAd.responseInfo;
}

- (void)show {
  UIViewController *unityController = [GADUPluginUtil unityGLViewController];
  __weak GADURewardedInterstitialAd *weakSelf = self;

  [self.rewardedInterstitialAd
      presentFromRootViewController:unityController
           userDidEarnRewardHandler:^void() {
             GADURewardedInterstitialAd *strongSelf = weakSelf;
             if (strongSelf.adUserEarnedRewardCallback) {
               strongSelf.adUserEarnedRewardCallback(
                   strongSelf.adClient,
                   [strongSelf.rewardedInterstitialAd.adReward.type
                       cStringUsingEncoding:NSUTF8StringEncoding],
                   strongSelf.rewardedInterstitialAd.adReward.amount.doubleValue);
             }
           }];
}

- (void)setServerSideVerificationOptions:(GADServerSideVerificationOptions *)options {
  self.rewardedInterstitialAd.serverSideVerificationOptions = options;
}

@end

#pragma mark - Interopt Methods

static NSString *GADUStringFromUTF8String(const char *bytes) { return bytes ? @(bytes) : nil; }

/// Returns a C string from a C array of UTF8-encoded bytes.
static const char *cStringCopy(const char *string) {
  if (!string) {
    return NULL;
  }
  char *res = (char *)malloc(strlen(string) + 1);
  strcpy(res, string);
  return res;
}

GADUTypeAdBridgeRef GADURewardedInterstitialAdCreate(GADUTypeAdClientRef *adClientRef) {
  GADURewardedInterstitialAd *adBridge =
      [[GADURewardedInterstitialAd alloc] initWithAdClientReference:adClientRef];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  cache[adBridge.gadu_referenceKey] = adBridge;
  return (__bridge GADUTypeAdBridgeRef)adBridge;
}

void GADURewardedInterstitialAdLoad(GADUTypeAdBridgeRef adBridgeRef, const char *adUnitID,
                                    GADUTypeRequestRef adRequestRef) {
  GADURewardedInterstitialAd *adBridge = (__bridge GADURewardedInterstitialAd *)adBridgeRef;
  GADURequest *adRequest = (__bridge GADURequest *)adRequestRef;
  [adBridge loadWithAdUnitID:GADUStringFromUTF8String(adUnitID) request:[adRequest request]];
}

void GADURewardedInterstitialAdShow(GADUTypeAdBridgeRef adBridgeRef) {
  GADURewardedInterstitialAd *adBridge = (__bridge GADURewardedInterstitialAd *)adBridgeRef;
  [adBridge show];
}

void GADURewardedInterstitialAdSetServerSideVerificationOptions(
    GADUTypeAdBridgeRef adBridgeRef, GADUTypeServerSideVerificationOptionsRef optionsRef) {
  GADURewardedInterstitialAd *adBridge = (__bridge GADURewardedInterstitialAd *)adBridgeRef;
  GADServerSideVerificationOptions *adOptions =
      (__bridge GADServerSideVerificationOptions *)optionsRef;
  [adBridge setServerSideVerificationOptions:adOptions];
}

const char *GADURewardedInterstitalAdGetRewardType(GADUTypeAdBridgeRef adBridgeRef) {
  GADURewardedInterstitialAd *adBridge = (__bridge GADURewardedInterstitialAd *)adBridgeRef;
  GADAdReward *reward = adBridge.rewardedInterstitialAd.adReward;
  return cStringCopy(reward.type.UTF8String);
}

double GADURewardedInterstitalAdGetRewardAmount(GADUTypeAdBridgeRef adBridgeRef) {
  GADURewardedInterstitialAd *adBridge = (__bridge GADURewardedInterstitialAd *)adBridgeRef;
  GADAdReward *reward = adBridge.rewardedInterstitialAd.adReward;
  return reward.amount.doubleValue;
}
