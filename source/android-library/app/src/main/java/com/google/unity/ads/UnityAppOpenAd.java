/*
 * Copyright (C) 2021 Google LLC
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
import android.util.Log;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import com.google.android.gms.ads.AdError;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.AdValue;
import com.google.android.gms.ads.FullScreenContentCallback;
import com.google.android.gms.ads.LoadAdError;
import com.google.android.gms.ads.MobileAds;
import com.google.android.gms.ads.OnPaidEventListener;
import com.google.android.gms.ads.ResponseInfo;
import com.google.android.gms.ads.appopen.AppOpenAd;
import com.google.android.gms.ads.appopen.AppOpenAd.AppOpenAdLoadCallback;

/**
 * Native app open ad implementation for the Google Mobile Ads Unity plugin.
 */
public class UnityAppOpenAd {

  /** The {@link AppOpenAd}. */
  AppOpenAd appOpenAd;

  /**
   * The {@code Activity} on which the app open ad will display.
   */
  private final Activity activity;

  /**
   * A callback implemented in Unity via {@code AndroidJavaProxy} to receive ad events.
   */
  private final UnityAppOpenAdCallback callback;

  private final OnPaidEventListener onPaidEventListener =
      new OnPaidEventListener() {
        @Override
        public void onPaidEvent(final AdValue adValue) {
          runOnNewThread(
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

  private final FullScreenContentCallback fullScreenContentCallback =
      new FullScreenContentCallback() {
        @Override
        public void onAdFailedToShowFullScreenContent(final AdError error) {
          runOnNewThread(
              () -> {
                if (callback != null) {
                  callback.onAdFailedToShowFullScreenContent(error);
                }
              });
        }

        @Override
        public void onAdShowedFullScreenContent() {
          runOnNewThread(
              () -> {
                if (callback != null) {
                  callback.onAdShowedFullScreenContent();
                }
              });
        }

        @Override
        public void onAdDismissedFullScreenContent() {
          runOnNewThread(
              () -> {
                if (callback != null) {
                  callback.onAdDismissedFullScreenContent();
                }
              });
        }

        @Override
        public void onAdImpression() {
          runOnNewThread(
              () -> {
                if (callback != null) {
                  callback.onAdImpression();
                }
              });
        }

        @Override
        public void onAdClicked() {
          runOnNewThread(
              () -> {
                if (callback != null) {
                  callback.onAdClicked();
                }
              });
        }
      };

  public UnityAppOpenAd(Activity activity, UnityAppOpenAdCallback callback) {
    this.activity = activity;
    this.callback = callback;
  }

  public void loadAd(final String adUnitId, final AdRequest request) {
    activity.runOnUiThread(
        () ->
            AppOpenAd.load(
                activity,
                adUnitId,
                request,
                new AppOpenAdLoadCallback() {
                  @Override
                  public void onAdLoaded(@NonNull AppOpenAd ad) {
                    appOpenAd = ad;
                    appOpenAd.setOnPaidEventListener(onPaidEventListener);
                    appOpenAd.setFullScreenContentCallback(fullScreenContentCallback);

                    runOnNewThread(
                        () -> {
                          if (callback != null) {
                            callback.onAppOpenAdLoaded();
                          }
                        });
                  }

                  @Override
                  public void onAdFailedToLoad(@NonNull final LoadAdError error) {
                    runOnNewThread(
                        () -> {
                          if (callback != null) {
                            callback.onAppOpenAdFailedToLoad(error);
                          }
                        });
                  }
                }));
  }

  void setAppOpenAd(AppOpenAd appOpenAd) {
    this.appOpenAd = appOpenAd;
    activity.runOnUiThread(
        () -> {
          this.appOpenAd.setOnPaidEventListener(onPaidEventListener);
          this.appOpenAd.setFullScreenContentCallback(fullScreenContentCallback);
        });
  }

  /**
   * Retrieves the next app open ad available in Preload queue, or {@code null} if no ad is
   * available.
   */
  public void pollAd(@NonNull String adUnitId) {
    appOpenAd = AppOpenAd.pollAd(activity, adUnitId);
    if (appOpenAd == null) {
      Log.e(PluginUtils.LOGTAG, "Failed to obtain an App Open Ad from the preloader.");
      LoadAdError error =
          new LoadAdError(
              AdRequest.ERROR_CODE_INTERNAL_ERROR,
              "Failed to obtain an App Open Ad from the preloader.",
              MobileAds.ERROR_DOMAIN,
              /* cause= */ null,
              /* responseInfo= */ null);
      new Thread(
              () -> {
                if (callback != null) {
                  callback.onAppOpenAdFailedToLoad(error);
                }
              })
          .start();
      return;
    }
    activity.runOnUiThread(() -> appOpenAd.setOnPaidEventListener(onPaidEventListener));
    appOpenAd.setFullScreenContentCallback(fullScreenContentCallback);
  }

  /** Returns {@code true} if there is an interstitial ad available in the pre-load queue. */
  public boolean isAdAvailable(@NonNull String adUnitId) {
    return AppOpenAd.isAdAvailable(activity, adUnitId);
  }

  public void show() {
    if (appOpenAd == null) {
      Log.e(PluginUtils.LOGTAG, "Tried to show app open ad before it was ready. This should "
          + "in theory never happen. If it does, please contact the plugin owners.");
      return;
    }

    activity.runOnUiThread(
        () -> {
          appOpenAd.setImmersiveMode(true);
          appOpenAd.show(activity);
        });
  }

  /** Returns the {@link AppOpenAd} ad unit ID (null if the ad is not loaded). */
  @Nullable
  public String getAdUnitId() {
    if (appOpenAd == null) {
      return null;
    }
    return appOpenAd.getAdUnitId();
  }

  /** Returns the request response info. */
  @Nullable
  public ResponseInfo getResponseInfo() {
    if (appOpenAd == null) {
      Log.e(PluginUtils.LOGTAG, "Tried to get response info before it was ready. Returning null.");
      return null;
    }
    return appOpenAd.getResponseInfo();
  }

  /**
   * Destroys the {@link AppOpenAd}.
   */
  public void destroy() {
    // Currently there is no appOpenAd.destroy() method. This method is a placeholder
    // in case there is any cleanup to do here in the future.
  }

  private void runOnNewThread(final Runnable action) {
    new Thread(action).start();
  }
}
