package com.google.ads.mediation.sample.sdk;

/**
 * A sample ad listener to listen for ad events. These ad events more or less represent the events
 * that a typical ad network would provide.
 */
public abstract class SampleAdListener {
  /**
   * Called when an ad is successfully fetched.
   */
  public void onAdFetchSucceeded() {
    // Default is to do nothing.
  }

  /**
   * Called when an ad fetch fails.
   * @param code The reason the fetch failed.
   */
  public void onAdFetchFailed(SampleErrorCode code) {
    // Default is to do nothing.
  }

  /**
   * Called when an ad goes full screen.
   */
  public void onAdFullScreen() {
    // Default is to do nothing.
  }

  /**
   * Called when an ad is closed.
   */
  public void onAdClosed() {
    // Default is to do nothing.
  }
}
