#import <UIKit/UIKit.h>
#import <XCTest/XCTest.h>

#import "GADUPluginUtil.h"

@interface GADUPluginUtilTest : XCTestCase
@end

@implementation GADUPluginUtilTest

- (void)testPauseOnBackgroundPropertyGetterAndSetter {
  // Tests setting and getting the pause-on-background state.
  BOOL originalValue = GADUPluginUtil.pauseOnBackground;

  GADUPluginUtil.pauseOnBackground = YES;
  XCTAssertTrue(GADUPluginUtil.pauseOnBackground);

  GADUPluginUtil.pauseOnBackground = NO;
  XCTAssertFalse(GADUPluginUtil.pauseOnBackground);

  GADUPluginUtil.pauseOnBackground = originalValue;
}

- (void)testStringFromUTF8StringWithNullReturnsNil {
  // Tests that passing NULL bytes returns nil.
  const char *nullBytes = NULL;

  NSString *result = [GADUPluginUtil GADUStringFromUTF8String:nullBytes];

  XCTAssertNil(result);
}

- (void)testStringFromUTF8StringWithValidBytesReturnsString {
  // Tests converting valid UTF-8 C string bytes to an NSString.
  const char *bytes = "hello world";

  NSString *result = [GADUPluginUtil GADUStringFromUTF8String:bytes];

  XCTAssertEqualObjects(result, @"hello world");
}

- (void)testStringFromUTF8StringWithEmptyBytesReturnsEmptyString {
  // Tests that passing an empty C string returns an empty NSString.
  const char *bytes = "";

  NSString *result = [GADUPluginUtil GADUStringFromUTF8String:bytes];

  XCTAssertEqualObjects(result, @"");
}

- (void)testPositionViewInParentViewTopCenterPosition {
  // Tests positioning an ad view at the top-center of a parent view.
  UIView *parentView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 320, 480)];
  UIView *adView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 320, 50)];

  [GADUPluginUtil positionView:adView inParentView:parentView adPosition:kGADAdPositionTopOfScreen];

  XCTAssertEqualWithAccuracy(adView.center.x, 160.0, 0.01);
  XCTAssertEqualWithAccuracy(adView.center.y, 25.0, 0.01);
}

- (void)testPositionViewInParentViewBottomCenterPosition {
  // Tests positioning an ad view at the bottom-center of a parent view.
  UIView *parentView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 320, 480)];
  UIView *adView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 320, 50)];

  [GADUPluginUtil positionView:adView
                  inParentView:parentView
                    adPosition:kGADAdPositionBottomOfScreen];

  XCTAssertEqualWithAccuracy(adView.center.x, 160.0, 0.01);
  XCTAssertEqualWithAccuracy(adView.center.y, 455.0, 0.01);
}

- (void)testPositionViewInParentViewCenterPosition {
  // Tests positioning an ad view at the center of a parent view.
  UIView *parentView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 320, 480)];
  UIView *adView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 320, 50)];

  [GADUPluginUtil positionView:adView
                  inParentView:parentView
                    adPosition:kGADAdPositionCenterOfScreen];

  XCTAssertEqualWithAccuracy(adView.center.x, 160.0, 0.01);
  XCTAssertEqualWithAccuracy(adView.center.y, 240.0, 0.01);
}

- (void)testPositionViewInParentViewCustomPosition {
  // Tests positioning an ad view at a custom offset inside a parent view.
  UIView *parentView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 320, 480)];
  UIView *adView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 100, 50)];
  CGPoint customPosition = CGPointMake(20, 30);

  [GADUPluginUtil positionView:adView inParentView:parentView customPosition:customPosition];

  XCTAssertEqualWithAccuracy(adView.center.x, 70.0, 0.01);
  XCTAssertEqualWithAccuracy(adView.center.y, 55.0, 0.01);
}

- (void)testAdSizeForWidthHeightReturnsCorrectSize {
  // Tests creating an ad size from explicit width and height dimensions.
  CGFloat width = 320.0;
  CGFloat height = 50.0;

  GADAdSize adSize = [GADUPluginUtil adSizeForWidth:width height:height];

  XCTAssertEqual(adSize.size.width, width);
  XCTAssertEqual(adSize.size.height, height);
}

- (void)testIsNullWithNilReturnsTrue {
  // Tests that `isNull:` returns YES for nil.
  id object = nil;

  BOOL result = [GADUPluginUtil isNull:object];

  XCTAssertTrue(result);
}

- (void)testIsNullWithNSNullReturnsTrue {
  // Tests that `isNull:` returns YES for NSNull instances.
  id object = [NSNull null];

  BOOL result = [GADUPluginUtil isNull:object];

  XCTAssertTrue(result);
}

- (void)testIsNullWithValidObjectReturnsFalse {
  // Tests that `isNull:` returns NO for non-null objects.
  id object = @"test";

  BOOL result = [GADUPluginUtil isNull:object];

  XCTAssertFalse(result);
}

- (void)testIsNotNullWithNilReturnsFalse {
  // Tests that `isNotNull:` returns NO for nil.
  id object = nil;

  BOOL result = [GADUPluginUtil isNotNull:object];

  XCTAssertFalse(result);
}

- (void)testIsNotNullWithNSNullReturnsFalse {
  // Tests that `isNotNull:` returns NO for NSNull instances.
  id object = [NSNull null];

  BOOL result = [GADUPluginUtil isNotNull:object];

  XCTAssertFalse(result);
}

- (void)testIsNotNullWithValidObjectReturnsTrue {
  // Tests that `isNotNull:` returns YES for non-null objects.
  id object = @"test";

  BOOL result = [GADUPluginUtil isNotNull:object];

  XCTAssertTrue(result);
}

@end
