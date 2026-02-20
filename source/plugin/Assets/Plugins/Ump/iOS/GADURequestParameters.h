// Copyright 2022 Google LLC. All Rights Reserved.

#import <Foundation/Foundation.h>
#import "GADUDebugSettings.h"

/// Parameters sent on updates to user consent info.
@interface GADURequestParameters : NSObject

/// Indicates whether the user is tagged for under age of consent.
@property(nonatomic) BOOL tagForUnderAgeOfConsent;

/// Consent sync ID for the request.
@property(nonatomic, nullable) NSString *consentSyncID;

/// Debug settings for the request.
@property(nonatomic, nullable) GADUDebugSettings *debugSettings;

@end
