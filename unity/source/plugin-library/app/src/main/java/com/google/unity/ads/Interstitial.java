// Copyright 2014 Google Inc. All Rights Reserved.

package com.google.unity.ads;

import com.google.android.gms.ads.AdListener;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.InterstitialAd;
import com.google.android.gms.ads.purchase.PlayStorePurchaseListener;

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
  private UnityAdListener adListener;

  /** Whether or not the {@link InterstitialAd} is ready to be shown. */
  private boolean isLoaded;

  public Interstitial(Activity activity, UnityAdListener adListener) {
    this.activity = activity;
    this.adListener = adListener;
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
            adListener.onAdLoaded();
          }

          @Override
          public void onAdFailedToLoad(int errorCode) {
            adListener.onAdFailedToLoad(PluginUtils.getErrorReason(errorCode));
          }

          @Override
          public void onAdOpened() {
            adListener.onAdOpened();
          }

          @Override
          public void onAdClosed() {
            adListener.onAdClosed();
          }

          @Override
          public void onAdLeftApplication() {
            adListener.onAdLeftApplication();
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
   * Sets Play Store purchase parameters. The PlayStorePurchaseListener is implemented on the Unity
   * side via {@code AndroidJavaProxy}.
   * @param purchaseListener A PlayStorePurchaseListener for monitoring purchase events.
   * @param publicKey The app's public key string.
   */
  public void setPlayStorePurchaseParams(final PlayStorePurchaseListener purchaseListener,
                                         final String publicKey) {
    activity.runOnUiThread(new Runnable() {
      @Override
      public void run() {
        interstitial.setPlayStorePurchaseParams(purchaseListener, publicKey);
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
