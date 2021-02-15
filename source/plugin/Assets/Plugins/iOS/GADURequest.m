// Copyright 2014 Google Inc. All Rights Reserved.

#import "GADURequest.h"

@implementation GADURequest

- (id)init {
  self = [super init];
  if (self) {
    _keywords = [[NSMutableArray alloc] init];
    _extras = [[NSMutableDictionary alloc] init];
    _mediationExtras = [[NSMutableArray alloc] init];
  }
  return self;
}


- (void)addKeyword:(NSString *)keyword {
  [self.keywords addObject:keyword];
}

- (void)setExtraWithKey:(NSString *)key value:(NSString *)value {
  [self.extras setValue:value forKey:key];
}

- (void)setMediationExtras:(id<GADAdNetworkExtras>)mediationExtras {
  [_mediationExtras addObject:mediationExtras];
}

- (GADRequest *)request {
  GADRequest *request = [GADRequest request];
  request.keywords = self.keywords;
  request.requestAgent = self.requestAgent;
  GADExtras *extras = [[GADExtras alloc] init];
  extras.additionalParameters = self.extras;
  [request registerAdNetworkExtras:extras];

  for (id<GADAdNetworkExtras> mediationExtras in self.mediationExtras) {
    [request registerAdNetworkExtras:mediationExtras];
  }
  return request;
}

@end
