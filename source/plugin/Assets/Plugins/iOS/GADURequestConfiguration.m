#import "GADURequestConfiguration.h"
@implementation GADURequestConfiguration
- (instancetype)init {
  self = [super init];
  if (self) {
    _tagForUnderAgeOfConsent = kGADURequestConfigurationTagForUnderAgeOfConsentUnspecified;
    _tagForChildDirectedTreatment =
        kGADURequestConfigurationTagForChildDirectedTreatmentUnspecified;
  }
  return self;
}
@end
