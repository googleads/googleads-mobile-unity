#import "AppDelegate.h"

// Minimal application delegate setting up a key window and root view controller
// required by `XCTest` to host unit tests in an iOS application environment.
@implementation AppDelegate

- (BOOL)application:(UIApplication *)application
    didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
  self.window = [[UIWindow alloc] initWithFrame:[[UIScreen mainScreen] bounds]];
  self.window.rootViewController = [[UIViewController alloc] init];
  [self.window makeKeyAndVisible];
  return YES;
}

@end
