// Copyright 2014 Google Inc. All Rights Reserved.

#import <GoogleMobileAds/GoogleMobileAds.h>
#import "GADUAdNetworkExtras.h"
#import "GADUAppOpenAd.h"
#import "GADUInterstitialAd.h"
#import "GADUBanner.h"
#import "GADUObjectCache.h"
#import "GADUPluginUtil.h"
#import "GADURequest.h"
#import "GADURequestConfiguration.h"
#import "GADURewardedAd.h"
#import "GADURewardedInterstitialAd.h"
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

void GADUInitializeWithCallback(GADUTypeMobileAdsClientRef *mobileAdsClientRef,
                                GADUInitializationCompleteCallback callback) {
  [[GADMobileAds sharedInstance]
      startWithCompletionHandler:^(GADInitializationStatus *_Nonnull status) {
        GADUObjectCache *cache = [GADUObjectCache sharedInstance];
        cache[status.gadu_referenceKey] = status;
        callback(mobileAdsClientRef, (__bridge GADUTypeInitializationStatusRef)status);
      }];
}

void GADUDisableMediationInitialization() {
  [[GADMobileAds sharedInstance] disableMediationInitialization];
}

const char *GADUGetInitDescription(GADUTypeInitializationStatusRef statusRef,
                                   const char *className) {
  GADInitializationStatus *status = (__bridge GADInitializationStatus *)statusRef;

  GADAdapterStatus *adapterStatus =
      status.adapterStatusesByClassName[GADUStringFromUTF8String(className)];
  return cStringCopy(adapterStatus.description.UTF8String);
}

int GADUGetInitLatency(GADUTypeInitializationStatusRef statusRef, const char *className) {
  GADInitializationStatus *status = (__bridge GADInitializationStatus *)statusRef;
  GADAdapterStatus *adapterStatus =
      status.adapterStatusesByClassName[GADUStringFromUTF8String(className)];
  return adapterStatus.latency;
}

int GADUGetInitState(GADUTypeInitializationStatusRef statusRef, const char *className) {
  GADInitializationStatus *status = (__bridge GADInitializationStatus *)statusRef;
  GADAdapterStatus *adapterStatus =
      status.adapterStatusesByClassName[GADUStringFromUTF8String(className)];
  return (int)adapterStatus.state;
}

const char **GADUGetInitAdapterClasses(GADUTypeInitializationStatusRef statusRef) {
  GADInitializationStatus *status = (__bridge GADInitializationStatus *)statusRef;
  NSDictionary<NSString *, GADAdapterStatus *> *map = status.adapterStatusesByClassName;
  NSArray<NSString *> *classes = map.allKeys;
  return cStringArrayCopy(classes);
}

int GADUGetInitNumberOfAdapterClasses(GADUTypeInitializationStatusRef statusRef) {
  GADInitializationStatus *status = (__bridge GADInitializationStatus *)statusRef;
  NSDictionary<NSString *, GADAdapterStatus *> *map = status.adapterStatusesByClassName;
  NSArray<NSString *> *classes = map.allKeys;
  return (int)classes.count;
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

float GADUDeviceScale() { return UIScreen.mainScreen.scale; }

/// Returns the safe width of the device.
int GADUDeviceSafeWidth() {
  CGRect screenBounds = [UIScreen mainScreen].bounds;
  if (GADUIsOperatingSystemAtLeastVersion(11)) {
    CGRect safeFrame = [UIApplication sharedApplication].keyWindow.safeAreaLayoutGuide.layoutFrame;
    if (!CGSizeEqualToSize(safeFrame.size, CGSizeZero)) {
      screenBounds = safeFrame;
    }
  }
  return (int)CGRectGetWidth(screenBounds);
}

/// Create an empty CreateRequestConfiguration
GADUTypeRequestConfigurationRef GADUCreateRequestConfiguration() {
  GADURequestConfiguration *requestConfiguration = [[GADURequestConfiguration alloc] init];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  cache[requestConfiguration.gadu_referenceKey] = requestConfiguration;
  return (__bridge GADUTypeRequestConfigurationRef)(requestConfiguration);
}

/// Set MobileAds RequestConfiguration
void GADUSetRequestConfiguration(GADUTypeRequestConfigurationRef requestConfiguration) {
  GADURequestConfiguration *internalRequestConfiguration =
      (__bridge GADURequestConfiguration *)requestConfiguration;
  GADMobileAds.sharedInstance.requestConfiguration.maxAdContentRating =
      internalRequestConfiguration.maxAdContentRating;
  GADMobileAds.sharedInstance.requestConfiguration.testDeviceIdentifiers =
      internalRequestConfiguration.testDeviceIdentifiers;

  switch (internalRequestConfiguration.tagForUnderAgeOfConsent) {
    case (kGADURequestConfigurationTagForUnderAgeOfConsentTrue): {
      [GADMobileAds.sharedInstance.requestConfiguration tagForUnderAgeOfConsent:true];
      break;
    }
    case (kGADURequestConfigurationTagForUnderAgeOfConsentFalse): {
      [GADMobileAds.sharedInstance.requestConfiguration tagForUnderAgeOfConsent:false];
      break;
    }
    case (kGADURequestConfigurationTagForUnderAgeOfConsentUnspecified): {
      break;
    }
  }
  switch (internalRequestConfiguration.tagForChildDirectedTreatment) {
    case (kGADURequestConfigurationTagForChildDirectedTreatmentTrue): {
      [GADMobileAds.sharedInstance.requestConfiguration tagForChildDirectedTreatment:true];
      break;
    }
    case (kGADURequestConfigurationTagForChildDirectedTreatmentFalse): {
      [GADMobileAds.sharedInstance.requestConfiguration tagForChildDirectedTreatment:false];
      break;
    }
    case (kGADURequestConfigurationTagForChildDirectedTreatmentUnspecified): {
      break;
    }
  }

  [GADMobileAds.sharedInstance.requestConfiguration
      setSameAppKeyEnabled:internalRequestConfiguration.sameAppKeyEnabled];
}

/// Set RequestConfiguration Max Ad Content Rating
void GADUSetRequestConfigurationMaxAdContentRating(
    GADUTypeRequestConfigurationRef requestConfiguration, const char *maxAdContentRating) {
  GADURequestConfiguration *internalRequestConfiguration =
      (__bridge GADURequestConfiguration *)requestConfiguration;
  [internalRequestConfiguration setMaxAdContentRating:GADUStringFromUTF8String(maxAdContentRating)];
}

/// Set RequestConfiguration Test Device Ids
void GADUSetRequestConfigurationTestDeviceIdentifiers(
    GADUTypeRequestConfigurationRef requestConfiguration, const char **testDeviceIDs,
    NSInteger testDeviceIDLength) {
  GADURequestConfiguration *internalRequestConfiguration =
      (__bridge GADURequestConfiguration *)requestConfiguration;
  NSMutableArray *testDeviceIDsArray = [[NSMutableArray alloc] init];
  for (int i = 0; i < testDeviceIDLength; i++) {
    [testDeviceIDsArray addObject:GADUStringFromUTF8String(testDeviceIDs[i])];
  }
  [internalRequestConfiguration setTestDeviceIdentifiers:testDeviceIDsArray];
}

/// Set RequestConfiguration tagForUnderAgeOfConsent
void GADUSetRequestConfigurationTagForUnderAgeOfConsent(
    GADUTypeRequestConfigurationRef requestConfiguration, int tagForUnderAgeOfConsent) {
  GADURequestConfiguration *internalRequestConfiguration =
      (__bridge GADURequestConfiguration *)requestConfiguration;
  internalRequestConfiguration.tagForUnderAgeOfConsent = tagForUnderAgeOfConsent;
}

/// Set RequestConfiguration tagForChildDirectedTreatment
void GADUSetRequestConfigurationTagForChildDirectedTreatment(
    GADUTypeRequestConfigurationRef requestConfiguration, int tagForChildDirectedTreatment) {
  GADURequestConfiguration *internalRequestConfiguration =
      (__bridge GADURequestConfiguration *)requestConfiguration;
  internalRequestConfiguration.tagForChildDirectedTreatment = tagForChildDirectedTreatment;
}

/// Set RequestConfiguration setSameAppKeyEnabled
void GADUSetRequestConfigurationSameAppKeyEnabled(
    GADUTypeRequestConfigurationRef requestConfiguration, BOOL enabled) {
  GADURequestConfiguration *internalRequestConfiguration =
      (__bridge GADURequestConfiguration *)requestConfiguration;
  internalRequestConfiguration.sameAppKeyEnabled = enabled;
}

/// Returns RequestConfiguration Max Ad Content Rating
const char *GADUGetMaxAdContentRating(GADUTypeRequestConfigurationRef requestConfiguration) {
  GADURequestConfiguration *internalRequestConfiguration =
      (__bridge GADURequestConfiguration *)requestConfiguration;
  return cStringCopy(internalRequestConfiguration.maxAdContentRating.UTF8String);
}

/// Returns RequestConfiguration tag For Under Age Of Consent
const int GADUGetRequestConfigurationTagForUnderAgeOfConsent(
    GADUTypeRequestConfigurationRef requestConfiguration) {
  GADURequestConfiguration *internalRequestConfiguration =
      (__bridge GADURequestConfiguration *)requestConfiguration;
  return internalRequestConfiguration.tagForUnderAgeOfConsent;
}

/// Returns RequestConfiguration tag For Child Directed Treatment
const int GADUGetRequestConfigurationTagForChildDirectedTreatment(
    GADUTypeRequestConfigurationRef requestConfiguration) {
  GADURequestConfiguration *internalRequestConfiguration =
      (__bridge GADURequestConfiguration *)requestConfiguration;
  return internalRequestConfiguration.tagForChildDirectedTreatment;
}

/// Returns List RequestConfiguration Test Device Ids
const char **GADUGetTestDeviceIdentifiers(GADUTypeRequestConfigurationRef requestConfiguration) {
  GADURequestConfiguration *internalRequestConfiguration =
      (__bridge GADURequestConfiguration *)requestConfiguration;
  NSArray<NSString *> *testDeviceIDs = internalRequestConfiguration.testDeviceIdentifiers;
  return cStringArrayCopy(testDeviceIDs);
}

/// Returns count of RequestConfiguration Test Device Ids
int GADUGetTestDeviceIdentifiersCount(GADUTypeRequestConfigurationRef requestConfiguration) {
  GADURequestConfiguration *internalRequestConfiguration =
      (__bridge GADURequestConfiguration *)requestConfiguration;
  NSArray<NSString *> *testDeviceIDs = internalRequestConfiguration.testDeviceIdentifiers;
  return testDeviceIDs.count;
}

/// Returns RequestConfiguration sameAppKeyEnabled
BOOL GADUGetRequestConfigurationSameAppKeyEnabled(
    GADUTypeRequestConfigurationRef requestConfiguration) {
  GADURequestConfiguration *internalRequestConfiguration =
      (__bridge GADURequestConfiguration *)requestConfiguration;
  return internalRequestConfiguration.sameAppKeyEnabled;
}

/// Creates an empty GADRequest and returns its reference.
GADUTypeRequestRef GADUCreateRequest() {
  GADURequest *request = [[GADURequest alloc] init];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  cache[request.gadu_referenceKey] = request;
  return (__bridge GADUTypeRequestRef)(request);
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
    [cache removeObjectForKey:[internalDictionary gadu_referenceKey]];
    return;
  }

  [internalRequest.mediationExtras
      addObject:[extra adNetworkExtrasWithDictionary:internalDictionary]];
  [cache removeObjectForKey:[internalDictionary gadu_referenceKey]];
}

/// Sets an extra parameter to be included in the ad request.
void GADUSetExtra(GADUTypeRequestRef request, const char *key, const char *value) {
  GADURequest *internalRequest = (__bridge GADURequest *)request;
  [internalRequest setExtraWithKey:GADUStringFromUTF8String(key)
                             value:GADUStringFromUTF8String(value)];
}

#pragma mark - ServerSideVerification

/// Creates an empty GADServerSideVerificationOptions and returns its reference.
GADUTypeServerSideVerificationOptionsRef GADUCreateServerSideVerificationOptions() {
  GADServerSideVerificationOptions *options = [[GADServerSideVerificationOptions alloc] init];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  cache[options.gadu_referenceKey] = options;
  return (__bridge GADUTypeServerSideVerificationOptionsRef)(options);
}

/// Sets the user id on the GADServerSideVerificationOptions
void GADUServerSideVerificationOptionsSetUserId(GADUTypeServerSideVerificationOptionsRef options,
                                                const char *userId) {
  GADServerSideVerificationOptions *internalOptions =
      (__bridge GADServerSideVerificationOptions *)options;
  internalOptions.userIdentifier = GADUStringFromUTF8String(userId);
}

/// Sets the custom reward string on the GADServerSideVerificationOptions
void GADUServerSideVerificationOptionsSetCustomRewardString(
    GADUTypeServerSideVerificationOptionsRef options, const char *customRewardString) {
  GADServerSideVerificationOptions *internalOptions =
      (__bridge GADServerSideVerificationOptions *)options;
  internalOptions.customRewardString = GADUStringFromUTF8String(customRewardString);
}

#pragma mark - Ad Inspector

void GADUPresentAdInspector(GADUTypeMobileAdsClientRef *mobileAdsClientRef,
                            GADUAdInspectorCompleteCallback adInspectorCompletionCallback) {
  UIViewController *unityController = [GADUPluginUtil unityGLViewController];
  [GADMobileAds.sharedInstance presentAdInspectorFromViewController:unityController
                                            completionHandler:^(NSError *_Nullable error) {
                                                if (adInspectorCompletionCallback) {
                                                  adInspectorCompletionCallback(mobileAdsClientRef,
                                                          (__bridge GADUTypeErrorRef) error);
                                                }
                                            }];
}

#pragma mark - ResponseInfo

const GADUTypeResponseInfoRef GADUGetAdErrorResponseInfo(GADUTypeErrorRef error){
  NSError *internalError = (__bridge NSError *)error;
  GADResponseInfo *responseInfo = internalError.userInfo[GADErrorUserInfoKeyResponseInfo];
  return (__bridge GADUTypeResponseInfoRef)(responseInfo);
}

const char *GADUResponseInfoMediationAdapterClassName(GADUTypeResponseInfoRef responseInfo){
  GADResponseInfo *internalResponseInfo = (__bridge GADResponseInfo *)responseInfo;
  return cStringCopy(internalResponseInfo.adNetworkClassName.UTF8String);
}

const char *GADUResponseInfoResponseId(GADUTypeResponseInfoRef responseInfo){
  GADResponseInfo *internalResponseInfo = (__bridge GADResponseInfo *)responseInfo;
  return cStringCopy(internalResponseInfo.responseIdentifier.UTF8String);
}

const char *GADUGetResponseInfoDescription(GADUTypeResponseInfoRef responseInfo){
  GADResponseInfo *internalResponseInfo = (__bridge GADResponseInfo *)responseInfo;
  return cStringCopy(internalResponseInfo.description.UTF8String);
}

#pragma mark - Errors

const char *GADUGetAdErrorDescription(GADUTypeErrorRef error){
  NSError *internalError = (__bridge NSError *)error;
  return cStringCopy(internalError.description.UTF8String);
}

const int GADUGetAdErrorCode(GADUTypeErrorRef error) {
  NSError *internalError = (__bridge NSError *)error;
  return internalError.code;
}

const char *GADUGetAdErrorDomain(GADUTypeErrorRef error) {
  NSError *internalError = (__bridge NSError *)error;
  return cStringCopy(internalError.domain.UTF8String);
}

const char *GADUGetAdErrorMessage(GADUTypeErrorRef error) {
  NSError *internalError = (__bridge NSError *)error;
  return cStringCopy(internalError.localizedDescription.UTF8String);
}

const GADUTypeErrorRef GADUGetAdErrorUnderLyingError(GADUTypeErrorRef error){
  NSError *internalError = (__bridge NSError *)error;
  NSError *underlyingError = internalError.userInfo[NSUnderlyingErrorKey];
  return (__bridge GADUTypeErrorRef)(underlyingError);
}


#pragma mark - Other methods
/// Removes an object from the cache.
void GADURelease(GADUTypeAdBridgeRef ref) {
  if (ref) {
    GADUObjectCache *cache = [GADUObjectCache sharedInstance];
    [cache removeObjectForKey:[(__bridge NSObject *)ref gadu_referenceKey]];
  }
}

/// Creates an empty NSMutableableDictionary returns its reference.
GADUTypeMutableDictionaryRef GADUCreateMutableDictionary() {
  NSMutableDictionary *dictionary = [[NSMutableDictionary alloc] init];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  cache[dictionary.gadu_referenceKey] = dictionary;
  return (__bridge GADUTypeMutableDictionaryRef)(dictionary);
}

/// Sets an mediation extra key value pair on a NSMutableableDictionary.
void GADUMutableDictionarySetValue(GADUTypeMutableDictionaryRef dictionary, const char *key,
                                   const char *value) {
  NSMutableDictionary *internalDictionary = (__bridge NSMutableDictionary *)dictionary;
  [internalDictionary setValue:GADUStringFromUTF8String(value)
                        forKey:GADUStringFromUTF8String(key)];
}
