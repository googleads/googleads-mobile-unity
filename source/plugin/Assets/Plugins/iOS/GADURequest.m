// Copyright 2014 Google Inc. All Rights Reserved.

#import "GADURequest.h"

@implementation GADURequest

- (id)init {
  self = [super init];
  if (self) {
    _testDevices = [[NSMutableArray alloc] init];
    _keywords = [[NSMutableArray alloc] init];
    _extras = [[NSMutableDictionary alloc] init];
    _mediationExtras = [[NSMutableArray alloc] init];
  }
  return self;
}

- (void)addTestDevice:(NSString *)deviceID {
  if ([deviceID isEqualToString:@"SIMULATOR"]) {
    [self.testDevices addObject:kGADSimulatorID];
  } else {
    [self.testDevices addObject:deviceID];
  }
}

- (void)addKeyword:(NSString *)keyword {
  [self.keywords addObject:keyword];
}

- (void)setBirthdayWithMonth:(NSInteger)month day:(NSInteger)day year:(NSInteger)year {
  NSDateComponents *components = [[NSDateComponents alloc] init];
  components.month = month;
  components.day = day;
  components.year = year;
  NSCalendar *gregorian =
      [[NSCalendar alloc] initWithCalendarIdentifier:NSCalendarIdentifierGregorian];
  self.birthday = [gregorian dateFromComponents:components];
}

- (void)setGenderWithCode:(GADUGender)gender {
  switch (gender) {
    case kGADUGenderMale:
      self.gender = kGADGenderMale;
      break;
    case kGADUGenderFemale:
      self.gender = kGADGenderFemale;
      break;
    default:
      self.gender = kGADGenderUnknown;
  }
}

- (void)setExtraWithKey:(NSString *)key value:(NSString *)value {
  [self.extras setValue:value forKey:key];
}

- (void)setMediationExtras:(id<GADAdNetworkExtras>)mediationExtras {
  [_mediationExtras addObject:mediationExtras];
}

- (GADRequest *)request {
  GADRequest *request = [GADRequest request];
  request.testDevices = self.testDevices;
  request.keywords = self.keywords;
  request.birthday = self.birthday;
  request.gender = self.gender;
  request.requestAgent = self.requestAgent;
  [request tagForChildDirectedTreatment:self.tagForChildDirectedTreatment];
  GADExtras *extras = [[GADExtras alloc] init];
  extras.additionalParameters = self.extras;
  [request registerAdNetworkExtras:extras];

  for (id<GADAdNetworkExtras> mediationExtras in self.mediationExtras) {
    [request registerAdNetworkExtras:mediationExtras];
  }
  return request;
}

@end
