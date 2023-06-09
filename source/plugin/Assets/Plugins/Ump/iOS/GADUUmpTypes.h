// Copyright 2022 Google LLC. All Rights Reserved.

#import <Foundation/Foundation.h>

/// Type representing a UMPConsentInformation type.
typedef const void *GADUTypeConsentInformationRef;

/// Type representing a UMPConsentInformationClient type.
typedef const void *GADUTypeConsentInformationClientRef;

/// Type representing a UMPRequestParameters type.
typedef const void *GADUTypeRequestParametersRef;

/// Type representing a UMPDebugSettings type.
typedef const void *GADUTypeDebugSettingsRef;

/// Type representing a UMPDebugGeography type.
typedef const void *GADUTypeDebugGeographyRef;

/// Type representing a Error type
typedef const void *GADUTypeErrorRef;

/// Type representing a FormError type.
typedef const void *GADUTypeFormErrorRef;

/// Type representing a UMPConsentForm type.
typedef const void *GADUTypeConsentFormRef;

/// Type representing a GADUTypeConsentFormClient type.
typedef const void *GADUTypeConsentFormClientRef;

/// CompletionHandler when Consent Information is updated.
typedef void (*GADUConsentInfoUpdateCompletionHandler)(
    GADUTypeConsentInformationClientRef *clientRef, const char *error);

/// CompletionHandler when Consent Form is loaded.
typedef void (*GADUConsentFormLoadCompletionHandler)(
    GADUTypeConsentFormClientRef *clientRef, const char *error);

/// CompletionHandler when Consent Form is presented.
typedef void (*GADUConsentFormPresentCompletionHandler)(
    GADUTypeConsentFormClientRef *clientRef, const char *error);
