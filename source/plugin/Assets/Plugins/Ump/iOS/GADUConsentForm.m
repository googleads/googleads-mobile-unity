// Copyright 2022 Google LLC. All Rights Reserved.

#import "GADUConsentForm.h"
#import <UserMessagingPlatform/UMPConsentForm.h>

#import "GADUConsentInformation.h"
#import "GADUDispatch.h"
#import "GADUPluginUtil.h"
#import "GADUUMPError.h"
#import "UnityInterface.h"

@implementation GADUConsentForm {
  // Keep a reference to the error objects so references to Unity-level
  // FormError object are not released until the ConsentForm object is released.
  NSError *_lastLoadError;
  NSError *_lastPresentError;
  GADUTypeConsentFormClientRef *_consentFormClient;
  UMPConsentForm *_consentForm;
}

- (nullable instancetype)initWithConsentFormClientReference:
    (GADUTypeConsentFormClientRef *)consentFormClient {
  self = [super init];
  if (self) {
    _consentFormClient = consentFormClient;
  }
  return self;
}

- (void)loadFormWithCompletionHandler:
    (GADUConsentFormLoadCompletionHandler)formLoadedCompletionHandler {
  GADUDispatchAsyncSafeMainQueue(^{
    __weak GADUConsentForm *weakSelf = self;
    [UMPConsentForm loadWithCompletionHandler:^(UMPConsentForm *_Nullable consentForm,
                                                NSError *_Nullable error) {
      GADUConsentForm *strongSelf = weakSelf;
      if (!strongSelf) {
        NSLog(@"Lost reference to the consent form. Please restart the application and try again.");
        return;
      }
      strongSelf->_consentForm = consentForm;
      if (formLoadedCompletionHandler) {
        strongSelf->_lastLoadError = error;
        dispatch_async(dispatch_get_main_queue(), ^{
          formLoadedCompletionHandler(strongSelf->_consentFormClient,
                                      (__bridge GADUTypeFormErrorRef)error);
        });
      }
    }];
  });
}

- (void)showWithCompletionHandler:
    (GADUConsentFormPresentCompletionHandler)formPresentedCompletionHandler {
  GADUDispatchAsyncSafeMainQueue(^{
    if (UMPConsentInformation.sharedInstance.formStatus != UMPFormStatusAvailable) {
      NSString *const description =
          @"Consent form unavailable. Please restart the application and try again.";
      if (formPresentedCompletionHandler) {
        NSLog(description);
        NSError *error =
            GADUUMPErrorWithCodeAndDescription(GADUUMPPluginErrorCodeUnavailable, description);
        dispatch_async(dispatch_get_main_queue(), ^{
          formPresentedCompletionHandler(self->_consentFormClient,
                                         (__bridge GADUTypeFormErrorRef)error);
        });
      }
      return;
    }
    UIViewController *unityController = [GADUPluginUtil unityGLViewController];
    if (!unityController) {
      NSString *const description = @"View controller not available.";
      NSLog(description);
      if (formPresentedCompletionHandler) {
        NSError *error = GADUUMPErrorWithCodeAndDescription(
            GADUUMPPluginErrorCodeInvalidViewController, description);
        dispatch_async(dispatch_get_main_queue(), ^{
          formPresentedCompletionHandler(self->_consentFormClient,
                                         (__bridge GADUTypeFormErrorRef)error);
        });
      }
      return;
    }
    __weak GADUConsentForm *weakSelf = self;
    [_consentForm presentFromViewController:unityController
                          completionHandler:^(NSError *_Nullable error) {
                            GADUConsentForm *strongSelf = weakSelf;
                            if (!strongSelf) {
                              NSLog(@"Lost reference to the consent form. Please restart the "
                                    @"application and try again.");
                              return;
                            }
                            if (formPresentedCompletionHandler) {
                              strongSelf->_lastPresentError = error;
                              formPresentedCompletionHandler(strongSelf->_consentFormClient,
                                                             (__bridge GADUTypeFormErrorRef)error);
                            }
                            strongSelf->_consentForm = nil;
                          }];
  });
}

- (void)loadAndPresentIfRequiredWithCompletionHandler:
    (nullable GADUConsentFormPresentCompletionHandler)completionHandler {
  __weak GADUConsentForm *weakSelf = self;
  GADUDispatchAsyncSafeMainQueue(^{
    UIViewController *unityController = [GADUPluginUtil unityGLViewController];
    if (!unityController) {
      NSString *const description = @"View controller not available.";
      NSLog(description);
      if (completionHandler) {
        NSError *error = GADUUMPErrorWithCodeAndDescription(
            GADUUMPPluginErrorCodeInvalidViewController, description);
        dispatch_async(dispatch_get_main_queue(), ^{
          completionHandler(self->_consentFormClient, (__bridge GADUTypeFormErrorRef)error);
        });
      }
      return;
    }
    [UMPConsentForm
        loadAndPresentIfRequiredFromViewController:unityController
                                 completionHandler:^(NSError *_Nullable error) {
                                   GADUConsentForm *strongSelf = weakSelf;
                                   if (!strongSelf) {
                                     NSLog(@"Lost reference to the consent form. Please restart the"
                                           @" application and try again.");
                                     return;
                                   }
                                   if (completionHandler) {
                                     strongSelf->_lastPresentError = error;
                                     completionHandler(strongSelf->_consentFormClient,
                                                       (__bridge GADUTypeFormErrorRef)error);
                                   }
                                   strongSelf->_consentForm = nil;
                                 }];
  });
}

- (void)presentPrivacyOptionsFormWithCompletionHandler:
    (nullable GADUConsentFormPresentCompletionHandler)completionHandler {
  __weak GADUConsentForm *weakSelf = self;
  GADUDispatchAsyncSafeMainQueue(^{
    UIViewController *unityController = [GADUPluginUtil unityGLViewController];
    if (!unityController) {
      NSString *const description = @"View controller not available.";
      NSLog(description);
      if (completionHandler) {
        NSError *error = GADUUMPErrorWithCodeAndDescription(
            GADUUMPPluginErrorCodeInvalidViewController, description);
        dispatch_async(dispatch_get_main_queue(), ^{
          completionHandler(self->_consentFormClient, (__bridge GADUTypeFormErrorRef)error);
        });
      }
      return;
    }
    [UMPConsentForm
        presentPrivacyOptionsFormFromViewController:unityController
                                  completionHandler:^(NSError *_Nullable error) {
                                    GADUConsentForm *strongSelf = weakSelf;
                                    if (!strongSelf) {
                                      NSLog(@"Lost reference to the consent form. Please restart "
                                            @"the application and try again.");
                                      return;
                                    }
                                    if (completionHandler) {
                                      strongSelf->_lastPresentError = error;
                                      completionHandler(strongSelf->_consentFormClient,
                                                        (__bridge GADUTypeFormErrorRef)error);
                                    }
                                    strongSelf->_consentForm = nil;
                                  }];
  });
}

@end
