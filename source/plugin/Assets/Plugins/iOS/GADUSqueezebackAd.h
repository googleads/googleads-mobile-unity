// Copyright 2026 Google LLC. All Rights Reserved.
#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>
#if __has_include(<GoogleMobileAds/GADSqueezebackAd_Preview.h>)
#import <GoogleMobileAds/GADSqueezebackAd_Preview.h>
#elif __has_include("GADSqueezebackAd_Preview.h")
#import "GADSqueezebackAd_Preview.h"
#endif

#import "GADUTypes.h"

@interface GADUSqueezebackAd : NSObject

/// Initializes a GADUSqueezebackAd.
- (nonnull instancetype)initWithSqueezebackAdClientReference:
    (_Nonnull GADUTypeSqueezebackAdClientRef *_Nonnull)squeezebackAdClient;

/// The underlying GADSqueezebackAd.
@property(nonatomic, strong, nullable)
    GADSqueezebackAd *squeezebackAd;

/// A reference to the Unity Squeezeback ad client.
@property(nonatomic, assign)
    _Nonnull GADUTypeSqueezebackAdClientRef *_Nonnull squeezebackAdClient;

/// The ad loaded callback into Unity.
@property(nonatomic, assign, nullable)
    GADUSqueezebackAdLoadedCallback adLoadedCallback;

/// The ad request failed callback into Unity.
@property(nonatomic, assign, nullable)
    GADUSqueezebackAdFailedToLoadCallback adFailedToLoadCallback;

/// The ad shown callback into Unity.
@property(nonatomic, assign, nullable)
    GADUSqueezebackAdShownCallback adShownCallback;

/// The ad hidden callback into Unity.
@property(nonatomic, assign, nullable)
    GADUSqueezebackAdHiddenCallback adHiddenCallback;

/// The ad impression callback into Unity.
@property(nonatomic, assign, nullable)
    GADUSqueezebackAdDidRecordImpressionCallback
        adDidRecordImpressionCallback;

/// The ad click callback into Unity.
@property(nonatomic, assign, nullable)
    GADUSqueezebackAdDidRecordClickCallback adDidRecordClickCallback;

/// The ad failed to show callback into Unity.
@property(nonatomic, assign, nullable)
    GADUSqueezebackAdDidFailToShowCallback
        adDidFailToShowCallback;

/// The ad will present full screen content callback into Unity.
@property(nonatomic, assign, nullable)
    GADUSqueezebackAdWillPresentFullScreenContentCallback
        adWillPresentFullScreenContentCallback;

/// The ad dismissed full screen content callback into Unity.
@property(nonatomic, assign, nullable)
    GADUSqueezebackAdDidDismissFullScreenContentCallback
        adDidDismissFullScreenContentCallback;

/// The paid event callback into Unity.
@property(nonatomic, assign, nullable)
    GADUSqueezebackAdPaidEventCallback paidEventCallback;

/// The response info for the loaded ad.
@property(nonatomic, readonly, copy, nullable) GADResponseInfo *responseInfo;

/// Loads a squeezeback ad with the specified ad unit ID and request.
- (void)loadWithAdUnitID:(nonnull NSString *)adUnitID
                 request:(nonnull GADRequest *)request;

/// Shows the squeezeback ad.
- (void)show;

/// Hides the squeezeback ad.
- (void)hide;

/// Destroys the squeezeback ad, removing it from view and clearing its reference.
- (void)destroy;

@end
