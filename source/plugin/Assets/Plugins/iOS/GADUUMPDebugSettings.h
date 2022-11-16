// Copyright 2022 Google LLC. All Rights Reserved.

#import <Foundation/Foundation.h>

/// Debug values for testing geography.
typedef NS_ENUM(NSInteger, GADUUMPDebugGeography) {
  kGADUUMPDebugGeographyDisabled = 0,  ///< Disable geography debugging.
  kGADUUMPDebugGeographyEEA = 1,       ///< Geography appears as in EEA for debug devices.
  kGADUUMPDebugGeographyNotEEA = 2,    ///< Geography appears as not in EEA for debug devices.
};

/// Overrides settings for debugging or testing.
@interface GADUUMPDebugSettings : NSObject

/// Array of device identifier strings. Debug features are enabled for devices with these
/// identifiers. Debug features are always enabled for simulators.
@property(nonatomic, nullable) NSArray<NSString *> *testDeviceIdentifiers;

/// Debug geography.
@property(nonatomic) GADUUMPDebugGeography geography;

@end
