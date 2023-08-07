// Copyright 2023 Google LLC. All Rights Reserved.

#import "GADUDispatch.h"

void GADUDispatchAsyncSafeMainQueue(_Nonnull dispatch_block_t block) {
  if (NSThread.isMainThread) {
    block();
  } else {
    dispatch_async(dispatch_get_main_queue(), block);
  }
}
