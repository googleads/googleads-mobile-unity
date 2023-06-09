package com.google.unity.ads.admanager;

import androidx.annotation.NonNull;
import com.google.android.gms.ads.AdError;
import com.google.android.gms.ads.AdValue;
import com.google.android.gms.ads.FullScreenContentCallback;
import com.google.android.gms.ads.OnPaidEventListener;
import com.google.android.gms.ads.admanager.AdManagerInterstitialAd;
import com.google.android.gms.ads.admanager.AdManagerInterstitialAdLoadCallback;
import com.google.android.gms.ads.admanager.AppEventListener;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

/**
 * This class invokes callbacks in {@link UnityAdManagerInterstitialAdCallback} when a corresponding
 * callback in {@link AdManagerInterstitialAdLoadCallback} is invoked.
 */
public class AdManagerInterstitialAdCallback extends AdManagerInterstitialAdLoadCallback {

  /** The {@link AdManagerInterstitialAd}. */
  private AdManagerInterstitialAd adManagerInterstitialAd;

  /** A listener implemented in Unity via {@code AndroidJavaProxy} to receive ad events. */
  private UnityAdManagerInterstitialAdCallback callback;

  /**
   * Creates an instance of AdManagerInterstitialAdCallback to be as a parameter while loading an
   * AdManager interstitial ad.
   *
   * @param adManagerInterstitialAd Reference to the {@link AdManagerInterstitialAd}.
   * @param callback Reference to the {@link UnityAdManagerInterstitialAdCallback}.
   */
  AdManagerInterstitialAdCallback(
      AdManagerInterstitialAd adManagerInterstitialAd,
      UnityAdManagerInterstitialAdCallback callback) {
    this.adManagerInterstitialAd = adManagerInterstitialAd;
    this.callback = callback;
  }

  @Override
  public void onAdLoaded(@NonNull AdManagerInterstitialAd ad) {
    ExecutorService service = Executors.newSingleThreadExecutor();
    adManagerInterstitialAd = ad;

    adManagerInterstitialAd.setOnPaidEventListener(
        new OnPaidEventListener() {
          @Override
          public void onPaidEvent(final AdValue adValue) {
            service.execute(
                () -> {
                  if (callback != null) {
                    callback.onPaidEvent(
                        adValue.getPrecisionType(),
                        adValue.getValueMicros(),
                        adValue.getCurrencyCode());
                  }
                });
          }
        });

    adManagerInterstitialAd.setAppEventListener(
        new AppEventListener() {
          @Override
          public void onAppEvent(final String name, final String data) {
            service.execute(
                () -> {
                  if (callback != null) {
                    callback.onAppEvent(name, data);
                  }
                });
          }
        });

    adManagerInterstitialAd.setFullScreenContentCallback(
        new FullScreenContentCallback() {
          @Override
          public void onAdFailedToShowFullScreenContent(final AdError error) {
            service.execute(
                () -> {
                  if (callback != null) {
                    callback.onAdFailedToShowFullScreenContent(error);
                  }
                });
          }

          @Override
          public void onAdShowedFullScreenContent() {
            service.execute(
                () -> {
                  if (callback != null) {
                    callback.onAdShowedFullScreenContent();
                  }
                });
          }

          @Override
          public void onAdDismissedFullScreenContent() {
            service.execute(
                () -> {
                  if (callback != null) {
                    callback.onAdDismissedFullScreenContent();
                  }
                });
          }

          @Override
          public void onAdImpression() {
            service.execute(
                () -> {
                  if (callback != null) {
                    callback.onAdImpression();
                  }
                });
          }

          @Override
          public void onAdClicked() {
            service.execute(
                () -> {
                  if (callback != null) {
                    callback.onAdClicked();
                  }
                });
          }
        });

    service.execute(
        () -> {
          if (callback != null) {
            callback.onInterstitialAdLoaded();
          }
        });
  }
}
