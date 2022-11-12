// Copyright 2022 Google LLC. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADUTypes.h"
#import "GADUUMPRequestParameters.h"

/// Consent status values.
typedef NS_ENUM(NSInteger, GADUUMPConsentStatus) {
  kGADUUMPConsentStatusUnknown = 0,      ///< Unknown consent status.
  kGADUUMPConsentStatusNotRequired = 1,  ///< Consent not required.
  kGADUUMPConsentStatusRequired = 2,     ///< User consent required but not yet obtained.
  kGADUUMPConsentStatusObtained =
      3,  ///< User consent obtained, personalized vs non-personalized undefined.
};

/// State values for whether the user has a consent form available to them. To check whether form
/// status has changed, an update can be requested through
/// requestConsentInfoUpdateWithParameters:completionHandler.
typedef NS_ENUM(NSInteger, GADUUMPFormStatus) {
  /// Whether a consent form is available is unknown. An update should be requested using
  /// requestConsentInfoUpdateWithParameters:completionHandler.
  kGADUUMPFormStatusUnknown = 0,

  /// Consent forms are available and can be loaded using [UMPConsentForm
  /// loadWithCompletionHandler:]
  kGADUUMPFormStatusAvailable = 1,

  /// Consent forms are unavailable. Showing a consent form is not required.
  kGADUUMPFormStatusUnavailable = 2,
};

@interface GADUUMPConsentInformation : NSObject

/// Initializes a ConsentInformation.
- (instancetype)initWithConsentInformationClientReference:
    (GADUTypeUMPConsentInformationClientRef *)consentInformationClient;

/// The Consent Form.
@property(nonatomic, strong) GADUUMPConsentInformation *consentInformation;

/// A reference to the Unity Consent Form client.
@property(nonatomic, assign) GADUTypeUMPConsentInformationClientRef *consentInformationClient;

/// The ad loaded callback into Unity.
@property(nonatomic, assign) GADUUMPConsentInfoUpdateCallback consentInfoUpdateCallback;

/// Requests consent information update. Must be called before loading a consent form.
- (void)requestConsentInfoUpdateWithParameters: (GADUUMPRequestParameters *)parameters;

/// The user's consent status. This value is cached between app sessions and can be read before
/// requesting updated parameters.
- (GADUUMPConsentStatus)getConsentStatus;

/// Consent form status. This value defaults to UMPFormStatusUnknown and requires a call to
/// requestConsentInfoUpdateWithParameters:completionHandler to update.
- (bool)isConsentFormAvailable;

/// Clears all consent state from persistent storage.
- (void)reset;

@end
