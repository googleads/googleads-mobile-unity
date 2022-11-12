// Copyright 2022 Google LLC. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>
#import <UserMessagingPlatform/UMPConsentForm.h>

#import "GADUTypes.h"

@interface GADUUMPConsentForm : NSObject

/// Initializes a ConsentForm.
- (instancetype)initWithConsentFormClientReference:(GADUTypeUMPConsentFormClientRef *)consentFormClient;

/// The Consent Form.
@property(nonatomic, strong) UMPConsentForm *consentForm;

/// A reference to the Unity Consent Form client.
@property(nonatomic, assign) GADUTypeUMPConsentFormClientRef *consentFormClient;

/// The ad loaded callback into Unity.
@property(nonatomic, assign) GADUUMPConsentFormLoadCompleteCallback formLoadedCallback;

/// The form presented callback into Unity.
@property(nonatomic, assign)
    GADUUMPConsentFormPresentCompleteCallback formPresentedCallback;

/// Loads a Consent Form.
- (void)loadForm;

/// Shows the Consent Form.
- (void)show;

@end
