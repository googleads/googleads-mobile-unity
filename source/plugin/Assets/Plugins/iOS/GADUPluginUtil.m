// Copyright 2016 Google Inc. All Rights Reserved.

#import "GADUPluginUtil.h"

#import "UnityAppController.h"

@implementation GADUPluginUtil

+ (UIViewController *)unityGLViewController {
  return ((UnityAppController *)[UIApplication sharedApplication].delegate).rootViewController;
}

+ (void)positionView:(UIView *)view
      inParentBounds:(CGRect)parentBounds
          adPosition:(GADAdPosition)adPosition {
  CGPoint center = CGPointMake(CGRectGetMidX(parentBounds), CGRectGetMidY(view.bounds));
  switch (adPosition) {
    case kGADAdPositionTopOfScreen:
      center = CGPointMake(CGRectGetMidX(parentBounds), CGRectGetMidY(view.bounds));
      break;
    case kGADAdPositionBottomOfScreen:
      center = CGPointMake(CGRectGetMidX(parentBounds),
                           CGRectGetMaxY(parentBounds) - CGRectGetMidY(view.bounds));
      break;
    case kGADAdPositionTopLeftOfScreen:
      center = CGPointMake(CGRectGetMidX(view.bounds), CGRectGetMidY(view.bounds));
      break;
    case kGADAdPositionTopRightOfScreen:
      center = CGPointMake(CGRectGetMaxX(parentBounds) - CGRectGetMidX(view.bounds),
                           CGRectGetMidY(view.bounds));
      break;
    case kGADAdPositionBottomLeftOfScreen:
      center = CGPointMake(CGRectGetMidX(view.bounds),
                           CGRectGetMaxY(parentBounds) - CGRectGetMidY(view.bounds));
      break;
    case kGADAdPositionBottomRightOfScreen:
      center = CGPointMake(CGRectGetMaxX(parentBounds) - CGRectGetMidX(view.bounds),
                           CGRectGetMaxY(parentBounds) - CGRectGetMidY(view.bounds));
      break;
    case kGADAdPositionCenterOfScreen:
      center = CGPointMake(CGRectGetMidX(parentBounds), CGRectGetMidY(parentBounds));
      break;
  }
  view.center = center;
}

+ (void)positionView:(UIView *)view
      inParentBounds:(CGRect)parentBounds
      customPosition:(CGPoint)adPosition {
  CGPoint center = CGPointMake(adPosition.x + CGRectGetMidX(view.bounds),
                               adPosition.y + CGRectGetMidY(view.bounds));
  view.center = center;
}

+ (GADAdSize)adSizeForWidth:(CGFloat)width height:(CGFloat)height {
  UIDeviceOrientation currentOrientation = [UIApplication sharedApplication].statusBarOrientation;

  if (width == kGADUAdSizeUseFullWidth && UIInterfaceOrientationIsPortrait(currentOrientation)) {
    return GADAdSizeFullWidthPortraitWithHeight(height);
  } else if ((width == kGADUAdSizeUseFullWidth &&
              UIInterfaceOrientationIsLandscape(currentOrientation))) {
    return GADAdSizeFullWidthLandscapeWithHeight(height);
  }
  return GADAdSizeFromCGSize(CGSizeMake(width, height));
}

@end
