// Copyright 2022 Google LLC. All Rights Reserved.

#import "GADUConsentForm.h"
#import <UserMessagingPlatform/UMPConsentForm.h>

#import "GADUConsentInformation.h"
#import "GADUPluginUtil.h"
#import "UnityInterface.h"

@implementation GADUConsentForm {
  // Keep a reference to the error objects so references to Unity-level
  // FormError object are not released until the ConsentForm object is released.
  NSError *_lastLoadError;
  NSError *_lastPresentError;
  GADUTypeConsentFormClientRef *_consentFormClient;
  UMPConsentForm *_consentForm;
}

- (instancetype)initWithConsentFormClientReference:
    (GADUTypeConsentFormClientRef *)consentFormClient {
  self = [super init];
  if (self) {
    _consentFormClient = consentFormClient;
  }
  return self;
}

- (void)loadFormWithCompletionHandler:
    (GADUConsentFormLoadCompletionHandler)formLoadedCompletionHandler {
  __weak GADUConsentForm *weakSelf = self;
  [UMPConsentForm loadWithCompletionHandler: ^(UMPConsentForm *_Nullable consentForm,
      NSError *_Nullable error) {
        GADUConsentForm *strongSelf = weakSelf;
        if (!strongSelf) {
            NSLog(@"<UMP SDK Bridge> Consent form unavailable. Please restart the application and "
                  @"try again.");
            return;
        }
        strongSelf->_consentForm = consentForm;
        if (formLoadedCompletionHandler) {
          if (error) {
            strongSelf->_lastLoadError = error;
          }
          formLoadedCompletionHandler(strongSelf->_consentFormClient,
              (__bridge GADUTypeFormErrorRef)error);
        }
      }];
}

- (void)showWithCompletionHandler:
    (GADUConsentFormPresentCompletionHandler)formPresentedCompletionHandler {
  if (UMPConsentInformation.sharedInstance.formStatus != UMPFormStatusAvailable) {
    NSLog(@"<UMP SDK Bridge> Consent form unavailable. Please restart the application and try "
          @"again.");
    return;
  }
  __weak GADUConsentForm *weakSelf = self;
  UIViewController *unityController = [GADUPluginUtil unityGLViewController];
  [_consentForm presentFromViewController:unityController
    completionHandler:^(NSError *_Nullable error) {
      GADUConsentForm *strongSelf = weakSelf;
      if (!strongSelf) {
        NSLog(@"<UMP SDK Bridge> Consent form unavailable. Please restart the application and "
              @"try again.");
        return;
      }
      if (formPresentedCompletionHandler) {
        if (error) {
          strongSelf->_lastPresentError = error;
        }
        formPresentedCompletionHandler(strongSelf->_consentFormClient,
            (__bridge GADUTypeFormErrorRef)error);
      }
      strongSelf->_consentForm = nil;
    }];
}

@end
