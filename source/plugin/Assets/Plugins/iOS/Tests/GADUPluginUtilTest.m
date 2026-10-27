#import <UIKit/UIKit.h>
#import <XCTest/XCTest.h>

#import "GADUPluginUtil.h"

@interface GADUPluginUtilTest : XCTestCase
@end

@implementation GADUPluginUtilTest

- (void)testStringFromUTF8StringWithNullReturnsNil {
  const char *nullBytes = NULL;

  NSString *result = [GADUPluginUtil GADUStringFromUTF8String:nullBytes];

  XCTAssertNil(result);
}

- (void)testStringFromUTF8StringWithValidBytesReturnsString {
  const char *bytes = "hello world";

  NSString *result = [GADUPluginUtil GADUStringFromUTF8String:bytes];

  XCTAssertEqualObjects(result, @"hello world");
}

- (void)testStringFromUTF8StringWithEmptyBytesReturnsEmptyString {
  const char *bytes = "";

  NSString *result = [GADUPluginUtil GADUStringFromUTF8String:bytes];

  XCTAssertEqualObjects(result, @"");
}

- (void)testIsNullWithNilReturnsTrue {
  id object = nil;

  BOOL result = [GADUPluginUtil isNull:object];

  XCTAssertTrue(result);
}

- (void)testIsNullWithNSNullReturnsTrue {
  id object = [NSNull null];

  BOOL result = [GADUPluginUtil isNull:object];

  XCTAssertTrue(result);
}

- (void)testIsNullWithValidObjectReturnsFalse {
  id object = @"test";

  BOOL result = [GADUPluginUtil isNull:object];

  XCTAssertFalse(result);
}

- (void)testIsNotNullWithNilReturnsFalse {
  id object = nil;

  BOOL result = [GADUPluginUtil isNotNull:object];

  XCTAssertFalse(result);
}

- (void)testIsNotNullWithNSNullReturnsFalse {
  id object = [NSNull null];

  BOOL result = [GADUPluginUtil isNotNull:object];

  XCTAssertFalse(result);
}

- (void)testIsNotNullWithValidObjectReturnsTrue {
  id object = @"test";

  BOOL result = [GADUPluginUtil isNotNull:object];

  XCTAssertTrue(result);
}

- (void)testPauseOnBackgroundPropertyGetterAndSetter {
  BOOL originalValue = GADUPluginUtil.pauseOnBackground;

  GADUPluginUtil.pauseOnBackground = YES;
  XCTAssertTrue(GADUPluginUtil.pauseOnBackground);

  GADUPluginUtil.pauseOnBackground = NO;
  XCTAssertFalse(GADUPluginUtil.pauseOnBackground);

  GADUPluginUtil.pauseOnBackground = originalValue;
}

- (void)testAdSizeForWidthHeightReturnsCorrectSize {
  CGFloat width = 320.0;
  CGFloat height = 50.0;

  GADAdSize adSize = [GADUPluginUtil adSizeForWidth:width height:height];

  XCTAssertEqual(adSize.size.width, width);
  XCTAssertEqual(adSize.size.height, height);
}

- (void)testPositionViewInParentViewTopCenterPosition {
  UIView *parentView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 320, 480)];
  UIView *adView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 320, 50)];

  [GADUPluginUtil positionView:adView inParentView:parentView adPosition:kGADAdPositionTopOfScreen];

  XCTAssertEqualWithAccuracy(adView.center.x, 160.0, 0.01);
  XCTAssertEqualWithAccuracy(adView.center.y, 25.0, 0.01);
}

- (void)testPositionViewInParentViewBottomCenterPosition {
  UIView *parentView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 320, 480)];
  UIView *adView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 320, 50)];

  [GADUPluginUtil positionView:adView
                  inParentView:parentView
                    adPosition:kGADAdPositionBottomOfScreen];

  XCTAssertEqualWithAccuracy(adView.center.x, 160.0, 0.01);
  XCTAssertEqualWithAccuracy(adView.center.y, 455.0, 0.01);
}

- (void)testPositionViewInParentViewCenterPosition {
  UIView *parentView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 320, 480)];
  UIView *adView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 320, 50)];

  [GADUPluginUtil positionView:adView
                  inParentView:parentView
                    adPosition:kGADAdPositionCenterOfScreen];

  XCTAssertEqualWithAccuracy(adView.center.x, 160.0, 0.01);
  XCTAssertEqualWithAccuracy(adView.center.y, 240.0, 0.01);
}

- (void)testPositionViewInParentViewCustomPosition {
  UIView *parentView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 320, 480)];
  UIView *adView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 100, 50)];
  CGPoint customPosition = CGPointMake(20, 30);

  [GADUPluginUtil positionView:adView inParentView:parentView customPosition:customPosition];

  XCTAssertEqualWithAccuracy(adView.center.x, 70.0, 0.01);
  XCTAssertEqualWithAccuracy(adView.center.y, 55.0, 0.01);
}

@end
