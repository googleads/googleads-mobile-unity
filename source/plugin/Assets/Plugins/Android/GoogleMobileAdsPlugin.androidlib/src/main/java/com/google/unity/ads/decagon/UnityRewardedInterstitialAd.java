/*
 * Copyright (C) 2025 Google LLC
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

package com.google.unity.ads.decagon;

import android.app.Activity;
import android.util.Log;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.annotation.VisibleForTesting;
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.AdValue;
import com.google.android.libraries.ads.mobile.sdk.common.FullScreenContentError;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
import com.google.android.libraries.ads.mobile.sdk.rewarded.OnUserEarnedRewardListener;
import com.google.android.libraries.ads.mobile.sdk.rewarded.RewardItem;
import com.google.android.libraries.ads.mobile.sdk.rewardedinterstitial.RewardedInterstitialAd;
import com.google.android.libraries.ads.mobile.sdk.rewardedinterstitial.RewardedInterstitialAdEventCallback;
import com.google.unity.ads.PluginUtils;
import java.util.concurrent.Executor;
import java.util.concurrent.Executors;

/** Rewarded ad implementation for the Google Mobile Ads Unity plugin. */
public class UnityRewardedInterstitialAd
    extends UnityAdBase<RewardedInterstitialAd, UnityRewardedInterstitialAdCallback> {

  private final AdWrapper<RewardedInterstitialAd> adWrapper;

  public UnityRewardedInterstitialAd(
      Activity activity, UnityRewardedInterstitialAdCallback callback) {
    this(
        activity,
        callback,
        AdWrapper.forRewardedInterstitial(),
        Executors.newSingleThreadExecutor());
  }

  @VisibleForTesting
  UnityRewardedInterstitialAd(
      Activity activity,
      UnityRewardedInterstitialAdCallback callback,
      AdWrapper<RewardedInterstitialAd> adWrapper,
      Executor executor) {
    super(activity, callback, executor);
    this.adWrapper = adWrapper;
  }

  /**
   * Loads a rewarded interstitial ad.
   *
   * @param request The {@link AdRequest} object with targeting parameters.
   */
  public void load(final AdRequest request) {
    activity.runOnUiThread(
        () ->
            this.adWrapper.load(
                request,
                new AdLoadCallback<RewardedInterstitialAd>() {
                  @Override
                  public void onAdLoaded(@NonNull RewardedInterstitialAd rewardedInterstitialAd) {
                    // Rewarded interstitial ad loaded.
                    UnityRewardedInterstitialAd.this.ad = rewardedInterstitialAd;
                    executor.execute(
                        () -> {
                          if (callback != null) {
                            callback.onRewardedInterstitialAdLoaded();
                          }
                        });
                  }

                  @Override
                  public void onAdFailedToLoad(@NonNull LoadAdError adError) {
                    // Rewarded interstitial ad failed to load.
                    executor.execute(
                        () -> {
                          if (callback != null) {
                            callback.onRewardedInterstitialAdFailedToLoad(adError);
                          }
                        });
                    ad = null;
                  }
                }));
  }

  /** Shows the rewarded interstitial ad if it has loaded. */
  @SuppressWarnings("EnumOrdinal")
  public void show() {
    if (ad == null) {
      Log.e(
          PluginUtils.LOGTAG,
          "Tried to show rewarded interstitial ad before it was ready. Please call load first and"
              + " wait for a successful onAdLoaded callback.");
      return;
    }

    // Listen for ad events.
    ad.setAdEventCallback(
        new RewardedInterstitialAdEventCallback() {
          @Override
          public void onAdShowedFullScreenContent() {
            executor.execute(
                () -> {
                  if (callback != null) {
                    callback.onAdShowedFullScreenContent();
                  }
                });
          }

          @Override
          public void onAdDismissedFullScreenContent() {
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
            executor.execute(
                () -> {
                  if (callback != null) {
                    callback.onAdFailedToShowFullScreenContent(fullScreenContentError);
                  }
                });
            ad = null;
          }

          @Override
          public void onAdImpression() {
            executor.execute(
                () -> {
                  if (callback != null) {
                    callback.onAdImpression();
                  }
                });
          }

          @Override
          public void onAdClicked() {
            executor.execute(
                () -> {
                  if (callback != null) {
                    callback.onAdClicked();
                  }
                });
          }

          @Override
          public void onAdPaid(@NonNull AdValue adValue) {
            executor.execute(
                () -> {
                  if (callback != null) {
                    callback.onPaidEvent(
                        // TODO(vkini): Remove this cast to int and use Utility method to convert to
                        // int.
                        adValue.getPrecisionType().ordinal(),
                        adValue.getValueMicros(),
                        adValue.getCurrencyCode());
                  }
                });
          }
        });

    activity.runOnUiThread(
        () -> {
          ad.setImmersiveMode(true);
          ad.show(
              this.activity,
              new OnUserEarnedRewardListener() {
                @Override
                public void onUserEarnedReward(@NonNull RewardItem rewardItem) {
                  executor.execute(
                      () -> {
                        if (callback != null) {
                          callback.onUserEarnedReward(rewardItem.getType(), rewardItem.getAmount());
                        }
                      });
                }
              });
        });
  }

  /** Gets the placement ID for the {@link RewardedInterstitialAd}. */
  public long getPlacementId() {
    if (ad == null) {
      return 0;
    }
    return ad.getPlacementId();
  }

  /**
   * Sets a placement ID for the {@link RewardedInterstitialAd}.
   *
   * <p>To ensure this placement ID is included in reporting, call this method before showing the
   * ad.
   *
   * @param placementId A long integer provided by the AdMob UI for the configured placement.
   */
  public void setPlacementId(long placementId) {
    if (ad == null) {
      return;
    }
    ad.setPlacementId(placementId);
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
