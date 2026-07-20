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

import android.app.Activity;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.annotation.VisibleForTesting;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.PreloadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.PreloadConfiguration;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
import com.google.android.libraries.ads.mobile.sdk.rewardedinterstitial.RewardedInterstitialAd;
import com.google.android.libraries.ads.mobile.sdk.rewardedinterstitial.RewardedInterstitialAdPreloader;
import java.util.Map;
import java.util.concurrent.ExecutorService;

/** Unity implementation of the {@link RewardedInterstitialAdPreloader}. */
public class UnityRewardedInterstitialAdPreloader {

  private final Activity activity;
  private final ExecutorService service;
  private final UnityPreloadCallback preloadCallback;
  private final RewardedInterstitialAdPreloaderWrapper preloaderWrapper;

  public UnityRewardedInterstitialAdPreloader(
      Activity activity, UnityPreloadCallback preloadCallback) {
    this(
        activity,
        preloadCallback,
        new RewardedInterstitialAdPreloaderWrapper(),
        UnityExecutor.getPreloaderExecutor());
  }

  @VisibleForTesting
  public UnityRewardedInterstitialAdPreloader(
      Activity activity,
      UnityPreloadCallback preloadCallback,
      RewardedInterstitialAdPreloaderWrapper preloaderWrapper,
      ExecutorService service) {
    this.activity = activity;
    this.preloadCallback = preloadCallback;
    this.service = service;
    this.preloaderWrapper = preloaderWrapper;
  }

  public boolean start(String preloadId, PreloadConfiguration preloadConfiguration) {
    return preloaderWrapper.start(
        preloadId,
        preloadConfiguration,
        new PreloadCallback() {
          @Override
          public void onAdPreloaded(@NonNull String preloadId, ResponseInfo responseInfo) {
            service.execute(
                () -> {
                  if (preloadCallback != null) {
                    preloadCallback.onAdPreloaded(preloadId, responseInfo);
                  }
                });
          }

          @Override
          public void onAdsExhausted(@NonNull String preloadId) {
            service.execute(
                () -> {
                  if (preloadCallback != null) {
                    preloadCallback.onAdsExhausted(preloadId);
                  }
                });
          }

          @Override
          public void onAdFailedToPreload(@NonNull String preloadId, @NonNull LoadAdError adError) {
            service.execute(
                () -> {
                  if (preloadCallback != null) {
                    preloadCallback.onAdFailedToPreload(preloadId, adError);
                  }
                });
          }
        });
  }

  public boolean isAdAvailable(String preloadId) {
    return preloaderWrapper.isAdAvailable(preloadId);
  }

  public int getNumAdsAvailable(String preloadId) {
    return preloaderWrapper.getNumAdsAvailable(preloadId);
  }

  @Nullable
  public UnityRewardedInterstitialAd pollAd(
      String preloadId, UnityRewardedInterstitialAdCallback callback) {
    RewardedInterstitialAd ad = preloaderWrapper.pollAd(preloadId);
    if (ad == null) {
      return null;
    }
    return new UnityRewardedInterstitialAd(activity, callback, ad);
  }

  @Nullable
  public PreloadConfiguration getConfiguration(String preloadId) {
    return preloaderWrapper.getConfiguration(preloadId);
  }

  public Map<String, PreloadConfiguration> getConfigurations() {
    return preloaderWrapper.getConfigurations();
  }

  public void destroy(String preloadId) {
    boolean unused = preloaderWrapper.destroy(preloadId);
  }

  public void destroyAll() {
    preloaderWrapper.destroyAll();
  }

  /** Wrapper for RewardedInterstitialAdPreloader static methods to facilitate testing. */
  @VisibleForTesting
  public static class RewardedInterstitialAdPreloaderWrapper {
    public boolean start(String preloadId, PreloadConfiguration config, PreloadCallback callback) {
      return RewardedInterstitialAdPreloader.start(preloadId, config, callback);
    }

    public boolean isAdAvailable(String preloadId) {
      return RewardedInterstitialAdPreloader.isAdAvailable(preloadId);
    }

    public int getNumAdsAvailable(String preloadId) {
      return RewardedInterstitialAdPreloader.getNumAdsAvailable(preloadId);
    }

    public RewardedInterstitialAd pollAd(String preloadId) {
      return RewardedInterstitialAdPreloader.pollAd(preloadId);
    }

    public PreloadConfiguration getConfiguration(String preloadId) {
      return RewardedInterstitialAdPreloader.getConfiguration(preloadId);
    }

    public Map<String, PreloadConfiguration> getConfigurations() {
      return RewardedInterstitialAdPreloader.getConfigurations();
    }

    public boolean destroy(String preloadId) {
      return RewardedInterstitialAdPreloader.destroy(preloadId);
    }

    public void destroyAll() {
      RewardedInterstitialAdPreloader.destroyAll();
    }
  }
}
