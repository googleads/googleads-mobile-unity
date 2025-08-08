/*
 * Copyright (C) 2023 Google, Inc.
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
package com.google.unity.ads.admanager;

import android.app.Activity;
import android.util.Log;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import com.google.android.gms.ads.AdError;
import com.google.android.gms.ads.AdValue;
import com.google.android.gms.ads.FullScreenContentCallback;
import com.google.android.gms.ads.LoadAdError;
import com.google.android.gms.ads.OnPaidEventListener;
import com.google.android.gms.ads.ResponseInfo;
import com.google.android.gms.ads.admanager.AdManagerAdRequest;
import com.google.android.gms.ads.admanager.AdManagerInterstitialAd;
import com.google.android.gms.ads.admanager.AdManagerInterstitialAdLoadCallback;
import com.google.android.gms.ads.admanager.AppEventListener;
import com.google.unity.ads.PluginUtils;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.FutureTask;

/** Android AdManager interstitial implementation for the Google Mobile Ads Unity plugin. */
public class UnityAdManagerInterstitialAd {
  // TODO: b/284206705 - Review and fix unit tests for this class.

  /** The {@link AdManagerInterstitialAd}. */
  private AdManagerInterstitialAd adManagerInterstitialAd;

  /** The {@code Activity} on which the interstitial will display. */
  private final Activity activity;

  /** A listener implemented in Unity via {@code AndroidJavaProxy} to receive ad events. */
  @Nullable private final UnityAdManagerInterstitialAdCallback callback;

  private final ExecutorService service = Executors.newSingleThreadExecutor();

  private final FullScreenContentCallback fullScreenContentCallback =
      new FullScreenContentCallback() {
        @Override
        public void onAdFailedToShowFullScreenContent(final AdError error) {
          service.execute(
              () -> {
                if (callback != null) {
                  callback.onAdFailedToShowFullScreenContent(error);
                }
              });
        }

        @Override
        public void onAdShowedFullScreenContent() {
          service.execute(
              () -> {
                if (callback != null) {
                  callback.onAdShowedFullScreenContent();
                }
              });
        }

        @Override
        public void onAdDismissedFullScreenContent() {
          service.execute(
              () -> {
                if (callback != null) {
                  callback.onAdDismissedFullScreenContent();
                }
              });
        }

        @Override
        public void onAdImpression() {
          service.execute(
              () -> {
                if (callback != null) {
                  callback.onAdImpression();
                }
              });
        }

        @Override
        public void onAdClicked() {
          service.execute(
              () -> {
                if (callback != null) {
                  callback.onAdClicked();
                }
              });
        }
      };

  private final OnPaidEventListener onPaidEventListener =
      new OnPaidEventListener() {
        @Override
        public void onPaidEvent(final AdValue adValue) {
          service.execute(
              () -> {
                if (callback != null) {
                  callback.onPaidEvent(
                      adValue.getPrecisionType(),
                      adValue.getValueMicros(),
                      adValue.getCurrencyCode());
                }
              });
        }
      };

  private final AppEventListener appEventListener =
      new AppEventListener() {
        @Override
        public void onAppEvent(String name, String data) {
          service.execute(
              () -> {
                if (callback != null) {
                  callback.onAppEvent(name, data);
                }
              });
        }
      };

  public UnityAdManagerInterstitialAd(
      Activity activity, @Nullable UnityAdManagerInterstitialAdCallback callback) {
    this.activity = activity;
    this.callback = callback;
  }

  /**
   * Loads an Ad Manager interstitial ad.
   *
   * @param adUnitId The ad unit ID.
   * @param request The {@link AdManagerAdRequest} object with targeting parameters.
   */
  public void loadAd(final String adUnitId, final AdManagerAdRequest request) {
    activity.runOnUiThread(
        () ->
            AdManagerInterstitialAd.load(
                activity,
                adUnitId,
                request,
                new AdManagerInterstitialAdLoadCallback() {

                  @Override
                  public void onAdLoaded(@NonNull AdManagerInterstitialAd ad) {
                    adManagerInterstitialAd = ad;
                    adManagerInterstitialAd.setOnPaidEventListener(onPaidEventListener);
                    adManagerInterstitialAd.setAppEventListener(appEventListener);
                    adManagerInterstitialAd.setFullScreenContentCallback(fullScreenContentCallback);

                    service.execute(
                        () -> {
                          if (callback != null) {
                            callback.onInterstitialAdLoaded();
                          }
                        });
                  }

                  @Override
                  public void onAdFailedToLoad(final LoadAdError error) {
                    service.execute(
                        () -> {
                          if (callback != null) {
                            callback.onInterstitialAdFailedToLoad(error);
                          }
                        });
                  }
                }));
  }

  /**
   * Returns {@code true} if there is an ad manager interstitial ad available in the pre-load queue.
   */
  public boolean isAdAvailable(@NonNull String adUnitId) {
    return AdManagerInterstitialAd.isAdAvailable(activity, adUnitId);
  }

  /**
   * Retrieves the next ad manager interstitial ad available in pre-load queue, or {@code null} if
   * no ad is available.
   */
  public void pollAd(@NonNull String adUnitId) {
    adManagerInterstitialAd =
        (AdManagerInterstitialAd) AdManagerInterstitialAd.pollAd(activity, adUnitId);
    if (adManagerInterstitialAd == null) {
      Log.e(
          PluginUtils.LOGTAG, "Failed to obtain an Ad Manager Interstitial Ad from the preloader.");
      return;
    }
    activity.runOnUiThread(
        () -> {
          adManagerInterstitialAd.setOnPaidEventListener(onPaidEventListener);
          adManagerInterstitialAd.setAppEventListener(appEventListener);
        });
    adManagerInterstitialAd.setFullScreenContentCallback(fullScreenContentCallback);
  }

  /** Returns the {@link AdManagerInterstitialAd} ad unit ID. */
  @Nullable
  public String getAdUnitId() {
    if (adManagerInterstitialAd == null) {
      return null;
    }
    return adManagerInterstitialAd.getAdUnitId();
  }

  /** Returns the request response info. */
  @Nullable
  public ResponseInfo getResponseInfo() {
    FutureTask<ResponseInfo> task =
        new FutureTask<>(() -> adManagerInterstitialAd.getResponseInfo());
    activity.runOnUiThread(task);

    ResponseInfo result = null;
    try {
      result = task.get();
    } catch (InterruptedException | ExecutionException exception) {
      Log.e(
          PluginUtils.LOGTAG,
          String.format(
              "Unable to check Ad Manager interstitial response info: %s",
              exception.getLocalizedMessage()));
    }
    return result;
  }

  /** Shows the interstitial if it has loaded. */
  public void show() {
    if (adManagerInterstitialAd == null) {
      Log.e(
          PluginUtils.LOGTAG,
          "Tried to show Ad Manager interstitial ad before it was ready. This should in theory "
              + "never happen. If it does, please contact the plugin owners.");
      return;
    }
    activity.runOnUiThread(() -> adManagerInterstitialAd.show(activity));
  }

  /** Destroys the {@link AdManagerInterstitialAd}. */
  public void destroy() {
    // Currently there is no interstitial.destroy() method. This method is a placeholder in case
    // there is any cleanup to do here in the future.
  }
}
