package com.google.ads.mediation.sample.sdk;

import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.view.View;
import android.widget.TextView;

import java.util.Random;

/**
 * An ad view for the sample ad network. This is an example of an ad view that most ad network SDKs
 * have.
 */
public class SampleAdView extends TextView {
  private SampleAdSize adSize;
  private String adUnit;
  private SampleAdListener listener;

  /**
   * Create a new {@link SampleAdView}.
   * @param context An Android {@link Context}.
   */
  public SampleAdView(Context context) {
    super(context);
  }

  /**
   * Sets the size of the banner.
   * @param size The banner size.
   */
  public void setSize(SampleAdSize size) {
    this.adSize = size;
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

    // If the publisher didn't set a size or ad unit, return a bad request.
    if (adSize == null || adUnit == null) {
      listener.onAdFetchFailed(SampleErrorCode.BAD_REQUEST);
    }

    // Randomly decide whether to succeed or fail.
    Random random = new Random();
    int nextInt = random.nextInt(100);
    if (listener != null) {
      if (nextInt < 85) {
        this.setText("Sample Text Ad");
        this.setOnClickListener(new OnClickListener() {
          @Override
          public void onClick(View view) {
            // Notify the developer that a full screen view will be presented.
            listener.onAdFullScreen();
            Intent intent = new Intent(Intent.ACTION_VIEW, Uri.parse("http://www.google.com"));
            SampleAdView.this.getContext().startActivity(intent);
          }
        });
        listener.onAdFetchSucceeded();
      } else if (nextInt < 90) {
        listener.onAdFetchFailed(SampleErrorCode.UNKNOWN);
      } else if (nextInt < 95) {
        listener.onAdFetchFailed(SampleErrorCode.NETWORK_ERROR);
      } else if (nextInt < 100) {
        listener.onAdFetchFailed(SampleErrorCode.NO_INVENTORY);
      }
    }
  }

  /**
   * Destroy the banner.
   */
  public void destroy() {
    listener = null;
  }
}
