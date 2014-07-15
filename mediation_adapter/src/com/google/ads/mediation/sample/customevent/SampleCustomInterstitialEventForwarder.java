package com.google.ads.mediation.sample.customevent;

import com.google.ads.mediation.sample.sdk.SampleAdListener;
import com.google.ads.mediation.sample.sdk.SampleErrorCode;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.mediation.customevent.CustomEventInterstitialListener;

/**
 * A {@link SampleAdListener} that forwards events to AdMob Mediation's
 * {@link CustomEventInterstitialListener}.
 */
public class SampleCustomInterstitialEventForwarder extends SampleAdListener {
  private CustomEventInterstitialListener interstitialListener;
  /**
   * Creates a new {@code SampleInterstitialEventForwarder}.
   * @param listener An AdMob Mediation {@link MediationInterstitialListener} that should receive
   *     forwarded events.
   */
  public SampleCustomInterstitialEventForwarder(CustomEventInterstitialListener listener) {
    this.interstitialListener = listener;
  }

  @Override
  public void onAdFetchSucceeded() {
    interstitialListener.onAdLoaded();
  }

  @Override
  public void onAdFetchFailed(SampleErrorCode errorCode) {
    switch(errorCode) {
      case UNKNOWN:
        interstitialListener.onAdFailedToLoad(AdRequest.ERROR_CODE_INTERNAL_ERROR);
        break;
      case BAD_REQUEST:
        interstitialListener.onAdFailedToLoad(AdRequest.ERROR_CODE_INVALID_REQUEST);
        break;
      case NETWORK_ERROR:
        interstitialListener.onAdFailedToLoad(AdRequest.ERROR_CODE_NETWORK_ERROR);
        break;
      case NO_INVENTORY:
        interstitialListener.onAdFailedToLoad(AdRequest.ERROR_CODE_NO_FILL);
        break;
    }
  }

  @Override
  public void onAdFullScreen() {
    interstitialListener.onAdOpened();
    // Only call onAdLeftApplication if your ad network actually exits the developer's app.
    interstitialListener.onAdLeftApplication();
  }

  @Override
  public void onAdClosed() {
    interstitialListener.onAdClosed();
  }
}
