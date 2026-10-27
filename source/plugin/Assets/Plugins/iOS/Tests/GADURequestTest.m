#import <XCTest/XCTest.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADURequest.h"

@interface GADURequestTest : XCTestCase
@end

@implementation GADURequestTest

- (void)testRequestInitializationCreatesEmptyContainers {
  GADURequest *request = [[GADURequest alloc] init];

  XCTAssertNotNil(request.keywords);
  XCTAssertEqual(request.keywords.count, 0);
  XCTAssertNotNil(request.extras);
  XCTAssertEqual(request.extras.count, 0);
  XCTAssertNotNil(request.mediationExtras);
  XCTAssertEqual(request.mediationExtras.count, 0);
}

- (void)testAddKeywordAppendsKeywordToList {
  GADURequest *request = [[GADURequest alloc] init];
  NSString *keyword = @"unity_test_keyword";

  [request addKeyword:keyword];

  XCTAssertTrue([request.keywords containsObject:keyword]);
}

- (void)testSetExtraWithKeyValueAddsExtraParameter {
  GADURequest *request = [[GADURequest alloc] init];
  NSString *key = @"extra_key";
  NSString *value = @"extra_value";

  [request setExtraWithKey:key value:value];

  XCTAssertEqualObjects(request.extras[key], value);
}

- (void)testBuildRequestPopulatesProperties {
  GADURequest *request = [[GADURequest alloc] init];
  request.requestAgent = @"Unity";
  request.placementID = 123456789;
  [request addKeyword:@"test_ad"];
  [request setExtraWithKey:@"key1" value:@"val1"];
  [request setCustomTargetingWithKey:@"target_key" value:@"target_val"];

  GADRequest *gadRequest = [request request];

  XCTAssertEqualObjects(gadRequest.requestAgent, @"Unity");
  XCTAssertEqual(gadRequest.placementID, 123456789);
  XCTAssertTrue([gadRequest.keywords containsObject:@"test_ad"]);
  XCTAssertEqualObjects(gadRequest.customTargeting[@"target_key"], @"target_val");
}

@end
