//
//  GADServerSideVerificationOptions.h
//  Google Mobile Ads SDK
//
//  Copyright 2018 Google Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <GoogleMobileAds/GoogleMobileAds.h>

/// Options for server-to-server verification callbacks for a rewarded ad.
@interface GADUServerSideVerificationOptions : NSObject

/// A unique identifier used to identify the user in server-to-server callbacks.
@property(nonatomic, copy, nullable) NSString *userIdentifier;

/// Custom reward string sent in server-to-server callbacks.
@property(nonatomic, copy, nullable) NSString *customRewardString;

- (GADServerSideVerificationOptions *_Nonnull)options;

@end
