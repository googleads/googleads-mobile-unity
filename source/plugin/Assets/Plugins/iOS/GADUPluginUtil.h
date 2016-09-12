// Copyright 2016 Google Inc. All Rights Reserved.

@import Foundation;

#import "GADUTypes.h"

@interface GADUPluginUtil : NSObject

/// Returns the Unity view controller.
+ (UIViewController *)unityGLViewController;

/// Position view in the bounds of the parent view, corresponding to the adPosition.
+ (void)positionView:(UIView *)view
      inParentBounds:(CGRect)parentBounds
      withAdPosition:(GADAdPosition)adPosition;

@end
