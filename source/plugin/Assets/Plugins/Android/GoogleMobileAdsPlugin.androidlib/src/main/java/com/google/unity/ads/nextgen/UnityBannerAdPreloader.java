package com.google.unity.ads.nextgen;

import android.app.Activity;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.annotation.VisibleForTesting;
import com.google.android.libraries.ads.mobile.sdk.banner.BannerAd;
import com.google.android.libraries.ads.mobile.sdk.banner.BannerAdPreloader;
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.PreloadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.PreloadConfiguration;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
import java.util.Map;
import java.util.concurrent.ExecutorService;

/** Unity implementation of the {@link BannerAdPreloader}. */
public class UnityBannerAdPreloader {

  /** The {@code Activity} on which the banner ad will display. */
  private final Activity activity;

  /** An executor used to run the callbacks. */
  private final ExecutorService service;

  /**
   * A {@code UnityPreloadCallback} implemented in Unity via {@code AndroidJavaProxy} to receive ad
   * events.
   */
  private final UnityPreloadCallback preloadCallback;

  private final BannerAdPreloaderWrapper preloaderWrapper;

  public UnityBannerAdPreloader(Activity activity, UnityPreloadCallback preloadCallback) {
    this(
        activity, preloadCallback, new BannerAdPreloaderWrapper(), PreloaderExecutor.getExecutor());
  }

  @VisibleForTesting
  public UnityBannerAdPreloader(
      Activity activity,
      UnityPreloadCallback preloadCallback,
      BannerAdPreloaderWrapper preloaderWrapper,
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

  @SuppressWarnings("VisibleForTests")
  @Nullable
  public UnityBannerAd pollAd(String preloadId, UnityBannerAdCallback callback) {
    final BannerAd pooledAd = preloaderWrapper.pollAd(preloadId);
    if (pooledAd == null) {
      return null;
    }
    AdWrapper<BannerAd> preloadedWrapper =
        new AdWrapper<BannerAd>(
            new AdWrapper.AdLoader<BannerAd>() {
              @Override
              public void load(AdRequest adRequest, AdLoadCallback<BannerAd> loadCallback) {
                loadCallback.onAdLoaded(pooledAd);
              }
            });
    return new UnityBannerAd(activity, callback, preloadedWrapper);
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

  /** Wrapper for BannerAdPreloader static methods to facilitate testing. */
  @VisibleForTesting
  public static class BannerAdPreloaderWrapper {
    public boolean start(String preloadId, PreloadConfiguration config, PreloadCallback callback) {
      return BannerAdPreloader.start(preloadId, config, callback);
    }

    public boolean isAdAvailable(String preloadId) {
      return BannerAdPreloader.isAdAvailable(preloadId);
    }

    public int getNumAdsAvailable(String preloadId) {
      return BannerAdPreloader.getNumAdsAvailable(preloadId);
    }

    public BannerAd pollAd(String preloadId) {
      return BannerAdPreloader.pollAd(preloadId);
    }

    public PreloadConfiguration getConfiguration(String preloadId) {
      return BannerAdPreloader.getConfiguration(preloadId);
    }

    public Map<String, PreloadConfiguration> getConfigurations() {
      return BannerAdPreloader.getConfigurations();
    }

    public boolean destroy(String preloadId) {
      return BannerAdPreloader.destroy(preloadId);
    }
  }
}
