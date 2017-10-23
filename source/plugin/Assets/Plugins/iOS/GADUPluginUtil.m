// Copyright 2016 Google Inc. All Rights Reserved.

#import "GADUPluginUtil.h"

#import "UnityAppController.h"

@implementation GADUPluginUtil

+ (UIViewController *)unityGLViewController {
  return ((UnityAppController *)[UIApplication sharedApplication].delegate).rootViewController;
}

+ (void)positionView:(UIView *)view
        inParentView:(UIView *)parentView
          adPosition:(GADAdPosition)adPosition {
  CGRect parentBounds = parentView.bounds;
  if (@available(iOS 11, *)) {
    parentBounds = parentView.safeAreaLayoutGuide.layoutFrame;
  }
  CGFloat top = CGRectGetMinY(parentBounds) + CGRectGetMidY(view.bounds);
  CGFloat left = CGRectGetMinX(parentBounds) + CGRectGetMidX(view.bounds);

  CGFloat bottom = CGRectGetMaxY(parentBounds) - CGRectGetMidY(view.bounds);
  CGFloat right = CGRectGetMaxX(parentBounds) - CGRectGetMidX(view.bounds);
  CGFloat centerX = CGRectGetMidX(parentBounds);
  CGFloat centerY = CGRectGetMidY(parentBounds);

  // If this view is of greater or equal width to the parent view, do not offset
  // to edge of safe area. Eg for smart banners that are still full screen
  // width.
  if (CGRectGetWidth(view.bounds) >= CGRectGetWidth(parentView.bounds)) {
    left = CGRectGetMidX(parentView.bounds);
  }

  // Similarly for height, if view is of custom size which is full screen
  // height, do not offset.
  if (CGRectGetHeight(view.bounds) >= CGRectGetHeight(parentView.bounds)) {
    top = CGRectGetMidY(parentView.bounds);
  }

  CGPoint center = CGPointMake(centerX, top);
  switch (adPosition) {
    case kGADAdPositionTopOfScreen:
      center = CGPointMake(centerX, top);
      break;
    case kGADAdPositionBottomOfScreen:
      center = CGPointMake(centerX, bottom);
      break;
    case kGADAdPositionTopLeftOfScreen:
      center = CGPointMake(left, top);
      break;
    case kGADAdPositionTopRightOfScreen:
      center = CGPointMake(right, top);
      break;
    case kGADAdPositionBottomLeftOfScreen:
      center = CGPointMake(left, bottom);
      break;
    case kGADAdPositionBottomRightOfScreen:
      center = CGPointMake(right, bottom);
      break;
    case kGADAdPositionCenterOfScreen:
      center = CGPointMake(centerX, centerY);
      break;
    default:
      break;
  }
  view.center = center;
}

+ (void)positionView:(UIView *)view
        inParentView:(UIView *)parentView
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
