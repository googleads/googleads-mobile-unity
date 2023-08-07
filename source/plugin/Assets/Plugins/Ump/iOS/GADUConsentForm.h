// Copyright 2022 Google LLC. All Rights Reserved.

#import <Foundation/Foundation.h>
#import <UserMessagingPlatform/UMPConsentForm.h>
#import "GADUUmpTypes.h"

/// A rendered form for collecting consent from a user.
@interface GADUConsentForm : NSObject

/// Initializes a consent form.
- (nullable instancetype)initWithConsentFormClientReference:
    (_Nonnull GADUTypeConsentFormClientRef *_Nonnull)consentFormClient;

/// Loads a consent form and calls formLoadedCompletionHandler on completion.
- (void)loadFormWithCompletionHandler:
    (nonnull GADUConsentFormLoadCompletionHandler)formLoadedCompletionHandler;

/// Presents the full screen consent form over the Unity controller.
/// UMPConsentInformation.sharedInstance.consentStatus is updated, the
/// formPresentedCompletionHandler is called and the form is dismissed when the user taps the
/// consent button.
- (void)showWithCompletionHandler:
    (nullable GADUConsentFormPresentCompletionHandler)formPresentedCompletionHandler;

/// Loads a consent form and immediately presents it from the provided viewController if
/// UMPConsentInformation.sharedInstance.consentStatus is UMPConsentStatusRequired. Calls
/// completionHandler after the user selects an option and the form is dismissed, or on the next run
/// loop if no form is presented. Must be called on the main queue.
- (void)loadAndPresentIfRequiredWithCompletionHandler:
    (nullable GADUConsentFormPresentCompletionHandler)completionHandler;

/// Presents a privacy options form from the provided viewController if
/// UMPConsentInformation.sharedInstance.privacyOptionsRequirementStatus is
/// UMPPrivacyOptionsRequirementStatusRequired. Calls completionHandler with nil error after the
/// user selects an option and the form is dismissed, or on the next run loop with a non-nil error
/// if no form is presented. Must be called on the main queue.
///
/// This method should only be called in response to a user input to request a privacy options form
/// to be shown. The privacy options form is preloaded by the SDK automatically when a form becomes
/// available. If no form is preloaded, the SDK will invoke the completionHandler on the next run
/// loop, but will asynchronously retry to load one.
- (void)presentPrivacyOptionsFormWithCompletionHandler:
    (nullable GADUConsentFormPresentCompletionHandler)completionHandler;

@end
