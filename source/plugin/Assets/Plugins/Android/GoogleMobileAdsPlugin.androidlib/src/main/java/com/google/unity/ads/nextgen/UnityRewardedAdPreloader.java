package com.google.unity.ads.nextgen;

import android.app.Activity;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.annotation.VisibleForTesting;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.PreloadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.PreloadConfiguration;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
import com.google.android.libraries.ads.mobile.sdk.rewarded.RewardedAd;
import com.google.android.libraries.ads.mobile.sdk.rewarded.RewardedAdPreloader;
import java.util.Map;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

/** Unity implementation of the {@link RewardedAdPreloader}. */
public class UnityRewardedAdPreloader {

  /** The {@code Activity} on which the rewarded ad will display. */
  private final Activity activity;

  /** An executor used to run the callbacks. */
  private final ExecutorService service;

  /**
   * A {@code UnityPreloadCallback} implemented in Unity via {@code AndroidJavaProxy} to receive ad
   * events.
   */
  private final UnityPreloadCallback preloadCallback;

  private final RewardedAdPreloaderWrapper preloaderWrapper;

  public UnityRewardedAdPreloader(Activity activity, UnityPreloadCallback preloadCallback) {
    this(
        activity,
        preloadCallback,
        new RewardedAdPreloaderWrapper(),
        Executors.newSingleThreadExecutor());
  }

  @VisibleForTesting
  public UnityRewardedAdPreloader(
      Activity activity,
      UnityPreloadCallback preloadCallback,
      RewardedAdPreloaderWrapper preloaderWrapper,
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
  public UnityRewardedAd pollAd(String preloadId, UnityRewardedAdCallback callback) {
    RewardedAd rewardedAd = preloaderWrapper.pollAd(preloadId);
    if (rewardedAd == null) {
      return null;
    }
    return new UnityRewardedAd(activity, callback, rewardedAd);
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

  /** Wrapper for RewardedAdPreloader static methods to facilitate testing. */
  @VisibleForTesting
  public static class RewardedAdPreloaderWrapper {
    public boolean start(String preloadId, PreloadConfiguration config, PreloadCallback callback) {
      return RewardedAdPreloader.start(preloadId, config, callback);
    }

    public boolean isAdAvailable(String preloadId) {
      return RewardedAdPreloader.isAdAvailable(preloadId);
    }

    public int getNumAdsAvailable(String preloadId) {
      return RewardedAdPreloader.getNumAdsAvailable(preloadId);
    }

    public RewardedAd pollAd(String preloadId) {
      return RewardedAdPreloader.pollAd(preloadId);
    }

    public PreloadConfiguration getConfiguration(String preloadId) {
      return RewardedAdPreloader.getConfiguration(preloadId);
    }

    public Map<String, PreloadConfiguration> getConfigurations() {
      return RewardedAdPreloader.getConfigurations();
    }

    public boolean destroy(String preloadId) {
      return RewardedAdPreloader.destroy(preloadId);
    }
  }
}
