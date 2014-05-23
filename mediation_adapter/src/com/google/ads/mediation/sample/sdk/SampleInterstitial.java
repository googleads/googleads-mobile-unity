package com.google.ads.mediation.sample.sdk;

import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;

import java.util.Random;

/**
 * A sample interstitial ad. This is an example of an interstitial class that most ad networks SDKs
 * have.
 */
public class SampleInterstitial {
  private Context context;
  private String adUnit;
  private SampleAdListener listener;

  /**
   * Create a new {@link SampleInterstitial}.
   * @param context An Android {@link Context}.
   */
  public SampleInterstitial(Context context) {
    this.context = context;
  }

  /**
   * Sets the sample ad unit.
   * @param sampleAdUnit The sample ad unit.
   */
  public void setAdUnit(String sampleAdUnit) {
    this.adUnit = sampleAdUnit;
  }

  /**
   * Sets a {@link SampleAdListener} to listen for ad events.
   * @param listener The ad listener.
   */
  public void setAdListener(SampleAdListener listener) {
    this.listener = listener;
  }

  /**
   * Fetch an ad. Instead of doing an actual ad fetch, we will randomly decide to succeed, or
   * fail with different error codes.
   * @param request The ad request with targeting information.
   */
  public void fetchAd(SampleAdRequest request) {
    if (listener == null) {
      return;
    }

    // If the publisher didn't set an ad unit, return a bad request.
    if (adUnit == null) {
      listener.onAdFetchFailed(SampleErrorCode.BAD_REQUEST);
    }

    Random random = new Random();
    int nextInt = random.nextInt(100);
    if (listener != null) {
      if (nextInt < 80) {
        listener.onAdFetchSucceeded();
      } else if (nextInt < 85) {
        listener.onAdFetchFailed(SampleErrorCode.UNKNOWN);
      } else if (nextInt < 90) {
        listener.onAdFetchFailed(SampleErrorCode.BAD_REQUEST);
      } else if (nextInt < 95) {
        listener.onAdFetchFailed(SampleErrorCode.NETWORK_ERROR);
      } else if (nextInt < 100) {
        listener.onAdFetchFailed(SampleErrorCode.NO_INVENTORY);
      }
    }
  }

  /**
   * Shows the interstitial.
   */
  public void show() {
    // Notify the developer that a full screen view will be presented.
    listener.onAdFullScreen();
    new AlertDialog.Builder(context)
        .setTitle("Sample Interstitial")
        .setMessage("You are viewing a sample interstitial ad.")
        .setNeutralButton(android.R.string.ok, new DialogInterface.OnClickListener() {
          @Override
          public void onClick(DialogInterface dialog, int which) {
            // Notify the developer that the interstitial was closed.
            listener.onAdClosed();
          }
        })
        .show();
  }

  /**
   * Destroy the interstitial.
   */
  public void destroy() {
    listener = null;
  }
}

