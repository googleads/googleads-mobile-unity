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
import android.graphics.Color;
import android.util.Log;
import android.view.View;
import android.view.ViewGroup;
import com.google.android.gms.ads.AdListener;
import com.google.android.gms.ads.AdSize;
import com.google.android.gms.ads.AdValue;
import com.google.android.gms.ads.LoadAdError;
import com.google.android.gms.ads.OnPaidEventListener;
import com.google.android.gms.ads.admanager.AdManagerAdRequest;
import com.google.android.gms.ads.admanager.AdManagerAdView;
import com.google.android.gms.ads.admanager.AppEventListener;
import com.google.unity.ads.Banner;
import com.google.unity.ads.PluginUtils;
import java.util.Arrays;
import java.util.List;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.FutureTask;

/**
 * This class represents the native implementation for the Google Mobile Ads Unity plugin. This
 * class is used to request Google Ad Manager Mobile ads natively via the Google Mobile Ads library
 * in Google Play services. The Google Play services library is a dependency for this plugin.
 */
public class UnityAdManagerBannerView extends Banner {
  // TODO (b/284206705): Improve Unit Test coverage around platform bridges and missing components

  // An executor used to run the callbacks.
  private final ExecutorService service;

  /**
   * Creates an instance of {@code UnityAdManagerBannerView}.
   *
   * @param activity The {@link Activity} that will contain an ad.
   * @param listener The {@link UnityAdManagerAdListener} used to receive synchronous ad events in
   *     Unity.
   */
  public UnityAdManagerBannerView(Activity activity, UnityAdManagerAdListener listener) {
    this.unityPlayerActivity = activity;
    this.unityListener = listener;
    service = Executors.newSingleThreadExecutor();
  }

  @Override
  protected void createAdView(final String publisherId, final AdSize adSize) {
    adView = new AdManagerAdView(unityPlayerActivity);
    // Setting the background color works around an issue where the first ad isn't visible.
    adView.setBackgroundColor(Color.TRANSPARENT);
    adView.setAdUnitId(publisherId);
    adView.setAdSize(adSize);
    adView.setVisibility(View.GONE);
    adView.setDescendantFocusability(ViewGroup.FOCUS_BLOCK_DESCENDANTS);
    unityPlayerActivity.addContentView(adView, getLayoutParams());
    adView.setAdListener(
        new AdListener() {
          @Override
          public void onAdLoaded() {
            if (!hidden) {
              show();
            }

            service.execute(
                () -> {
                  if (unityListener != null) {
                    unityListener.onAdLoaded();
                  }
                });
          }

          @Override
          public void onAdFailedToLoad(final LoadAdError error) {
            service.execute(
                () -> {
                  if (unityListener != null) {
                    unityListener.onAdFailedToLoad(error);
                  }
                });
          }

          @Override
          public void onAdOpened() {
            service.execute(
                () -> {
                  if (unityListener != null) {
                    unityListener.onAdOpened();
                  }
                });
          }

          @Override
          public void onAdClosed() {
            service.execute(
                () -> {
                  if (unityListener != null) {
                    unityListener.onAdClosed();
                  }
                });
          }

          @Override
          public void onAdImpression() {
            service.execute(
                () -> {
                  if (unityListener != null) {
                    unityListener.onAdImpression();
                  }
                });
          }

          @Override
          public void onAdClicked() {
            service.execute(
                () -> {
                  if (unityListener != null) {
                    unityListener.onAdClicked();
                  }
                });
          }
        });

    adView.setOnPaidEventListener(
        new OnPaidEventListener() {
          @Override
          public void onPaidEvent(final AdValue adValue) {
            service.execute(
                () -> {
                  if (unityListener != null) {
                    unityListener.onPaidEvent(
                        adValue.getPrecisionType(),
                        adValue.getValueMicros(),
                        adValue.getCurrencyCode());
                  }
                });
          }
        });

    ((AdManagerAdView) adView)
        .setAppEventListener(
            new AppEventListener() {
              @Override
              public void onAppEvent(final String name, final String data) {
                service.execute(
                    () -> {
                      if (unityListener != null) {
                        ((UnityAdManagerAdListener) unityListener).onAppEvent(name, data);
                      }
                    });
              }
            });

    setLayoutChangeListener();
  }

  /**
   * Loads an Ad Manager banner ad with an AdManagerAdRequest.
   *
   * @param request The {@link AdManagerAdRequest} object with targeting parameters.
   */
  public void loadAd(final AdManagerAdRequest request) {
    unityPlayerActivity.runOnUiThread(() -> ((AdManagerAdView) adView).loadAd(request));
  }

  /**
   * Returns a list of ad sizes supported by this {@link AdManagerAdView}. Use {@link
   * AdView#getAdSize} for the size of the currently displayed banner ad.
   */
  public List<AdSize> getAdSizes() {
    FutureTask<AdSize[]> task = new FutureTask<>(() -> ((AdManagerAdView) adView).getAdSizes());
    unityPlayerActivity.runOnUiThread(task);

    AdSize[] result = new AdSize[0];
    try {
      result = task.get();
    } catch (InterruptedException | ExecutionException e) {
      Log.e(
          PluginUtils.LOGTAG, String.format("Failed to get ad sizes: %s", e.getLocalizedMessage()));
    }
    return Arrays.asList(result);
  }

  /**
   * Sets the supported sizes of the banner ad. In most cases, only one ad size will be specified.
   *
   * <p>Multiple ad sizes can be specified if your application can appropriately handle multiple ad
   * sizes. For example, your application might call {@link AdView#getAdSize} during the {@link
   * AdListener#onAdLoaded} callback and change the layout according to the size of the ad that was
   * loaded. If multiple ad sizes are specified, the {@link AdManagerAdView} will assume the size of
   * the first ad size until an ad is loaded.
   *
   * <p>This method also immediately resizes the currently displayed ad, so calling this method
   * after an ad has been loaded is not recommended unless you know for certain that the content of
   * the ad will render correctly in the new ad size. This can be used if an ad needs to be resized
   * after it has been loaded. If more than one ad size is specified, the currently displayed ad
   * will be resized to the first ad size.
   *
   * @attr ref com.google.android.gms.ads.R.styleable#AdsAttrs_adSizes
   * @throws IllegalArgumentException If {@code adSizes} is {@code null} or empty.
   */
  public void setAdSizes(List<AdSize> adSizes) {
    if ((adSizes == null) || (adSizes.size() < 1)) {
      throw new IllegalArgumentException(
          "The supported ad sizes must contain at least one valid ad size.");
    }
    unityPlayerActivity.runOnUiThread(
        () -> ((AdManagerAdView) adView).setAdSizes(adSizes.toArray(new AdSize[adSizes.size()])));
  }
}
