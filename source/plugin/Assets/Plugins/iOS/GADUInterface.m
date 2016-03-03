// Copyright 2014 Google Inc. All Rights Reserved.

#import "GADUBanner.h"
#import "GADUInterstitial.h"
#import "GADUObjectCache.h"
#import "GADURequest.h"
#import "GADURewardBasedVideoAd.h"
#import "GADUTypes.h"

/// Returns an NSString copying the characters from |bytes|, a C array of UTF8-encoded bytes.
/// Returns nil if |bytes| is NULL.
static NSString *GADUStringFromUTF8String(const char *bytes) {
  if (bytes) {
    return @(bytes);
  } else {
    return nil;
  }
}

/// Creates a GADBannerView with the specified width, height, and position. Returns a reference to
/// the GADUBannerView.
GADUTypeBannerRef GADUCreateBannerView(GADUTypeBannerClientRef *bannerClient, const char *adUnitID,
                                       NSInteger width, NSInteger height,
                                       GADAdPosition adPosition) {
  GADUBanner *banner =
      [[GADUBanner alloc] initWithBannerClientReference:bannerClient
                                               adUnitID:GADUStringFromUTF8String(adUnitID)
                                                  width:width
                                                 height:height
                                             adPosition:adPosition];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  [cache.references setObject:banner forKey:[banner gadu_referenceKey]];
  return (__bridge GADUTypeBannerRef)banner;
}

/// Creates a full-width GADBannerView in the current orientation. Returns a reference to the
/// GADUBannerView.
GADUTypeBannerRef GADUCreateSmartBannerView(GADUTypeBannerClientRef *bannerClient,
                                            const char *adUnitID, GADAdPosition adPosition) {
  GADUBanner *banner = [[GADUBanner alloc]
      initWithSmartBannerSizeAndBannerClientReference:bannerClient
                                             adUnitID:GADUStringFromUTF8String(adUnitID)
                                           adPosition:adPosition];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  [cache.references setObject:banner forKey:[banner gadu_referenceKey]];
  return (__bridge GADUTypeBannerRef)banner;
}

/// Creates a GADUInterstitial and returns its reference.
GADUTypeBannerRef GADUCreateInterstitial(GADUTypeInterstitialClientRef *interstitialClient,
                                         const char *adUnitID) {
  GADUInterstitial *interstitial = [[GADUInterstitial alloc]
      initWithInterstitialClientReference:interstitialClient
                                 adUnitID:GADUStringFromUTF8String(adUnitID)];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  [cache.references setObject:interstitial forKey:[interstitial gadu_referenceKey]];
  return (__bridge GADUTypeBannerRef)interstitial;
}

/// Creates a GADURewardBasedVideo and returns its reference.
GADUTypeRewardBasedVideoAdRef GADUCreateRewardBasedVideoAd(
    GADUTypeRewardBasedVideoAdClientRef *rewardBasedVideoAdClient) {
  GADURewardBasedVideoAd *rewardBasedVideoAd = [[GADURewardBasedVideoAd alloc]
      initWithRewardBasedVideoClientReference:rewardBasedVideoAdClient];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  [cache.references setObject:rewardBasedVideoAd forKey:[rewardBasedVideoAd gadu_referenceKey]];
  return (__bridge GADUTypeRewardBasedVideoAdRef)rewardBasedVideoAd;
}

/// Sets the banner callback methods to be invoked during banner ad events.
void GADUSetBannerCallbacks(GADUTypeBannerRef banner,
                            GADUAdViewDidReceiveAdCallback adReceivedCallback,
                            GADUAdViewDidFailToReceiveAdWithErrorCallback adFailedCallback,
                            GADUAdViewWillPresentScreenCallback willPresentCallback,
                            GADUAdViewDidDismissScreenCallback didDismissCallback,
                            GADUAdViewWillLeaveApplicationCallback willLeaveCallback) {
  GADUBanner *internalBanner = (__bridge GADUBanner *)banner;
  internalBanner.adReceivedCallback = adReceivedCallback;
  internalBanner.adFailedCallback = adFailedCallback;
  internalBanner.willPresentCallback = willPresentCallback;
  internalBanner.didDismissCallback = didDismissCallback;
  internalBanner.willLeaveCallback = willLeaveCallback;
}

/// Sets the interstitial callback methods to be invoked during interstitial ad events.
void GADUSetInterstitialCallbacks(
    GADUTypeInterstitialRef interstitial, GADUInterstitialDidReceiveAdCallback adReceivedCallback,
    GADUInterstitialDidFailToReceiveAdWithErrorCallback adFailedCallback,
    GADUInterstitialWillPresentScreenCallback willPresentCallback,
    GADUInterstitialDidDismissScreenCallback didDismissCallback,
    GADUInterstitialWillLeaveApplicationCallback willLeaveCallback) {
  GADUInterstitial *internalInterstitial = (__bridge GADUInterstitial *)interstitial;
  internalInterstitial.adReceivedCallback = adReceivedCallback;
  internalInterstitial.adFailedCallback = adFailedCallback;
  internalInterstitial.willPresentCallback = willPresentCallback;
  internalInterstitial.didDismissCallback = didDismissCallback;
  internalInterstitial.willLeaveCallback = willLeaveCallback;
}

/// Sets the reward based video callback methods to be invoked during reward based video ad events.
void GADUSetRewardBasedVideoAdCallbacks(
    GADUTypeRewardBasedVideoAdRef rewardBasedVideoAd,
    GADURewardBasedVideoAdDidReceiveAdCallback adReceivedCallback,
    GADURewardBasedVideoAdDidFailToReceiveAdWithErrorCallback adFailedCallback,
    GADURewardBasedVideoAdDidOpenCallback didOpenCallback,
    GADURewardBasedVideoAdDidStartPlayingCallback didStartCallback,
    GADURewardBasedVideoAdDidCloseCallback didCloseCallback,
    GADURewardBasedVideoAdDidRewardCallback didRewardCallback,
    GADURewardBasedVideoAdWillLeaveApplicationCallback willLeaveCallback) {
  GADURewardBasedVideoAd *internalRewardBasedVideoAd =
      (__bridge GADURewardBasedVideoAd *)rewardBasedVideoAd;
  internalRewardBasedVideoAd.adReceivedCallback = adReceivedCallback;
  internalRewardBasedVideoAd.adFailedCallback = adFailedCallback;
  internalRewardBasedVideoAd.didOpenCallback = didOpenCallback;
  internalRewardBasedVideoAd.didStartPlayingCallback = didStartCallback;
  internalRewardBasedVideoAd.didCloseCallback = didCloseCallback;
  internalRewardBasedVideoAd.didRewardCallback = didRewardCallback;
  internalRewardBasedVideoAd.willLeaveCallback = willLeaveCallback;
}

/// Sets the GADBannerView's hidden property to YES.
void GADUHideBannerView(GADUTypeBannerRef banner) {
  GADUBanner *internalBanner = (__bridge GADUBanner *)banner;
  [internalBanner hideBannerView];
}

/// Sets the GADBannerView's hidden property to NO.
void GADUShowBannerView(GADUTypeBannerRef banner) {
  GADUBanner *internalBanner = (__bridge GADUBanner *)banner;
  [internalBanner showBannerView];
}

/// Removes the GADURemoveBannerView from the view hierarchy.
void GADURemoveBannerView(GADUTypeBannerRef banner) {
  GADUBanner *internalBanner = (__bridge GADUBanner *)banner;
  [internalBanner removeBannerView];
}

/// Returns YES if the GADInterstitial is ready to be shown.
BOOL GADUInterstitialReady(GADUTypeInterstitialRef interstitial) {
  GADUInterstitial *internalInterstitial = (__bridge GADUInterstitial *)interstitial;
  return [internalInterstitial isReady];
}

/// Shows the GADInterstitial.
void GADUShowInterstitial(GADUTypeInterstitialRef interstitial) {
  GADUInterstitial *internalInterstitial = (__bridge GADUInterstitial *)interstitial;
  [internalInterstitial show];
}

/// Returns YES if the GADRewardBasedVideo is ready to be shown.
BOOL GADURewardBasedVideoAdReady(GADUTypeRewardBasedVideoAdRef rewardBasedVideo) {
  GADURewardBasedVideoAd *internalRewardBasedVideoAd =
      (__bridge GADURewardBasedVideoAd *)rewardBasedVideo;
  return [internalRewardBasedVideoAd isReady];
}

/// Shows the GADRewardBasedVideo.
void GADUShowRewardBasedVideoAd(GADUTypeRewardBasedVideoAdRef rewardBasedVideoAd) {
  GADURewardBasedVideoAd *internalRewardBasedVideoAd =
      (__bridge GADURewardBasedVideoAd *)rewardBasedVideoAd;
  [internalRewardBasedVideoAd show];
}

/// Creates an empty GADRequest and returns its reference.
GADUTypeRequestRef GADUCreateRequest() {
  GADURequest *request = [[GADURequest alloc] init];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  [cache.references setObject:request forKey:[request gadu_referenceKey]];
  return (__bridge GADUTypeRequestRef)(request);
}

/// Adds a test device to the GADRequest.
void GADUAddTestDevice(GADUTypeRequestRef request, const char *deviceID) {
  GADURequest *internalRequest = (__bridge GADURequest *)request;
  [internalRequest addTestDevice:GADUStringFromUTF8String(deviceID)];
}

/// Adds a keyword to the GADRequest.
void GADUAddKeyword(GADUTypeRequestRef request, const char *keyword) {
  GADURequest *internalRequest = (__bridge GADURequest *)request;
  [internalRequest addKeyword:GADUStringFromUTF8String(keyword)];
}

/// Sets the request agent for the GADRequest.
void GADUSetRequestAgent(GADUTypeRequestRef request, const char *requestAgent) {
  GADURequest *internalRequest = (__bridge GADURequest *)request;
  [internalRequest setRequestAgent:GADUStringFromUTF8String(requestAgent)];
}

/// Sets the user's birthday on the GADRequest.
void GADUSetBirthday(GADUTypeRequestRef request, NSInteger year, NSInteger month, NSInteger day) {
  GADURequest *internalRequest = (__bridge GADURequest *)request;
  [internalRequest setBirthdayWithMonth:month day:day year:year];
}

/// Sets the user's gender on the GADRequest.
void GADUSetGender(GADUTypeRequestRef request, NSInteger genderCode) {
  GADURequest *internalRequest = (__bridge GADURequest *)request;
  [internalRequest setGenderWithCode:genderCode];
}

/// Tags a GADRequest to specify whether it should be treated as child-directed for purposes of the
/// Children’s Online Privacy Protection Act (COPPA) -
/// http://business.ftc.gov/privacy-and-security/childrens-privacy.
void GADUTagForChildDirectedTreatment(GADUTypeRequestRef request, BOOL childDirectedTreatment) {
  GADURequest *internalRequest = (__bridge GADURequest *)request;
  internalRequest.tagForChildDirectedTreatment = childDirectedTreatment;
}

/// Sets an extra parameter to be included in the ad request.
void GADUSetExtra(GADUTypeRequestRef request, const char *key, const char *value) {
  GADURequest *internalRequest = (__bridge GADURequest *)request;
  [internalRequest setExtraWithKey:GADUStringFromUTF8String(key)
                             value:GADUStringFromUTF8String(value)];
}

/// Makes a banner ad request.
void GADURequestBannerAd(GADUTypeBannerRef banner, GADUTypeRequestRef request) {
  GADUBanner *internalBanner = (__bridge GADUBanner *)banner;
  GADURequest *internalRequest = (__bridge GADURequest *)request;
  [internalBanner loadRequest:[internalRequest request]];
}

/// Makes an interstitial ad request.
void GADURequestInterstitial(GADUTypeInterstitialRef interstitial, GADUTypeRequestRef request) {
  GADUInterstitial *internalInterstitial = (__bridge GADUInterstitial *)interstitial;
  GADURequest *internalRequest = (__bridge GADURequest *)request;
  [internalInterstitial loadRequest:[internalRequest request]];
}

/// Makes a rewarded video ad request.
void GADURequestRewardBasedVideoAd(GADUTypeRewardBasedVideoAdRef rewardBasedVideoAd,
                                   GADUTypeRequestRef request, const char *adUnitID) {
  GADURewardBasedVideoAd *internalRewardBasedVideoAd =
      (__bridge GADURewardBasedVideoAd *)rewardBasedVideoAd;
  GADURequest *internalRequest = (__bridge GADURequest *)request;
  [internalRewardBasedVideoAd loadRequest:[internalRequest request]
                             withAdUnitID:GADUStringFromUTF8String(adUnitID)];
}

/// Removes an object from the cache.
void GADURelease(GADUTypeRef ref) {
  if (ref) {
    GADUObjectCache *cache = [GADUObjectCache sharedInstance];
    [cache.references removeObjectForKey:[(__bridge NSObject *)ref gadu_referenceKey]];
  }
}
