package com.google.unity.ads;

import android.app.Activity;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import com.google.android.gms.ads.AdError;
import com.google.android.gms.ads.ResponseInfo;
import com.google.android.gms.ads.appopen.AppOpenAd;
import com.google.android.gms.ads.appopen.AppOpenAdPreloader;
import com.google.android.gms.ads.preload.PreloadCallbackV2;
import com.google.android.gms.ads.preload.PreloadConfiguration;
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

  public UnityAppOpenAdPreloader(Activity activity, UnityPreloadCallback preloadCallback) {
    this.activity = activity;
    this.preloadCallback = preloadCallback;
    service = Executors.newSingleThreadExecutor();
  }

  public boolean start(String preloadId, PreloadConfiguration preloadConfiguration) {
    return AppOpenAdPreloader.start(
        preloadId,
        preloadConfiguration,
        new PreloadCallbackV2() {
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
          public void onAdFailedToPreload(@NonNull String preloadId, @NonNull AdError adError) {
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
    return AppOpenAdPreloader.isAdAvailable(preloadId);
  }

  public int getNumAdsAvailable(String preloadId) {
    return AppOpenAdPreloader.getNumAdsAvailable(preloadId);
  }

  @Nullable
  public UnityAppOpenAd pollAd(String preloadId, UnityAppOpenAdCallback callback) {
    AppOpenAd appOpenAd = AppOpenAdPreloader.pollAd(preloadId);
    if (appOpenAd == null) {
      return null;
    }
    UnityAppOpenAd unityAppOpenAd = new UnityAppOpenAd(activity, callback);
    unityAppOpenAd.setAppOpenAd(appOpenAd);
    return unityAppOpenAd;
  }

  @Nullable
  public PreloadConfiguration getConfiguration(String preloadId) {
    return AppOpenAdPreloader.getConfiguration(preloadId);
  }

  public Map<String, PreloadConfiguration> getConfigurations() {
    return AppOpenAdPreloader.getConfigurations();
  }

  public void destroy(String preloadId) {
    boolean unused = AppOpenAdPreloader.destroy(preloadId);
  }

  public void destroyAll() {
    AppOpenAdPreloader.destroyAll();
  }
}
