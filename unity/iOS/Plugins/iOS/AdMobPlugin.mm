// AdMobPlugin.mm
// Copyright 2013 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#import <Foundation/Foundation.h>

#import "AdMobPlugin.h"
#import "GADAdMobExtras.h"
#import "GADAdSize.h"
#import "GADBannerView.h"

@interface AdMobPlugin ()

// Root view controller for Unity applications can be accessed using this
// method.
extern UIViewController *UnityGetGLViewController();

@end

@implementation AdMobPlugin

@synthesize bannerView = bannerView_;
@synthesize callbackHandlerName = callbackHandlerName_;

#pragma mark Unity bridge

+ (AdMobPlugin *)pluginSharedInstance {
  static AdMobPlugin *sharedInstance = nil;
  static dispatch_once_t onceToken;
  dispatch_once(&onceToken, ^{
    sharedInstance = [[AdMobPlugin alloc] init];
  });
  return sharedInstance;
}

- (void)createBannerViewWithPubId:(NSString *)publisherId
                 bannerTypeString:(NSString *)adSizeString
                    positionAtTop:(bool)positionAtTop {
  GADAdSize adSize = [self GADAdSizeFromString:adSizeString];
  // We need values for adSize and publisherId if we don't want to fail. The
  // AdMob library will spit an error if we try using invalid GADAdSize so just
  // handle publisherId.
  if (!publisherId) {
    NSLog(@"AdMobPlugin: Failed because no publisher ID is set.");
    return;
  }
  positionAdAtTop_ = positionAtTop;
  [self createGADBannerViewWithPubId:publisherId
                          bannerType:adSize];
}

- (void)requestAdWithTesting:(BOOL)isTesting
                extrasString:(NSString *)extrasString {
  if (!self.bannerView) {
    NSLog(@"AdMobPlugin: Failed because CreateBannerView() must be called"
          @"before RequestBannerAd().");
    return;
  }
  // Turn the incoming JSON string into a NSDictionary.
  NSError *error = nil;
  NSData *extrasJsonData =
      [extrasString dataUsingEncoding:NSUTF8StringEncoding];
  NSDictionary *extrasJsonDictionary =
      [NSJSONSerialization JSONObjectWithData:extrasJsonData
                                      options:NSJSONReadingMutableContainers
                                        error:&error];
  if (extrasJsonDictionary) {
    // Add a flag to denote that this request is coming from the unity plugin.
    NSMutableDictionary *modifiedExtrasDict =
        [[NSMutableDictionary alloc] initWithDictionary:extrasJsonDictionary];
    [modifiedExtrasDict removeObjectForKey:@"unity"];
    [modifiedExtrasDict setValue:@"1" forKey:@"unity"];
    [self requestAdWithTesting:isTesting
                        extras:modifiedExtrasDict];
  } else {
    NSLog(@"AdMobPlugin: Error parsing JSON for extras: %@", error);
  }
}

- (void)hideBannerView {
  if (!self.bannerView) {
    NSLog(@"AdMobPlugin: Failed because a GADBannerView was never created.");
    return;
  }
  self.bannerView.hidden = YES;
}

- (void)showBannerView {
  if (!self.bannerView) {
    NSLog(@"AdMobPlugin: Failed because a GADBannerView was never created.");
    return;
  }
  self.bannerView.hidden = NO;
}

- (GADAdSize)GADAdSizeFromString:(NSString *)string {
  if ([string isEqualToString:@"BANNER"]) {
    return kGADAdSizeBanner;
  } else if ([string isEqualToString:@"IAB_MRECT"]) {
    return kGADAdSizeMediumRectangle;
  } else if ([string isEqualToString:@"IAB_BANNER"]) {
    return kGADAdSizeFullBanner;
  } else if ([string isEqualToString:@"IAB_LEADERBOARD"]) {
    return kGADAdSizeLeaderboard;
  } else if ([string isEqualToString:@"SMART_BANNER"]) {
    // Have to choose the right Smart Banner constant according to orientation.
    UIDeviceOrientation currentOrientation =
        [[UIDevice currentDevice] orientation];
    if (UIInterfaceOrientationIsPortrait(currentOrientation)) {
      return kGADAdSizeSmartBannerPortrait;
    } else {
      return kGADAdSizeSmartBannerLandscape;
    }
  } else {
    return kGADAdSizeInvalid;
  }
}

#pragma mark Ad Banner logic

- (void)createGADBannerViewWithPubId:(NSString *)pubId
                          bannerType:(GADAdSize)adSize {
  self.bannerView = [[[GADBannerView alloc] initWithAdSize:adSize] autorelease];
  self.bannerView.adUnitID = pubId;
  self.bannerView.delegate = self;
  self.bannerView.rootViewController = UnityGetGLViewController();
}

- (void)requestAdWithTesting:(BOOL)isTesting
                      extras:(NSDictionary *)extrasDict {
  GADRequest *request = [GADRequest request];
  if (isTesting) {
    // Make the request for a test ad. Put in an identifier for the simulator as
    // well as any devices you want to receive test ads.
    request.testDevices =
        [NSArray arrayWithObjects:
            GAD_SIMULATOR_ID,
            // TODO: Add your device test identifiers here. They are
            // printed to the console when the app is launched.
            nil];
  }
  if (extrasDict) {
    GADAdMobExtras *extras = [[[GADAdMobExtras alloc] init] autorelease];
    extras.additionalParameters = extrasDict;
    [request registerAdNetworkExtras:extras];
  }
  [self.bannerView loadRequest:request];
  // Add the ad to the main container view.
  [UnityGetGLViewController().view addSubview:self.bannerView];
}

#pragma mark GADBannerViewDelegate implementation

- (void)adViewDidReceiveAd:(GADBannerView *)adView {
  UnitySendMessage([callbackHandlerName_ UTF8String],
                   "AdViewDidReceiveAd",
                   "Received ad successfully.");
}

- (void)adView:(GADBannerView *)view
    didFailToReceiveAdWithError:(GADRequestError *)error {
  NSString *errorMsg =
      [NSString stringWithFormat:@"Failed to receive ad with error: %@",
          [error localizedFailureReason]];
  UnitySendMessage([callbackHandlerName_ UTF8String],
                   "AdViewDidFailToReceiveAd",
                   [errorMsg UTF8String]);
}

- (void)adViewWillPresentScreen:(GADBannerView *)adView {
  UnitySendMessage([callbackHandlerName_ UTF8String],
                   "AdViewWillPresentScreen",
                   "Calling AdViewWillPresentScreen.");
}

- (void)adViewDidDismissScreen:(GADBannerView *)adView {
  UnitySendMessage([callbackHandlerName_ UTF8String],
                   "AdViewDidDismissScreen",
                   "Calling AdViewDidDismissScreen.");
}

- (void)adViewWillDismissScreen:(GADBannerView *)adView {
  UnitySendMessage([callbackHandlerName_ UTF8String],
                   "AdViewWillDismissScreen",
                   "Calling adViewWillDismissScreen.");
}

- (void)adViewWillLeaveApplication:(GADBannerView *)adView {
  UnitySendMessage([callbackHandlerName_ UTF8String],
                   "AdViewWillLeaveApplication",
                   "Calling AdViewWillLeaveApplication.");
}

#pragma mark Cleanup

- (void)dealloc {
  bannerView_.delegate = nil;
  [bannerView_ release];
  [super dealloc];
}

@end

// Helper method used to convert NSStrings into C-style strings.
NSString *CreateNSString(const char* string) {
  if (string) {
    return [NSString stringWithUTF8String:string];
  } else {
    return [NSString stringWithUTF8String:""];
  }
}

// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.
extern "C" {

  void _CreateBannerView(const char *publisherId,
                         const char *adSize,
                         bool positionAtTop) {
    AdMobPlugin *adMobPlugin = [AdMobPlugin pluginSharedInstance];
    [adMobPlugin createBannerViewWithPubId:CreateNSString(publisherId)
                          bannerTypeString:CreateNSString(adSize)
                             positionAtTop:(BOOL)positionAtTop];
  }

  void _RequestBannerAd(bool isTesting, const char *extras) {
    AdMobPlugin *adMobPlugin = [AdMobPlugin pluginSharedInstance];
    [adMobPlugin requestAdWithTesting:(BOOL)isTesting
                         extrasString:CreateNSString(extras)];
  }

  void _SetCallbackHandlerName(const char *callbackHandlerName) {
    AdMobPlugin *adMobPlugin = [AdMobPlugin pluginSharedInstance];
    [adMobPlugin setCallbackHandlerName:CreateNSString(callbackHandlerName)];
  }

  void _HideBannerView() {
    AdMobPlugin *adMobPlugin = [AdMobPlugin pluginSharedInstance];
    [adMobPlugin hideBannerView];
  }

  void _ShowBannerView() {
    AdMobPlugin *adMobPlugin = [AdMobPlugin pluginSharedInstance];
    [adMobPlugin showBannerView];
  }
}
