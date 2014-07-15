package com.google.ads.mediation.sample.customevent;

import com.google.ads.mediation.sample.sdk.SampleAdListener;
import com.google.ads.mediation.sample.sdk.SampleAdView;
import com.google.ads.mediation.sample.sdk.SampleErrorCode;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.mediation.customevent.CustomEventBannerListener;

/**
 * A {@link SampleAdListener} that forwards events to AdMob's
 * {@link CustomEventBannerListener}.
 */
public class SampleCustomBannerEventForwarder extends SampleAdListener {
  private CustomEventBannerListener bannerListener;
  private SampleAdView adView;

  /**
   * Creates a new {@code SampleBannerEventForwarder}.
   * @param listener An AdMob Mediation {@link CustomEventBannerListener} that should receive
   *     forwarded events.
   * @param adView A {@link SampleAdView}.
   */
  public SampleCustomBannerEventForwarder(
      CustomEventBannerListener listener, SampleAdView adView) {
    this.bannerListener = listener;
    this.adView = adView;
  }

  @Override
  public void onAdFetchSucceeded() {
    bannerListener.onAdLoaded(adView);
  }

  @Override
  public void onAdFetchFailed(SampleErrorCode errorCode) {
    switch(errorCode) {
      case UNKNOWN:
        bannerListener.onAdFailedToLoad(AdRequest.ERROR_CODE_INTERNAL_ERROR);
        break;
      case BAD_REQUEST:
        bannerListener.onAdFailedToLoad(AdRequest.ERROR_CODE_INVALID_REQUEST);
        break;
      case NETWORK_ERROR:
        bannerListener.onAdFailedToLoad(AdRequest.ERROR_CODE_NETWORK_ERROR);
        break;
      case NO_INVENTORY:
        bannerListener.onAdFailedToLoad(AdRequest.ERROR_CODE_NO_FILL);
        break;
    }
  }
  
  @Override
  public void onAdFullScreen() {
    bannerListener.onAdClicked();
    bannerListener.onAdOpened();
    // Only call onAdLeftApplication if your ad network actually exits the developer's app.
    bannerListener.onAdLeftApplication();
  }

  @Override
  public void onAdClosed() {
    bannerListener.onAdClosed();
  }
}
