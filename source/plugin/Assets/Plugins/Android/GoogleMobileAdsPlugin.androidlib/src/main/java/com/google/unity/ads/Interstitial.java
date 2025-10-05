/*
 * Copyright (C) 2015 Google, Inc.
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
import com.appharbr.sdk.engine.AdStateResult;
import com.appharbr.sdk.engine.AppHarbr;
import com.appharbr.sdk.engine.adformat.AdFormat;
import com.appharbr.sdk.engine.mediators.admob.interstitial.AHAdMobInterstitialAd;
import com.appharbr.unity.mediation.AHUnityMediators;
import com.google.android.gms.ads.AdError;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.AdValue;
import com.google.android.gms.ads.FullScreenContentCallback;
import com.google.android.gms.ads.LoadAdError;
import com.google.android.gms.ads.MobileAds;
import com.google.android.gms.ads.OnPaidEventListener;
import com.google.android.gms.ads.ResponseInfo;
import com.google.android.gms.ads.interstitial.InterstitialAd;
import com.google.android.gms.ads.interstitial.InterstitialAdLoadCallback;

/**
 * Native interstitial implementation for the Google Mobile Ads Unity plugin.
 */
public class Interstitial {

  /**
   * The {@link InterstitialAd}.
   */
  private InterstitialAd interstitialAd;
    //************************************************************************//
    private final AHAdMobInterstitialAd ahAdMobInterstitialAd;
    private InterstitialAdLoadCallback internalCallback;
    private String adUnitId;
    //************************************************************************//

  /** The {@code Activity} on which the interstitial will display. */
  private final Activity activity;

  /** A listener implemented in Unity via {@code AndroidJavaProxy} to receive ad events. */
  private final UnityInterstitialAdCallback callback;

  private final FullScreenContentCallback fullScreenContentCallback =
      new FullScreenContentCallback() {
        @Override
        public void onAdFailedToShowFullScreenContent(final AdError error) {
          new Thread(
                  () -> {
                    if (callback != null) {
                      callback.onAdFailedToShowFullScreenContent(error);
                    }
                  })
              .start();
        }

        @Override
        public void onAdShowedFullScreenContent() {
          new Thread(
                  () -> {
                    if (callback != null) {
                      callback.onAdShowedFullScreenContent();
                    }
                  })
              .start();
        }

        @Override
        public void onAdDismissedFullScreenContent() {
          new Thread(
                  () -> {
                    if (callback != null) {
                      callback.onAdDismissedFullScreenContent();
                    }
                  })
              .start();
        }

        @Override
        public void onAdImpression() {
          new Thread(
                  () -> {
                    if (callback != null) {
                      callback.onAdImpression();
                    }
                  })
              .start();
        }

        @Override
        public void onAdClicked() {
          new Thread(
                  () -> {
                    if (callback != null) {
                      callback.onAdClicked();
                    }
                  })
              .start();
        }
      };

    public Interstitial(Activity activity, UnityInterstitialAdCallback callback) {
        this.activity = activity;
        this.callback = callback;
        //************************************************************************//
        this.ahAdMobInterstitialAd = new AHAdMobInterstitialAd();
        //************************************************************************//
    }

  public void setInterstitialAd(InterstitialAd interstitialAd) {
    this.interstitialAd = interstitialAd;
    activity.runOnUiThread(
        () -> {
          this.interstitialAd.setOnPaidEventListener(onPaidEventListener);
          this.interstitialAd.setFullScreenContentCallback(fullScreenContentCallback);
        });
  }

  private final OnPaidEventListener onPaidEventListener =
      new OnPaidEventListener() {
        @Override
        public void onPaidEvent(final AdValue adValue) {
          new Thread(
                  () -> {
                    if (callback != null) {
                      callback.onPaidEvent(
                          adValue.getPrecisionType(),
                          adValue.getValueMicros(),
                          adValue.getCurrencyCode());
                    }
                  })
              .start();
        }
      };

  /**
   * Loads an interstitial ad.
   *
   * @param adUnitId The ad unit ID.
   * @param request The {@link AdRequest} object with targeting parameters.
   */
  public void loadAd(final String adUnitId, final AdRequest request) {
      //************************************************************************//
      InterstitialAdLoadCallback ahWrapperCallback = null;
      if (AHUnityMediators.isWatchingAdUnitId(AdFormat.INTERSTITIAL, adUnitId)) {
          this.adUnitId = adUnitId;
          ahWrapperCallback = AppHarbr.addInterstitial(AHUnityMediators.mediationSdk,
                  ahAdMobInterstitialAd,
                  mInterstitialAdCallback,
                  AHUnityMediators.ahAnalyze);
      }
      internalCallback = ahWrapperCallback != null ? ahWrapperCallback : mInterstitialAdCallback;
      //************************************************************************//
    activity.runOnUiThread(
        () ->
            InterstitialAd.load(
                activity,
                adUnitId,
                request,
                //************************************************************************//
                internalCallback
                //************************************************************************//
                ));
  }

  private final InterstitialAdLoadCallback mInterstitialAdCallback = new InterstitialAdLoadCallback() {
      @Override
      public void onAdLoaded(@NonNull InterstitialAd ad) {
          interstitialAd = ad;
          interstitialAd.setOnPaidEventListener(onPaidEventListener);
          interstitialAd.setFullScreenContentCallback(fullScreenContentCallback);

          new Thread(
                  () -> {
                      if (callback != null) {
                          callback.onInterstitialAdLoaded();
                      }
                  })
                  .start();
      }

      @Override
      public void onAdFailedToLoad(final LoadAdError error) {
          new Thread(
                  () -> {
                      if (callback != null) {
                          callback.onInterstitialAdFailedToLoad(error);
                      }
                  })
                  .start();
      }
  };

  /** Returns {@code true} if there is an interstitial ad available in the pre-load queue. */
  public boolean isAdAvailable(@NonNull String adUnitId) {
    return InterstitialAd.isAdAvailable(activity, adUnitId);
  }

  /**
   * Retrieves the next interstitial ad available in pre-load queue, or {@code null} if no ad is
   * available.
   */
  public void pollAd(@NonNull String adUnitId) {
      //************************************************************************//
      if (AHUnityMediators.isWatchingAdUnitId(AdFormat.INTERSTITIAL, adUnitId)) {
          Log.w("AppHarbrUnityPackage", "AppHarbr Do Not Support Pre Loading");
      }
      //************************************************************************//
    interstitialAd = InterstitialAd.pollAd(activity, adUnitId);
    if (interstitialAd == null) {
      Log.e(PluginUtils.LOGTAG, "Failed to obtain an Interstitial Ad from the preloader.");
      LoadAdError error =
          new LoadAdError(
              AdRequest.ERROR_CODE_INTERNAL_ERROR,
              "Failed to obtain an Interstitial Ad from the preloader.",
              MobileAds.ERROR_DOMAIN,
              /* cause= */ null,
              /* responseInfo= */ null);
      new Thread(
              () -> {
                if (callback != null) {
                  callback.onInterstitialAdFailedToLoad(error);
                }
              })
          .start();
      return;
    }
    activity.runOnUiThread(() -> interstitialAd.setOnPaidEventListener(onPaidEventListener));
    interstitialAd.setFullScreenContentCallback(fullScreenContentCallback);
    // TODO(vkini): Check if this callback is needed as other ad formats don't have it.
    if (callback != null) {
      callback.onInterstitialAdLoaded();
    }
  }

  /** Gets the placement ID for the {@link InterstitialAd}. */
  public long getPlacementId() {
    if (interstitialAd == null) {
      return 0;
    }
    return interstitialAd.getPlacementId();
  }

  /**
   * Sets a placement ID for the {@link InterstitialAd}.
   *
   * <p>To ensure this placement ID is included in reporting, call this method before showing the
   * ad.
   *
   * @param placementId A long integer provided by the AdMob UI for the configured placement.
   */
  public void setPlacementId(long placementId) {
    if (interstitialAd == null) {
      return;
    }
    interstitialAd.setPlacementId(placementId);
  }

  /** Returns the {@link InterstitialAd} ad unit ID. */
  @Nullable
  public String getAdUnitId() {
    if (interstitialAd == null) {
      return null;
    }
    return interstitialAd.getAdUnitId();
  }

  /** Returns the request response info. */
  @Nullable
  public ResponseInfo getResponseInfo() {
    if (interstitialAd == null) {
      Log.e(PluginUtils.LOGTAG, "Tried to get response info before it was ready. Returning null.");
      return null;
    }
    return interstitialAd.getResponseInfo();
  }

  /**
   * Shows the interstitial if it has loaded.
   */
  public void show() {
    if (interstitialAd == null) {
      Log.e(PluginUtils.LOGTAG, "Tried to show interstitial ad before it was ready. "
          + "This should in theory never happen. If it does, please contact the plugin owners.");
      return;
    }
      //************************************************************************//
      if (AHUnityMediators.isWatchingAdUnitId(AdFormat.INTERSTITIAL, getAdUnitId())) {
          if (ahAdMobInterstitialAd.getStatus() == AdStateResult.BLOCKED) {
              return;
          }
      }
      //************************************************************************//
    activity.runOnUiThread(
        () -> {
          interstitialAd.setImmersiveMode(true);
          interstitialAd.show(activity);
        });
  }

  /**
   * Destroys the {@link InterstitialAd}.
   */
  public void destroy() {
    // Currently there is no interstitial.destroy() method. This method is a placeholder in case
    // there is any cleanup to do here in the future.
      //************************************************************************//
      AHUnityMediators.unwatch(AdFormat.INTERSTITIAL, adUnitId, ahAdMobInterstitialAd);
      //************************************************************************//
  }
}
