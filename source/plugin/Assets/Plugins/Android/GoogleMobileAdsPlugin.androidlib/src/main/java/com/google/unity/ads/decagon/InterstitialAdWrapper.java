package com.google.unity.ads.decagon;

import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest;
import com.google.android.libraries.ads.mobile.sdk.interstitial.InterstitialAd;

/**
 * A wrapper for loading interstitial ads. This wrapper is used for enabling unit testing by being
 * replaced with a mock Ad.
 */
class InterstitialAdWrapper {
  public void load(AdRequest adRequest, AdLoadCallback<InterstitialAd> callback) {
    InterstitialAd.load(adRequest, callback);
  }
}
