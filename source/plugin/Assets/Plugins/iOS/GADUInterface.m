// Copyright 2014 Google Inc. All Rights Reserved.

#import "GADUAdLoader.h"
#import "GADUBanner.h"
#import "GADUInterstitial.h"
#import "GADUNativeCustomTemplateAd.h"
#import "GADUPluginUtil.h"
#import "GADUAdNetworkExtras.h"
#import "GADUObjectCache.h"
#import "GADURequest.h"
#import "GADURewardBasedVideoAd.h"
#import "GADUTypes.h"

/// Returns an NSString copying the characters from |bytes|, a C array of UTF8-encoded bytes.
/// Returns nil if |bytes| is NULL.
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

/// Returns a C string from a C array of UTF8-encoded bytes.
static const char **cStringArrayCopy(NSArray *array) {
  if (array == nil) {
    return nil;
  }

  const char **stringArray;

  stringArray = calloc(array.count, sizeof(char *));
  for (int i = 0; i < array.count; i++) {
    stringArray[i] = cStringCopy([array[i] UTF8String]);
  }
  return stringArray;
}

/// Defines the native ad types.
struct AdTypes {
  int CustomTemplateAd;
};

/// Configures the SDK using the settings associated with the given application ID.
void GADUInitialize(const char *appId) {
  [GADMobileAds configureWithApplicationID:GADUStringFromUTF8String(appId)];
}

// The application’s audio volume. Affects audio volumes of all ads relative to
// other audio output. Valid ad volume values range from 0.0 (silent) to 1.0
// (current device volume). Use this method only if your application has its own
// volume controls (e.g., custom music or sound effect volumes). Defaults
// to 1.0.
void GADUSetApplicationVolume(float volume) {
  [[GADMobileAds sharedInstance] setApplicationVolume:volume];
}

// Indicates if the application’s audio is muted. Affects initial mute state for
// all ads. Use this method only if your application has its own volume controls
// (e.g., custom music or sound effect muting). Defaults to NO.
void GADUSetApplicationMuted(BOOL muted) {
  [[GADMobileAds sharedInstance] setApplicationMuted:muted];
}

// Indicates if the Unity app should be paused when a full screen ad (interstitial
// or rewarded video ad) is displayed.
void GADUSetiOSAppPauseOnBackground(BOOL pause) { [GADUPluginUtil setPauseOnBackground:pause]; }

/// Creates a GADBannerView with the specified width, height, and position. Returns a reference to
/// the GADUBannerView.
GADUTypeBannerRef GADUCreateBannerView(GADUTypeBannerClientRef *bannerClient, const char *adUnitID,
                                       NSInteger width, NSInteger height,
                                       GADAdPosition adPosition) {
  GADUBanner *banner =
      [[GADUBanner alloc] initWithBannerClientReference:bannerClient
                                               adUnitID:GADUStringFromUTF8String(adUnitID)
                                                  width:(int)width
                                                 height:(int)height
                                             adPosition:adPosition];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  [cache.references setObject:banner forKey:[banner gadu_referenceKey]];
  return (__bridge GADUTypeBannerRef)banner;
}

/// Creates a GADBannerView with the specified width, height, and custom position. Returns
/// a reference to the GADUBannerView.
GADUTypeBannerRef GADUCreateBannerViewWithCustomPosition(GADUTypeBannerClientRef *bannerClient,
                                                         const char *adUnitID, NSInteger width,
                                                         NSInteger height, NSInteger x,
                                                         NSInteger y) {
  CGPoint adPosition = CGPointMake(x, y);
  GADUBanner *banner =
      [[GADUBanner alloc] initWithBannerClientReference:bannerClient
                                               adUnitID:GADUStringFromUTF8String(adUnitID)
                                                  width:(int)width
                                                 height:(int)height
                                       customAdPosition:adPosition];
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

/// Creates a full-width GADBannerView in the current orientation with a custom position. Returns a
/// reference to the GADUBannerView.
GADUTypeBannerRef GADUCreateSmartBannerViewWithCustomPosition(GADUTypeBannerClientRef *bannerClient,
                                                              const char *adUnitID, NSInteger x,
                                                              NSInteger y) {
  CGPoint adPosition = CGPointMake(x, y);
  GADUBanner *banner = [[GADUBanner alloc]
      initWithSmartBannerSizeAndBannerClientReference:bannerClient
                                             adUnitID:GADUStringFromUTF8String(adUnitID)
                                     customAdPosition:adPosition];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  [cache.references setObject:banner forKey:[banner gadu_referenceKey]];
  return (__bridge GADUTypeBannerRef)banner;
}

/// Creates a GADUInterstitial and returns its reference.
GADUTypeInterstitialRef GADUCreateInterstitial(GADUTypeInterstitialClientRef *interstitialClient,
                                               const char *adUnitID) {
  GADUInterstitial *interstitial = [[GADUInterstitial alloc]
      initWithInterstitialClientReference:interstitialClient
                                 adUnitID:GADUStringFromUTF8String(adUnitID)];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  [cache.references setObject:interstitial forKey:[interstitial gadu_referenceKey]];
  return (__bridge GADUTypeInterstitialRef)interstitial;
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

/// Creates a GADUAdLoader and returns its reference.
GADUTypeAdLoaderRef GADUCreateAdLoader(GADUTypeAdLoaderClientRef *adLoaderClient,
                                       const char *adUnitID,
                                       const char **templateIDs, NSInteger templateIDLength,
                                       struct AdTypes *types) {
  NSMutableArray *templateIDsArray = [[NSMutableArray alloc] init];
  for (int i = 0; i < templateIDLength; i++) {
    [templateIDsArray addObject:GADUStringFromUTF8String(templateIDs[i])];
  }
  NSMutableArray *adTypesArray = [[NSMutableArray alloc] init];
  if (types->CustomTemplateAd) {
    [adTypesArray addObject:kGADAdLoaderAdTypeNativeCustomTemplate];
  }
  NSArray *options = nil;

  GADUAdLoader *adLoader =
      [[GADUAdLoader alloc] initWithAdLoaderClientReference:adLoaderClient
                                                   adUnitID:GADUStringFromUTF8String(adUnitID)
                                                templateIDs:templateIDsArray
                                                    adTypes:adTypesArray
                                                    options:options];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  [cache.references setObject:adLoader forKey:[adLoader gadu_referenceKey]];
  return (__bridge GADUTypeAdLoaderRef)adLoader;
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
    GADURewardBasedVideoAdWillLeaveApplicationCallback willLeaveCallback,
    GADURewardBasedVideoAdDidCompleteCallback didCompleteCallback) {
  GADURewardBasedVideoAd *internalRewardBasedVideoAd =
      (__bridge GADURewardBasedVideoAd *)rewardBasedVideoAd;
  internalRewardBasedVideoAd.adReceivedCallback = adReceivedCallback;
  internalRewardBasedVideoAd.adFailedCallback = adFailedCallback;
  internalRewardBasedVideoAd.didOpenCallback = didOpenCallback;
  internalRewardBasedVideoAd.didStartPlayingCallback = didStartCallback;
  internalRewardBasedVideoAd.didCloseCallback = didCloseCallback;
  internalRewardBasedVideoAd.didRewardCallback = didRewardCallback;
  internalRewardBasedVideoAd.willLeaveCallback = willLeaveCallback;
  internalRewardBasedVideoAd.didCompleteCallback = didCompleteCallback;
}

/// Sets the banner callback methods to be invoked during native ad events.
void GADUSetAdLoaderCallbacks(
    GADUTypeAdLoaderRef adLoader,
    GADUAdLoaderDidReceiveNativeCustomTemplateAdCallback customTemplateAdReceivedCallback,
    GADUAdLoaderDidFailToReceiveAdWithErrorCallback adFailedCallback) {
  GADUAdLoader *internalAdLoader = (__bridge GADUAdLoader *)adLoader;
  internalAdLoader.customTemplateAdReceivedCallback = customTemplateAdReceivedCallback;
  internalAdLoader.adFailedCallback = adFailedCallback;
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

float GADUGetBannerViewHeightInPixels(GADUTypeBannerRef banner) {
  GADUBanner *internalBanner = (__bridge GADUBanner *)banner;
  return internalBanner.heightInPixels;
}

float GADUGetBannerViewWidthInPixels(GADUTypeBannerRef banner) {
  GADUBanner *internalBanner = (__bridge GADUBanner *)banner;
  return internalBanner.widthInPixels;
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

/// Sets the user ID to be used in server-to-server reward callbacks.
void GADUSetRewardBasedVideoAdUserId(GADUTypeRewardBasedVideoAdRef rewardBasedVideo,
                                     const char *userId) {
  GADURewardBasedVideoAd *internalRewardBasedVideoAd =
      (__bridge GADURewardBasedVideoAd *)rewardBasedVideo;
  [internalRewardBasedVideoAd setUserId:GADUStringFromUTF8String(userId)];
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

/// Creates an empty NSMutableableDictionary returns its reference.
GADUTypeMutableDictionaryRef GADUCreateMutableDictionary() {
  NSMutableDictionary *dictionary = [[NSMutableDictionary alloc] init];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  [cache.references setObject:dictionary forKey:[dictionary gadu_referenceKey]];
  return (__bridge GADUTypeMutableDictionaryRef)(dictionary);
}

/// Sets an mediation extra key value pair on a NSMutableableDictionary.
void GADUMutableDictionarySetValue(GADUTypeMutableDictionaryRef dictionary, const char *key,
                                   const char *value) {
  NSMutableDictionary *internalDictionary = (__bridge NSMutableDictionary *)dictionary;
  [internalDictionary setValue:GADUStringFromUTF8String(value)
                        forKey:GADUStringFromUTF8String(key)];
}

/// Create a GADMediatonExtras object from the specified NSMutableableDictionary of extras and
/// include it in the ad request.
void GADUSetMediationExtras(GADUTypeRequestRef request, GADUTypeMutableDictionaryRef dictionary,
                            const char *adNetworkExtraClassName) {
  GADURequest *internalRequest = (__bridge GADURequest *)request;
  NSMutableDictionary *internalDictionary = (__bridge NSMutableDictionary *)dictionary;
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];

  id<GADUAdNetworkExtras> extra =
      [[NSClassFromString(GADUStringFromUTF8String(adNetworkExtraClassName)) alloc] init];
  if (![extra respondsToSelector:@selector(adNetworkExtrasWithDictionary:)]) {
    NSLog(@"Unable to create mediation ad network class: %@",
          GADUStringFromUTF8String(adNetworkExtraClassName));
    [cache.references removeObjectForKey:[internalDictionary gadu_referenceKey]];
    return;
  }

  [internalRequest.mediationExtras
      addObject:[extra adNetworkExtrasWithDictionary:internalDictionary]];
  [cache.references removeObjectForKey:[internalDictionary gadu_referenceKey]];
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

void GADUSetBannerViewAdPosition(GADUTypeBannerRef banner, int position) {
  GADUBanner *internalBanner = (__bridge GADUBanner *)banner;
  [internalBanner setAdPosition:(GADAdPosition)position];
}

void GADUSetBannerViewCustomPosition(GADUTypeBannerRef banner, int x, int y) {
  GADUBanner *internalBanner = (__bridge GADUBanner *)banner;
  [internalBanner setCustomAdPosition:CGPointMake(x, y)];
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

/// Makes a native ad request.
void GADURequestNativeAd(GADUTypeAdLoaderRef adLoader, GADUTypeRequestRef request) {
  GADUAdLoader *internalAdLoader = (__bridge GADUAdLoader *)adLoader;
  GADURequest *internalRequest = (__bridge GADURequest *)request;
  [internalAdLoader loadRequest:[internalRequest request]];
}

/// Return the template ID of the native custom template ad.
const char *GADUNativeCustomTemplateAdTemplateID(
    GADUTypeNativeCustomTemplateAdRef nativeCustomTemplateAd) {
  GADUNativeCustomTemplateAd *internalNativeCustomTemplateAd =
      (__bridge GADUNativeCustomTemplateAd *)nativeCustomTemplateAd;
  return cStringCopy([internalNativeCustomTemplateAd templateID].UTF8String);
}

/// Returns the image corresponding to the specifed key as a base64 encoded byte array.
const char *GADUNativeCustomTemplateAdImageAsBytesForKey(
    GADUTypeNativeCustomTemplateAdRef nativeCustomTemplateAd, const char *key) {
  GADUNativeCustomTemplateAd *internalNativeCustomTemplateAd =
      (__bridge GADUNativeCustomTemplateAd *)nativeCustomTemplateAd;
  NSData *imageData = UIImageJPEGRepresentation(
      [internalNativeCustomTemplateAd imageForKey:GADUStringFromUTF8String(key)], 0.0);
  NSString *base64String = [imageData base64EncodedStringWithOptions:nil];
  return cStringCopy(base64String.UTF8String);
}

/// Returns the string corresponding to the specifed key.
const char *GADUNativeCustomTemplateAdStringForKey(
    GADUTypeNativeCustomTemplateAdRef nativeCustomTemplateAd, const char *key) {
  GADUNativeCustomTemplateAd *internalNativeCustomTemplateAd =
      (__bridge GADUNativeCustomTemplateAd *)nativeCustomTemplateAd;
  return cStringCopy(
      [internalNativeCustomTemplateAd stringForKey:GADUStringFromUTF8String(key)].UTF8String);
}

/// Call when the ad is played on screen to the user.
void GADUNativeCustomTemplateAdRecordImpression(
    GADUTypeNativeCustomTemplateAdRef nativeCustomTemplateAd) {
  GADUNativeCustomTemplateAd *internalNativeCustomTemplateAd =
      (__bridge GADUNativeCustomTemplateAd *)nativeCustomTemplateAd;
  [internalNativeCustomTemplateAd recordImpression];
}

/// Call when the user clicks on an ad.
void GADUNativeCustomTemplateAdPerformClickOnAssetWithKey(
    GADUTypeNativeCustomTemplateAdRef nativeCustomTemplateAd, const char *key,
    BOOL customClickAction) {
  GADUNativeCustomTemplateAd *internalNativeCustomTemplateAd =
      (__bridge GADUNativeCustomTemplateAd *)nativeCustomTemplateAd;
  [internalNativeCustomTemplateAd performClickOnAssetWithKey:GADUStringFromUTF8String(key)
                                       withCustomClickAction:customClickAction];
}

/// Returns the list of available asset keys for a custom native template ad.
const char **GADUNativeCustomTemplateAdAvailableAssetKeys(
    GADUTypeNativeCustomTemplateAdRef nativeCustomTemplateAd) {
  GADUNativeCustomTemplateAd *internalNativeCustomTemplateAd =
      (__bridge GADUNativeCustomTemplateAd *)nativeCustomTemplateAd;
  NSArray *availableAssetKeys = [internalNativeCustomTemplateAd availableAssetKeys];
  return cStringArrayCopy(availableAssetKeys);
}

/// Returns the number of available asset keys for a custom native template ad.
int GADUNativeCustomTemplateAdNumberOfAvailableAssetKeys(
    GADUTypeNativeCustomTemplateAdRef nativeCustomTemplateAd) {
  GADUNativeCustomTemplateAd *internalNativeCustomTemplateAd =
      (__bridge GADUNativeCustomTemplateAd *)nativeCustomTemplateAd;
  return (int)[internalNativeCustomTemplateAd availableAssetKeys].count;
}

/// Sets the Unity native custom template ad client reference on GADUNativeCustomTemplateAd.
void GADUSetNativeCustomTemplateAdUnityClient(
    GADUTypeNativeCustomTemplateAdRef nativeCustomTemplateAd,
    GADUTypeNativeCustomTemplateAdClientRef *nativeCustomTemplateClient) {
  GADUNativeCustomTemplateAd *internalNativeCustomTemplateAd =
      (__bridge GADUNativeCustomTemplateAd *)nativeCustomTemplateAd;
  internalNativeCustomTemplateAd.nativeCustomTemplateClient = nativeCustomTemplateClient;
}

/// Sets the ad callback methods to be invoked during native custom template ad events.
void GADUSetNativeCustomTemplateAdCallbacks(
    GADUTypeNativeCustomTemplateAdRef nativeCustomTemplateAd,
    GADUNativeCustomTemplateDidReceiveClickCallback adClickedCallback) {
  GADUNativeCustomTemplateAd *internalNativeCustomTemplateAd =
      (__bridge GADUNativeCustomTemplateAd *)nativeCustomTemplateAd;
  internalNativeCustomTemplateAd.didReceiveClickCallback = adClickedCallback;
}

/// Removes an object from the cache.
void GADURelease(GADUTypeRef ref) {
  if (ref) {
    GADUObjectCache *cache = [GADUObjectCache sharedInstance];
    [cache.references removeObjectForKey:[(__bridge NSObject *)ref gadu_referenceKey]];
  }
}

const char *GADUMediationAdapterClassNameForBannerView(GADUTypeBannerRef bannerView) {
  GADUBanner *banner = (__bridge GADUBanner *)bannerView;
  return cStringCopy(banner.mediationAdapterClassName.UTF8String);
}

const char *GADUMediationAdapterClassNameForRewardedVideo(
    GADUTypeRewardBasedVideoAdRef rewardedVideo) {
  GADURewardBasedVideoAd *rewarded = (__bridge GADURewardBasedVideoAd *)rewardedVideo;
  return cStringCopy(rewarded.mediationAdapterClassName.UTF8String);
}

const char *GADUMediationAdapterClassNameForInterstitial(GADUTypeInterstitialRef interstitial) {
  GADUInterstitial *interstitialAd = (__bridge GADUInterstitial *)interstitial;
  return cStringCopy(interstitialAd.mediationAdapterClassName.UTF8String);
}
