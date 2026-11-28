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

import com.google.android.libraries.ads.mobile.sdk.common.FullScreenContentError;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;

/**
 * An interface form of {@link UnitySqueezebackAdCallback} that can be implemented via {@code
 * AndroidJavaProxy} in Unity to receive Squeezeback ad events.
 */
public interface UnitySqueezebackAdCallback
    extends UnityFullScreenContentCallback, UnityPaidEventListener {

  /** Called when the Squeezeback ad is loaded. */
  void onAdLoaded();

  /** Called when the Squeezeback ad fails to load. */
  void onAdFailedToLoad(LoadAdError error);

  /** Called when the Squeezeback ad is displayed on the screen. */
  void onAdShown();

  /** Called when the Squeezeback ad is hidden from the screen. */
  void onAdHidden();

  /** Called when the Squeezeback ad fails to show. */
  void onAdFailedToShow(FullScreenContentError error);
}
