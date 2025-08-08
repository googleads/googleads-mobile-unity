/*
 * Copyright (C) 2025 Google LLC
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
import androidx.annotation.Nullable;
import com.google.android.gms.ads.AdError;
import com.google.android.gms.ads.ResponseInfo;
import com.google.android.gms.ads.interstitial.InterstitialAd;
import com.google.android.gms.ads.interstitial.InterstitialAdPreloader;
import com.google.android.gms.ads.preload.PreloadCallbackV2;
import com.google.android.gms.ads.preload.PreloadConfiguration;
import java.util.Map;

/** Unity implementation of the {@link InterstitialAdPreloader}. */
public class UnityInterstitialAdPreloader {

  /** The {@code Activity} on which the interstitial ad will display. */
  private final Activity activity;

  /** A preloadCallback implemented in Unity via {@code AndroidJavaProxy} to receive ad events. */
  private final UnityPreloadCallback preloadCallback;

  public UnityInterstitialAdPreloader(Activity activity, UnityPreloadCallback preloadCallback) {
    this.activity = activity;
    this.preloadCallback = preloadCallback;
  }

  public boolean start(String preloadId, PreloadConfiguration preloadConfiguration) {
    return InterstitialAdPreloader.start(
        preloadId,
        preloadConfiguration,
        new PreloadCallbackV2() {
          @Override
          public void onAdPreloaded(@NonNull String preloadId, ResponseInfo responseInfo) {
            runOnNewThread(
                () -> {
                  if (preloadCallback != null) {
                    preloadCallback.onAdPreloaded(preloadId, responseInfo);
                  }
                });
          }

          @Override
          public void onAdsExhausted(@NonNull String preloadId) {
            runOnNewThread(
                () -> {
                  if (preloadCallback != null) {
                    preloadCallback.onAdsExhausted(preloadId);
                  }
                });
          }

          @Override
          public void onAdFailedToPreload(@NonNull String preloadId, @NonNull AdError adError) {
            runOnNewThread(
                () -> {
                  if (preloadCallback != null) {
                    preloadCallback.onAdFailedToPreload(preloadId, adError);
                  }
                });
          }
        });
  }

  public boolean isAdAvailable(String preloadId) {
    return InterstitialAdPreloader.isAdAvailable(preloadId);
  }

  public int getNumAdsAvailable(String preloadId) {
    return InterstitialAdPreloader.getNumAdsAvailable(preloadId);
  }

  public void destroy(String preloadId) {
    boolean unused = InterstitialAdPreloader.destroy(preloadId);
  }

  @Nullable
  public Interstitial pollAd(String preloadId, UnityInterstitialAdCallback callback) {
    InterstitialAd rewardedAd = InterstitialAdPreloader.pollAd(preloadId);
    if (rewardedAd == null) {
      return null;
    }
    Interstitial unityInterstitialAd = new Interstitial(activity, callback);
    unityInterstitialAd.setInterstitialAd(rewardedAd);
    return unityInterstitialAd;
  }

  @Nullable
  public PreloadConfiguration getConfiguration(String preloadId) {
    return InterstitialAdPreloader.getConfiguration(preloadId);
  }

  public Map<String, PreloadConfiguration> getConfigurations() {
    return InterstitialAdPreloader.getConfigurations();
  }

  public void destroyAll() {
    InterstitialAdPreloader.destroyAll();
  }

  private void runOnNewThread(final Runnable action) {
    new Thread(action).start();
  }
}
