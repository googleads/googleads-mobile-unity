// Copyright 2023 Google LLC. All Rights Reserved.

#import <Foundation/Foundation.h>

/// Runs a block on the main thread immediately if the current thread is the
/// main thread, otherwise dispatches asynchronously to the main thread.
void GADUDispatchAsyncSafeMainQueue(_Nonnull dispatch_block_t block);
