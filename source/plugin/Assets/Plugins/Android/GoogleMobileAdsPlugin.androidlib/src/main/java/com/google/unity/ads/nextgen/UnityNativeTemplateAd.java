/*
 * Copyright (C) 2026 Google, Inc.
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
import android.os.Build;
import android.util.Log;
import android.view.DisplayCutout;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.ViewParent;
import android.widget.FrameLayout;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.annotation.VisibleForTesting;
import com.google.android.libraries.ads.mobile.sdk.banner.AdSize;
import com.google.android.libraries.ads.mobile.sdk.common.AdValue;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
import com.google.android.libraries.ads.mobile.sdk.nativead.NativeAd;
import com.google.android.libraries.ads.mobile.sdk.nativead.NativeAdEventCallback;
import com.google.android.libraries.ads.mobile.sdk.nativead.NativeAdLoader;
import com.google.android.libraries.ads.mobile.sdk.nativead.NativeAdLoaderCallback;
import com.google.android.libraries.ads.mobile.sdk.nativead.NativeAdRequest;
import com.google.unity.ads.PluginUtils;
import com.google.unity.ads.nativead.UnityNativeTemplateStyle;
import java.util.concurrent.Executor;

/** Native Template ad implementation for the Google Mobile Ads Unity plugin. */
public class UnityNativeTemplateAd {

  /** Class to hold the insets of the cutout area. */
  protected static class Insets {
    int top = 0;
    int bottom = 0;
    int left = 0;
    int right = 0;
  }

  /** The {@link NativeAd}. */
  private NativeAd nativeAd;

  /** The {@code Activity} on which the native template will display. */
  private final Activity activity;

  /** The template view rendering the native ad. */
  private TemplateView templateView;

  /**
   * A {@code View.OnLayoutChangeListener} used to detect orientation changes and reposition native
   * overlay ads as required.
   */
  private View.OnLayoutChangeListener layoutChangeListener;

  /** A code indicating where to place the ad. */
  private int positionCode;

  /** AdSize to track the size of the Template view. */
  private AdSize adSize;

  /** A boolean indicating whether the ad has been hidden. */
  protected boolean hidden;

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

  /** A listener implemented in Unity via {@code AndroidJavaProxy} to receive ad events. */
  private final UnityNativeTemplateAdCallback callback;

  /** Interface for loading native ads, used to enable unit testing. */
  protected interface NativeAdLoaderWrapper {
    void load(NativeAdRequest request, NativeAdLoaderCallback callback);
  }

  private final NativeAdLoaderWrapper adLoaderWrapper;

  /** The executor for running callbacks. */
  private final Executor executor;

  public UnityNativeTemplateAd(Activity activity, UnityNativeTemplateAdCallback callback) {
    this(
        activity,
        callback,
        new NativeAdLoaderWrapper() {
          @Override
          public void load(NativeAdRequest request, NativeAdLoaderCallback callback) {
            NativeAdLoader.load(request, callback);
          }
        },
        UnityExecutor.getExecutor());
  }

  @VisibleForTesting
  UnityNativeTemplateAd(
      Activity activity,
      UnityNativeTemplateAdCallback callback,
      NativeAdLoaderWrapper adLoaderWrapper) {
    this(activity, callback, adLoaderWrapper, UnityExecutor.getExecutor());
  }

  @VisibleForTesting
  UnityNativeTemplateAd(
      Activity activity,
      UnityNativeTemplateAdCallback callback,
      NativeAdLoaderWrapper adLoaderWrapper,
      Executor executor) {
    this.activity = activity;
    this.callback = callback;
    this.adLoaderWrapper = adLoaderWrapper;
    this.executor = executor;
    hidden = false;
    horizontalOffset = 0;
    verticalOffset = 0;
  }

  public void loadAd(final NativeAdRequest request) {
    adLoaderWrapper.load(
        request,
        new NativeAdLoaderCallback() {
          @Override
          public void onNativeAdLoaded(@NonNull NativeAd ad) {
            nativeAd = ad;
            executor.execute(
                () -> {
                  if (callback != null) {
                    callback.onNativeAdLoaded();
                  }
                });
            setAdEventsListener(nativeAd);
          }

          @Override
          public void onAdFailedToLoad(@NonNull LoadAdError adError) {
            executor.execute(
                () -> {
                  if (callback != null) {
                    callback.onNativeAdFailedToLoad(adError);
                  }
                });
          }
        });
  }

  private void setAdEventsListener(NativeAd ad) {
    ad.setAdEventCallback(
        new NativeAdEventCallback() {
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
          }

          @Override
          public void onAdPaid(@NonNull AdValue adValue) {
            executor.execute(
                () -> {
                  if (callback != null) {
                    callback.onPaidEvent(
                        Util.getAdValuePrecisionType(adValue.getPrecisionType()),
                        adValue.getValueMicros(),
                        adValue.getCurrencyCode());
                  }
                });
          }
        });
  }

  /** Gets the placement ID of the {@link NativeAd}. */
  public long getPlacementId() {
    if (nativeAd == null) {
      return 0;
    }
    return nativeAd.getPlacementId();
  }

  /**
   * Sets the placement ID of the {@link NativeAd}.
   *
   * <p>To ensure this placement ID is included in reporting, call this method before showing the
   * ad.
   *
   * @param placementId A long integer provided by the AdMob UI for the configured placement.
   */
  public void setPlacementId(long placementId) {
    if (nativeAd == null) {
      return;
    }
    nativeAd.setPlacementId(placementId);
  }

  /**
   * Updates the Native Template position.
   *
   * @param newPositionCode A code indicating where to place the ad.
   */
  public void setPositionCode(final int newPositionCode) {
    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            positionCode = newPositionCode;
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
            positionCode = PluginUtils.POSITION_CUSTOM;
            horizontalOffset = positionX;
            verticalOffset = positionY;
            updatePosition();
          }
        });
  }

  /**
   * Renders the Native Template position at the x,y coordinates using default sizing.
   *
   * @param templateStyle Template Style.
   * @param positionX Position of template ad on the x axis.
   * @param positionY Position of template ad on the y axis.
   */
  public void renderDefaultSizeAtPosition(
      final UnityNativeTemplateStyle templateStyle, final int positionX, final int positionY) {
    removeTemplateView();

    positionCode = PluginUtils.POSITION_CUSTOM;
    horizontalOffset = positionX;
    verticalOffset = positionY;
    adSize = null;

    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            initializeTemplateView(templateStyle.getTemplateType().resourceId(), templateStyle);

            FrameLayout layout = new FrameLayout(activity);
            layout.addView(templateView, getLayoutParams());
            activity.addContentView(
                layout,
                new FrameLayout.LayoutParams(
                    FrameLayout.LayoutParams.MATCH_PARENT, FrameLayout.LayoutParams.MATCH_PARENT));
            setLayoutChangeListener();
          }
        });
  }

  /**
   * Renders the Native Template position at the positionCode using default sizing.
   *
   * @param templateStyle Template Style.
   * @param newPositionCode A code indicating where to place the template ad.
   */
  public void renderDefaultSizeAtPositionCode(
      final UnityNativeTemplateStyle templateStyle, final int newPositionCode) {
    removeTemplateView();

    positionCode = newPositionCode;
    adSize = null;

    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            initializeTemplateView(templateStyle.getTemplateType().resourceId(), templateStyle);

            FrameLayout layout = new FrameLayout(activity);
            layout.addView(templateView, getLayoutParams());
            activity.addContentView(
                layout,
                new FrameLayout.LayoutParams(
                    FrameLayout.LayoutParams.MATCH_PARENT, FrameLayout.LayoutParams.MATCH_PARENT));
            setLayoutChangeListener();
          }
        });
  }

  /**
   * Renders the Native Template position with the provided adsize at the x,y coordinates.
   *
   * @param templateStyle Template Style.
   * @param adSize AdSize of the template to be displayed.
   * @param positionX Position of template ad on the x axis.
   * @param positionY Position of template ad on the y axis.
   */
  public void renderCustomSizeAtPosition(
      final UnityNativeTemplateStyle templateStyle,
      final AdSize adSize,
      final int positionX,
      final int positionY) {
    removeTemplateView();

    positionCode = PluginUtils.POSITION_CUSTOM;
    horizontalOffset = positionX;
    verticalOffset = positionY;
    this.adSize = adSize;

    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            initializeTemplateView(templateStyle.getTemplateType().resourceId(), templateStyle);

            FrameLayout layout = new FrameLayout(activity);
            FrameLayout.LayoutParams layoutParams = getLayoutParams();
            layoutParams.height = adSize.getHeight();
            layoutParams.width = adSize.getWidth();
            layout.addView(templateView, layoutParams);
            activity.addContentView(
                layout,
                new FrameLayout.LayoutParams(
                    FrameLayout.LayoutParams.MATCH_PARENT, FrameLayout.LayoutParams.MATCH_PARENT));
            setLayoutChangeListener();
          }
        });
  }

  /**
   * Renders the Native Template ad with the provided adsize at the positionCode.
   *
   * @param templateStyle Template Style.
   * @param adSize AdSize of the template to be displayed.
   * @param newPositionCode A code indicating where to place the template ad.
   */
  public void renderCustomSizeAtPositionCode(
      final UnityNativeTemplateStyle templateStyle,
      final AdSize adSize,
      final int newPositionCode) {
    removeTemplateView();

    positionCode = newPositionCode;
    this.adSize = adSize;

    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            initializeTemplateView(templateStyle.getTemplateType().resourceId(), templateStyle);

            FrameLayout layout = new FrameLayout(activity);
            FrameLayout.LayoutParams layoutParams = getLayoutParams();
            layoutParams.height = adSize.getHeight();
            layoutParams.width = adSize.getWidth();
            layout.addView(templateView, layoutParams);
            activity.addContentView(
                layout,
                new FrameLayout.LayoutParams(
                    FrameLayout.LayoutParams.MATCH_PARENT, FrameLayout.LayoutParams.MATCH_PARENT));
            setLayoutChangeListener();
          }
        });
  }

  /** Returns the request response info. */
  @Nullable
  public ResponseInfo getResponseInfo() {
    if (nativeAd == null) {
      return null;
    }
    try {
      return nativeAd.getResponseInfo();
    } catch (RuntimeException exception) {
      Log.e(
          PluginUtils.LOGTAG,
          String.format(
              "Unable to check native response info: %s", exception.getLocalizedMessage()));
      return null;
    }
  }

  /** Sets the Native Template View to be visible. */
  public void show() {
    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            if (templateView == null) {
              return;
            }
            hidden = false;
            templateView.setVisibility(View.VISIBLE);
            updatePosition();
          }
        });
  }

  /** Sets the Native Template View to be gone. */
  public void hide() {
    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            if (templateView == null) {
              return;
            }
            hidden = true;
            templateView.setVisibility(View.GONE);
          }
        });
  }

  /**
   * Get Native Template View height.
   *
   * @return the height of the Native Template View.
   */
  public float getHeightInPixels() {
    if (templateView == null) {
      return 0;
    }
    return templateView.getHeight();
  }

  /**
   * Get Native Template View width.
   *
   * @return the width of the Native Template View.
   */
  public float getWidthInPixels() {
    if (templateView == null) {
      return 0;
    }
    return templateView.getWidth();
  }

  /** Destroys the Native Template View. */
  public void destroy() {
    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            if (templateView != null) {
              templateView.destroyNativeAd();
              ViewParent parentView = templateView.getParent();
              if (parentView instanceof ViewGroup) {
                ViewGroup parentViewGroup = (ViewGroup) parentView;
                parentViewGroup.removeView(templateView);
                ViewParent grandParentView = parentViewGroup.getParent();
                if (grandParentView instanceof ViewGroup) {
                  ((ViewGroup) grandParentView).removeView(parentViewGroup);
                }
              }
            }

            if (layoutChangeListener != null) {
              activity
                  .getWindow()
                  .getDecorView()
                  .getRootView()
                  .removeOnLayoutChangeListener(layoutChangeListener);
              layoutChangeListener = null;
            }
          }
        });
  }

  /** Sets a listener to update the position of the Native Overlay in case of layout changes */
  protected void setLayoutChangeListener() {
    if (layoutChangeListener != null) {
      return;
    }

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
            boolean isViewBoundsSame =
                left == oldLeft && right == oldRight && bottom == oldBottom && top == oldTop;
            if (isViewBoundsSame) {
              return;
            }

            if (!hidden) {
              updatePosition();
            }
          }
        };

    activity
        .getWindow()
        .getDecorView()
        .getRootView()
        .addOnLayoutChangeListener(layoutChangeListener);
  }

  /** Update the Native Overlay View position based on current parameters. */
  private void updatePosition() {
    if (templateView == null) {
      return;
    }
    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            FrameLayout.LayoutParams layoutParams = getLayoutParams();
            if (adSize != null) {
              layoutParams.height = adSize.getHeight();
              layoutParams.width = adSize.getWidth();
            }
            templateView.setLayoutParams(layoutParams);
          }
        });
  }

  /** Removes the currently shown Native Overlay View. */
  private void removeTemplateView() {
    if (templateView != null) {
      activity.runOnUiThread(
          new Runnable() {
            @Override
            public void run() {
              ViewParent parentView = templateView.getParent();
              if (parentView instanceof ViewGroup) {
                ViewGroup parentViewGroup = (ViewGroup) parentView;
                parentViewGroup.removeView(templateView);
                ViewParent grandParentView = parentViewGroup.getParent();
                if (grandParentView instanceof ViewGroup) {
                  ((ViewGroup) grandParentView).removeView(parentViewGroup);
                }
              }
            }
          });
    }
  }

  private void initializeTemplateView(int layoutId, UnityNativeTemplateStyle templateStyle) {
    LayoutInflater layoutInflater = LayoutInflater.from(activity);
    templateView = (TemplateView) layoutInflater.inflate(layoutId, null);
    templateView.setStyles(templateStyle.asNativeTemplateStyle());
    templateView.setNativeAd(nativeAd);
  }

  /**
   * Create layout params for the ad view with relevant positioning details.
   *
   * @return configured {@link FrameLayout.LayoutParams}.
   */
  protected FrameLayout.LayoutParams getLayoutParams() {
    final FrameLayout.LayoutParams adParams =
        new FrameLayout.LayoutParams(
            FrameLayout.LayoutParams.MATCH_PARENT, FrameLayout.LayoutParams.WRAP_CONTENT);
    adParams.gravity = PluginUtils.getLayoutGravityForPositionCode(positionCode);

    Insets insets = getInsets();
    int insetLeft = insets.left;
    int insetTop = insets.top;
    adParams.bottomMargin = insets.bottom;
    adParams.rightMargin = insets.right;
    if (positionCode == PluginUtils.POSITION_CUSTOM) {
      int leftOffset = (int) PluginUtils.convertDpToPixel(horizontalOffset);
      if (leftOffset < insetLeft) {
        leftOffset = insetLeft;
      }
      int topOffset = (int) PluginUtils.convertDpToPixel(verticalOffset);
      if (topOffset < insetTop) {
        topOffset = insetTop;
      }
      adParams.leftMargin = leftOffset;
      adParams.topMargin = topOffset;
    } else {
      adParams.leftMargin = insetLeft;
      if (positionCode == PluginUtils.POSITION_TOP
          || positionCode == PluginUtils.POSITION_TOP_LEFT
          || positionCode == PluginUtils.POSITION_TOP_RIGHT) {
        adParams.topMargin = insetTop;
      }
    }
    return adParams;
  }

  private Insets getInsets() {
    Insets insets = new Insets();
    if (Build.VERSION.SDK_INT < Build.VERSION_CODES.P
        || activity.getWindow() == null
        || activity.getWindow().getDecorView().getRootWindowInsets() == null
        || activity.getWindow().getDecorView().getRootWindowInsets().getDisplayCutout() == null) {
      return insets;
    }

    DisplayCutout displayCutout =
        activity.getWindow().getDecorView().getRootWindowInsets().getDisplayCutout();
    insets.top = displayCutout.getSafeInsetTop();
    insets.left = displayCutout.getSafeInsetLeft();
    insets.bottom = displayCutout.getSafeInsetBottom();
    insets.right = displayCutout.getSafeInsetRight();
    return insets;
  }
}
