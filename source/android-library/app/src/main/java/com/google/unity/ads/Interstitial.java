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
import com.google.android.gms.ads.AdError;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.AdValue;
import com.google.android.gms.ads.FullScreenContentCallback;
import com.google.android.gms.ads.LoadAdError;
import com.google.android.gms.ads.OnPaidEventListener;
import com.google.android.gms.ads.ResponseInfo;
import com.google.android.gms.ads.interstitial.InterstitialAd;
import com.google.android.gms.ads.interstitial.InterstitialAdLoadCallback;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.FutureTask;

/**
 * Native interstitial implementation for the Google Mobile Ads Unity plugin.
 */
public class Interstitial {

  /**
   * The {@link InterstitialAd}.
   */
  private InterstitialAd interstitialAd;

  /**
   * The {@code Activity} on which the interstitial will display.
   */
  private Activity activity;

  /**
   * A listener implemented in Unity via {@code AndroidJavaProxy} to receive ad events.
   */
  private UnityInterstitialAdCallback callback;

  public Interstitial(Activity activity, UnityInterstitialAdCallback callback) {
    this.activity = activity;
    this.callback = callback;
  }

  /**
   * Loads an interstitial ad.
   *
   * @param adUnitId The ad unit ID.
   * @param request The {@link AdRequest} object with targeting parameters.
   */
  public void loadAd(final String adUnitId, final AdRequest request) {
    activity.runOnUiThread(new Runnable() {
      @Override
      public void run() {
        InterstitialAd.load(
            activity,
            adUnitId,
            request,
            new InterstitialAdLoadCallback() {
              @Override
              public void onAdLoaded(@NonNull InterstitialAd ad) {
                interstitialAd = ad;

                interstitialAd.setOnPaidEventListener(
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
                            }
                        ).start();
                      }
                    }
                );

                interstitialAd.setFullScreenContentCallback(
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
                            }
                        ).start();
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
                            }).start();
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
                            }).start();
                      }

                      @Override
                      public void onAdImpression() {
                        new Thread(
                            new Runnable() {
                              @Override
                              public void run() {
                                if (callback != null) {
                                  callback.onAdImpression();
                                }
                              }
                            }).start();
                      }

                      @Override
                      public void onAdClicked() {
                        new Thread(
                            new Runnable() {
                              @Override
                              public void run() {
                                if (callback != null) {
                                  callback.onAdClicked();
                                }
                              }
                            }).start();
                      }
                    });

                new Thread(
                    new Runnable() {
                      @Override
                      public void run() {
                        if (callback != null) {
                          callback.onInterstitialAdLoaded();
                        }
                      }
                    }
                ).start();
              }

              @Override
              public void onAdFailedToLoad(final LoadAdError error) {
                new Thread(
                    new Runnable() {
                      @Override
                      public void run() {
                        if (callback != null) {
                          callback.onInterstitialAdFailedToLoad(error);
                        }
                      }
                    }
                ).start();
              }
            }
        );
      }
    });
  }

  /**
   * Returns the request response info.
   */
  public ResponseInfo getResponseInfo() {
    FutureTask<ResponseInfo> task = new FutureTask<>(new Callable<ResponseInfo>() {
      @Override
      public ResponseInfo call() {
        return interstitialAd.getResponseInfo();
      }
    });
    activity.runOnUiThread(task);

    ResponseInfo result = null;
    try {
      result = task.get();
    } catch (InterruptedException exception) {
      Log.e(PluginUtils.LOGTAG,
          String.format("Unable to check interstitial response info: %s",
              exception.getLocalizedMessage()));
    } catch (ExecutionException exception) {
      Log.e(PluginUtils.LOGTAG,
          String.format("Unable to check interstitial response info: %s",
              exception.getLocalizedMessage()));
    }
    return result;
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
    activity.runOnUiThread(new Runnable() {
      @Override
      public void run() {
        interstitialAd.show(activity);
      }
    });
  }

  /**
   * Destroys the {@link InterstitialAd}.
   */
  public void destroy() {
    // Currently there is no interstitial.destroy() method. This method is a placeholder in case
    // there is any cleanup to do here in the future.
  }
}
