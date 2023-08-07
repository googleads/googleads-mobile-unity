// Copyright 2023 Google LLC. All Rights Reserved.

#import "GADUUMPError.h"

NSErrorDomain const UMPErrorDomain = @"com.google.user_messaging_platform.unity_ios_bridge";

NSError *_Nonnull UMPErrorWithCodeAndDescription(NSInteger code, NSString *_Nonnull description) {
  return [[NSError alloc]
      initWithDomain:GADUUMPErrorDomain
                code:code
            userInfo:@{NSLocalizedDescriptionKey : description ?: @"Internal error."}];
}
