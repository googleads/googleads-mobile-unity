#import <Foundation/Foundation.h>
#import <GoogleMobileAds/GoogleMobileAds.h>
typedef NS_ENUM(NSInteger, GADURequestConfigurationTagForChildDirectedTreatment) {
  kGADURequestConfigurationTagForChildDirectedTreatmentTrue = 0,          // true.
  kGADURequestConfigurationTagForChildDirectedTreatmentFalse = 1,         /// false
  kGADURequestConfigurationTagForChildDirectedTreatmentUnspecified = -1,  // Unspecified
};

typedef NS_ENUM(NSInteger, GADURequestConfigurationTagForUnderAgeOfConsent) {
  kGADURequestConfigurationTagForUnderAgeOfConsentTrue = 0,          // true.
  kGADURequestConfigurationTagForUnderAgeOfConsentFalse = 1,         /// false
  kGADURequestConfigurationTagForUnderAgeOfConsentUnspecified = -1,  // Unspecified
};

@interface GADURequestConfiguration : NSObject

@property(nonatomic, copy, nullable) GADMaxAdContentRating maxAdContentRating;

@property(nonatomic, copy, nullable) NSArray<NSString *> *testDeviceIdentifiers;

@property(nonatomic, assign)
    GADURequestConfigurationTagForChildDirectedTreatment tagForChildDirectedTreatment;

@property(nonatomic, assign)
    GADURequestConfigurationTagForUnderAgeOfConsent tagForUnderAgeOfConsent;

@end
