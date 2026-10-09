/*
 * Copyright (C) 2015 Google, Inc.
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

import com.google.android.gms.ads.LoadAdError;

/**
 * An interface form of {@link UnityAdListener} that can be implemented via {@code AndroidJavaProxy}
 * in Unity to receive ad events synchronously.
 */
public interface UnityAdListener extends UnityPaidEventListener {

    void onAdLoaded();
    void onAdFailedToLoad(LoadAdError error);
    void onAdOpened();
    void onAdClosed();
    void onAdLeftApplication();
    void onAdImpression();
    void onAdClicked();
}
