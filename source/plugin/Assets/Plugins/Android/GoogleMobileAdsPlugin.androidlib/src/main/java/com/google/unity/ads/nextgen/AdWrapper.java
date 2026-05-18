package com.google.unity.ads.nextgen;

import com.google.android.libraries.ads.mobile.sdk.appopen.AppOpenAd;
import com.google.android.libraries.ads.mobile.sdk.banner.BannerAd;
import com.google.android.libraries.ads.mobile.sdk.banner.BannerAdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest;
import com.google.android.libraries.ads.mobile.sdk.interstitial.InterstitialAd;
import com.google.android.libraries.ads.mobile.sdk.rewarded.RewardedAd;
import com.google.android.libraries.ads.mobile.sdk.rewardedinterstitial.RewardedInterstitialAd;

/**
 * A generic wrapper for loading ads. This wrapper is used for enabling unit testing by being
 * replaced with a mock Ad.
 *
 * @param <T> The type of ad to load (e.g., InterstitialAd, RewardedAd, RewardedInterstitialAd).
 */
class AdWrapper<T> {

  /** A functional interface that matches the signature of the static `load` methods. */
  interface AdLoader<T> {
    void load(AdRequest adRequest, AdLoadCallback<T> callback);
  }

  private final AdLoader<T> adLoader;

  public AdWrapper(AdLoader<T> adLoader) {
    this.adLoader = adLoader;
  }

  public void load(AdRequest adRequest, AdLoadCallback<T> callback) {
    adLoader.load(adRequest, callback);
  }

  /** Creates a new AdWrapper for loading AppOpenAds. */
  public static AdWrapper<AppOpenAd> forAppOpen() {
    return new AdWrapper<>(AppOpenAd::load);
  }

  /** Creates a new AdWrapper for loading BannerAds. */
  public static AdWrapper<BannerAd> forBanner() {
    return new AdWrapper<BannerAd>(
        new AdLoader<BannerAd>() {
          @Override
          public void load(AdRequest adRequest, AdLoadCallback<BannerAd> callback) {
            if (adRequest instanceof BannerAdRequest) {
              BannerAd.load((BannerAdRequest) adRequest, callback);
            } else {
              throw new IllegalArgumentException(
                  "AdRequest must be of type BannerAdRequest for Banner Ads");
            }
          }
        });
  }

  /** Creates a new AdWrapper for loading InterstitialAds. */
  public static AdWrapper<InterstitialAd> forInterstitial() {
    return new AdWrapper<>(InterstitialAd::load);
  }

  /** Creates a new AdWrapper for loading RewardedAds. */
  public static AdWrapper<RewardedAd> forRewarded() {
    return new AdWrapper<>(RewardedAd::load);
  }

  /** Creates a new AdWrapper for loading RewardedInterstitialAds. */
  public static AdWrapper<RewardedInterstitialAd> forRewardedInterstitial() {
    return new AdWrapper<>(RewardedInterstitialAd::load);
  }
}
