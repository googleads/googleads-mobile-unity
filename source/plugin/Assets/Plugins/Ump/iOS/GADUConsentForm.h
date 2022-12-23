// Copyright 2022 Google LLC. All Rights Reserved.

#import <Foundation/Foundation.h>
#import <UserMessagingPlatform/UMPConsentForm.h>
#import "GADUUmpTypes.h"

/// A rendered form for collecting consent from a user.
@interface GADUConsentForm : NSObject

/// Initializes a consent form.
- (instancetype)initWithConsentFormClientReference:
    (GADUTypeConsentFormClientRef *)consentFormClient;

/// Loads a consent form and calls formLoadedCompletionHandler on completion.
- (void)loadFormWithCompletionHandler:
    (GADUConsentFormLoadCompletionHandler)formLoadedCompletionHandler;

/// Presents the full screen consent form over the Unity controller.
/// UMPConsentInformation.sharedInstance.consentStatus is updated, the
/// formPresentedCompletionHandler is called and the form is dismissed when the user taps the
/// consent button.
- (void)showWithCompletionHandler:
    (GADUConsentFormPresentCompletionHandler)formPresentedCompletionHandler;

@end
