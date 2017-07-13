// Copyright 2016 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADUTypes.h"

@interface GADUPluginUtil : NSObject

/// Returns the Unity view controller.
+ (UIViewController *)unityGLViewController;

/// Position view in the bounds of the parent view, corresponding to the adPosition.
+ (void)positionView:(UIView *)view
      inParentBounds:(CGRect)parentBounds
          adPosition:(GADAdPosition)adPosition;

/// Position view in the bounds of the parent view, corresponding to the CGPoint.
+ (void)positionView:(UIView *)view
      inParentBounds:(CGRect)parentBounds
      customPosition:(CGPoint)adPosition;

/// Returns a GADAdSize for a specified width and height.
+ (GADAdSize)adSizeForWidth:(CGFloat)width height:(CGFloat)height;

@end
