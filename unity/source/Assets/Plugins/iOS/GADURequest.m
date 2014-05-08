// Copyright 2014 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>

#import "GADAdMobExtras.h"
#import "GADRequest.h"
#import "GADURequest.h"

@implementation GADURequest

- (id)init {
  self = [super init];
  if (self) {
    _testDevices = [[NSMutableArray alloc] init];
    _keywords = [[NSMutableArray alloc] init];
    _extras = [[NSMutableDictionary alloc] init];
  }
  return self;
}

- (void)addTestDevice:(NSString *)deviceID {
  if ([deviceID isEqualToString:GADU_SIMULATOR_ID]) {
    [self.testDevices addObject:GAD_SIMULATOR_ID];
  } else {
    [self.testDevices addObject:deviceID];
  }
}

- (void)addKeyword:(NSString *)keyword {
  [self.keywords addObject:keyword];
}

- (void)setBirthdayWithMonth:(NSInteger)month day:(NSInteger)day year:(NSInteger)year {
  NSDateComponents *components = [[[NSDateComponents alloc] init] autorelease];
  components.month = month;
  components.day = day;
  components.year = year;
  NSCalendar *gregorian = [[NSCalendar alloc] initWithCalendarIdentifier:NSGregorianCalendar];
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

- (GADRequest *)request {
  GADRequest *request = [GADRequest request];
  request.testDevices = self.testDevices;
  request.keywords = self.keywords;
  request.birthday = self.birthday;
  request.gender = self.gender;
  [request tagForChildDirectedTreatment:self.tagForChildDirectedTreatment];
  [self.extras setValue:@"1" forKey:@"unity"];
  GADAdMobExtras *extras = [[[GADAdMobExtras alloc] init] autorelease];
  extras.additionalParameters = self.extras;
  [request registerAdNetworkExtras:extras];
  return request;
}

@end
