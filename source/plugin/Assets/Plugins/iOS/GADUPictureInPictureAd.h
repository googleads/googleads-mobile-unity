// Copyright 2026 Google LLC. All Rights Reserved.
#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#if GMA_PREVIEW_FEATURES
#import <GoogleMobileAds/GoogleMobileAds_Beta.h>
#else
/// Options for the position of a Picture-in-Picture (PiP) ad.
typedef NS_ENUM(NSInteger, GADPictureInPictureAdPosition) {
  GADPictureInPictureAdPositionDefault = 0,
  GADPictureInPictureAdPositionBottomRight = 1,
  GADPictureInPictureAdPositionBottomLeft = 2,
  GADPictureInPictureAdPositionTopLeft = 3,
  GADPictureInPictureAdPositionTopRight = 4
};
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

/// The ad failed to show callback into Unity.
@property(nonatomic, assign, nullable)
    GADUPictureInPictureAdDidFailToShowCallback
        adDidFailToShowCallback;

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
- (void)showWithPosition:(GADPictureInPictureAdPosition)position;

/// Hides the PiP ad.
- (void)hide;

/// Destroys the PiP ad, removing it from view and clearing its reference.
- (void)destroy;

/// Returns the current position of the PiP ad.
- (GADPictureInPictureAdPosition)position;

@end
