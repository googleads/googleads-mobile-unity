// Copyright 2022 Google LLC. All Rights Reserved.
#import <UserMessagingPlatform/UMPConsentInformation.h>

#import <Foundation/Foundation.h>
#import "GADURequestParameters.h"
#import "GADUUmpTypes.h"

/// Utility methods for collecting consent from users.
@interface GADUConsentInformation : NSObject

/// The user's consent status. This value is cached between app sessions and can be read before
/// requesting updated parameters.
@property(readonly, nonatomic) UMPConsentStatus consentStatus;

/// Check if the app has finished all the required consent flow and can request ads now.
/// A return value of true means the app can request ads now.
@property(nonatomic, readonly) BOOL canRequestAds;

/// The privacy options requirement status.
@property(nonatomic, readonly) UMPPrivacyOptionsRequirementStatus privacyOptionsRequirementStatus;

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
