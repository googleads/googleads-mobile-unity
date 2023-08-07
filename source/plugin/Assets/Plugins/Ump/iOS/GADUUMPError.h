// Copyright 2023 Google LLC. All Rights Reserved.

#import <Foundation/Foundation.h>

/// Error domain for UMP Unty SDK errors.
extern NSErrorDomain _Nonnull const GADUUMPErrorDomain;

/// Returns an NSError with the plugin consent domain and provided description.
NSError *_Nonnull GADUUMPErrorWithCodeAndDescription(NSInteger code,
                                                     NSString *_Nonnull description);

/// Error codes used when loading and showing forms.
typedef NS_ENUM(NSInteger, GADUUMPPluginErrorCode) {
  GADUUMPPluginErrorCodeInternal = 105,     ///< Internal error.
  GADUUMPPluginErrorCodeAlreadyUsed = 106,  ///< Form was already used.
  GADUUMPPluginErrorCodeUnavailable = 107,  ///< Form is unavailable.
  GADUUMPPluginErrorCodeTimeout = 108,      ///< Loading a form timed out.
  GADUUMPPluginErrorCodeInvalidViewController =
      109,  ///< Form cannot be presented from the provided view controller.
};
