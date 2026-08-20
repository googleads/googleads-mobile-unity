/*
 * Copyright (C) 2026 Google, Inc.
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

import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;

/**
 * An interface form of {@link UnityPictureInPictureAdCallback} that can be implemented via {@code
 * AndroidJavaProxy} in Unity to receive ad events synchronously.
 */
public interface UnityPictureInPictureAdCallback extends UnityPaidEventListener {

  /** Called when the picture in picture ad is loaded. */
  void onAdLoaded();

  /** Called when the picture in picture ad fails to load. */
  void onAdFailedToLoad(LoadAdError error);

  /** Called when the picture in picture ad is displayed on the screen. */
  void onAdShown();

  /** Called when the picture in picture ad is hidden from the screen. */
  void onAdHidden();

  /** Called when the picture in picture ad records an impression. */
  void onAdImpression();

  /** Called when the picture in picture ad records a click. */
  void onAdClicked();

  /** Called when the picture in picture ad opens an overlay that covers the screen. */
  void onAdShowedFullScreenContent();

  /** Called when the picture in picture ad dismisses an overlay that it previously showed. */
  void onAdDismissedFullScreenContent();
}
