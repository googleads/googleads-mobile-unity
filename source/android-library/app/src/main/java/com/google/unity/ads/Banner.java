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
  private static class Insets {
    int top = 0;
    int bottom = 0;
    int left = 0;
    int right = 0;
  }

  /** The {@link AdView} to display to the user. */
  private AdView mAdView;

  /** The {@code Activity} that the banner will be displayed in. */
  private Activity mUnityPlayerActivity;

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
  private boolean mHidden;

  /** A listener implemented in Unity via {@code AndroidJavaProxy} to receive ad events. */
  private UnityAdListener mUnityListener;

  /**
   * A {@code View.OnLayoutChangeListener} used to detect orientation changes and reposition banner
   * ads as required.
   */
  private View.OnLayoutChangeListener mLayoutChangeListener;

  /**
   * Creates an instance of {@code Banner}.
   *
   * @param activity The {@link Activity} that will contain an ad.
   * @param listener The {@link UnityAdListener} used to receive synchronous ad events in Unity.
   */
  public Banner(Activity activity, UnityAdListener listener) {
    this.mUnityPlayerActivity = activity;
    this.mUnityListener = listener;
  }

  /**
   * Creates an {@link AdView} to hold banner ads.
   *
   * @param publisherId Your ad unit ID.
   * @param adSize The size of the banner.
   * @param positionCode A code indicating where to place the ad.
   */
  public void create(final String publisherId, final AdSize adSize, final int positionCode) {
    mUnityPlayerActivity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            createAdView(publisherId, adSize);
            mHorizontalOffset = 0;
            mVerticalOffset = 0;
            mPositionCode = positionCode;
            mHidden = false;
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
    mUnityPlayerActivity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            createAdView(publisherId, adSize);
            mPositionCode = PluginUtils.POSITION_CUSTOM;
            mHorizontalOffset = positionX;
            mVerticalOffset = positionY;
            mHidden = false;
          }
        });
  }

  private void createAdView(final String publisherId, final AdSize adSize) {
    mAdView = new AdView(mUnityPlayerActivity);
    // Setting the background color works around an issue where the first ad isn't visible.
    mAdView.setBackgroundColor(Color.TRANSPARENT);
    mAdView.setAdUnitId(publisherId);
    mAdView.setAdSize(adSize);
    mAdView.setVisibility(View.GONE);
    mUnityPlayerActivity.addContentView(mAdView, getLayoutParams());
    mAdView.setAdListener(
        new AdListener() {
          @Override
          public void onAdLoaded() {
            if (mUnityListener != null) {
              if (!mHidden) {
                show();
              }

              new Thread(
                      new Runnable() {
                        @Override
                        public void run() {
                          if (mUnityListener != null) {
                            mUnityListener.onAdLoaded();
                          }
                        }
                      })
                  .start();
            }
          }

          @Override
          public void onAdFailedToLoad(final LoadAdError error) {
            if (mUnityListener != null) {
              new Thread(
                      new Runnable() {
                        @Override
                        public void run() {
                          if (mUnityListener != null) {
                            mUnityListener.onAdFailedToLoad(error);
                          }
                        }
                      })
                  .start();
            }
          }

          @Override
          public void onAdOpened() {
            if (mUnityListener != null) {
              new Thread(
                      new Runnable() {
                        @Override
                        public void run() {
                          if (mUnityListener != null) {
                            mUnityListener.onAdOpened();
                          }
                        }
                      })
                  .start();
            }
          }

          @Override
          public void onAdClosed() {
            if (mUnityListener != null) {
              new Thread(
                      new Runnable() {
                        @Override
                        public void run() {
                          if (mUnityListener != null) {
                            mUnityListener.onAdClosed();
                          }
                        }
                      })
                  .start();
            }
          }

          @Override
          public void onAdLeftApplication() {
            if (mUnityListener != null) {
              new Thread(
                      new Runnable() {
                        @Override
                        public void run() {
                          if (mUnityListener != null) {
                            mUnityListener.onAdLeftApplication();
                          }
                        }
                      })
                  .start();
            }
          }
        });


    mAdView.setOnPaidEventListener(
        new OnPaidEventListener() {
          @Override
          public void onPaidEvent(final AdValue adValue) {
            if (mUnityListener != null) {
              new Thread(
                      new Runnable() {
                        @Override
                        public void run() {
                          if (mUnityListener != null) {
                            mUnityListener.onPaidEvent(
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

            if (!mHidden) {
              updatePosition();
            }
          }
        };

    mUnityPlayerActivity
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
    mUnityPlayerActivity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            Log.d(PluginUtils.LOGTAG, "Calling loadAd() on Android");
            mAdView.loadAd(request);
          }
        });
  }

  /** Sets the {@link AdView} to be visible. */
  public void show() {
    mUnityPlayerActivity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            Log.d(PluginUtils.LOGTAG, "Calling show() on Android");
            mHidden = false;
            mAdView.setVisibility(View.VISIBLE);
            updatePosition();
            mAdView.resume();
          }
        });
  }

  /** Sets the {@link AdView} to be gone. */
  public void hide() {
    mUnityPlayerActivity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            Log.d(PluginUtils.LOGTAG, "Calling hide() on Android");
            mHidden = true;
            mAdView.setVisibility(View.GONE);
            mAdView.pause();
          }
        });
  }

  /** Destroys the {@link AdView}. */
  public void destroy() {
    mUnityPlayerActivity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            Log.d(PluginUtils.LOGTAG, "Calling destroy() on Android");
            if (mAdView != null) {
              mAdView.destroy();
              ViewParent parentView = mAdView.getParent();
              if (parentView instanceof ViewGroup) {
                ((ViewGroup) parentView).removeView(mAdView);
              }
            }
          }
        });

    mUnityPlayerActivity
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
                return mAdView.getAdSize().getHeightInPixels(mUnityPlayerActivity);
              }
            });
    mUnityPlayerActivity.runOnUiThread(task);

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
                return mAdView.getAdSize().getWidthInPixels(mUnityPlayerActivity);
              }
            });
    mUnityPlayerActivity.runOnUiThread(task);

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
    mUnityPlayerActivity.runOnUiThread(
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
    mUnityPlayerActivity.runOnUiThread(
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
  private FrameLayout.LayoutParams getLayoutParams() {
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
    if (mAdView == null || mHidden) {
      return;
    }
    mUnityPlayerActivity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            FrameLayout.LayoutParams layoutParams = getLayoutParams();
            mAdView.setLayoutParams(layoutParams);
          }
        });
  }

  private Insets getSafeInsets() {
    Insets insets = new Insets();

    if (Build.VERSION.SDK_INT < Build.VERSION_CODES.P) {
      return insets;
    }
    Window window = mUnityPlayerActivity.getWindow();
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
   * Returns the mediation adapter class name. In the case of a mediated ad response, this is the
   * name of the class that was responsible for performing the ad request and rendering the ad. For
   * non-mediated responses, this value will be {@code null}.
   */
  public String getMediationAdapterClassName() {
    return mAdView != null ? mAdView.getMediationAdapterClassName() : null;
  }

  /**
   * Returns the request response info.
   */
  public ResponseInfo getResponseInfo() {
    FutureTask<ResponseInfo> task = new FutureTask<>(new Callable<ResponseInfo>() {
      @Override
      public ResponseInfo call() {
        return mAdView.getResponseInfo();
      }
    });
    mUnityPlayerActivity.runOnUiThread(task);

    ResponseInfo result = null;
    try {
      result = task.get();
    } catch (InterruptedException exception) {
      Log.e(PluginUtils.LOGTAG,
              String.format("Unable to check banner response info: %s",
                      exception.getLocalizedMessage()));
    } catch (ExecutionException exception) {
      Log.e(PluginUtils.LOGTAG,
              String.format("Unable to check banner response info: %s",
                      exception.getLocalizedMessage()));
    }
    return result;
  }
}
