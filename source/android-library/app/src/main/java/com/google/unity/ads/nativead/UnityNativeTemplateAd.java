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
import android.os.Build;
import android.util.Log;
import android.view.DisplayCutout;
import android.view.View;
import android.view.ViewGroup;
import android.view.ViewParent;
import android.widget.FrameLayout;
import com.google.android.ads.nativetemplates.TemplateView;
import com.google.android.gms.ads.AdListener;
import com.google.android.gms.ads.AdLoader;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.AdSize;
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
  private Activity activity;

  /** The template view rendering the native ad. */
  private TemplateView templateView;

  /**
   * A {@code View.OnLayoutChangeListener} used to detect orientation changes and reposition native
   * overlay ads as required.
   */
  private View.OnLayoutChangeListener mLayoutChangeListener;

  /** A code indicating where to place the ad. */
  private int mPositionCode;

  /** AdSize to track the size of the Template view. */
  private AdSize mAdSize;

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

    mPositionCode = PluginUtils.POSITION_CUSTOM;
    mHorizontalOffset = positionX;
    mVerticalOffset = positionY;
    mAdSize = null;

    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            templateView = templateStyle.asTemplateView(activity);
            templateView.setNativeAd(nativeAd);

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
   * @param positionCode A code indicating where to place the template ad.
   */
  public void renderDefaultSizeAtPositionCode(
      final UnityNativeTemplateStyle templateStyle, final int positionCode) {
    removeTemplateView();

    mPositionCode = positionCode;
    mAdSize = null;

    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            templateView = templateStyle.asTemplateView(activity);
            templateView.setNativeAd(nativeAd);

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

    mPositionCode = PluginUtils.POSITION_CUSTOM;
    mHorizontalOffset = positionX;
    mVerticalOffset = positionY;
    mAdSize = adSize;

    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            templateView = templateStyle.asTemplateView(activity);
            templateView.setNativeAd(nativeAd);

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
   * @param positionCode A code indicating where to place the template ad.
   */
  public void renderCustomSizeAtPositionCode(
      final UnityNativeTemplateStyle templateStyle, final AdSize adSize, final int positionCode) {
    removeTemplateView();

    mPositionCode = positionCode;
    mAdSize = adSize;

    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            templateView = templateStyle.asTemplateView(activity);
            templateView.setNativeAd(nativeAd);

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
                ((ViewGroup) parentView).removeView(templateView);
              }
            }
          }
        });

    activity
        .getWindow()
        .getDecorView()
        .getRootView()
        .removeOnLayoutChangeListener(mLayoutChangeListener);

    mLayoutChangeListener = null;
  }

  /** Sets a listener to update the position of the Native Overlay in case of layout changes */
  protected void setLayoutChangeListener() {
    if (mLayoutChangeListener != null) {
      return;
    }

    mLayoutChangeListener =
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
        .addOnLayoutChangeListener(mLayoutChangeListener);
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
            if (mAdSize != null) {
              layoutParams.height = mAdSize.getHeight();
              layoutParams.width = mAdSize.getWidth();
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
              ViewGroup vg = (ViewGroup) (templateView.getParent());
              vg.removeView(templateView);
            }
          });
    }
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
    adParams.gravity = PluginUtils.getLayoutGravityForPositionCode(mPositionCode);

    Insets insets = getInsets();
    int insetLeft = insets.left;
    int insetTop = insets.top;
    adParams.bottomMargin = insets.bottom;
    adParams.rightMargin = insets.right;
    if (mPositionCode == PluginUtils.POSITION_CUSTOM) {
      int leftOffset = (int) PluginUtils.convertDpToPixel(mHorizontalOffset);
      if (leftOffset < insetLeft) {
        leftOffset = insetLeft;
      }
      int topOffset = (int) PluginUtils.convertDpToPixel(mVerticalOffset);
      if (topOffset < insetTop) {
        topOffset = insetTop;
      }
      adParams.leftMargin = leftOffset;
      adParams.topMargin = topOffset;
    } else {
      adParams.leftMargin = insetLeft;
      if (mPositionCode == PluginUtils.POSITION_TOP
          || mPositionCode == PluginUtils.POSITION_TOP_LEFT
          || mPositionCode == PluginUtils.POSITION_TOP_RIGHT) {
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
