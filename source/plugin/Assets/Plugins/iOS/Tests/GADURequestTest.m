#import <XCTest/XCTest.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADURequest.h"

@interface GADURequestTest : XCTestCase
@end

@implementation GADURequestTest

- (void)testRequestInitializationCreatesEmptyContainers {
  // Tests that initialization creates empty collections for request metadata.
  GADURequest *request = [[GADURequest alloc] init];

  XCTAssertNotNil(request.keywords);
  XCTAssertEqual(request.keywords.count, 0);
  XCTAssertNotNil(request.extras);
  XCTAssertEqual(request.extras.count, 0);
  XCTAssertNotNil(request.mediationExtras);
  XCTAssertEqual(request.mediationExtras.count, 0);
}

- (void)testAddKeywordAppendsKeywordToList {
  // Tests appending a keyword to the request keywords list.
  GADURequest *request = [[GADURequest alloc] init];
  NSString *keyword = @"unity_test_keyword";

  [request addKeyword:keyword];

  XCTAssertTrue([request.keywords containsObject:keyword]);
}

- (void)testSetExtraWithKeyValueAddsExtraParameter {
  // Tests adding key-value extras to the request parameters.
  GADURequest *request = [[GADURequest alloc] init];
  NSString *key = @"extra_key";
  NSString *value = @"extra_value";

  [request setExtraWithKey:key value:value];

  XCTAssertEqualObjects(request.extras[key], value);
}

- (void)testSetCustomTargetingWithKeyValueSetsTargeting {
  // Tests configuring custom targeting key-value pairs on the request.
  GADURequest *request = [[GADURequest alloc] init];
  NSString *key = @"target_key";
  NSString *value = @"target_val";

  [request setCustomTargetingWithKey:key value:value];
  GADRequest *gadRequest = [request request];

  XCTAssertEqualObjects(gadRequest.customTargeting[key], value);
}

- (void)testBuildRequestPopulatesProperties {
  // Tests that `request` populates all configured properties onto GADRequest.
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
