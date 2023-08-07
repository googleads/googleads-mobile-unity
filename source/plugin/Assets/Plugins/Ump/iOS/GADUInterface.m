// Copyright 2022 Google LLC. All Rights Reserved.

#import "GADUConsentForm.h"
#import "GADUConsentInformation.h"
#import "GADUDebugSettings.h"
#import "GADUObjectCache.h"
#import "GADUUmpTypes.h"

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

#pragma mark - UMP SDK

// FormError Methods

const int GADUGetFormErrorCode(GADUTypeErrorRef error) {
  NSError *internalError = (__bridge NSError *)error;
  return internalError.code;
}

const char *GADUGetFormErrorMessage(GADUTypeErrorRef error) {
  NSError *internalError = (__bridge NSError *)error;
  return cStringCopy(internalError.localizedDescription.UTF8String);
}

// Consent Information Methods

/// Create an empty GADURequestParameters
GADUTypeRequestParametersRef GADUCreateRequestParameters() {
  GADURequestParameters *requestParameters = [[GADURequestParameters alloc] init];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  cache[requestParameters.gadu_referenceKey] = requestParameters;
  return (__bridge GADUTypeRequestParametersRef)requestParameters;
}

/// Set GADURequestParameters tagForUnderAgeOfConsent
void GADUSetRequestParametersTagForUnderAgeOfConsent(
    GADUTypeRequestParametersRef requestParameters, BOOL tagForUnderAgeOfConsent) {
  GADURequestParameters *internalRequestParameters =
      (__bridge GADURequestParameters *)requestParameters;
  internalRequestParameters.tagForUnderAgeOfConsent = tagForUnderAgeOfConsent;
}

/// Create an empty GADUDebugSettings.
GADUTypeDebugSettingsRef GADUCreateDebugSettings() {
  GADUDebugSettings *debugSettings = [[GADUDebugSettings alloc] init];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  cache[debugSettings.gadu_referenceKey] = debugSettings;
  return (__bridge GADUTypeDebugSettingsRef)debugSettings;
}

/// Set GADUDebugSettings DebugGeography.
void GADUSetDebugSettingsDebugGeography(
    GADUTypeDebugSettingsRef debugSettings, NSInteger debugGeography) {
  GADUDebugSettings *internalDebugSettings = (__bridge GADUDebugSettings *)debugSettings;
  internalDebugSettings.geography = debugGeography;
}

/// Set GADUDebugSettings test device IDs.
void GADUSetDebugSettingsTestDeviceIdentifiers(
    GADUTypeDebugSettingsRef debugSettings, const char **testDeviceIDs,
    NSInteger testDeviceIDLength) {
  GADUDebugSettings *internalDebugSettings =
      (__bridge GADUDebugSettings *)debugSettings;
  NSMutableArray *testDeviceIDsArray = [[NSMutableArray alloc] init];
  for (int i = 0; i < testDeviceIDLength; i++) {
    [testDeviceIDsArray addObject:GADUStringFromUTF8String(testDeviceIDs[i])];
  }
  [internalDebugSettings setTestDeviceIdentifiers:testDeviceIDsArray];
}

/// Set GADURequestParameters tag for under age of consent.
void GADUSetRequestParametersDebugSettings(
    GADUTypeRequestParametersRef requestParameters, GADUTypeDebugSettingsRef debugSettings) {
  GADURequestParameters *internalRequestParameters =
      (__bridge GADURequestParameters *)requestParameters;
  internalRequestParameters.debugSettings = (__bridge GADUDebugSettings *)debugSettings;
}

/// Create an empty ConsentInformation object (in bridge layer).
GADUTypeConsentInformationRef GADUCreateConsentInformation(
    GADUTypeConsentInformationClientRef *consentInformationClient) {
  GADUConsentInformation *internalConsentInfo =
      [[GADUConsentInformation alloc]
          initWithConsentInformationClientReference:consentInformationClient];
    GADUObjectCache *cache = [GADUObjectCache sharedInstance];
    cache[internalConsentInfo.gadu_referenceKey] = internalConsentInfo;
    return (__bridge GADUTypeConsentInformationRef)internalConsentInfo;
}

/// Update GADUConsentInformation with the RequestParameters.
void GADURequestConsentInfoUpdate(
    GADUTypeConsentInformationRef consentInformation,
    GADUTypeRequestParametersRef parameters,
    GADUConsentInfoUpdateCompletionHandler completionHandler) {
  GADUConsentInformation *internalConsentInfo =
      (__bridge GADUConsentInformation *)consentInformation;
  GADURequestParameters *internalRequestParameters =
      (__bridge GADURequestParameters *)parameters;
  [internalConsentInfo requestConsentInfoUpdateWithParameters:internalRequestParameters
                                            completionHandler:completionHandler];
}

/// Get the current consent status.
const int GADUGetConsentStatus(GADUTypeConsentInformationRef consentInformation) {
  GADUConsentInformation *internalConsentInfo =
      (__bridge GADUConsentInformation *)consentInformation;
  return internalConsentInfo.consentStatus;
}

/// Get the privacy options requirement status.
const int GADUGetPrivacyOptionsRequirementStatus(GADUTypeConsentInformationRef consentInformation) {
  GADUConsentInformation *internalConsentInfo =
      (__bridge GADUConsentInformation *)consentInformation;
  return internalConsentInfo.privacyOptionsRequirementStatus;
}

/// Check if the app has finished all the required consent flow and can request ads now.
/// A return value of true means the app can request ads now.
const bool GADUUMPCanRequestAds(GADUTypeConsentInformationRef consentInformation) {
  GADUConsentInformation *internalConsentInfo =
      (__bridge GADUConsentInformation *)consentInformation;
  return internalConsentInfo.canRequestAds;
}

/// Check if there is a GADUConsentForm available to load.
const bool GADUIsConsentFormAvailable(GADUTypeConsentInformationRef consentInformation) {
  GADUConsentInformation *internalConsentInfo =
      (__bridge GADUConsentInformation *)consentInformation;
  return [internalConsentInfo isConsentFormAvailable];
}

/// Erase / Reset GADUConsentInformation to default (unknown).
void GADUResetConsentInformation(GADUTypeConsentInformationRef consentInformation) {
  GADUConsentInformation *internalConsentInfo =
      (__bridge GADUConsentInformation *)consentInformation;
  [internalConsentInfo reset];
}

// Consent Form Methods

/// Create an empty GADUConsentForm object.
GADUTypeConsentFormRef GADUCreateConsentForm(
    GADUTypeConsentFormClientRef *consentFormClient) {
  GADUConsentForm *consentForm =
      [[GADUConsentForm alloc] initWithConsentFormClientReference:consentFormClient];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  cache[consentForm.gadu_referenceKey] = consentForm;
  return (__bridge GADUTypeConsentFormRef)consentForm;
}

/// Try loading a GADUConsentForm if required / available.
void GADULoadConsentForm(GADUTypeConsentFormRef form,
                         GADUConsentFormLoadCompletionHandler completionHandler) {
  GADUConsentForm *internalConsentForm = (__bridge GADUConsentForm *)form;
  [internalConsentForm loadFormWithCompletionHandler:completionHandler];
}

/// Present the loaded GADUConsentForm.
void GADUPresentConsentForm(GADUTypeConsentFormRef form,
                            GADUConsentFormPresentCompletionHandler completionHandler) {
  GADUConsentForm *internalConsentForm = (__bridge GADUConsentForm *)form;
  [internalConsentForm showWithCompletionHandler:completionHandler];
}

void GADULoadAndPresentConsentForm(GADUTypeConsentFormRef form,
                                   GADUConsentFormPresentCompletionHandler completionHandler) {
  GADUConsentForm *internalConsentForm = (__bridge GADUConsentForm *)form;
  [internalConsentForm loadAndPresentIfRequiredWithCompletionHandler:completionHandler];
}

void GADUPresentPrivacyOptionsForm(GADUTypeConsentFormRef form,
                                   GADUConsentFormPresentCompletionHandler completionHandler) {
  GADUConsentForm *internalConsentForm = (__bridge GADUConsentForm *)form;
  [internalConsentForm presentPrivacyOptionsFormWithCompletionHandler:completionHandler];
}
