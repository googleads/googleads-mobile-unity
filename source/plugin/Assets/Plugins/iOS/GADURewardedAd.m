// Copyright 2018 Google Inc. All Rights Reserved.

#import "GADURewardedAd.h"

#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

#import "GADUObjectCache.h"
#import "GADUPluginUtil.h"
#import "GADURequest.h"
#import "UnityInterface.h"

@implementation GADURewardedAd {
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
  __weak GADURewardedAd *weakSelf = self;

  [GADRewardedAd
       loadWithAdUnitID:adUnitID
                request:request
      completionHandler:^(GADRewardedAd *_Nullable rewardedAd, NSError *_Nullable error) {
        GADURewardedAd *strongSelf = weakSelf;
        if (error || !rewardedAd) {
          if (strongSelf.adLoadFailedCallback) {
            _lastLoadError = error;
            strongSelf.adLoadFailedCallback(strongSelf.adClient, (__bridge GADUTypeErrorRef)error);
          }
          return;
        }
        strongSelf.rewardedAd = rewardedAd;
        //rewardedAd.fullScreenContentDelegate = strongSelf;
        rewardedAd.paidEventHandler = ^void(GADAdValue *_Nonnull adValue) {
          GADURewardedAd *strongSecondSelf = weakSelf;
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
  return self.rewardedAd.responseInfo;
}

- (void)show {
  UIViewController *unityController = [GADUPluginUtil unityGLViewController];
  __weak GADURewardedAd *weakSelf = self;

  [self.rewardedAd
      presentFromRootViewController:unityController
           userDidEarnRewardHandler:^void() {
             GADURewardedAd *strongSelf = weakSelf;
             if (strongSelf.adUserEarnedRewardCallback) {
               strongSelf.adUserEarnedRewardCallback(
                   strongSelf.adClient,
                   [strongSelf.rewardedAd.adReward.type cStringUsingEncoding:NSUTF8StringEncoding],
                   strongSelf.rewardedAd.adReward.amount.doubleValue);
             }
           }];
}

- (void)setServerSideVerificationOptions:(GADServerSideVerificationOptions *)options {
  self.rewardedAd.serverSideVerificationOptions = options;
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

GADUTypeAdBridgeRef GADURewardedAdCreate(GADUTypeAdClientRef *adClientRef) {
  GADURewardedAd *adBridge = [[GADURewardedAd alloc] initWithAdClientReference:adClientRef];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  cache[adBridge.gadu_referenceKey] = adBridge;
  return (__bridge GADUTypeAdBridgeRef)adBridge;
}

void GADURewardedAdLoad(GADUTypeAdBridgeRef adBridgeRef, const char *adUnitID,
                        GADUTypeRequestRef adRequestRef) {
  GADURewardedAd *adBridge = (__bridge GADURewardedAd *)adBridgeRef;
  GADURequest *adRequest = (__bridge GADURequest *)adRequestRef;
  [adBridge loadWithAdUnitID:GADUStringFromUTF8String(adUnitID) request:[adRequest request]];
}

void GADURewardedAdShow(GADUTypeAdBridgeRef adBridgeRef) {
  GADURewardedAd *adBridge = (__bridge GADURewardedAd *)adBridgeRef;
  [adBridge show];
}

void GADURewardedAdSetServerSideVerificationOptions(
    GADUTypeAdBridgeRef adBridgeRef, GADUTypeServerSideVerificationOptionsRef optionsRef) {
  GADURewardedAd *adBridge = (__bridge GADURewardedAd *)adBridgeRef;
  GADServerSideVerificationOptions *adOptions =
      (__bridge GADServerSideVerificationOptions *)optionsRef;
  [adBridge setServerSideVerificationOptions:adOptions];
}

const char *GADURewardedAdGetRewardType(GADUTypeAdBridgeRef adBridgeRef) {
  GADURewardedAd *adBridge = (__bridge GADURewardedAd *)adBridgeRef;
  GADAdReward *reward = adBridge.rewardedAd.adReward;
  return cStringCopy(reward.type.UTF8String);
}

double GADURewardedAdGetRewardAmount(GADUTypeAdBridgeRef adBridgeRef) {
  GADURewardedAd *adBridge = (__bridge GADURewardedAd *)adBridgeRef;
  GADAdReward *reward = adBridge.rewardedAd.adReward;
  return reward.amount.doubleValue;
}
