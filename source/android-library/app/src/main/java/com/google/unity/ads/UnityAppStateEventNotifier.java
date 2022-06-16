/*
 * Copyright (C) 2022 Google LLC
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

package com.google.unity.ads;

import android.app.Activity;
import androidx.annotation.NonNull;
import androidx.lifecycle.DefaultLifecycleObserver;
import androidx.lifecycle.LifecycleOwner;
import androidx.lifecycle.ProcessLifecycleOwner;

/**
 * Proxy for the ProcessLifecycleOwner for the Unity AppStateEventNotifier.
 */
public class UnityAppStateEventNotifier implements DefaultLifecycleObserver {

  /**
   * The {@code Activity} on which the app open ad will display.
   */
  private final Activity activity;

  /**
   * A callback implemented in Unity via {@code AndroidJavaProxy} to receive lifecycle events.
   */
  private final UnityAppStateEventCallback callback;

  public UnityAppStateEventNotifier(Activity activity, UnityAppStateEventCallback callback) {
    this.activity = activity;
    this.callback = callback;
  }

  /** Attaches as an observer of the ProcessLifecycleOwner. */
  public void startListening() {
    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            ProcessLifecycleOwner.get()
                .getLifecycle()
                .addObserver(UnityAppStateEventNotifier.this);
          }
        });
  }

  /** Detatches as an observer of the ProcessLifecycleOwner. */
  public void stopListening() {
    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            ProcessLifecycleOwner.get()
                .getLifecycle()
                .removeObserver(UnityAppStateEventNotifier.this);
          }
        });
  }

  /** DefaultLifecycleObserver implementation. */

  @Override
  public void onStart(@NonNull LifecycleOwner owner) {
    callback.onAppStateChanged(/*isBackground=*/ false);
  }

  @Override
  public void onStop(@NonNull LifecycleOwner owner) {
    callback.onAppStateChanged(/*isBackground=*/ true);
  }

  @Override
  public void onCreate(@NonNull LifecycleOwner owner) {
  }

  @Override
  public void onDestroy(@NonNull LifecycleOwner owner) {
  }

  @Override
  public void onResume(@NonNull LifecycleOwner owner) {
  }

  @Override
  public void onPause(@NonNull LifecycleOwner owner) {
  }

}
