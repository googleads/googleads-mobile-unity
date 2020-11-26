/*
 * Copyright (C) 2020 Google LLC
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
import com.google.android.gms.ads.AdError;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.AdValue;
import com.google.android.gms.ads.FullScreenContentCallback;
import com.google.android.gms.ads.LoadAdError;
import com.google.android.gms.ads.OnPaidEventListener;
import com.google.android.gms.ads.OnUserEarnedRewardListener;
import com.google.android.gms.ads.ResponseInfo;
import com.google.android.gms.ads.rewarded.RewardItem;
import com.google.android.gms.ads.rewarded.ServerSideVerificationOptions;
import com.google.android.gms.ads.rewardedinterstitial.RewardedInterstitialAd;
import com.google.android.gms.ads.rewardedinterstitial.RewardedInterstitialAdLoadCallback;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.FutureTask;

/** Native rewarded interstitial ad implementation for the Google Mobile Ads Unity plugin. */
public class UnityRewardedInterstitialAd {

  /** The {@link RewardedInterstitialAd}. */
  private RewardedInterstitialAd rewardedInterstitialAd;

  /** The {@code Activity} on which the rewarded ad will display. */
  private Activity activity;

  /** A callback implemented in Unity via {@code AndroidJavaProxy} to receive ad events. */
  private UnityRewardedInterstitialAdCallback callback;

  public UnityRewardedInterstitialAd(
      Activity activity, UnityRewardedInterstitialAdCallback callback) {
    this.activity = activity;
    this.callback = callback;
  }

  /**
   * Loads a rewarded interstitial ad.
   *
   * @param adUnitId The ad unit ID.
   * @param request The {@link AdRequest} object with targeting parameters.
   */
  public void loadAd(final String adUnitId, final AdRequest request) {
    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            RewardedInterstitialAd.load(
                activity,
                adUnitId,
                request,
                new RewardedInterstitialAdLoadCallback() {
                  @Override
                  public void onRewardedInterstitialAdLoaded(@NonNull RewardedInterstitialAd ad) {
                    rewardedInterstitialAd = ad;

                    rewardedInterstitialAd.setOnPaidEventListener(
                        new OnPaidEventListener() {
                          @Override
                          public void onPaidEvent(final AdValue adValue) {
                            new Thread(
                                    new Runnable() {
                                      @Override
                                      public void run() {
                                        if (callback != null) {
                                          callback.onPaidEvent(
                                              adValue.getPrecisionType(),
                                              adValue.getValueMicros(),
                                              adValue.getCurrencyCode());
                                        }
                                      }
                                    })
                                .start();
                          }
                        });

                    rewardedInterstitialAd.setFullScreenContentCallback(
                        new FullScreenContentCallback() {
                          @Override
                          public void onAdFailedToShowFullScreenContent(final AdError error) {
                            new Thread(
                                    new Runnable() {
                                      @Override
                                      public void run() {
                                        if (callback != null) {
                                          callback.onAdFailedToShowFullScreenContent(error);
                                        }
                                      }
                                    })
                                .start();
                          }

                          @Override
                          public void onAdShowedFullScreenContent() {
                            new Thread(
                                    new Runnable() {
                                      @Override
                                      public void run() {
                                        if (callback != null) {
                                          callback.onAdShowedFullScreenContent();
                                        }
                                      }
                                    })
                                .start();
                          }

                          @Override
                          public void onAdDismissedFullScreenContent() {
                            new Thread(
                                    new Runnable() {
                                      @Override
                                      public void run() {
                                        if (callback != null) {
                                          callback.onAdDismissedFullScreenContent();
                                        }
                                      }
                                    })
                                .start();
                          }
                        });

                    new Thread(
                            new Runnable() {
                              @Override
                              public void run() {
                                if (callback != null) {
                                  callback.onRewardedInterstitialAdLoaded();
                                }
                              }
                            })
                        .start();
                  }

                  @Override
                  public void onRewardedInterstitialAdFailedToLoad(final LoadAdError error) {
                    new Thread(
                            new Runnable() {
                              @Override
                              public void run() {
                                if (callback != null) {
                                  callback.onRewardedInterstitialAdFailedToLoad(error);
                                }
                              }
                            })
                        .start();
                  }
                });
          }
        });
  }

  /** Shows the rewarded interstitial ad if it has loaded. */
  public void show() {
    if (rewardedInterstitialAd == null) {
      return;
    }
    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            rewardedInterstitialAd.show(
                activity,
                new OnUserEarnedRewardListener() {
                  @Override
                  public void onUserEarnedReward(@NonNull final RewardItem rewardItem) {
                    new Thread(
                            new Runnable() {
                              @Override
                              public void run() {
                                if (callback != null) {
                                  callback.onUserEarnedReward(
                                      rewardItem.getType(), rewardItem.getAmount());
                                }
                              }
                            })
                        .start();
                  }
                });
          }
        });
  }

  /** Sets server side verification options. */
  public void setServerSideVerificationOptions(
      final ServerSideVerificationOptions serverSideVerificationOptions) {
    if (rewardedInterstitialAd == null) {
      return;
    }
    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            rewardedInterstitialAd.setServerSideVerificationOptions(serverSideVerificationOptions);
          }
        });
  }

  /** Returns the request response info. */
  public ResponseInfo getResponseInfo() {
    if (rewardedInterstitialAd == null) {
      return null;
    }
    FutureTask<ResponseInfo> task =
        new FutureTask<>(
            new Callable<ResponseInfo>() {
              @Override
              public ResponseInfo call() {
                return rewardedInterstitialAd.getResponseInfo();
              }
            });
    activity.runOnUiThread(task);

    ResponseInfo result = null;
    try {
      result = task.get();
    } catch (InterruptedException exception) {
      Log.e(
          PluginUtils.LOGTAG,
          String.format(
              "Unable to check unity rewarded ad response info: %s",
              exception.getLocalizedMessage()));
    } catch (ExecutionException exception) {
      Log.e(
          PluginUtils.LOGTAG,
          String.format(
              "Unable to check unity rewarded ad response info: %s",
              exception.getLocalizedMessage()));
    }
    return result;
  }

  public RewardItem getRewardItem() {
    if (rewardedInterstitialAd == null) {
      return null;
    }
    FutureTask<RewardItem> task =
        new FutureTask<>(
            new Callable<RewardItem>() {
              @Override
              public RewardItem call() {
                return rewardedInterstitialAd.getRewardItem();
              }
            });
    activity.runOnUiThread(task);

    RewardItem result = null;
    try {
      result = task.get();
    } catch (InterruptedException e) {
      Log.e(
          PluginUtils.LOGTAG,
          String.format("Unable to get rewarded ad reward item: %s", e.getLocalizedMessage()));
    } catch (ExecutionException e) {
      Log.e(
          PluginUtils.LOGTAG,
          String.format("Unable to get rewarded ad reward item: %s", e.getLocalizedMessage()));
    }
    return result;
  }
}
