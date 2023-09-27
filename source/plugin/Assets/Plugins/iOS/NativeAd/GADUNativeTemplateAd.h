// Copyright 2023 Google LLC. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADUNativeAdOptions.h"
#import "GADUNativeTemplateStyle.h"
#import "GADUTypes.h"

/// A wrapper around GADNativeAd. Includes the ability to create GADNativeAd objects, load
/// them with ads, show them via Native Templates, listen for ad events and destroy them.
@interface GADUNativeTemplateAd : NSObject

/// Initializes a GADUNativeTemplateAdClient.
- (nonnull instancetype)initWithNativeTemplateAdClientReference:
    (_Nonnull GADUTypeNativeTemplateAdClientRef *_Nonnull)nativeTemplateAdClient;

/// A reference to the Unity Native Template Ad client.
@property(nonatomic, assign)
    _Nonnull GADUTypeNativeTemplateAdClientRef *_Nonnull nativeTemplateAdClient;

/// The ad received callback into Unity.
@property(nonatomic, assign, nullable) GADUNativeTemplateAdLoadedCallback adLoadedCallback;

/// The ad failed callback into Unity.
@property(nonatomic, assign, nullable)
    GADUNativeTemplateAdFailedToLoadCallback adFailedToLoadCallback;

/// The ad impression callback into Unity.
@property(nonatomic, assign, nullable)
    GADUNativeTemplateAdDidRecordImpressionCallback adDidRecordImpressionCallback;

/// The ad click callback into Unity.
@property(nonatomic, assign, nullable)
    GADUNativeTemplateAdDidRecordClickCallback adDidRecordClickCallback;

/// The paid event callback into Unity.
@property(nonatomic, assign, nullable) GADUNativeTemplateAdPaidEventCallback paidEventCallback;

/// The presentation of a full screen view callback into Unity.
@property(nonatomic, assign, nullable)
    GADUNativeTemplateAdWillPresentScreenCallback adWillPresentScreenCallback;

/// The dismissal of a full screen view callback into Unity.
@property(nonatomic, assign, nullable)
    GADUNativeTemplateAdDidDismissScreenCallback adDidDismissScreenCallback;

/// Returns the native ad response info.
@property(nullable, nonatomic, readonly, copy) GADResponseInfo *responseInfo;

/// Returns the height of the template view in pixels.
@property(nonatomic, readonly) CGFloat heightInPixels;

/// Returns the width of the template view in pixels.
@property(nonatomic, readonly) CGFloat widthInPixels;

/// Makes an ad request. Additional targeting options can be supplied with a request object.
- (void)loadWithAdUnitID:(nonnull NSString *)adUnitID
                 request:(nonnull GADRequest *)request
               adOptions:(nullable GADUNativeAdOptions *)options;

/// Shows the native ad.
- (void)show:(nonnull GADUNativeTemplateStyle *)templateStyle
       width:(CGFloat)width
      height:(CGFloat)height;

/// Hides the native ad.
- (void)hide;

/// Shows a hidden native ad. Calling show without a previously provided template style will do
/// nothing. Similar behavior when calling show after the native ad is destroyed.
- (void)show;

/// Sets the GADTemplateView's position on screen using a standard position.
- (void)setAdPosition:(GADAdPosition)adPosition;

/// Sets the GADTemplateView's position on screen using a custom position.
- (void)setCustomAdPosition:(CGPoint)customPosition;

/// Destroys the native template ad.
- (void)destroy;

@end
