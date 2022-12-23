// Copyright 2022 Google LLC. All Rights Reserved.

#import <Foundation/Foundation.h>
#import "GADURequestParameters.h"
#import "GADUUmpTypes.h"

/// Consent status values.
typedef NS_ENUM(NSInteger, GADUConsentStatus) {
  kGADUConsentStatusUnknown = 0,      ///< Unknown consent status.
  kGADUConsentStatusNotRequired = 1,  ///< Consent not required.
  kGADUConsentStatusRequired = 2,     ///< User consent required but not yet obtained.
  kGADUConsentStatusObtained =
      3,  ///< User consent obtained, personalized vs non-personalized undefined.
};

/// State values for whether the user has a consent form available to them. To check whether form
/// status has changed, an update can be requested through
/// requestConsentInfoUpdateWithParameters:completionHandler.
typedef NS_ENUM(NSInteger, GADUFormStatus) {
  /// Whether a consent form is available is unknown. An update should be requested using
  /// requestConsentInfoUpdateWithParameters:completionHandler.
  kGADUFormStatusUnknown = 0,

  /// Consent forms are available and can be loaded using [UMPConsentForm
  /// loadWithCompletionHandler:].
  kGADUFormStatusAvailable = 1,

  /// Consent forms are unavailable. Showing a consent form is not required.
  kGADUFormStatusUnavailable = 2,
};

/// Utility methods for collecting consent from users.
@interface GADUConsentInformation : NSObject

/// The user's consent status. This value is cached between app sessions and can be read before
/// requesting updated parameters.
@property(readonly, nonatomic) GADUConsentStatus consentStatus;

/// YES if consent form is available, NO otherwise. An update should be requested using
/// requestConsentInfoUpdateWithParameters.
@property(readonly, nonatomic) BOOL isConsentFormAvailable;

/// Initializes a ConsentInformation.
- (instancetype)initWithConsentInformationClientReference:
    (GADUTypeConsentInformationClientRef *)consentInformationClient;

/// Requests consent information update. Must be called before loading a consent form.
/// @param bridgeParams UMPRequestParameters.
- (void)requestConsentInfoUpdateWithParameters:(GADURequestParameters *)bridgeParams
                             completionHandler:(GADUConsentInfoUpdateCompletionHandler)
                                                   consentInfoUpdateCompletionHandler;

/// Clears all consent state from persistent storage.
- (void)reset;

@end
