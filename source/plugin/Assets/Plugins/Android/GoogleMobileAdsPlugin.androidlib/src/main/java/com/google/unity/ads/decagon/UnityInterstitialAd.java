package com.google.unity.ads.decagon;

import android.app.Activity;
import android.util.Log;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.annotation.VisibleForTesting;
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.FullScreenContentError;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
import com.google.android.libraries.ads.mobile.sdk.interstitial.InterstitialAd;
import com.google.android.libraries.ads.mobile.sdk.interstitial.InterstitialAdEventCallback;
import com.google.unity.ads.PluginUtils;
import java.util.concurrent.Executor;
import java.util.concurrent.Executors;

/** Interstitial ad implementation for the Google Mobile Ads Unity plugin. */
public class UnityInterstitialAd extends UnityAdBase<InterstitialAd, UnityInterstitialAdCallback> {

  private final AdWrapper<InterstitialAd> adWrapper;

  public UnityInterstitialAd(Activity activity, UnityInterstitialAdCallback callback) {
    this(activity, callback, AdWrapper.forInterstitial(), Executors.newSingleThreadExecutor());
  }

  @VisibleForTesting
  UnityInterstitialAd(
      Activity activity,
      UnityInterstitialAdCallback callback,
      AdWrapper<InterstitialAd> adWrapper,
      Executor executor) {
    super(activity, callback, executor);
    this.adWrapper = adWrapper;
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
                    UnityInterstitialAd.this.ad = ad;
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
    if (ad == null) {
      Log.e(
          PluginUtils.LOGTAG,
          "Tried to show intertitial ad before it was ready. Please call loadAd first and wait for"
              + " a successful onAdLoaded callback.");
      return;
    }

    // Listen for ad events.
    ad.setAdEventCallback(
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
            ad = null;
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
          ad.setImmersiveMode(true);
          ad.show(this.activity);
        });
  }

  /** Returns the request response info. */
  @Nullable
  public ResponseInfo getResponseInfo() {
    if (ad == null) {
      Log.e(PluginUtils.LOGTAG, "Tried to get response info before it was ready. Returning null.");
      return null;
    }
    return ad.getResponseInfo();
  }
}
