// Copyright 2022 Google LLC. All Rights Reserved.

#import "GADUConsentInformation.h"
#import <UserMessagingPlatform/UMPConsentInformation.h>

#import "GADUDebugSettings.h"
#import "GADUDispatch.h"
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
  __block GADUConsentStatus status;
  if (NSThread.isMainThread) {
    status = (GADUConsentStatus)UMPConsentInformation.sharedInstance.consentStatus;
  } else {
    dispatch_sync(dispatch_get_main_queue(), ^{
      status = (GADUConsentStatus)UMPConsentInformation.sharedInstance.consentStatus;
    });
  }
  return status;
}

- (BOOL)canRequestAds {
  __block BOOL status;
  if (NSThread.isMainThread) {
    status = UMPConsentInformation.sharedInstance.canRequestAds;
  } else {
    dispatch_sync(dispatch_get_main_queue(), ^{
      status = UMPConsentInformation.sharedInstance.canRequestAds;
    });
  }
  return status;
}

- (GADUPrivacyOptionsRequirementStatus)privacyOptionsRequirementStatus {
  __block GADUPrivacyOptionsRequirementStatus status;
  if (NSThread.isMainThread) {
    status =
        (GADUPrivacyOptionsRequirementStatus)[UMPConsentInformation
                                                  .sharedInstance privacyOptionsRequirementStatus];
  } else {
    dispatch_sync(dispatch_get_main_queue(), ^{
      status = (GADUPrivacyOptionsRequirementStatus)[UMPConsentInformation.sharedInstance
                                                         privacyOptionsRequirementStatus];
    });
  }
  return status;
}

- (BOOL)isConsentFormAvailable {
  __block BOOL status;
  if (NSThread.isMainThread) {
    status = UMPConsentInformation.sharedInstance.formStatus == kGADUFormStatusAvailable;
  } else {
    dispatch_sync(dispatch_get_main_queue(), ^{
      status = UMPConsentInformation.sharedInstance.formStatus == kGADUFormStatusAvailable;
    });
  }
  return status;
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
  GADUDispatchAsyncSafeMainQueue(^{
    [UMPConsentInformation.sharedInstance
        requestConsentInfoUpdateWithParameters:parameters
                             completionHandler:^(NSError *_Nullable error) {
                               GADUConsentInformation *strongSelf = weakSelf;
                               if (!strongSelf) {
                                 NSLog(@"Consent information unavailable. Please restart the "
                                       @"application and try again.");
                                 return;
                               }
                               if (consentInfoUpdateCompletionHandler) {
                                 strongSelf->_lastUpdateError = error;
                                 consentInfoUpdateCompletionHandler(
                                     strongSelf->_consentInformationClient,
                                     (__bridge GADUTypeFormErrorRef)error);
                               }
                             }];
  });
}

- (void)reset {
  GADUDispatchAsyncSafeMainQueue(^{
    [UMPConsentInformation.sharedInstance reset];
  });
}

@end
