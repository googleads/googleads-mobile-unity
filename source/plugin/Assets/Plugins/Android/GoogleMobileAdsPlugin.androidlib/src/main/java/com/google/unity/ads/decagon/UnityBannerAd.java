/*
 * Copyright (C) 2025 Google, Inc.
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
import android.os.Build;
import android.util.Log;
import android.view.DisplayCutout;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.view.WindowInsets;
import android.widget.FrameLayout;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import com.google.android.libraries.ads.mobile.sdk.banner.BannerAd;
import com.google.android.libraries.ads.mobile.sdk.banner.BannerAdEventCallback;
import com.google.android.libraries.ads.mobile.sdk.banner.BannerAdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdValue;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
import com.google.unity.ads.PluginUtils;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.FutureTask;

/**
 * This class represents the native implementation for the Google Mobile Ads Unity plugin. This
 * class is used to request Google Mobile ads natively via the Google Mobile Ads Beta library.
 */
public class UnityBannerAd {

  /** Class to hold the insets of the cutout area. */
  protected static class Insets {
    int top = 0;
    int bottom = 0;
    int left = 0;
    int right = 0;
  }

  /** The {@link BannerAd} to display to the user. */
  private BannerAd bannerAd;

  /** The {@link View} that contains the banner ad. */
  private View adView;

  /** A code indicating where to place the ad. */
  private int positionCode;

  /**
   * Offset for the ad in the x-axis when a custom position is used. Value will be 0 for non-custom
   * positions.
   */
  private int horizontalOffset;

  /**
   * Offset for the ad in the y-axis when a custom position is used. Value will be 0 for non-custom
   * positions.
   */
  private int verticalOffset;

  /** The layout that contains the banner ad. */
  @Nullable private FrameLayout bannerLayout;

  /** A boolean indicating whether the ad has been hidden. */
  protected boolean hidden;

  /** The {@code Activity} that the banner will be displayed in. */
  protected Activity unityPlayerActivity;

  /** A callback implemented in Unity via {@code AndroidJavaProxy} to receive ad events. */
  private final UnityBannerAdCallback callback;

  /**
   * A {@code View.OnLayoutChangeListener} used to detect orientation changes and reposition banner
   * ads as required.
   */
  private View.OnLayoutChangeListener layoutChangeListener;

  public UnityBannerAd(Activity activity, UnityBannerAdCallback callback) {
    unityPlayerActivity = activity;
    this.callback = callback;
  }

  /**
   * Creates an {@link AdView} to hold banner ads.
   *
   * @param positionCode A code indicating where to place the ad.
   */
  public void create(int positionCode) {
    horizontalOffset = 0;
    verticalOffset = 0;
    this.positionCode = positionCode;
    hidden = false;
  }

  /**
   * Creates an {@link AdView} to hold banner ads with a custom position.
   *
   * @param positionX Position of banner ad on the x axis.
   * @param positionY Position of banner ad on the y axis.
   */
  public void create(final int positionX, final int positionY) {
    positionCode = PluginUtils.POSITION_CUSTOM;
    horizontalOffset = positionX;
    verticalOffset = positionY;
    hidden = false;
  }

  protected void load(final BannerAdRequest adRequest) {
    BannerAd.load(
        adRequest,
        new AdLoadCallback<BannerAd>() {
          @Override
          public void onAdLoaded(@NonNull BannerAd ad) {
            bannerAd = ad;
            if (!hidden) {
              show();
            }
            new Thread(
                    () -> {
                      if (callback != null) {
                        callback.onAdLoaded();
                      }
                    })
                .start();

            ad.setAdEventCallback(
                new BannerAdEventCallback() {
                  @Override
                  public void onAdImpression() {
                    if (callback != null) {
                      callback.onAdImpression();
                    }
                  }

                  @Override
                  public void onAdClicked() {
                    if (callback != null) {
                      callback.onAdClicked();
                    }
                  }

                  @Override
                  public void onAdShowedFullScreenContent() {
                    if (callback != null) {
                      callback.onAdOpened();
                    }
                  }

                  @Override
                  public void onAdDismissedFullScreenContent() {
                    if (callback != null) {
                      callback.onAdClosed();
                    }
                  }

                  @Override
                  public void onAdPaid(@NonNull AdValue adValue) {
                    if (callback != null) {
                      int precisionType = 0;
                      switch (adValue.getPrecisionType()) {
                        case ESTIMATED:
                          precisionType = 1;
                          break;
                        case PUBLISHER_PROVIDED:
                          precisionType = 2;
                          break;
                        case PRECISE:
                          precisionType = 3;
                          break;
                        case UNKNOWN:
                          precisionType = 0;
                          break;
                      }
                      callback.onPaidEvent(
                          precisionType, adValue.getValueMicros(), adValue.getCurrencyCode());
                    }
                  }
                });
          }

          @Override
          public void onAdFailedToLoad(@NonNull LoadAdError adError) {
            new Thread(
                    () -> {
                      if (callback != null) {
                        callback.onAdFailedToLoad(adError);
                      }
                    })
                .start();
          }
        });
    setLayoutChangeListener();
  }

  protected void show() {
    unityPlayerActivity.runOnUiThread(
        () -> {
          if (bannerAd == null) {
            Log.w(PluginUtils.LOGTAG, "Tried to show banner before ad was ready.");
            return;
          }

          if (bannerLayout == null) {
            bannerLayout = new FrameLayout(unityPlayerActivity);
          }

          if (bannerLayout.getParent() == null) {
            unityPlayerActivity.addContentView(
                bannerLayout,
                new ViewGroup.LayoutParams(
                    ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT));
          }

          adView = bannerAd.getView(unityPlayerActivity);
          bannerLayout.removeAllViews();
          bannerLayout.addView(adView);
          adView.setVisibility(View.VISIBLE);
          updatePosition();
          hidden = false;
        });
  }

  protected void hide() {
    unityPlayerActivity.runOnUiThread(
        () -> {
          if (bannerLayout != null && bannerLayout.getParent() != null) {
            ((ViewGroup) bannerLayout.getParent()).removeView(bannerLayout);
            hidden = true;
          }
        });
  }

  /** Returns the request response info. */
  @Nullable
  public ResponseInfo getResponseInfo() {
    if (bannerAd == null) {
      Log.e(PluginUtils.LOGTAG, "Tried to get response info before it was ready. Returning null.");
      return null;
    }
    return bannerAd.getResponseInfo();
  }

  /** Update the {@link AdView} position based on current parameters. */
  private void updatePosition() {
    if (adView == null || hidden) {
      return;
    }
    unityPlayerActivity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            FrameLayout.LayoutParams layoutParams = getLayoutParams();
            adView.setLayoutParams(layoutParams);
          }
        });
  }

  /**
   * Updates the {@link AdView} position.
   *
   * @param code A code indicating where to place the ad.
   */
  public void setPosition(final int code) {
    unityPlayerActivity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            positionCode = code;
            updatePosition();
          }
        });
  }

  /**
   * Updates the {@link AdView} position.
   *
   * @param positionX Position of banner ad on the x axis.
   * @param positionY Position of banner ad on the y axis.
   */
  public void setPosition(final int positionX, final int positionY) {
    unityPlayerActivity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            positionCode = PluginUtils.POSITION_CUSTOM;
            horizontalOffset = positionX;
            verticalOffset = positionY;
            updatePosition();
          }
        });
  }

  /**
   * Get {@link AdView} height.
   *
   * @return the height of the {@link AdView}.
   */
  public float getHeightInPixels() {
    if (bannerAd == null) {
      Log.e(PluginUtils.LOGTAG, "Tried to get height of a null banner ad.");
      return -1;
    }

    FutureTask<Integer> task =
        new FutureTask<>(() -> bannerAd.getAdSize().getHeightInPixels(unityPlayerActivity));
    unityPlayerActivity.runOnUiThread(task);

    try {
      return task.get();
    } catch (InterruptedException | ExecutionException e) {
      Log.e(
          PluginUtils.LOGTAG,
          String.format("Failed to get ad view height: %s", e.getLocalizedMessage()));
      return -1;
    }
  }

  /**
   * Get {@link AdView} width.
   *
   * @return the width of the {@link AdView}.
   */
  public float getWidthInPixels() {
    if (bannerAd == null) {
      Log.e(PluginUtils.LOGTAG, "Tried to get width of a null banner ad.");
      return -1;
    }

    FutureTask<Integer> task =
        new FutureTask<>(() -> bannerAd.getAdSize().getWidthInPixels(unityPlayerActivity));
    unityPlayerActivity.runOnUiThread(task);

    try {
      return task.get();
    } catch (InterruptedException | ExecutionException e) {
      Log.e(
          PluginUtils.LOGTAG,
          String.format("Failed to get ad view width: %s", e.getLocalizedMessage()));
      return -1;
    }
  }

  /** Returns whether the {@link AdView} loaded a collapsible banner. */
  public boolean isCollapsible() {
    if (bannerAd == null) {
      return false;
    }
    return bannerAd.isCollapsible();
  }

  /** Destroys the {@link AdView}. */
  public void destroy() {
    unityPlayerActivity.runOnUiThread(
        () -> {
          if (bannerAd == null) {
            return;
          }
          if (bannerLayout != null) {
            bannerLayout.removeView(adView);
          }
          bannerAd.destroy();
        });

    unityPlayerActivity
        .getWindow()
        .getDecorView()
        .getRootView()
        .removeOnLayoutChangeListener(layoutChangeListener);
  }

  /**
   * Sets the layout change listener for the {@link AdView} to detect orientation changes and
   * reposition banner ads as required.
   */
  protected void setLayoutChangeListener() {
    layoutChangeListener =
        new View.OnLayoutChangeListener() {
          @Override
          public void onLayoutChange(
              View v,
              int left,
              int top,
              int right,
              int bottom,
              int oldLeft,
              int oldTop,
              int oldRight,
              int oldBottom) {
            boolean viewBoundsChanged =
                left != oldLeft || right != oldRight || bottom != oldBottom || top != oldTop;
            if (!viewBoundsChanged) {
              return;
            }

            if (!hidden) {
              updatePosition();
            }
          }
        };

    unityPlayerActivity
        .getWindow()
        .getDecorView()
        .getRootView()
        .addOnLayoutChangeListener(layoutChangeListener);
  }

  /**
   * Create layout params for the ad view with relevant positioning details.
   *
   * @return configured {@link FrameLayout.LayoutParams }.
   */
  protected FrameLayout.LayoutParams getLayoutParams() {
    final FrameLayout.LayoutParams adParams =
        new FrameLayout.LayoutParams(
            FrameLayout.LayoutParams.WRAP_CONTENT, FrameLayout.LayoutParams.WRAP_CONTENT);
    adParams.gravity = PluginUtils.getLayoutGravityForPositionCode(positionCode);

    Insets insets = getSafeInsets();
    int safeInsetLeft = insets.left;
    int safeInsetTop = insets.top;
    adParams.bottomMargin = insets.bottom;
    adParams.rightMargin = insets.right;

    if (positionCode == PluginUtils.POSITION_CUSTOM) {
      int leftOffset = (int) PluginUtils.convertDpToPixel(horizontalOffset);
      if (leftOffset < safeInsetLeft) {
        leftOffset = safeInsetLeft;
      }
      int topOffset = (int) PluginUtils.convertDpToPixel(verticalOffset);
      if (topOffset < safeInsetTop) {
        topOffset = safeInsetTop;
      }
      adParams.leftMargin = leftOffset;
      adParams.topMargin = topOffset;
    } else {
      adParams.leftMargin = safeInsetLeft;
      if (positionCode == PluginUtils.POSITION_TOP
          || positionCode == PluginUtils.POSITION_TOP_LEFT
          || positionCode == PluginUtils.POSITION_TOP_RIGHT) {
        adParams.topMargin = safeInsetTop;
      }
    }
    return adParams;
  }

  private Insets getSafeInsets() {
    Insets insets = new Insets();

    if (Build.VERSION.SDK_INT < Build.VERSION_CODES.P) {
      return insets;
    }
    Window window = unityPlayerActivity.getWindow();
    if (window == null) {
      return insets;
    }
    WindowInsets windowInsets = window.getDecorView().getRootWindowInsets();
    if (windowInsets == null) {
      return insets;
    }
    DisplayCutout displayCutout = windowInsets.getDisplayCutout();
    if (displayCutout == null) {
      return insets;
    }
    insets.top = displayCutout.getSafeInsetTop();
    insets.left = displayCutout.getSafeInsetLeft();
    insets.bottom = displayCutout.getSafeInsetBottom();
    insets.right = displayCutout.getSafeInsetRight();
    return insets;
  }
}
