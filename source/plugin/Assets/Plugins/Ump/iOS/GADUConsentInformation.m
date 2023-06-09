// Copyright 2022 Google LLC. All Rights Reserved.

#import "GADUConsentInformation.h"
#import <UserMessagingPlatform/UMPConsentInformation.h>

#import "GADUDebugSettings.h"
#import "GADUPluginUtil.h"
#import "GADURequestParameters.h"
#import "UnityInterface.h"

@implementation GADUConsentInformation {
  // Keep a reference to the error objects so references to Unity-level
  // FormError object are not released until the ConsentInformation object is released.
  NSError *_lastUpdateError;
  GADUTypeConsentInformationClientRef *_consentInformationClient;
}

- (instancetype)initWithConsentInformationClientReference:
    (GADUTypeConsentInformationClientRef *)consentInformationClient {
  self = [super init];
  if (self) {
    _consentInformationClient = consentInformationClient;
  }
  return self;
}

- (GADUConsentStatus)consentStatus {
  return (GADUConsentStatus)UMPConsentInformation.sharedInstance.consentStatus;
}

- (BOOL)isConsentFormAvailable {
  return UMPConsentInformation.sharedInstance.formStatus == kGADUFormStatusAvailable;
}

- (void)requestConsentInfoUpdateWithParameters: (GADURequestParameters *)bridgeParams
    completionHandler:(GADUConsentInfoUpdateCompletionHandler)consentInfoUpdateCompletionHandler
    {
  __weak GADUConsentInformation *weakSelf = self;
  UMPRequestParameters *parameters = [[UMPRequestParameters alloc] init];
  parameters.tagForUnderAgeOfConsent = bridgeParams.tagForUnderAgeOfConsent;
  UMPDebugSettings *debugSettings = [[UMPDebugSettings alloc] init];
  debugSettings.geography = (NSInteger)bridgeParams.debugSettings.geography;
  debugSettings.testDeviceIdentifiers = bridgeParams.debugSettings.testDeviceIdentifiers;
  parameters.debugSettings = debugSettings;
  [UMPConsentInformation.sharedInstance requestConsentInfoUpdateWithParameters:parameters
    completionHandler:^(NSError *_Nullable error) {
      GADUConsentInformation *strongSelf = weakSelf;
      if (!strongSelf) {
        NSLog(@"<UMP SDK Bridge> Consent information unavailable. Please restart the application "
              @"and try again.");
        return;
      }
      if (consentInfoUpdateCompletionHandler) {
        if (error) {
          strongSelf->_lastUpdateError = error;
        }
        consentInfoUpdateCompletionHandler(strongSelf->_consentInformationClient,
            (__bridge GADUTypeFormErrorRef)error);
      }
  }];
}

- (void)reset {
  [UMPConsentInformation.sharedInstance reset];
}

@end
