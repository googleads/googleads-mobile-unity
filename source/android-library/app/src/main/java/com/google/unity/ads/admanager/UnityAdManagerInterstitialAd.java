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
import com.google.android.gms.ads.ResponseInfo;
import com.google.android.gms.ads.admanager.AdManagerAdRequest;
import com.google.android.gms.ads.admanager.AdManagerInterstitialAd;
import com.google.unity.ads.PluginUtils;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.FutureTask;

/** Android AdManager interstitial implementation for the Google Mobile Ads Unity plugin. */
public class UnityAdManagerInterstitialAd {
  // TODO (b/284206705): Improve Unit Test coverage around platform bridges and missing components

  /** The {@link AdManagerInterstitialAd}. */
  private AdManagerInterstitialAd adManagerInterstitialAd;

  /** The {@code Activity} on which the interstitial will display. */
  private final Activity activity;

  /** A listener implemented in Unity via {@code AndroidJavaProxy} to receive ad events. */
  private final UnityAdManagerInterstitialAdCallback callback;

  public UnityAdManagerInterstitialAd(
      Activity activity, UnityAdManagerInterstitialAdCallback callback) {
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
    AdManagerInterstitialAdCallback adManagerInterstitialAdCallback =
        new AdManagerInterstitialAdCallback(adManagerInterstitialAd, callback);

    activity.runOnUiThread(
        () ->
            AdManagerInterstitialAd.load(
                activity, adUnitId, request, adManagerInterstitialAdCallback));
  }

  /** Returns the request response info. */
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
