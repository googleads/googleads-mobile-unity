// Copyright 2022 Google LLC. All Rights Reserved.

#import <UserMessagingPlatform/UMPConsentForm.h>
#import "GADUPluginUtil.h"
#import "GADUUMPConsentForm.h"
#import "GADUUMPConsentInformation.h"
#import "UnityInterface.h"

@interface GADUUMPConsentForm ()
@end

@implementation GADUUMPConsentForm {
  // Keep a reference to the error objects so references to Unity-level
  // FormError object are not released until the ConsentForm object is released.
  NSError *_lastLoadError;
  NSError *_lastPresentError;
}

- (instancetype)initWithConsentFormClientReference:
    (GADUTypeUMPConsentFormClientRef *)consentFormClient {
  self = [super init];
  _consentFormClient = consentFormClient;
  return self;
}

/**
 * Loads a consent form and calls completionHandler on completion.
 */
- (void)loadForm {
  __weak GADUUMPConsentForm *weakSelf = self;
  [UMPConsentForm loadWithCompletionHandler: ^(UMPConsentForm *_Nullable consentForm,
      NSError *_Nullable error) {
        GADUUMPConsentForm *strongSelf = weakSelf;
        strongSelf.consentForm = consentForm;
        if (strongSelf.formLoadedCallback) {
          if (error) {
            _lastLoadError = error;
          }
          strongSelf.formLoadedCallback(strongSelf.consentFormClient,
              (__bridge GADUTypeFormErrorRef)error);
        }
      }];
}

/**
 * Presents the full screen consent form over the Unity controller.
 * UMPConsentInformation.sharedInstance.consentStatus is updated, the completionHandler is called
 * and the form is dismissed when the user taps the consent button.
 */
- (void)show {
  __weak GADUUMPConsentForm *weakSelf = self;
  if(UMPConsentInformation.sharedInstance.formStatus == UMPFormStatusAvailable) {
    UIViewController *unityController = [GADUPluginUtil unityGLViewController];
    [self.consentForm presentFromViewController:unityController
        completionHandler:^(NSError *_Nullable error) {
          GADUUMPConsentForm *strongSelf = weakSelf;
          if (strongSelf.formPresentedCallback) {
            if (error) {
              _lastPresentError = error;
            }
            strongSelf.formPresentedCallback(strongSelf.consentFormClient,
                (__bridge GADUTypeFormErrorRef)error);
          }
        }];
  }
}

@end
