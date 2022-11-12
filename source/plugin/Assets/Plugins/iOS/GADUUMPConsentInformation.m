// Copyright 2022 Google LLC. All Rights Reserved.

#import <UserMessagingPlatform/UMPConsentInformation.h>
#import "GADUPluginUtil.h"
#import "GADUUMPConsentInformation.h"
#import "GADUUMPDebugSettings.h"
#import "GADUUMPRequestParameters.h"
#import "UnityInterface.h"

@interface GADUUMPConsentInformation ()
@end

@implementation GADUUMPConsentInformation {
  // Keep a reference to the error objects so references to Unity-level
  // FormError object are not released until the ConsentInformation object is released.
  NSError *_lastUpdateError;
}

- (instancetype)initWithConsentInformationClientReference:
    (GADUTypeUMPConsentInformationClientRef *)consentInformationClient {
  self = [super init];
  _consentInformationClient = consentInformationClient;
  return self;
}

/**
 * Requests consent information update. Must be called before loading a consent form.
 *
 * @param bridgeParams UMPRequestParameters.
 */
- (void)requestConsentInfoUpdateWithParameters: (GADUUMPRequestParameters *)bridgeParams {
  __weak GADUUMPConsentInformation *weakSelf = self;
  UMPRequestParameters *parameters = [[UMPRequestParameters alloc] init];
  parameters.tagForUnderAgeOfConsent = bridgeParams.tagForUnderAgeOfConsent;
  UMPDebugSettings *debugSettings = [[UMPDebugSettings alloc] init];
  debugSettings.geography = (int)bridgeParams.debugSettings.geography;
  debugSettings.testDeviceIdentifiers = bridgeParams.debugSettings.testDeviceIdentifiers;
  parameters.debugSettings = debugSettings;
  [UMPConsentInformation.sharedInstance requestConsentInfoUpdateWithParameters:parameters
      completionHandler:^(NSError *_Nullable error) {
        GADUUMPConsentInformation *strongSelf = weakSelf;
        if (strongSelf.consentInfoUpdateCallback) {
          if (error) {
            _lastUpdateError = error;
          }
          strongSelf.consentInfoUpdateCallback(strongSelf.consentInformationClient,
          (__bridge GADUTypeFormErrorRef)error);
        }
      }];
}

/**
 * The user's consent status. This value is cached between app sessions and can be read before
 * requesting updated parameters.
 *
 * @return Consent status.
 */
- (GADUUMPConsentStatus)getConsentStatus {
    GADUUMPConsentStatus status = (int)UMPConsentInformation.sharedInstance.consentStatus;
    switch(status) {
        case 1:
        return 2;
        case 2:
        return 1;
        case 0:
        case 3:
        default:
        return status;
    }
}

/**
 * Consent form status. This value defaults to UMPFormStatusUnknown and requires a call to
 * requestConsentInfoUpdateWithParameters:completionHandler to update.
 *
 * @return true if consent form is available, false otherwise.
 */
- (bool)isConsentFormAvailable {
    return UMPConsentInformation.sharedInstance.formStatus == UMPFormStatusAvailable;
}

/**
 * Clears all consent state from persistent storage.
 */
- (void)reset {
    [UMPConsentInformation.sharedInstance reset];
}

@end
