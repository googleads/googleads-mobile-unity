package com.google.unity.ads.decagon;

import android.app.Activity;
import android.util.Log;
import androidx.annotation.NonNull;
import androidx.annotation.VisibleForTesting;
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.FullScreenContentError;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.interstitial.InterstitialAd;
import com.google.android.libraries.ads.mobile.sdk.interstitial.InterstitialAdEventCallback;
import com.google.unity.ads.PluginUtils;
import java.util.concurrent.Executor;
import java.util.concurrent.Executors;

/** Interstitial ad implementation for the Google Mobile Ads Unity plugin. */
public class UnityInterstitialAd {

  /** The {@link InterstitialAd}. */
  private InterstitialAd interstitialAd;

  /** The {@code Activity} on which the interstitial ad will display. */
  private final Activity activity;

  /** A callback implemented in Unity via {@code AndroidJavaProxy} to receive ad events. */
  private final UnityInterstitialAdCallback callback;

  private final InterstitialAdWrapper adWrapper;
  private final Executor executor;

  public UnityInterstitialAd(Activity activity, UnityInterstitialAdCallback callback) {
    this(activity, callback, new InterstitialAdWrapper(), Executors.newSingleThreadExecutor());
  }

  @VisibleForTesting
  UnityInterstitialAd(
      Activity activity,
      UnityInterstitialAdCallback callback,
      InterstitialAdWrapper adWrapper,
      Executor executor) {
    this.activity = activity;
    this.callback = callback;
    this.adWrapper = adWrapper;
    this.executor = executor;
  }

  /**
   * Loads an interstitial ad.
   *
   * @param request The {@link AdRequest} object with targeting parameters.
   */
  public void load(final AdRequest request) {
    activity.runOnUiThread(
        () ->
            adWrapper.load(
                request,
                new AdLoadCallback<InterstitialAd>() {
                  @Override
                  public void onAdLoaded(@NonNull InterstitialAd ad) {
                    interstitialAd = ad;
                    executor.execute(
                        () -> {
                          if (callback != null) {
                            callback.onInterstitialAdLoaded();
                          }
                        });
                  }

                  @Override
                  public void onAdFailedToLoad(@NonNull LoadAdError adError) {
                    executor.execute(
                        () -> {
                          if (callback != null) {
                            callback.onInterstitialAdFailedToLoad(adError);
                          }
                        });
                  }
                }));
  }

  /** Shows the interstitial ad if it has loaded. */
  public void show() {
    if (interstitialAd == null) {
      Log.e(
          PluginUtils.LOGTAG,
          "Tried to show intertitial ad before it was ready. Please call loadAd first and wait for"
              + " a successful onAdLoaded callback.");
      return;
    }

    // Listen for ad events.
    interstitialAd.setAdEventCallback(
        new InterstitialAdEventCallback() {
          @Override
          public void onAdShowedFullScreenContent() {
            // Interstitial ad did show.
            executor.execute(
                () -> {
                  if (callback != null) {
                    callback.onAdShowedFullScreenContent();
                  }
                });
          }

          @Override
          public void onAdDismissedFullScreenContent() {
            // Interstitial ad did dismiss.
            executor.execute(
                () -> {
                  if (callback != null) {
                    callback.onAdDismissedFullScreenContent();
                  }
                });

            interstitialAd = null;
          }

          @Override
          public void onAdFailedToShowFullScreenContent(
              @NonNull FullScreenContentError fullScreenContentError) {
            // Interstitial ad failed to show.
            executor.execute(
                () -> {
                  if (callback != null) {
                    callback.onAdFailedToShowFullScreenContent(fullScreenContentError);
                  }
                });
          }

          @Override
          public void onAdImpression() {
            // Interstitial ad did record an impression.
            executor.execute(
                () -> {
                  if (callback != null) {
                    callback.onAdImpression();
                  }
                });
          }

          @Override
          public void onAdClicked() {
            // Interstitial ad did record a click.
            executor.execute(
                () -> {
                  if (callback != null) {
                    callback.onAdClicked();
                  }
                });
          }
        });

    activity.runOnUiThread(
        () -> {
          interstitialAd.setImmersiveMode(true);
          interstitialAd.show(this.activity);
        });
  }
}
