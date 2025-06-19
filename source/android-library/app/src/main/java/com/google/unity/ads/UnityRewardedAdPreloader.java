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
import com.google.android.gms.ads.preload.PreloadCallbackV2;
import com.google.android.gms.ads.preload.PreloadConfiguration;
import com.google.android.gms.ads.rewarded.RewardedAd;
import com.google.android.gms.ads.rewarded.RewardedAdPreloader;
import java.util.Map;

/** Unity implementation of the {@link RewardedAdPreloader}. */
public class UnityRewardedAdPreloader {

  /** The {@code Activity} on which the app open ad will display. */
  private final Activity activity;

  /** A preloadCallback implemented in Unity via {@code AndroidJavaProxy} to receive ad events. */
  private final UnityPreloadCallback preloadCallback;

  public UnityRewardedAdPreloader(Activity activity, UnityPreloadCallback preloadCallback) {
    this.activity = activity;
    this.preloadCallback = preloadCallback;
  }

  public boolean start(String preloadId, PreloadConfiguration preloadConfiguration) {
    return RewardedAdPreloader.start(
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
    return RewardedAdPreloader.isAdAvailable(preloadId);
  }

  public int getNumAdsAvailable(String preloadId) {
    return RewardedAdPreloader.getNumAdsAvailable(preloadId);
  }

  public boolean destroy(String preloadId) {
    return RewardedAdPreloader.destroy(preloadId);
  }

  @Nullable
  public UnityRewardedAd pollAd(String preloadId, UnityRewardedAdCallback callback) {
    RewardedAd rewardedAd = RewardedAdPreloader.pollAd(preloadId);
    if (rewardedAd == null) {
      return null;
    }
    UnityRewardedAd unityRewardedAd = new UnityRewardedAd(activity, callback);
    unityRewardedAd.setRewardedAd(rewardedAd);
    return unityRewardedAd;
  }

  @Nullable
  public PreloadConfiguration getConfiguration(String preloadId) {
    return RewardedAdPreloader.getConfiguration(preloadId);
  }

  public Map<String, PreloadConfiguration> getConfigurations() {
    return RewardedAdPreloader.getConfigurations();
  }

  public void destroyAll() {
    RewardedAdPreloader.destroyAll();
  }

  private void runOnNewThread(final Runnable action) {
    new Thread(action).start();
  }
}
