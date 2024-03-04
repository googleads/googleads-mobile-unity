/*
 * Copyright (C) 2024 Google, Inc.
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

package com.google.unity.ads.nativead;

import android.app.Activity;
import android.util.Log;
import com.google.android.gms.ads.AdListener;
import com.google.android.gms.ads.AdLoader;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.AdValue;
import com.google.android.gms.ads.LoadAdError;
import com.google.android.gms.ads.OnPaidEventListener;
import com.google.android.gms.ads.ResponseInfo;
import com.google.android.gms.ads.nativead.NativeAd;
import com.google.android.gms.ads.nativead.NativeAdOptions;
import com.google.unity.ads.PluginUtils;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.FutureTask;

/** Native Template ad implementation for the Google Mobile Ads Unity plugin. */
public class UnityNativeTemplateAd {

  /** The {@link NativeAd}. */
  private NativeAd nativeAd;

  /** The {@link AdLoader} used to configure and load the native ad. */
  private AdLoader adLoader;

  /** The {@code Activity} on which the native template will display. */
  private Activity activity;

  /** A code indicating where to place the ad. */
  private int mPositionCode;

  /** A boolean indicating whether the ad has been hidden. */
  protected boolean hidden;

  /**
   * Offset for the ad in the x-axis when a custom position is used. Value will be 0 for non-custom
   * positions.
   */
  private int mHorizontalOffset;

  /**
   * Offset for the ad in the y-axis when a custom position is used. Value will be 0 for non-custom
   * positions.
   */
  private int mVerticalOffset;

  /** A listener implemented in Unity via {@code AndroidJavaProxy} to receive ad events. */
  private UnityNativeTemplateAdCallback callback;

  public UnityNativeTemplateAd(Activity activity, UnityNativeTemplateAdCallback callback) {
    this.activity = activity;
    this.callback = callback;
    hidden = false;
    mHorizontalOffset = 0;
    mVerticalOffset = 0;
  }

  /**
   * Loads a native ad using the provided NativeAdOptions and AdRequest.
   *
   * @param adUnitId Your ad unit ID.
   * @param options The NativeAdOptions used to customize the native ad request.
   * @param request The AdRequest used to fetch the native ad.
   */
  public void loadAd(
      final String adUnitId, final NativeAdOptions options, final AdRequest request) {
    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            AdLoader adLoader =
                new AdLoader.Builder(activity, adUnitId)
                    .forNativeAd(
                        new NativeAd.OnNativeAdLoadedListener() {
                          @Override
                          public void onNativeAdLoaded(NativeAd ad) {
                            nativeAd = ad;
                            callback.onNativeAdLoaded();

                            nativeAd.setOnPaidEventListener(
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
                          }
                        })
                    .withAdListener(
                        new AdListener() {
                          @Override
                          public void onAdFailedToLoad(LoadAdError adError) {
                            callback.onNativeAdFailedToLoad(adError);
                          }

                          @Override
                          public void onAdImpression() {
                            callback.onAdImpression();
                          }

                          @Override
                          public void onAdClicked() {
                            callback.onAdClicked();
                          }

                          @Override
                          public void onAdClosed() {
                            callback.onAdDismissedFullScreenContent();
                          }

                          @Override
                          public void onAdOpened() {
                            callback.onAdShowedFullScreenContent();
                          }
                        })
                    .withNativeAdOptions(options)
                    .build();
            adLoader.loadAd(request);
          }
        });
  }

  /**
   * Updates the Native Template position.
   *
   * @param positionCode A code indicating where to place the ad.
   */
  public void setPositionCode(final int positionCode) {
    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            mPositionCode = positionCode;
            updatePosition();
          }
        });
  }

  /**
   * Updates the Native Template position.
   *
   * @param positionX Position of template ad on the x axis.
   * @param positionY Position of template ad on the y axis.
   */
  public void setPosition(final int positionX, final int positionY) {
    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            mPositionCode = PluginUtils.POSITION_CUSTOM;
            mHorizontalOffset = positionX;
            mVerticalOffset = positionY;
            updatePosition();
          }
        });
  }

  /** Returns the request response info. */
  public ResponseInfo getResponseInfo() {
    FutureTask<ResponseInfo> task =
        new FutureTask<>(
            new Callable<ResponseInfo>() {
              @Override
              public ResponseInfo call() {
                return nativeAd.getResponseInfo();
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
              "Unable to check native response info: %s", exception.getLocalizedMessage()));
    } catch (ExecutionException exception) {
      Log.e(
          PluginUtils.LOGTAG,
          String.format(
              "Unable to check native response info: %s", exception.getLocalizedMessage()));
    }
    return result;
  }
}
