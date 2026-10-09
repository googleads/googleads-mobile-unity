/*
 * Copyright (C) 2026 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.google.unity.ads.nextgen;

import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.ThreadFactory;

/** Centralized executor services for running nextgen ad and preloader callbacks. */
public final class UnityExecutor {

  private static final ExecutorService adEventExecutor =
      Executors.newSingleThreadExecutor(
          new ThreadFactory() {
            @Override
            public Thread newThread(Runnable r) {
              return new Thread(r, "GMAUnityAdEventsThread");
            }
          });

  private static final ExecutorService preloaderExecutor =
      Executors.newSingleThreadExecutor(
          new ThreadFactory() {
            @Override
            public Thread newThread(Runnable r) {
              return new Thread(r, "GMAUnityPreloaderThread");
            }
          });

  /** Returns the executor service for regular ad callbacks. */
  public static ExecutorService getExecutor() {
    return adEventExecutor;
  }

  /** Returns the executor service for ad preloader callbacks. */
  public static ExecutorService getPreloaderExecutor() {
    return preloaderExecutor;
  }

  private UnityExecutor() {}
}
