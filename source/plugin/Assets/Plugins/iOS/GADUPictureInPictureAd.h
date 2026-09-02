// Copyright 2026 Google LLC. All Rights Reserved.
#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#if GMA_PREVIEW_FEATURES
#import <GoogleMobileAds/GADPictureInPictureAd_Preview.h>
#import <GoogleMobileAds/GADPictureInPictureAdOptions_Preview.h>
#import <GoogleMobileAds/GADPictureInPictureAdDelegate_Preview.h>
#endif
#import "GADUTypes.h"

@interface GADUPictureInPictureAd : NSObject

/// Initializes a GADUPictureInPictureAd.
- (nonnull instancetype)initWithPictureInPictureAdClientReference:
    (_Nonnull GADUTypePictureInPictureAdClientRef *_Nonnull)pipAdClient;

#if GMA_PREVIEW_FEATURES
/// The underlying GADPictureInPictureAd.
@property(nonatomic, strong, nullable)
    GADPictureInPictureAd *pictureInPictureAd;
#endif

/// A reference to the Unity PiP ad client.
@property(nonatomic, assign)
    _Nonnull GADUTypePictureInPictureAdClientRef *_Nonnull pipAdClient;

/// The ad loaded callback into Unity.
@property(nonatomic, assign, nullable)
    GADUPictureInPictureAdLoadedCallback adLoadedCallback;

/// The ad request failed callback into Unity.
@property(nonatomic, assign, nullable)
    GADUPictureInPictureAdFailedToLoadCallback adFailedToLoadCallback;

/// The ad shown callback into Unity.
@property(nonatomic, assign, nullable)
    GADUPictureInPictureAdShownCallback adShownCallback;

/// The ad hidden callback into Unity.
@property(nonatomic, assign, nullable)
    GADUPictureInPictureAdHiddenCallback adHiddenCallback;

/// The ad impression callback into Unity.
@property(nonatomic, assign, nullable)
    GADUPictureInPictureAdDidRecordImpressionCallback
        adDidRecordImpressionCallback;

/// The ad click callback into Unity.
@property(nonatomic, assign, nullable)
    GADUPictureInPictureAdDidRecordClickCallback adDidRecordClickCallback;

/// The ad failed to present full screen content callback into Unity.
@property(nonatomic, assign, nullable)
    GADUPictureInPictureAdFailedToPresentFullScreenContentCallback
        adFailedToPresentFullScreenContentCallback;

/// The ad will present full screen content callback into Unity.
@property(nonatomic, assign, nullable)
    GADUPictureInPictureAdWillPresentFullScreenContentCallback
        adWillPresentFullScreenContentCallback;

/// The ad dismissed full screen content callback into Unity.
@property(nonatomic, assign, nullable)
    GADUPictureInPictureAdDidDismissFullScreenContentCallback
        adDidDismissFullScreenContentCallback;

/// The paid event callback into Unity.
@property(nonatomic, assign, nullable)
    GADUPictureInPictureAdPaidEventCallback paidEventCallback;

/// The response info for the loaded ad.
@property(nonatomic, readonly, copy, nullable) GADResponseInfo *responseInfo;

/// Loads a PiP ad with the specified ad unit ID and request.
- (void)loadWithAdUnitID:(nonnull NSString *)adUnitID
                 request:(nonnull GADRequest *)request;

/// Shows the PiP ad at the specified position.
- (void)showWithPosition:(NSInteger)position;

/// Hides the PiP ad.
- (void)hide;

/// Returns the current position of the PiP ad.
- (NSInteger)position;

@end
