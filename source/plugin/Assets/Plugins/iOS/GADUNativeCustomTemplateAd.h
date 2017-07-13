// Copyright 2014 Google Inc. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADUTypes.h"

@interface GADUNativeCustomTemplateAd : NSObject

/// A reference to the Unity native custom template ad client.
@property(nonatomic, assign) GADUTypeNativeCustomTemplateAdClientRef *nativeCustomTemplateClient;

/// A GADNativeCustomTemplateAd which loads native ads.
@property(nonatomic, strong) GADNativeCustomTemplateAd *nativeCustomTemplateAd;

/// Initializes a GADUNativeCustomTemplateAd.
- (instancetype)initWithAd:(GADNativeCustomTemplateAd *)nativeCustomTemplateAd;

/// The ad clicked callback into Unity.
@property(nonatomic, assign)
    GADUNativeCustomTemplateDidReceiveClickCallback didReceiveClickCallback;

/// The custom template ID for the ad.
- (NSString *)templateID;

/// Returns the string corresponding to the specified key.
- (NSString *)stringForKey:(NSString *)key;

/// Returns the native ad image corresponding to the specified key.
- (UIImage *)imageForKey:(NSString *)key;

/// Call when the user clicks on the ad.
- (void)performClickOnAssetWithKey:(NSString *)key withCustomClickAction:(bool)clickAction;

/// Call when the ad is displayed on screen to the user.
- (void)recordImpression;

/// An array of available asset keys.
- (NSArray *)availableAssetKeys;

@end
