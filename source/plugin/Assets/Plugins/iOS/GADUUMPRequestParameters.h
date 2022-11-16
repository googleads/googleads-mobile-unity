// Copyright 2022 Google LLC. All Rights Reserved.

#import <Foundation/Foundation.h>
#import "GADUUMPDebugSettings.h"

/// Parameters sent on updates to user consent info.
@interface GADUUMPRequestParameters : NSObject

/// Indicates whether the user is tagged for under age of consent.
@property(nonatomic) BOOL tagForUnderAgeOfConsent;

/// Debug settings for the request.
@property(nonatomic, nullable) GADUUMPDebugSettings *debugSettings;

@end
