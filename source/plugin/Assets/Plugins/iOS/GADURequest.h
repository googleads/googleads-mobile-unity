// Copyright 2014 Google Inc. All Rights Reserved.

@import Foundation;
@import GoogleMobileAds;

/// Genders to help deliver more relevant ads.
typedef NS_ENUM(NSInteger, GADUGender) {
  kGADUGenderUnknown = 0,  ///< Unknown.
  kGADUGenderMale = 1,     ///< Male.
  kGADUGenderFemale = 2    ///< Female.
};

// Specifies optional parameters for ad requests.
@interface GADURequest : NSObject

/// Returns an initialized GADURequest object.
- (id)init;

/// An array of device identifiers to receive test ads.
@property(nonatomic, strong) NSMutableArray *testDevices;

/// Words or phrase describing the current activity of the user.
@property(nonatomic, strong) NSMutableArray *keywords;

/// The user's birthday may be used to deliver more relevant ads.
@property(nonatomic, strong) NSDate *birthday;

/// The user's gender may be used to deliver more relevant ads.
@property(nonatomic, assign) GADGender *gender;

/// [Optional] This method allows you to specify whether you would like your app to be treated as
/// child-directed for purposes of the Children’s Online Privacy Protection Act (COPPA) -
/// http://business.ftc.gov/privacy-and-security/childrens-privacy.
@property(nonatomic, assign) BOOL tagForChildDirectedTreatment;

/// Extra parameters to be sent up in the ad request.
@property(nonatomic, strong) NSMutableDictionary *extras;

/// Convenience method for adding a single test device.
- (void)addTestDevice:(NSString *)deviceID;

/// Convenience method for adding a single keyword.
- (void)addKeyword:(NSString *)keyword;

/// Convenience method for setting the user's birthday.
- (void)setBirthdayWithMonth:(NSInteger)month day:(NSInteger)day year:(NSInteger)year;

/// Convenience method for setting the user's birthday with a GADUGender.
- (void)setGenderWithCode:(GADUGender)gender;

/// Convenience method for setting an extra parameters.
- (void)setExtraWithKey:(NSString *)key value:(NSString *)value;

/// Constructs a GADRequest with the defined targeting values.
- (GADRequest *)request;

@end
