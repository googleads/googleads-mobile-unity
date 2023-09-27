#import "GADUNativeTemplateAd.h"
#import "GADUPluginUtil.h"

@interface GADUNativeTemplateAd () <GADAdLoaderDelegate,
                                    GADNativeAdLoaderDelegate,
                                    GADNativeAdDelegate>
@end

@implementation GADUNativeTemplateAd {
  /// A GADAdLoader that is used to request native ads.
  GADAdLoader *_adLoader;
  /// _nativeAd is used to keep a reference to the returned native ad.
  GADNativeAd *_nativeAd;
  /// _templateWrapper holds the requested TemplateView with the native ad in the desired styling.
  GADUNativeTemplateViewWrapper *_templateWrapper;
  /// Defines where the ad should be positioned on the screen with a GADAdPosition.
  GADAdPosition _adPosition;
  /// Defines where the ad should be positioned on the screen with a CGPoint.
  CGPoint _customAdPosition;
  /// Keep a reference to the error objects so references to Unity-level
  /// ResponseInfo object are not released until the ad object is released.
  NSError *_lastLoadError;
  NSError *_lastPresentError;
}

- (nonnull instancetype)initWithNativeTemplateAdClientReference:
    (_Nonnull GADUTypeNativeTemplateAdClientRef *_Nonnull)nativeTemplateAdClient {
  self = [super init];
  if (self) {
    _nativeTemplateAdClient = nativeTemplateAdClient;
  }
  return self;
}

- (void)loadWithAdUnitID:(nonnull NSString *)adUnitID
                 request:(nonnull GADRequest *)request
               adOptions:(nullable GADUNativeAdOptions *)options {
  _adLoader = [[GADAdLoader alloc] initWithAdUnitID:adUnitID
                                 rootViewController:[GADUPluginUtil unityGLViewController]
                                            adTypes:@[ GADAdLoaderAdTypeNative ]
                                            options:options.asGADAdLoaderOptions];
  _adLoader.delegate = self;
  [_adLoader loadRequest:request];
}

- (void)show:(nonnull GADUNativeTemplateStyle *)templateStyle
       width:(CGFloat)width
      height:(CGFloat)height {
  if (_templateWrapper)
  {
    // If ad is shown before, destroy ad before showing with provided template style.
    [_templateWrapper removeFromSuperview];
    _templateWrapper = nil;
  }

  UIViewController *unityController = [GADUPluginUtil unityGLViewController];
  _templateWrapper = [templateStyle getDisplayedView:_nativeAd];
  if(!_templateWrapper) {
    NSLog(@"Requested Template View couldn't be loaded!");
    return;
  }

  CGRect frame = _templateWrapper.frame;
  frame.size.width = width;
  frame.size.height = height;
  _templateWrapper.frame = frame;

  [unityController.view addSubview:_templateWrapper];
}

- (void)hide
{
  if (!_templateWrapper) {
    NSLog(@"Template View is nil. Ignoring call to hide Template Native Ad View");
    return;
  }
  _templateWrapper.hidden = YES;
}

- (void)show
{
  if (!_templateWrapper) {
    NSLog(@"Template View is nil. Ignoring call to show Template Native Ad View");
    return;
  }
  _templateWrapper.hidden = NO;
}

- (void)setAdPosition:(GADAdPosition)adPosition
{
  _adPosition = adPosition;
  [self positionNativeTemplateView];
}

- (void)setCustomAdPosition:(CGPoint)customPosition
{
  _customAdPosition = customPosition;
  _adPosition = kGADAdPositionCustom;
  [self positionNativeTemplateView];
}

- (void)positionNativeTemplateView {
  UIView *unityView = [GADUPluginUtil unityGLViewController].view;

  if (_adPosition != kGADAdPositionCustom) {
    [GADUPluginUtil positionView:_templateWrapper inParentView:unityView adPosition:_adPosition];
  } else {
    [GADUPluginUtil positionView:_templateWrapper
                    inParentView:unityView
                  customPosition:_customAdPosition];
  }
}

- (void)destroy
{
  if (!_templateWrapper) {
    NSLog(@"Template View is nil. Ignoring call to show Template Native Ad View");
    return;
  }
  [_templateWrapper removeFromSuperview];
  _templateWrapper = nil;
}

- (nullable GADResponseInfo *)responseInfo {
  return _nativeAd.responseInfo;
}

- (CGFloat)heightInPixels {
  return CGRectGetHeight(CGRectStandardize(_templateWrapper.frame)) * [UIScreen mainScreen].scale;
}

- (CGFloat)widthInPixels {
  return CGRectGetWidth(CGRectStandardize(_templateWrapper.frame)) * [UIScreen mainScreen].scale;
}

# pragma mark - GADAdLoaderDelegate
- (void)adLoader:(GADAdLoader *)adLoader didFailToReceiveAdWithError:(NSError *)error {
  if (self.adFailedToLoadCallback) {
    _lastLoadError = error;
    self.adFailedToLoadCallback(self.nativeTemplateAdClient, (__bridge GADUTypeErrorRef)error);
  }
}

# pragma mark - GADNativeAdLoaderDelegate
- (void)adLoader:(nonnull GADAdLoader *)adLoader didReceiveNativeAd:(nonnull GADNativeAd *)ad {
  _nativeAd = ad;
  _nativeAd.delegate = self;

  if (self.adLoadedCallback) {
    self.adLoadedCallback(self.nativeTemplateAdClient);
  }

  __weak GADUNativeTemplateAd *weakSelf = self;
  _nativeAd.paidEventHandler = ^void(GADAdValue *_Nonnull adValue) {
    GADUNativeTemplateAd *strongSelf = weakSelf;
    if (!strongSelf) {
      return;
    }
    if (strongSelf.paidEventCallback) {
      int64_t valueInMicros = [adValue.value decimalNumberByMultiplyingByPowerOf10:6].longLongValue;
      strongSelf.paidEventCallback(
          strongSelf.nativeTemplateAdClient, (int)adValue.precision, valueInMicros,
          [adValue.currencyCode cStringUsingEncoding:NSUTF8StringEncoding]);
    }
  };
}

# pragma mark - GADNativeAdDelegate
- (void)nativeAdDidRecordImpression:(nonnull id)nativeAd {
  if (self.adDidRecordImpressionCallback) {
    self.adDidRecordImpressionCallback(self.nativeTemplateAdClient);
  }
}

- (void)nativeAdDidRecordClick:(nonnull id)nativeAd {
  if (self.adDidRecordClickCallback) {
    self.adDidRecordClickCallback(self.nativeTemplateAdClient);
  }
}

- (void)nativeAdWillPresentScreen:(nonnull id)nativeAd {
  if (self.adWillPresentScreenCallback) {
    self.adWillPresentScreenCallback(self.nativeTemplateAdClient);
  }
}

- (void)nativeAdDidDismissScreen:(nonnull id)nativeAd {
  if (self.adDidDismissScreenCallback) {
    self.adDidDismissScreenCallback(self.nativeTemplateAdClient);
  }
}

@end
