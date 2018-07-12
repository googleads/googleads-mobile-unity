// Copyright 2016 Google Inc. All Rights Reserved.

#import "GADUPluginUtil.h"

#import "UnityAppController.h"

@interface UIView (unityStub)
@property UILayoutGuide *safeAreaLayoutGuide;
@end

static BOOL IsOperatingSystemAtLeastVersion(NSInteger majorVersion) {
  NSProcessInfo *processInfo = NSProcessInfo.processInfo;
  if ([processInfo respondsToSelector:@selector(isOperatingSystemAtLeastVersion:)]) {
    // iOS 8+.
    NSOperatingSystemVersion version = {majorVersion};
    return [processInfo isOperatingSystemAtLeastVersion:version];
  } else {
    // pre-iOS 8. App supports iOS 7+, so this process must be running on iOS 7.
    return majorVersion >= 7;
  }
}

static CGFloat FullSafeWidthLandscape(void) {
  CGRect screenBounds = [UIScreen mainScreen].bounds;
  if (IsOperatingSystemAtLeastVersion(11)) {
    CGRect safeFrame = [UIApplication sharedApplication].keyWindow.safeAreaLayoutGuide.layoutFrame;
    if (!CGSizeEqualToSize(safeFrame.size, CGSizeZero)) {
      screenBounds = safeFrame;
    }
  }
  return MAX(CGRectGetWidth(screenBounds), CGRectGetHeight(screenBounds));
}

@implementation GADUPluginUtil

static BOOL _pauseOnBackground = NO;

+ (BOOL)pauseOnBackground {
  return _pauseOnBackground;
}

+ (void)setPauseOnBackground:(BOOL)pause {
  _pauseOnBackground = pause;
}

+ (NSString *)GADUStringFromUTF8String:(const char *)bytes {
  return bytes ? @(bytes) : nil;
}

+ (GADAdSize)safeAdSizeForAdSize:(GADAdSize)adSize {
  if (IsOperatingSystemAtLeastVersion(11) &&
      GADAdSizeEqualToSize(kGADAdSizeSmartBannerLandscape, adSize)) {
    CGSize usualSize = CGSizeFromGADAdSize(kGADAdSizeSmartBannerLandscape);
    CGSize bannerSize = CGSizeMake(FullSafeWidthLandscape(), usualSize.height);
    return GADAdSizeFromCGSize(bannerSize);
  } else {
    return adSize;
  }
}

+ (UIViewController *)unityGLViewController {
  return ((UnityAppController *)[UIApplication sharedApplication].delegate).rootViewController;
}

+ (void)positionView:(UIView *)view
        inParentView:(UIView *)parentView
          adPosition:(GADAdPosition)adPosition {
  CGRect parentBounds = parentView.bounds;
  if (IsOperatingSystemAtLeastVersion(11)) {
    CGRect safeAreaFrame = parentView.safeAreaLayoutGuide.layoutFrame;
    if (!CGSizeEqualToSize(CGSizeZero, safeAreaFrame.size)) {
      parentBounds = safeAreaFrame;
    }
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
  CGPoint origin = parentView.bounds.origin;
  if (IsOperatingSystemAtLeastVersion(11)) {
    CGRect safeAreaFrame = parentView.safeAreaLayoutGuide.layoutFrame;
    if (!CGSizeEqualToSize(CGSizeZero, safeAreaFrame.size)) {
      origin = safeAreaFrame.origin;
    }
  }

  CGPoint center = CGPointMake(origin.x + adPosition.x + CGRectGetMidX(view.bounds),
                               origin.y + adPosition.y + CGRectGetMidY(view.bounds));
  view.center = center;
}

+ (GADAdSize)adSizeForWidth:(CGFloat)width height:(CGFloat)height {
  UIDeviceOrientation currentOrientation = [UIApplication sharedApplication].statusBarOrientation;

  if (width == kGADUAdSizeUseFullWidth && UIInterfaceOrientationIsPortrait(currentOrientation)) {
    return GADAdSizeFullWidthPortraitWithHeight(height);
  } else if ((width == kGADUAdSizeUseFullWidth &&
              UIInterfaceOrientationIsLandscape(currentOrientation))) {
    return GADAdSizeFromCGSize(CGSizeMake(FullSafeWidthLandscape(), height));
  }
  return GADAdSizeFromCGSize(CGSizeMake(width, height));
}

@end
