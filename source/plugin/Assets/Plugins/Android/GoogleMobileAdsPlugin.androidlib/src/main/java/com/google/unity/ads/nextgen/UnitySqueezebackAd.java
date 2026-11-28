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
import android.util.Log;
import androidx.annotation.ColorInt;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.annotation.VisibleForTesting;
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdValue;
import com.google.android.libraries.ads.mobile.sdk.common.FullScreenContentError;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
import com.google.android.libraries.ads.mobile.sdk.squeezeback.SqueezebackAd;
import com.google.android.libraries.ads.mobile.sdk.squeezeback.SqueezebackAdEventCallback;
import com.google.android.libraries.ads.mobile.sdk.squeezeback.SqueezebackAdOptions;
import com.google.android.libraries.ads.mobile.sdk.squeezeback.SqueezebackAdRequest;
import java.util.concurrent.Executor;

/** Native Java implementation for Squeezeback ads in the Google Mobile Ads Unity plugin. */
public class UnitySqueezebackAd {

  private static final String LOG_TAG = "GoogleMobileAds";

  private final Activity activity;
  @Nullable private final UnitySqueezebackAdCallback callback;
  private final AdWrapper<SqueezebackAd> adWrapper;
  private final Executor executor;
  @Nullable private volatile SqueezebackAd squeezebackAd;

  public UnitySqueezebackAd(Activity activity, @Nullable UnitySqueezebackAdCallback callback) {
    this(activity, callback, AdWrapper.forSqueezeback(), UnityExecutor.getExecutor());
  }

  @VisibleForTesting
  UnitySqueezebackAd(
      Activity activity,
      @Nullable UnitySqueezebackAdCallback callback,
      AdWrapper<SqueezebackAd> adWrapper,
      Executor executor) {
    this.activity = activity;
    this.callback = callback;
    this.adWrapper = adWrapper;
    this.executor = executor;
  }

  /**
   * Loads a Squeezeback ad.
   *
   * @param request The native {@link SqueezebackAdRequest} object.
   */
  public void load(final SqueezebackAdRequest request) {
    try {
      adWrapper.load(
          request,
          new AdLoadCallback<SqueezebackAd>() {
            @Override
            public void onAdLoaded(@NonNull SqueezebackAd ad) {
              ad.setAdEventCallback(createEventCallback());
              squeezebackAd = ad;
              executor.execute(() -> {
                if (callback != null) {
                  callback.onAdLoaded();
                }
              });
            }

            @Override
            public void onAdFailedToLoad(@NonNull LoadAdError loadAdError) {
              executor.execute(() -> {
                if (callback != null) {
                  callback.onAdFailedToLoad(loadAdError);
                }
              });
            }
          });
    } catch (Throwable t) {
      Log.e(LOG_TAG, "Failed to load SqueezebackAd: " + t, t);
      LoadAdError error =
          new LoadAdError(
              LoadAdError.ErrorCode.INTERNAL_ERROR,
              t.getMessage() != null ? t.getMessage() : t.toString(),
              /* responseInfo= */ null);
      executor.execute(() -> {
        if (callback != null) {
          callback.onAdFailedToLoad(error);
        }
      });
    }
  }

  /** Shows the Squeezeback ad with default options. */
  public void show() {
    showWithOptions(/* options= */ null);
  }

  /**
   * Shows the Squeezeback ad with the specified fallback background color.
   *
   * @param fallbackBackgroundColor The fallback ARGB background color ({@code 0xAARRGGBB}).
   */
  public void show(@ColorInt int fallbackBackgroundColor) {
    SqueezebackAdOptions options =
        new SqueezebackAdOptions.Builder()
            .setFallbackBackgroundColor(fallbackBackgroundColor)
            .build();
    showWithOptions(options);
  }

  /** Hides the Squeezeback ad. */
  public void hide() {
    activity.runOnUiThread(() -> {
      SqueezebackAd loadedAd = squeezebackAd;
      if (loadedAd != null) {
        loadedAd.hide();
      }
    });
  }

  /** Destroys the Squeezeback ad and cleans up references. */
  public void destroy() {
    activity.runOnUiThread(() -> {
      SqueezebackAd loadedAd = squeezebackAd;
      squeezebackAd = null;
      if (loadedAd != null) {
        loadedAd.destroy();
      }
    });
  }

  /** Returns {@link ResponseInfo} for the loaded ad, if available. */
  @Nullable
  public ResponseInfo getResponseInfo() {
    SqueezebackAd loadedAd = squeezebackAd;
    return loadedAd != null ? loadedAd.getResponseInfo() : null;
  }

  /** Returns the {@link SqueezebackAd} ad unit ID, if available. */
  @Nullable
  public String getAdUnitId() {
    SqueezebackAd loadedAd = squeezebackAd;
    return loadedAd != null ? loadedAd.getAdUnitId() : null;
  }

  private void showWithOptions(@Nullable SqueezebackAdOptions options) {
    activity.runOnUiThread(() -> {
      SqueezebackAd loadedAd = squeezebackAd;
      if (loadedAd == null) {
        Log.e(LOG_TAG, "Failed to show SqueezebackAd: ad is null.");
        return;
      }
      if (options != null) {
        loadedAd.show(activity, options);
      } else {
        loadedAd.show(activity);
      }
    });
  }

  private SqueezebackAdEventCallback createEventCallback() {
    return new SqueezebackAdEventCallback() {
      @Override
      public void onAdShown() {
        executor.execute(() -> {
          if (callback != null) {
            callback.onAdShown();
          }
        });
      }

      @Override
      public void onAdHidden() {
        executor.execute(() -> {
          if (callback != null) {
            callback.onAdHidden();
          }
        });
      }

      @Override
      public void onAdFailedToShow(@NonNull FullScreenContentError error) {
        executor.execute(() -> {
          if (callback != null) {
            callback.onAdFailedToShow(error);
          }
        });
      }

      @Override
      public void onAdClicked() {
        executor.execute(() -> {
          if (callback != null) {
            callback.onAdClicked();
          }
        });
      }

      @Override
      public void onAdImpression() {
        executor.execute(() -> {
          if (callback != null) {
            callback.onAdImpression();
          }
        });
      }

      @Override
      public void onAdShowedFullScreenContent() {
        executor.execute(() -> {
          if (callback != null) {
            callback.onAdShowedFullScreenContent();
          }
        });
      }

      @Override
      public void onAdDismissedFullScreenContent() {
        executor.execute(() -> {
          if (callback != null) {
            callback.onAdDismissedFullScreenContent();
          }
        });
      }

      @Override
      public void onAdFailedToShowFullScreenContent(
          @NonNull FullScreenContentError fullScreenContentError) {
        executor.execute(() -> {
          if (callback != null) {
            callback.onAdFailedToShowFullScreenContent(fullScreenContentError);
          }
        });
      }

      @Override
      public void onAdPaid(@NonNull AdValue adValue) {
        executor.execute(() -> {
          if (callback != null) {
            callback.onPaidEvent(
                Util.getAdValuePrecisionType(adValue.getPrecisionType()),
                adValue.getValueMicros(),
                adValue.getCurrencyCode());
          }
        });
      }
    };
  }
}
