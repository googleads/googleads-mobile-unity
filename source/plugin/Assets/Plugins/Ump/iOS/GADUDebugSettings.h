// Copyright 2022 Google LLC. All Rights Reserved.

#import <Foundation/Foundation.h>

/// Debug values for testing geography.
typedef NS_ENUM(NSInteger, GADUDebugGeography) {
  kGADUDebugGeographyDisabled = 0,  ///< Disable geography debugging.
  kGADUDebugGeographyEEA = 1,       ///< Geography appears as in EEA for debug devices.
  kGADUDebugGeographyNotEEA = 2,    ///< Geography appears as not in EEA for debug devices.
};

/// Settings that publishers can use for debugging or testing.
@interface GADUDebugSettings : NSObject

/// Array of device identifier strings. Debug features are enabled for devices with these
/// identifiers. Debug features are always enabled for simulators.
@property(nonatomic, nullable, copy) NSArray<NSString *> *testDeviceIdentifiers;

/// Debug geography.
@property(nonatomic) GADUDebugGeography geography;

@end
