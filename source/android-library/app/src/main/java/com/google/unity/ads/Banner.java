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
import android.graphics.Color;
import android.os.Build;
import android.util.Log;
import android.view.DisplayCutout;
import android.view.View;
import android.view.ViewGroup;
import android.view.ViewParent;
import android.view.Window;
import android.view.WindowInsets;
import android.widget.FrameLayout;
import com.google.android.gms.ads.AdListener;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.AdSize;
import com.google.android.gms.ads.AdValue;
import com.google.android.gms.ads.AdView;
import com.google.android.gms.ads.BaseAdView;
import com.google.android.gms.ads.LoadAdError;
import com.google.android.gms.ads.OnPaidEventListener;
import com.google.android.gms.ads.ResponseInfo;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.FutureTask;

/**
 * This class represents the native implementation for the Google Mobile Ads Unity plugin. This
 * class is used to request Google Mobile ads natively via the Google Mobile Ads library in Google
 * Play services. The Google Play services library is a dependency for this plugin.
 */
public class Banner {

  /** Class to hold the insets of the cutout area. */
  protected static class Insets {
    int top = 0;
    int bottom = 0;
    int left = 0;
    int right = 0;
  }

  /** The {@link AdView} to display to the user. */
  protected BaseAdView adView;

  /** The {@code Activity} that the banner will be displayed in. */
  protected Activity unityPlayerActivity;

  /** A code indicating where to place the ad. */
  private int mPositionCode;

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

  /** A boolean indicating whether the ad has been hidden. */
  protected boolean hidden;

  /** A listener implemented in Unity via {@code AndroidJavaProxy} to receive ad events. */
  protected UnityAdListener unityListener;

  /**
   * A {@code View.OnLayoutChangeListener} used to detect orientation changes and reposition banner
   * ads as required.
   */
  private View.OnLayoutChangeListener mLayoutChangeListener;

  protected Banner() {}

  /**
   * Creates an instance of {@code Banner}.
   *
   * @param activity The {@link Activity} that will contain an ad.
   * @param listener The {@link UnityAdListener} used to receive synchronous ad events in Unity.
   */
  public Banner(Activity activity, UnityAdListener listener) {
    this.unityPlayerActivity = activity;
    this.unityListener = listener;
  }

  /**
   * Creates an {@link AdView} to hold banner ads.
   *
   * @param publisherId Your ad unit ID.
   * @param adSize The size of the banner.
   * @param positionCode A code indicating where to place the ad.
   */
  public void create(final String publisherId, final AdSize adSize, final int positionCode) {
    unityPlayerActivity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            createAdView(publisherId, adSize);
            mHorizontalOffset = 0;
            mVerticalOffset = 0;
            mPositionCode = positionCode;
            hidden = false;
          }
        });
  }

  /**
   * Creates an {@link AdView} to hold banner ads with a custom position.
   *
   * @param publisherId Your ad unit ID.
   * @param adSize The size of the banner.
   * @param positionX Position of banner ad on the x axis.
   * @param positionY Position of banner ad on the y axis.
   */
  public void create(
      final String publisherId, final AdSize adSize, final int positionX, final int positionY) {
    unityPlayerActivity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            createAdView(publisherId, adSize);
            mPositionCode = PluginUtils.POSITION_CUSTOM;
            mHorizontalOffset = positionX;
            mVerticalOffset = positionY;
            hidden = false;
          }
        });
  }

  protected void createAdView(final String publisherId, final AdSize adSize) {
    adView = new AdView(unityPlayerActivity);
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
            if (unityListener != null) {
              if (!hidden) {
                show();
              }

              new Thread(
                      new Runnable() {
                        @Override
                        public void run() {
                          if (unityListener != null) {
                            unityListener.onAdLoaded();
                          }
                        }
                      })
                  .start();
            }
          }

          @Override
          public void onAdFailedToLoad(final LoadAdError error) {
            if (unityListener != null) {
              new Thread(
                      new Runnable() {
                        @Override
                        public void run() {
                          if (unityListener != null) {
                            unityListener.onAdFailedToLoad(error);
                          }
                        }
                      })
                  .start();
            }
          }

          @Override
          public void onAdOpened() {
            if (unityListener != null) {
              new Thread(
                      new Runnable() {
                        @Override
                        public void run() {
                          if (unityListener != null) {
                            unityListener.onAdOpened();
                          }
                        }
                      })
                  .start();
            }
          }

          @Override
          public void onAdClosed() {
            if (unityListener != null) {
              new Thread(
                      new Runnable() {
                        @Override
                        public void run() {
                          if (unityListener != null) {
                            unityListener.onAdClosed();
                          }
                        }
                      })
                  .start();
            }
          }

          @Override
          public void onAdImpression() {
            if (unityListener != null) {
              new Thread(
                      new Runnable() {
                        @Override
                        public void run() {
                          if (unityListener != null) {
                            unityListener.onAdImpression();
                          }
                        }
                      })
                  .start();
            }
          }

          @Override
          public void onAdClicked() {
            if (unityListener != null) {
              new Thread(
                      new Runnable() {
                        @Override
                        public void run() {
                          if (unityListener != null) {
                            unityListener.onAdClicked();
                          }
                        }
                      })
                  .start();
            }
          }
        });

    adView.setOnPaidEventListener(
        new OnPaidEventListener() {
          @Override
          public void onPaidEvent(final AdValue adValue) {
            if (unityListener != null) {
              new Thread(
                      new Runnable() {
                        @Override
                        public void run() {
                          if (unityListener != null) {
                            unityListener.onPaidEvent(
                                adValue.getPrecisionType(),
                                adValue.getValueMicros(),
                                adValue.getCurrencyCode());
                          }
                        }
                      })
                  .start();
            }
          }
        });

    setLayoutChangeListener();
  }

  protected void setLayoutChangeListener() {
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
        .addOnLayoutChangeListener(mLayoutChangeListener);
  }

  /**
   * Loads an ad on a background thread.
   *
   * @param request The {@link AdRequest} object with targeting parameters.
   */
  public void loadAd(final AdRequest request) {
    unityPlayerActivity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            Log.d(PluginUtils.LOGTAG, "Calling loadAd() on Android");
            adView.loadAd(request);
          }
        });
  }

  /** Sets the {@link AdView} to be visible. */
  public void show() {
    unityPlayerActivity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            Log.d(PluginUtils.LOGTAG, "Calling show() on Android");
            hidden = false;
            adView.setVisibility(View.VISIBLE);
            updatePosition();
            adView.resume();
          }
        });
  }

  /** Sets the {@link AdView} to be gone. */
  public void hide() {
    unityPlayerActivity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            Log.d(PluginUtils.LOGTAG, "Calling hide() on Android");
            hidden = true;
            adView.setVisibility(View.GONE);
            adView.pause();
          }
        });
  }

  /** Destroys the {@link AdView}. */
  public void destroy() {
    unityPlayerActivity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            Log.d(PluginUtils.LOGTAG, "Calling destroy() on Android");
            if (adView != null) {
              adView.destroy();
              ViewParent parentView = adView.getParent();
              if (parentView instanceof ViewGroup) {
                ((ViewGroup) parentView).removeView(adView);
              }
            }
          }
        });

    unityPlayerActivity
        .getWindow()
        .getDecorView()
        .getRootView()
        .removeOnLayoutChangeListener(mLayoutChangeListener);
  }

  /**
   * Get {@link AdView} height.
   *
   * @return the height of the {@link AdView}.
   */
  public float getHeightInPixels() {
    FutureTask<Integer> task =
        new FutureTask<>(
            new Callable<Integer>() {
              @Override
              public Integer call() throws Exception {
                return adView.getAdSize().getHeightInPixels(unityPlayerActivity);
              }
            });
    unityPlayerActivity.runOnUiThread(task);

    float result = -1;
    try {
      result = task.get();
    } catch (InterruptedException e) {
      Log.e(
          PluginUtils.LOGTAG,
          String.format("Failed to get ad view height: %s", e.getLocalizedMessage()));
    } catch (ExecutionException e) {
      Log.e(
          PluginUtils.LOGTAG,
          String.format("Failed to get ad view height: %s", e.getLocalizedMessage()));
    }
    return result;
  }

  /**
   * Get {@link AdView} width.
   *
   * @return the width of the {@link AdView}.
   */
  public float getWidthInPixels() {
    FutureTask<Integer> task =
        new FutureTask<>(
            new Callable<Integer>() {
              @Override
              public Integer call() throws Exception {
                return adView.getAdSize().getWidthInPixels(unityPlayerActivity);
              }
            });
    unityPlayerActivity.runOnUiThread(task);

    float result = -1;
    try {
      result = task.get();
    } catch (InterruptedException e) {
      Log.e(
          PluginUtils.LOGTAG,
          String.format("Failed to get ad view width: %s", e.getLocalizedMessage()));
    } catch (ExecutionException e) {
      Log.e(
          PluginUtils.LOGTAG,
          String.format("Failed to get ad view width: %s", e.getLocalizedMessage()));
    }
    return result;
  }

  /**
   * Updates the {@link AdView} position.
   *
   * @param positionCode A code indicating where to place the ad.
   */
  public void setPosition(final int positionCode) {
    unityPlayerActivity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            mPositionCode = positionCode;
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
            mPositionCode = PluginUtils.POSITION_CUSTOM;
            mHorizontalOffset = positionX;
            mVerticalOffset = positionY;
            updatePosition();
          }
        });
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
    adParams.gravity = PluginUtils.getLayoutGravityForPositionCode(mPositionCode);

    Insets insets = getSafeInsets();
    int safeInsetLeft = insets.left;
    int safeInsetTop = insets.top;
    adParams.bottomMargin = insets.bottom;
    adParams.rightMargin = insets.right;

    if (mPositionCode == PluginUtils.POSITION_CUSTOM) {
      int leftOffset = (int) PluginUtils.convertDpToPixel(mHorizontalOffset);
      if (leftOffset < safeInsetLeft) {
        leftOffset = safeInsetLeft;
      }
      int topOffset = (int) PluginUtils.convertDpToPixel(mVerticalOffset);
      if (topOffset < safeInsetTop) {
        topOffset = safeInsetTop;
      }
      adParams.leftMargin = leftOffset;
      adParams.topMargin = topOffset;
    } else {
      adParams.leftMargin = safeInsetLeft;
      if (mPositionCode == PluginUtils.POSITION_TOP
          || mPositionCode == PluginUtils.POSITION_TOP_LEFT
          || mPositionCode == PluginUtils.POSITION_TOP_RIGHT) {
        adParams.topMargin = safeInsetTop;
      }
    }
    return adParams;
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

  /**
   * Returns the request response info.
   */
  public ResponseInfo getResponseInfo() {
    FutureTask<ResponseInfo> task = new FutureTask<>(() -> adView.getResponseInfo());
    unityPlayerActivity.runOnUiThread(task);

    ResponseInfo result = null;
    try {
      result = task.get();
    } catch (InterruptedException | ExecutionException exception) {
      Log.e(PluginUtils.LOGTAG,
              String.format("Unable to check banner response info: %s",
                      exception.getLocalizedMessage()));
    }
    return result;
  }
}
