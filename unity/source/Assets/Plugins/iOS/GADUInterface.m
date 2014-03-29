// Copyright 2014 Google Inc. All Rights Reserved.

#import "GADUBanner.h"
#import "GADUObjectCache.h"
#import "GADURequest.h"
#import "GADUTypes.h"

/// Helper method used to convert C-style strings into NSStrings.
NSString *GADUStringFromUTF8String(const char *bytes) {
  if (bytes) {
    return @(bytes);
  } else {
    return @"";
  }
}

/// Creates a GADBannerView with the specified width, height, and position. Returns a reference to
/// the GADUBannerView.
GADUTypeBannerRef GADUCreateBannerView(GADUTypeBannerClientRef *bannerClient, const char *adUnitID,
                                       NSInteger width, NSInteger height,
                                       GADAdPosition adPosition) {
  GADUBanner *banner =
      [[[GADUBanner alloc] initWithBannerClientReference:bannerClient
                                                adUnitID:GADUStringFromUTF8String(adUnitID)
                                                   width:width
                                                  height:height
                                              adPosition:adPosition] autorelease];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  [cache.references setObject:banner forKey:[banner gadu_referenceKey]];
  return banner;
}

/// Creates a full-width GADBannerView in the current orientation and returns a reference to the
/// GADUBannerView.
GADUTypeBannerRef GADUCreateSmartBannerView(GADUTypeBannerClientRef *bannerClient,
                                            const char *adUnitID, GADAdPosition adPosition) {
  GADUBanner *banner = [[[GADUBanner alloc]
      initWithSmartBannerSizeAndBannerClientReference:bannerClient
                                             adUnitID:GADUStringFromUTF8String(adUnitID)
                                           adPosition:adPosition] autorelease];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  [cache.references setObject:banner forKey:[banner gadu_referenceKey]];
  return banner;
}

/// Sets the callback methods to be invoked during ad events.
void GADUSetCallbacks(GADUTypeBannerRef banner, GADUAdViewDidReceiveAdCallback adReceivedCallback,
                      GADUAdViewDidFailToReceiveAdWithErrorCallback adFailedCallback,
                      GADUAdViewWillPresentScreenCallback willPresentCallback,
                      GADUAdViewWillDismissScreenCallback willDismissCallback,
                      GADUAdViewDidDismissScreenCallback didDismissCallback,
                      GADUAdViewWillLeaveApplicationCallback willLeaveCallback) {
  GADUBanner *internalBanner = (GADUBanner *)banner;
  internalBanner.adReceivedCallback = adReceivedCallback;
  internalBanner.adFailedCallback = adFailedCallback;
  internalBanner.willPresentCallback = willPresentCallback;
  internalBanner.willDismissCallback = willDismissCallback;
  internalBanner.didDismissCallback = didDismissCallback;
  internalBanner.willLeaveCallback = willLeaveCallback;
}

/// Sets the GADBannerView's hidden property to YES.
void GADUHideBannerView(GADUTypeBannerRef banner) {
  GADUBanner *internalBanner = (GADUBanner *)banner;
  [internalBanner hideBannerView];
}

/// Sets the GADBannerView's hidden property to NO.
void GADUShowBannerView(GADUTypeBannerRef banner) {
  GADUBanner *internalBanner = (GADUBanner *)banner;
  [internalBanner showBannerView];
}

/// Creates an empty GADRequest and returns its reference.
GADUTypeRequestRef GADUCreateRequest() {
  GADURequest *request = [[[GADURequest alloc] init] autorelease];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  [cache.references setObject:request forKey:[request gadu_referenceKey]];
  return request;
}

/// Adds a test device to the GADRequest.
void GADUAddTestDevice(GADUTypeRequestRef request, const char *deviceID) {
  GADURequest *internalRequest = (GADURequest *)request;
  [internalRequest addTestDevice:GADUStringFromUTF8String(deviceID)];
}

/// Adds a keyword to the GADRequest.
void GADUAddKeyword(GADUTypeRequestRef request, const char *keyword) {
  GADURequest *internalRequest = (GADURequest *)request;
  [internalRequest addKeyword:GADUStringFromUTF8String(keyword)];
}

/// Sets the user's birthday on the GADRequest.
void GADUSetBirthday(GADUTypeRequestRef request, NSInteger year, NSInteger month, NSInteger day) {
  GADURequest *internalRequest = (GADURequest *)request;
  [internalRequest setBirthdayWithMonth:month day:day year:year];
}

/// Sets the user's gender on the GADRequest.
void GADUSetGender(GADUTypeRequestRef request, NSInteger genderCode) {
  GADURequest *internalRequest = (GADURequest *)request;
  [internalRequest setGenderWithCode:genderCode];
}

/// Tags a GADRequest to specify whether it should be treated as child-directed for purposes of the
/// Children’s Online Privacy Protection Act (COPPA) -
/// http://business.ftc.gov/privacy-and-security/childrens-privacy.
void GADUTagForChildDirectedTreatment(GADUTypeRequestRef request, BOOL childDirectedTreatment) {
  GADURequest *internalRequest = (GADURequest *)request;
  internalRequest.tagForChildDirectedTreatment = childDirectedTreatment;
}

/// Sets an extra parameter to be included in the ad request.
void GADUSetExtra(GADUTypeRequestRef request, const char *key, const char *value) {
  GADURequest *internalRequest = (GADURequest *)request;
  [internalRequest setExtraWithKey:GADUStringFromUTF8String(key)
                             value:GADUStringFromUTF8String(value)];
}

/// Makes an ad request.
void GADURequestBannerAd(GADUTypeBannerRef banner, GADUTypeRequestRef request) {
  GADUBanner *internalBanner = (GADUBanner *)banner;
  GADURequest *internalRequest = (GADURequest *)request;
  [internalBanner loadRequest:[internalRequest request]];
}

/// Removes an object from the cache.
void GADURelease(GADUTypeRef ref) {
  NSLog(@"Invoking Release");
  if (ref) {
    GADUObjectCache *cache = [GADUObjectCache sharedInstance];
    [cache.references removeObjectForKey:[(NSObject *)ref gadu_referenceKey]];
  }
}
