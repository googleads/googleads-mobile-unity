/*
 * Copyright (C) 2023 Google, Inc.
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
package com.google.unity.ads.admanager;

import com.google.unity.ads.UnityFullScreenContentCallback;
import com.google.unity.ads.UnityInterstitialAdCallback;
import com.google.unity.ads.UnityPaidEventListener;

/**
 * An interface form of {@link AdManagerInterstitialAdLoadCallback} that can be implemented via
 * {@code AndroidJavaProxy} in Unity to receive ad events synchronously.
 */
public interface UnityAdManagerInterstitialAdCallback
    extends UnityInterstitialAdCallback, UnityPaidEventListener, UnityFullScreenContentCallback {

  void onAppEvent(String name, String data);
}
