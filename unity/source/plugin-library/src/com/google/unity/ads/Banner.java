// Copyright 2014 Google Inc. All Rights Reserved.

package com.google.unity.ads;

import com.google.android.gms.ads.AdListener;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.AdSize;
import com.google.android.gms.ads.AdView;

import android.app.Activity;
import android.graphics.Color;
import android.util.Log;
import android.view.Gravity;
import android.view.View;
import android.widget.FrameLayout;

/**
 * This class represents the native implementation for the Google Mobile Ads Unity plugin. This
 * class is used to request Google Mobile ads natively via the Google Mobile Ads library in Google
 * Play services. The Google Play services library is a dependency for this plugin.
 */
public class Banner {
  /** Banner position constant for the top of the screen. */
  private static final int POSITION_TOP = 0;

  /** Banner position constant for the bottom of the screen. */
  private static final int POSITION_BOTTOM = 1;

  /** The {@link AdView} to display to the user. */
  private AdView adView;

  /** The {@code Activity} that the banner will be displayed in. */
  private Activity activity;

  /** A listener implemented in Unity via {@code AndroidJavaProxy} to receive ad events. */
  private UnityAdListener listener;

  /**
   * Creates an instance of {@code Banner}.
   *
   * @param activity The {@link Activity} that will contain an ad.
   * @param listener The {@link UnityAdListener} used to receive synchronous ad events in Unity.
   */
  public Banner(Activity activity, UnityAdListener listener) {
    this.activity = activity;
    this.listener = listener;
  }

  /**
   * Creates an {@link AdView} to hold banner ads.
   *
   * @param publisherId Your ad unit ID.
   * @param adSize The size of the banner.
   * @param positionCode A code indicating where to place the ad.
   */
  public void create(final String publisherId, final AdSize adSize, final int positionCode) {
    activity.runOnUiThread(new Runnable() {
      @Override
      public void run() {
        adView = new AdView(activity);
        // Setting the background color works around an issue where the first ad isn't visible.
        adView.setBackgroundColor(Color.TRANSPARENT);
        adView.setAdUnitId(publisherId);
        adView.setAdSize(adSize);
        adView.setAdListener(new AdListener() {
          @Override
          public void onAdLoaded() {
            if (listener != null) {
              listener.onAdLoaded();
            }
          }

          @Override
          public void onAdFailedToLoad(int errorCode) {
            if (listener != null) {
              listener.onAdFailedToLoad(PluginUtils.getErrorReason(errorCode));
            }
          }

          @Override
          public void onAdOpened() {
            if (listener != null) {
              listener.onAdOpened();
            }
          }

          @Override
          public void onAdClosed() {
            if (listener != null) {
              listener.onAdClosed();
            }
          }

          @Override
          public void onAdLeftApplication() {
            if (listener != null) {
              listener.onAdLeftApplication();
            }
          }
        });
        FrameLayout.LayoutParams adParams = new FrameLayout.LayoutParams(
            FrameLayout.LayoutParams.MATCH_PARENT, FrameLayout.LayoutParams.WRAP_CONTENT);
        switch(positionCode) {
          case POSITION_TOP:
            adParams.gravity = Gravity.TOP;
            break;
          case POSITION_BOTTOM:
            adParams.gravity = Gravity.BOTTOM;
            break;
        }
        activity.addContentView(adView, adParams);
      }
    });
  }

  public void setAdListener(UnityAdListener listener) {
    this.listener = listener;
  }

  /**
   * Loads an ad on a background thread.
   *
   * @param request The {@link AdRequest} object with targeting parameters.
   */
  public void loadAd(final AdRequest request) {
    activity.runOnUiThread(new Runnable() {
      @Override
      public void run() {
        Log.d(PluginUtils.LOGTAG, "Calling loadAd() on Android");
        adView.loadAd(request);
      }
    });
  }

  /**
   * Sets the {@link AdView} to be visible.
   */
  public void show() {
    activity.runOnUiThread(new Runnable() {
      @Override
      public void run() {
        Log.d(PluginUtils.LOGTAG, "Calling show() on Android");
        adView.setVisibility(View.VISIBLE);
        adView.resume();
      }
    });
  }

  /**
   * Sets the {@link AdView} to be gone.
   */
  public void hide() {
    activity.runOnUiThread(new Runnable() {
      @Override
      public void run() {
        Log.d(PluginUtils.LOGTAG, "Calling hide() on Android");
        adView.setVisibility(View.GONE);
        adView.pause();
      }
    });
  }

  /**
   * Destroys the {@link AdView}.
   */
  public void destroy() {
    activity.runOnUiThread(new Runnable() {
      @Override
      public void run() {
        Log.d(PluginUtils.LOGTAG, "Calling destroy() on Android");
        adView.destroy();
      }
    });
  }
}
