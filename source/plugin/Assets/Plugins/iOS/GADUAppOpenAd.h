// Copyright 2021 Google LLC. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADAppOpenAd_Preview.h"
#import "GADUTypes.h"

@interface GADUAppOpenAd : NSObject

/// Initializes a GADUAppOpenAd.
- (nonnull instancetype)initWithAppOpenAdClientReference:(_Nonnull GADUTypeAppOpenAdClientRef *_Nonnull)appOpenAdClient;

/// The app open ad.
@property(nonatomic, strong, nullable) GADAppOpenAd *appOpenAd;

/// A reference to the Unity app open ad client.
@property(nonatomic, assign) _Nonnull GADUTypeAppOpenAdClientRef *_Nonnull appOpenAdClient;

/// The ad loaded callback into Unity.
@property(nonatomic, assign, nullable) GADUAppOpenAdLoadedCallback adLoadedCallback;

/// The ad request failed callback into Unity.
@property(nonatomic, assign, nullable) GADUAppOpenAdFailedToLoadCallback adFailedToLoadCallback;

/// The paid event callback into Unity.
@property(nonatomic, assign, nullable) GADUAppOpenAdPaidEventCallback paidEventCallback;

/// The ad failed to present full screen content callback into Unity.
@property(nonatomic, assign, nullable) GADUAppOpenAdFailedToPresentFullScreenContentCallback
    adFailedToPresentFullScreenContentCallback;

/// The ad will present full screen content callback into Unity.
@property(nonatomic, assign, nullable)
    GADUAppOpenAdWillPresentFullScreenContentCallback adWillPresentFullScreenContentCallback;

/// The ad dismissed full screen content callback into Unity.
@property(nonatomic, assign, nullable)
    GADUAppOpenAdDidDismissFullScreenContentCallback adDidDismissFullScreenContentCallback;

/// The ad impression callback into Unity.
@property(nonatomic, assign, nullable)
    GADUAppOpenAdDidRecordImpressionCallback adDidRecordImpressionCallback;

/// The ad click callback into Unity.
@property(nonatomic, assign, nullable) GADUAppOpenAdDidRecordClickCallback adDidRecordClickCallback;

// The app open ad response info.
@property(nonatomic, readonly, copy, nullable) GADResponseInfo *responseInfo;

/// Returns whether an app open ad is preloaded for the given ad unit ID.
+ (BOOL)isPreloadedAdAvailable:(nonnull NSString *)adUnitID;

/// Loads a preloaded interstitial ad corresponding to the given ad unit ID if available.
- (void)preloadedAdWithAdUnitID:(nonnull NSString *)adUnitID;

/// Makes an ad request. Additional targeting options can be supplied with a request object.
- (void)loadWithAdUnitID:(nonnull NSString *)adUnit
                 request:(nonnull GADRequest *)request;

/// Shows the app open ad.
- (void)show;

@end
