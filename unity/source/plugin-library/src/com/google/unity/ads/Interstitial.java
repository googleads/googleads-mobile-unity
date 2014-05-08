// Copyright 2014 Google Inc. All Rights Reserved.

package com.google.unity.ads;

import com.google.android.gms.ads.AdListener;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.InterstitialAd;

import android.app.Activity;
import android.util.Log;

/**
 * Native interstitial implementation for the Google Mobile Ads Unity plugin.
 */
public class Interstitial {
  /** The {@link InterstitialAd}. */
  private InterstitialAd interstitial;

  /** The {@code Activity} on which the interstitial will display. */
  private Activity activity;

  /** A listener implemented in Unity via {@code AndroidJavaProxy} to receive ad events. */
  private UnityAdListener listener;

  /** Whether or not the {@link InterstitialAd} is ready to be shown. */
  private boolean isLoaded;

  public Interstitial(Activity activity, UnityAdListener listener) {
    this.activity = activity;
    this.listener = listener;
    this.isLoaded = false;
  }

  /**
   * Creates an {@link InterstitialAd}.
   *
   * @param adUnitId Your interstitial ad unit ID.
   */
  public void create(final String adUnitId) {
    activity.runOnUiThread(new Runnable() {
      @Override
      public void run() {
        interstitial = new InterstitialAd(activity);
        interstitial.setAdUnitId(adUnitId);
        interstitial.setAdListener(new AdListener() {
          @Override
          public void onAdLoaded() {
            isLoaded = true;
            listener.onAdLoaded();
          }

          @Override
          public void onAdFailedToLoad(int errorCode) {
            listener.onAdFailedToLoad(PluginUtils.getErrorReason(errorCode));
          }

          @Override
          public void onAdOpened() {
            listener.onAdOpened();
          }

          @Override
          public void onAdClosed() {
            listener.onAdClosed();
          }

          @Override
          public void onAdLeftApplication() {
            listener.onAdLeftApplication();
          }
        });
      }
    });
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
        interstitial.loadAd(request);
      }
    });
  }

  /**
   * Returns {@code True} if the interstitial has loaded.
   *
   * @return {@code True} if the interstitial has loaded.
   */
  public boolean isLoaded() {
    return isLoaded;
  }

  /**
   * Shows the interstitial if it has loaded.
   */
  public void show() {
    activity.runOnUiThread(new Runnable() {
      @Override
      public void run() {
        if (interstitial.isLoaded()) {
          isLoaded = false;
          interstitial.show();
        } else {
          Log.d(PluginUtils.LOGTAG, "Interstitial was not ready to be shown.");
        }
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
