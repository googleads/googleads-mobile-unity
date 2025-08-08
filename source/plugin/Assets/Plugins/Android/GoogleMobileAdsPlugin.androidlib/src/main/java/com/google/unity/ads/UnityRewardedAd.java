/*
 * Copyright (C) 2018 Google LLC
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
import com.google.android.gms.ads.OnUserEarnedRewardListener;
import com.google.android.gms.ads.ResponseInfo;
import com.google.android.gms.ads.rewarded.RewardItem;
import com.google.android.gms.ads.rewarded.RewardedAd;
import com.google.android.gms.ads.rewarded.RewardedAdLoadCallback;
import com.google.android.gms.ads.rewarded.ServerSideVerificationOptions;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.FutureTask;

/**
 * Native rewarded ad implementation for the Google Mobile Ads Unity plugin.
 */
public class UnityRewardedAd {

  /**
   * The {@link RewardedAd}.
   */
  private RewardedAd rewardedAd;

  /** The {@code Activity} on which the rewarded ad will display. */
  private final Activity activity;

  /**
   * A callback implemented in Unity via {@code AndroidJavaProxy} to receive ad events.
   */
  private UnityRewardedAdCallback callback;

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

  public UnityRewardedAd(Activity activity, UnityRewardedAdCallback callback) {
    this.activity = activity;
    this.callback = callback;
  }

  public void setRewardedAd(RewardedAd rewardedAd) {
    this.rewardedAd = rewardedAd;
    activity.runOnUiThread(
        () -> {
          this.rewardedAd.setOnPaidEventListener(onPaidEventListener);
          this.rewardedAd.setFullScreenContentCallback(fullScreenContentCallback);
        });
  }

  /**
   * Loads a rewarded ad.
   *
   * @param request The {@link AdRequest} object with targeting parameters.
   */
  public void loadAd(final String adUnitId, final AdRequest request) {
    activity.runOnUiThread(
        () ->
            RewardedAd.load(
                activity,
                adUnitId,
                request,
                new RewardedAdLoadCallback() {
                  @Override
                  public void onAdLoaded(@NonNull RewardedAd ad) {
                    rewardedAd = ad;
                    rewardedAd.setOnPaidEventListener(onPaidEventListener);
                    rewardedAd.setFullScreenContentCallback(fullScreenContentCallback);
                    new Thread(
                            () -> {
                              if (callback != null) {
                                callback.onRewardedAdLoaded();
                              }
                            })
                        .start();
                  }

                  @Override
                  public void onAdFailedToLoad(final LoadAdError error) {
                    new Thread(
                            () -> {
                              if (callback != null) {
                                callback.onRewardedAdFailedToLoad(error);
                              }
                            })
                        .start();
                  }
                }));
  }

  /**
   * Retrieves the next rewarded ad available in Preload queue, or {@code null} if no ad is
   * available.
   */
  public void pollAd(@NonNull String adUnitId) {
    rewardedAd = RewardedAd.pollAd(activity, adUnitId);
    if (rewardedAd == null) {
      Log.e(PluginUtils.LOGTAG, "Failed to obtain a Rewarded Ad from the preloader.");
      LoadAdError error =
          new LoadAdError(
              AdRequest.ERROR_CODE_INTERNAL_ERROR,
              "Failed to obtain a Rewarded Ad from the preloader.",
              MobileAds.ERROR_DOMAIN,
              /* cause= */ null,
              /* responseInfo= */ null);
      new Thread(
              () -> {
                if (callback != null) {
                  callback.onRewardedAdFailedToLoad(error);
                }
              })
          .start();
      return;
    }

    activity.runOnUiThread(() -> rewardedAd.setOnPaidEventListener(onPaidEventListener));
    rewardedAd.setFullScreenContentCallback(fullScreenContentCallback);
  }

  /** Returns {@code true} if there is a rewarded ad available in the pre-load queue. */
  public boolean isAdAvailable(@NonNull String adUnitId) {
    return RewardedAd.isAdAvailable(activity, adUnitId);
  }

  /**
   * Shows the rewarded ad if it has loaded.
   */
  public void show() {
    if (rewardedAd == null) {
      Log.e(PluginUtils.LOGTAG, "Tried to show rewarded ad before it was ready. This should "
          + "in theory never happen. If it does, please contact the plugin owners.");
      return;
    }
    activity.runOnUiThread(
        () -> {
          rewardedAd.setImmersiveMode(true);
          rewardedAd.show(
              activity,
              new OnUserEarnedRewardListener() {
                @Override
                public void onUserEarnedReward(@NonNull final RewardItem rewardItem) {
                  new Thread(
                          () -> {
                            if (callback != null) {
                              callback.onUserEarnedReward(
                                  rewardItem.getType(), rewardItem.getAmount());
                            }
                          })
                      .start();
                }
              });
        });
  }

  /**
   * Sets server side verification options.
   */
  public void setServerSideVerificationOptions(
      final ServerSideVerificationOptions serverSideVerificationOptions) {
    if (rewardedAd == null) {
      Log.e(PluginUtils.LOGTAG, "Tried set server side verification before it was ready. "
          + "This should in theory never happen. If it does, please contact the plugin owners.");
      return;
    }
    activity.runOnUiThread(
        () -> rewardedAd.setServerSideVerificationOptions(serverSideVerificationOptions));
  }

  /** Returns the {@link RewardedAd} ad unit ID. */
  @Nullable
  public String getAdUnitId() {
    if (rewardedAd == null) {
      return null;
    }
    return rewardedAd.getAdUnitId();
  }

  /** Returns the request response info. */
  @Nullable
  public ResponseInfo getResponseInfo() {
    if (rewardedAd == null) {
      Log.e(PluginUtils.LOGTAG, "Tried to get response info before it was ready. Returning null.");
      return null;
    }
    return rewardedAd.getResponseInfo();
  }

  @Nullable
  public RewardItem getRewardItem() {
    if (rewardedAd == null) {
      Log.e(PluginUtils.LOGTAG, "Tried to get reward item before it was ready. This should "
          + "in theory never happen. If it does, please contact the plugin owners.");
      return null;
    }
    FutureTask<RewardItem> task =
        new FutureTask<>(
            new Callable<RewardItem>() {
              @Override
              public RewardItem call() {
                return rewardedAd.getRewardItem();
              }
            });
    activity.runOnUiThread(task);

    RewardItem result = null;
    try {
      result = task.get();
    } catch (InterruptedException e) {
      Log.e(
          PluginUtils.LOGTAG,
          String.format("Unable to get reward item: %s", e.getLocalizedMessage()));
    } catch (ExecutionException e) {
      Log.e(
          PluginUtils.LOGTAG,
          String.format("Unable to get reward item: %s", e.getLocalizedMessage()));
    }
    return result;
  }

  /**
   * Destroys the {@link RewardedAd}.
   */
  public void destroy() {
    // Currently there is no rewardedAd.destroy() method. This method is a placeholder in case
    // there is any cleanup to do here in the future.
  }
}
