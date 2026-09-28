// Copyright 2026 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#import "AdMobTestBridge.h"

#if defined(__cplusplus)
extern "C" {
#endif

// Unity runtime C exports.
extern UIViewController* UnityGetGLViewController(void);
extern void UnitySendMessage(const char* obj, const char* method, const char* msg);

#if defined(__cplusplus)
}
#endif

// Internal overlay manager maintaining native UIKit accessibility elements.
@interface AdMobTestBridgeOverlay : NSObject
+ (instancetype)sharedInstance;
- (void)recordEvent:(NSString*)eventName;
- (void)registerButton:(NSString*)buttonId
                 normX:(CGFloat)normX
                 normY:(CGFloat)normY
                 normW:(CGFloat)normW
                 normH:(CGFloat)normH;
- (void)clearButtons;
@end

@implementation AdMobTestBridgeOverlay {
  UILabel* _eventLabel;
  NSMutableDictionary<NSString*, UIButton*>* _registeredButtons;
  UIView* _containerView;
}

+ (instancetype)sharedInstance {
  static AdMobTestBridgeOverlay* instance = nil;
  static dispatch_once_t onceToken;
  dispatch_once(&onceToken, ^{
    instance = [[AdMobTestBridgeOverlay alloc] init];
  });
  return instance;
}

- (instancetype)init {
  self = [super init];
  if (self) {
    _registeredButtons = [[NSMutableDictionary alloc] init];
    dispatch_async(dispatch_get_main_queue(), ^{
      [self setupContainerAndEventLabel];
    });
  }
  return self;
}

- (UIViewController*)rootViewController {
  UIViewController* rootVC = nil;
  if (&UnityGetGLViewController != NULL) {
    rootVC = UnityGetGLViewController();
  }
  if (!rootVC) {
    for (UIScene* scene in [UIApplication sharedApplication].connectedScenes) {
      if ([scene isKindOfClass:[UIWindowScene class]]) {
        UIWindowScene* windowScene = (UIWindowScene*)scene;
        for (UIWindow* window in windowScene.windows) {
          if (window.isKeyWindow) {
            rootVC = window.rootViewController;
            break;
          }
        }
      }
    }
  }
  if (!rootVC) {
    rootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
  }
  return rootVC;
}

- (void)setupContainerAndEventLabel {
  UIViewController* rootVC = [self rootViewController];
  if (!rootVC || !rootVC.view) {
    dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(0.1 * NSEC_PER_SEC)),
                   dispatch_get_main_queue(), ^{
                     [self setupContainerAndEventLabel];
                   });
    return;
  }

  if (!_containerView) {
    _containerView = [[UIView alloc] initWithFrame:rootVC.view.bounds];
    _containerView.autoresizingMask =
        UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight;
    _containerView.userInteractionEnabled = YES;
    _containerView.backgroundColor = [UIColor clearColor];
    _containerView.accessibilityIdentifier = @"AdMobTestOverlayContainer";
    [rootVC.view addSubview:_containerView];
    [rootVC.view bringSubviewToFront:_containerView];
  }

  if (!_eventLabel) {
    _eventLabel = [[UILabel alloc] initWithFrame:CGRectMake(0, 0, 1, 1)];
    _eventLabel.isAccessibilityElement = YES;
    _eventLabel.accessibilityIdentifier = @"AdMobTestEventLabel";
    _eventLabel.text = @"IDLE";
    _eventLabel.accessibilityLabel = @"IDLE";
    _eventLabel.accessibilityValue = @"IDLE";
    _eventLabel.alpha = 0.01;
    [_containerView addSubview:_eventLabel];
  }
}

- (void)recordEvent:(NSString*)eventName {
  dispatch_async(dispatch_get_main_queue(), ^{
    [self setupContainerAndEventLabel];
    if (self->_eventLabel) {
      self->_eventLabel.text = eventName;
      self->_eventLabel.accessibilityLabel = eventName;
      self->_eventLabel.accessibilityValue = eventName;
      UIAccessibilityPostNotification(UIAccessibilityLayoutChangedNotification, self->_eventLabel);
      NSLog(@"[AdMobTestBridge] Event recorded: %@", eventName);
    }
  });
}

- (void)registerButton:(NSString*)buttonId
                 normX:(CGFloat)normX
                 normY:(CGFloat)normY
                 normW:(CGFloat)normW
                 normH:(CGFloat)normH {
  dispatch_async(dispatch_get_main_queue(), ^{
    [self setupContainerAndEventLabel];
    if (!self->_containerView) {
      return;
    }

    CGFloat screenWidth = self->_containerView.bounds.size.width;
    CGFloat screenHeight = self->_containerView.bounds.size.height;
    CGRect frame = CGRectMake(normX * screenWidth, normY * screenHeight, normW * screenWidth,
                              normH * screenHeight);

    UIButton* button = self->_registeredButtons[buttonId];
    if (!button) {
      button = [UIButton buttonWithType:UIButtonTypeCustom];
      button.isAccessibilityElement = YES;
      button.accessibilityIdentifier = buttonId;
      button.accessibilityLabel = buttonId;
      button.backgroundColor = [UIColor clearColor];
      [button addTarget:self
                    action:@selector(onButtonTapped:)
          forControlEvents:UIControlEventTouchUpInside];
      [self->_containerView addSubview:button];
      self->_registeredButtons[buttonId] = button;
    }

    button.frame = frame;
    button.hidden = NO;
    button.userInteractionEnabled = YES;
    [self->_containerView bringSubviewToFront:button];
  });
}

- (void)clearButtons {
  dispatch_async(dispatch_get_main_queue(), ^{
    for (UIButton* button in self->_registeredButtons.allValues) {
      [button removeFromSuperview];
    }
    [self->_registeredButtons removeAllObjects];
  });
}

- (void)onButtonTapped:(UIButton*)sender {
  NSString* buttonId = sender.accessibilityIdentifier;
  if (buttonId.length > 0) {
    NSLog(@"[AdMobTestBridge] Native overlay button tapped: %@", buttonId);
    UnitySendMessage("AdMobTestBridge", "TriggerButtonClick", [buttonId UTF8String]);
  }
}

@end

extern "C" {

void AdMobTest_Init(void) { [AdMobTestBridgeOverlay sharedInstance]; }

void AdMobTest_SetLastEvent(const char* eventName) {
  if (eventName) {
    [[AdMobTestBridgeOverlay sharedInstance] recordEvent:[NSString stringWithUTF8String:eventName]];
  }
}

void AdMobTest_RegisterButton(const char* buttonId, float normX, float normY, float normWidth,
                              float normHeight) {
  if (buttonId) {
    [[AdMobTestBridgeOverlay sharedInstance] registerButton:[NSString stringWithUTF8String:buttonId]
                                                      normX:(CGFloat)normX
                                                      normY:(CGFloat)normY
                                                      normW:(CGFloat)normWidth
                                                      normH:(CGFloat)normHeight];
  }
}

void AdMobTest_ClearButtons(void) { [[AdMobTestBridgeOverlay sharedInstance] clearButtons]; }
}
