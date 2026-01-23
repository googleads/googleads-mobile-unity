package com.google.unity.ads.nextgen;

import android.app.Activity;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.annotation.VisibleForTesting;
import com.google.android.libraries.ads.mobile.sdk.appopen.AppOpenAd;
import com.google.android.libraries.ads.mobile.sdk.appopen.AppOpenAdPreloader;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.PreloadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.PreloadConfiguration;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
import java.util.Map;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

/** Unity implementation of the {@link AppOpenAdPreloader}. */
public class UnityAppOpenAdPreloader {

  /** The {@code Activity} on which the app open ad will display. */
  private final Activity activity;

  /** An executor used to run the callbacks. */
  private final ExecutorService service;

  /**
   * A {@code UnityPreloadCallback} implemented in Unity via {@code AndroidJavaProxy} to receive ad
   * events.
   */
  private final UnityPreloadCallback preloadCallback;

  private final AppOpenAdPreloaderWrapper preloaderWrapper;

  public UnityAppOpenAdPreloader(Activity activity, UnityPreloadCallback preloadCallback) {
    this(
        activity,
        preloadCallback,
        new AppOpenAdPreloaderWrapper(),
        Executors.newSingleThreadExecutor());
  }

  @VisibleForTesting
  public UnityAppOpenAdPreloader(
      Activity activity,
      UnityPreloadCallback preloadCallback,
      AppOpenAdPreloaderWrapper preloaderWrapper,
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
  public UnityAppOpenAd pollAd(String preloadId, UnityAppOpenAdCallback callback) {
    AppOpenAd appOpenAd = preloaderWrapper.pollAd(preloadId);
    if (appOpenAd == null) {
      return null;
    }
    return new UnityAppOpenAd(activity, callback, appOpenAd);
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

  /** Wrapper for AppOpenAdPreloader static methods to facilitate testing. */
  @VisibleForTesting
  public static class AppOpenAdPreloaderWrapper {
    public boolean start(String preloadId, PreloadConfiguration config, PreloadCallback callback) {
      return AppOpenAdPreloader.start(preloadId, config, callback);
    }

    public boolean isAdAvailable(String preloadId) {
      return AppOpenAdPreloader.isAdAvailable(preloadId);
    }

    public int getNumAdsAvailable(String preloadId) {
      return AppOpenAdPreloader.getNumAdsAvailable(preloadId);
    }

    public AppOpenAd pollAd(String preloadId) {
      return AppOpenAdPreloader.pollAd(preloadId);
    }

    public PreloadConfiguration getConfiguration(String preloadId) {
      return AppOpenAdPreloader.getConfiguration(preloadId);
    }

    public Map<String, PreloadConfiguration> getConfigurations() {
      return AppOpenAdPreloader.getConfigurations();
    }

    public boolean destroy(String preloadId) {
      return AppOpenAdPreloader.destroy(preloadId);
    }
  }
}
