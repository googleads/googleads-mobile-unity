// Copyright 2016 Google Inc. All Rights Reserved.

#import "GADUPluginUtil.h"

@interface UIView (unityStub)
@property UILayoutGuide *safeAreaLayoutGuide;
@end

BOOL GADUIsOperatingSystemAtLeastVersion(NSInteger majorVersion) {
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

static CGFloat GADUSafeWidthLandscape(void) {
  CGRect screenBounds = [UIScreen mainScreen].bounds;
  if (GADUIsOperatingSystemAtLeastVersion(11)) {
    CGRect safeFrame = UIApplication.sharedApplication.keyWindow.safeAreaLayoutGuide.layoutFrame;
    if (!CGSizeEqualToSize(safeFrame.size, CGSizeZero)) {
      screenBounds = safeFrame;
    }
  }
  return MAX(CGRectGetWidth(screenBounds), CGRectGetHeight(screenBounds));
}

UIInterfaceOrientation GADUUIInterfaceOrientationForGADUScreenOrientation(
    GADUScreenOrientation orientation) {
  UIInterfaceOrientation uiOrientation;
  switch (orientation) {
    case kGADUScreenOrientationLandscapeLeft:
    case kGADUScreenOrientationLandscapeRight:
      uiOrientation = UIInterfaceOrientationLandscapeLeft;
      break;
    default:
      uiOrientation = UIInterfaceOrientationPortrait;
  }
  return uiOrientation;
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
  if (GADUIsOperatingSystemAtLeastVersion(11) &&
      GADAdSizeEqualToSize(kGADAdSizeSmartBannerLandscape, adSize)) {
    CGSize usualSize = CGSizeFromGADAdSize(kGADAdSizeSmartBannerLandscape);
    CGSize bannerSize = CGSizeMake(GADUSafeWidthLandscape(), usualSize.height);
    return GADAdSizeFromCGSize(bannerSize);
  } else {
    return adSize;
  }
}

+ (UIViewController *)unityGLViewController {
  id<UIApplicationDelegate> appDelegate = [UIApplication sharedApplication].delegate;
  if ([appDelegate respondsToSelector:@selector(rootViewController)]) {
    return [[[UIApplication sharedApplication].delegate window] rootViewController];
  }
  return nil;
}

+ (void)positionView:(UIView *)view
        inParentView:(UIView *)parentView
          adPosition:(GADAdPosition)adPosition {
  CGRect parentBounds = parentView.bounds;
  if (GADUIsOperatingSystemAtLeastVersion(11)) {
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
  if (GADUIsOperatingSystemAtLeastVersion(11)) {
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
  UIInterfaceOrientation currentOrientation = UIApplication.sharedApplication.statusBarOrientation;

  if (width == kGADUAdSizeUseFullWidth && UIInterfaceOrientationIsPortrait(currentOrientation)) {
    return GADAdSizeFullWidthPortraitWithHeight(height);
  } else if ((width == kGADUAdSizeUseFullWidth &&
              UIInterfaceOrientationIsLandscape(currentOrientation))) {
    return GADAdSizeFromCGSize(CGSizeMake(GADUSafeWidthLandscape(), height));
  }
  return GADAdSizeFromCGSize(CGSizeMake(width, height));
}

+ (GADAdSize)adaptiveAdSizeForWidth:(CGFloat)width orientation:(GADUBannerOrientation)orientation {
  if (width == kGADUAdSizeUseFullWidth) {
    width = GADUDeviceSafeWidth();
  }
  switch (orientation) {
    case kGADUBannerOrientationCurrent:
      return GADCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(width);
    case kGADUBannerOrientationLandscape:
      return GADLandscapeAnchoredAdaptiveBannerAdSizeWithWidth(width);
    case kGADUBannerOrientationPortrait:
      return GADPortraitAnchoredAdaptiveBannerAdSizeWithWidth(width);
  }
}

/* NSError */
+ (char *)GADUStringFromNSError:(NSError *)error {
  NSMutableDictionary *dict = [GADUPluginUtil NSMutableDictionaryFromNSError:error];
  return [GADUPluginUtil GADUStringFromNSMutableDictionary:dict];
}

/* NSError */
+ (NSMutableDictionary *)NSMutableDictionaryFromNSError:(NSError *)error {
  NSMutableDictionary *dict = [[NSMutableDictionary alloc] init];
  [dict setValue:[NSNumber numberWithInt:error.code] forKey:@"code"];
  [dict setValue:error.domain forKey:@"domain"];
  [dict setValue:error.localizedDescription forKey:@"message"];
  [dict setValue:error.description forKey:@"description"];
  if (error.userInfo[NSUnderlyingErrorKey]) {
    NSMutableDictionary *cause =
        [GADUPluginUtil NSMutableDictionaryFromNSError:error.userInfo[NSUnderlyingErrorKey]];
    [dict setValue:[GADUPluginUtil NSStringFromNSMutableDictionary:cause] forKey:@"cause"];
  }
  if (error.userInfo[GADErrorUserInfoKeyResponseInfo]) {
    GADResponseInfo *response = error.userInfo[GADErrorUserInfoKeyResponseInfo];
    [dict setValue:[GADUPluginUtil NSMutableDictionaryFromGADResponseInfo:response]
            forKey:@"responseInfo"];
  }
  return dict;
}

/* GADResponseInfo */
+ (char *)GADUStringFromGADResponseInfo:(GADResponseInfo *)responseInfo {
  NSMutableDictionary *dict = [GADUPluginUtil NSMutableDictionaryFromGADResponseInfo:responseInfo];
  return [GADUPluginUtil GADUStringFromNSMutableDictionary:dict];
}

/* GADResponseInfo */
+ (NSMutableDictionary *)NSMutableDictionaryFromGADResponseInfo:(GADResponseInfo *)responseInfo {
  NSMutableDictionary *dict = [[NSMutableDictionary alloc] init];

  NSMutableDictionary *responseInfoDic = [GADUPluginUtil
      GADUMutableDictionaryFromGADAdNetworkResponseInfo:responseInfo.loadedAdNetworkResponseInfo];

  [dict setValue:responseInfoDic forKey:@"adapterResponseInfo"];

  NSMutableArray *responseInfosDic = [[NSMutableArray alloc] init];
  for (GADAdNetworkResponseInfo *adNetwork in responseInfo.adNetworkInfoArray) {
    NSMutableDictionary *responseInfoJson =
        [GADUPluginUtil GADUMutableDictionaryFromGADAdNetworkResponseInfo:adNetwork];
    [responseInfosDic addObject:responseInfoJson];
  }
  [dict setValue:responseInfosDic forKey:@"adapterResponseInfos"];

  [dict setValue:responseInfo.adNetworkClassName forKey:@"adNetworkName"];
  [dict setValue:responseInfo.description forKey:@"description"];
  [dict setValue:responseInfo.responseIdentifier forKey:@"responseId"];
  return dict;
}

/* GADAdNetworkResponseInfo */
+ (char *)GADUStringFromGADAdNetworkResponseInfo:(GADAdNetworkResponseInfo *)responseInfo {
  return convertToCString(@"");
}

+ (NSMutableDictionary *)GADUMutableDictionaryFromGADAdNetworkResponseInfo:
    (GADAdNetworkResponseInfo *)responseInfo {
  NSMutableDictionary *dict = [[NSMutableDictionary alloc] init];
  [dict setValue:responseInfo.adNetworkClassName forKey:@"adapterClassName"];
  [dict setValue:responseInfo.adSourceInstanceID forKey:@"adSourceInstanceId"];

  [dict setValue:[GADUPluginUtil GADUDictionaryFromNSDictionary:responseInfo.adUnitMapping]
          forKey:@"adUnitMapping"];
  NSMutableDictionary *adError =
      [GADUPluginUtil NSMutableDictionaryFromNSError:responseInfo.error];
  [dict setValue:[GADUPluginUtil NSStringFromNSMutableDictionary:adError] forKey:@"adError"];
  //Unity expects a long lantecyMillis.
  [dict setValue:[NSNumber numberWithLong:responseInfo.latency * 1000] forKey:@"latencyMillis"];
  [dict setValue:responseInfo.description forKey:@"description"];
  return dict;
}

/* NSMutableDictionary */
+ (NSString *)NSStringFromNSMutableDictionary:(NSMutableDictionary *)dict {
  NSError *jsonError;
  NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict
                                                     options:0  // unformatted
                                                       error:&jsonError];
  NSString *jsonString = @"";
  if (jsonError) {
    NSLog(@"GoogleMobileAdsPlugin: %@", [jsonError localizedDescription]);
  } else {
    jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
  }
  return jsonString;
}

/* NSMutableDictionary */
+ (char *)GADUStringFromNSMutableDictionary:(NSMutableDictionary *)dict {
  NSError *jsonError;
  NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict
                                                     options:0  // unformatted
                                                       error:&jsonError];
  NSString *jsonString = @"";
  if (jsonError) {
    NSLog(@"GoogleMobileAdsPlugin: %@", [jsonError localizedDescription]);
  } else {
    jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
  }
  return convertToCString(jsonString);
}

/* NSDictionary */
+ (NSMutableArray *)GADUDictionaryFromNSDictionary:(NSDictionary<NSString *, id> *)dict {
  NSMutableArray *result = [[NSMutableArray alloc] init];
  for (id key in dict) {
    NSMutableDictionary *pair = [[NSMutableDictionary alloc] init];
    [pair setValue:key forKey:@"key"];
    [pair setValue:[dict valueForKey:key] forKey:@"value"];
    [result addObject:pair];
  }
  return result;
}

/* convertToCString */
char *convertToCString(const NSString *nsString) {
  if (nsString == NULL) return NULL;

  const char *nsStringUtf8 = [nsString UTF8String];
  // create a null terminated C string on the heap so that our string's memory isn't wiped out right
  // after method's return
  char *cString = (char *)malloc(strlen(nsStringUtf8) + 1);
  strcpy(cString, nsStringUtf8);

  return cString;
}

@end
