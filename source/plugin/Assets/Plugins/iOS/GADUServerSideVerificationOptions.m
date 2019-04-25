//
//  GADUServerSideVerificationOptions.m
//  Unity-iPhone
//
//  Created by Sharma, Jiten on 24/04/19.
//

#import "GADUServerSideVerificationOptions.h"

@implementation GADUServerSideVerificationOptions 

- (GADServerSideVerificationOptions *)options {
    GADServerSideVerificationOptions *options = [[GADServerSideVerificationOptions alloc] init];
    options.userIdentifier = self.userIdentifier;
    options.customRewardString = self.customRewardString;
    
    return options;
}

@end
