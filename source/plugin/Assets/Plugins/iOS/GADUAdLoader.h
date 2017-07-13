// Copyright 2014 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADUTypes.h"

@interface GADUAdLoader : NSObject

/// A reference to the Unity ad loader client.
@property(nonatomic, assign) GADUTypeAdLoaderClientRef *adLoaderClient;

/// A GADAdLoader that loads native ads.
@property(nonatomic, strong) GADAdLoader *adLoader;

// List of native ad types the ad loader will request.
@property(nonatomic, copy) NSArray *adTypes;

/// List of templateIDs.
@property(nonatomic, copy) NSArray *templateIDs;

/// The custom template ad received callback into Unity.
@property(nonatomic, assign)
    GADUAdLoaderDidReceiveNativeCustomTemplateAdCallback customTemplateAdReceivedCallback;

/// The ad failed to load callback into Unity.
@property(nonatomic, assign) GADUAdLoaderDidFailToReceiveAdWithErrorCallback adFailedCallback;

/// Initializes the GADUAdLoader.
- (instancetype)initWithAdLoaderClientReference:(GADUTypeAdLoaderClientRef *)adLoaderClient
                                       adUnitID:(NSString *)adUnitID
                                    templateIDs:(NSArray *)templateIDs
                                        adTypes:(NSArray *)adTypes;

/// Makes an ad request. Additional targeting options can be supplied with a request object.
- (void)loadRequest:(GADRequest *)request;

@end
