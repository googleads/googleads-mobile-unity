package com.google.unity.ads.decagon;

import android.app.Activity;
import android.util.Log;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.annotation.VisibleForTesting;
import com.google.android.libraries.ads.mobile.sdk.appopen.AppOpenAd;
import com.google.android.libraries.ads.mobile.sdk.appopen.AppOpenAdEventCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.AdValue;
import com.google.android.libraries.ads.mobile.sdk.common.FullScreenContentError;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
import com.google.unity.ads.PluginUtils;
import java.util.concurrent.Executor;
import java.util.concurrent.Executors;

/** AppOpen ad implementation for the Google Mobile Ads Unity plugin. */
public class UnityAppOpenAd extends UnityAdBase<AppOpenAd, UnityAppOpenAdCallback> {

  private final AdWrapper<AppOpenAd> adWrapper;

  public UnityAppOpenAd(Activity activity, UnityAppOpenAdCallback callback) {
    this(activity, callback, AdWrapper.forAppOpen(), Executors.newSingleThreadExecutor());
  }

  @VisibleForTesting
  UnityAppOpenAd(
      Activity activity,
      UnityAppOpenAdCallback callback,
      AdWrapper<AppOpenAd> adWrapper,
      Executor executor) {
    super(activity, callback, executor);
    this.adWrapper = adWrapper;
  }

  /**
   * Loads an app open ad.
   *
   * @param request The {@link AdRequest} object with targeting parameters.
   */
  public void load(final AdRequest request) {
    activity.runOnUiThread(
        () ->
            adWrapper.load(
                request,
                new AdLoadCallback<AppOpenAd>() {
                  @Override
                  public void onAdLoaded(@NonNull AppOpenAd ad) {
                    UnityAppOpenAd.this.ad = ad;
                    executor.execute(
                        () -> {
                          if (callback != null) {
                            callback.onAppOpenAdLoaded();
                          }
                        });
                  }

                  @Override
                  public void onAdFailedToLoad(@NonNull LoadAdError adError) {
                    executor.execute(
                        () -> {
                          if (callback != null) {
                            callback.onAppOpenAdFailedToLoad(adError);
                          }
                        });
                  }
                }));
  }

  /** Shows the app open ad if it has loaded. */
  @SuppressWarnings("EnumOrdinal")
  public void show() {
    if (ad == null) {
      Log.e(
          PluginUtils.LOGTAG,
          "Tried to show app open ad before it was ready. Please call loadAd first and wait for"
              + " a successful onAdLoaded callback.");
      return;
    }

    // Listen for ad events.
    ad.setAdEventCallback(
        new AppOpenAdEventCallback() {
          @Override
          public void onAdShowedFullScreenContent() {
            // App open ad did show.
            executor.execute(
                () -> {
                  if (callback != null) {
                    callback.onAdShowedFullScreenContent();
                  }
                });
          }

          @Override
          public void onAdDismissedFullScreenContent() {
            executor.execute(
                () -> {
                  if (callback != null) {
                    callback.onAdDismissedFullScreenContent();
                  }
                });
            ad = null;
          }

          @Override
          public void onAdFailedToShowFullScreenContent(
              @NonNull FullScreenContentError fullScreenContentError) {
            executor.execute(
                () -> {
                  if (callback != null) {
                    callback.onAdFailedToShowFullScreenContent(fullScreenContentError);
                  }
                });
          }

          @Override
          public void onAdImpression() {
            executor.execute(
                () -> {
                  if (callback != null) {
                    callback.onAdImpression();
                  }
                });
          }

          @Override
          public void onAdClicked() {
            executor.execute(
                () -> {
                  if (callback != null) {
                    callback.onAdClicked();
                  }
                });
          }

          @Override
          public void onAdPaid(@NonNull AdValue adValue) {
            executor.execute(
                () -> {
                  if (callback != null) {
                    callback.onPaidEvent(
                        // TODO(vkini): Remove this cast to int and use Utility method to convert to
                        // int.
                        adValue.getPrecisionType().ordinal(),
                        adValue.getValueMicros(),
                        adValue.getCurrencyCode());
                  }
                });
          }
        });

    activity.runOnUiThread(
        () -> {
          ad.setImmersiveMode(true);
          ad.show(this.activity);
        });
  }

  /** Gets the placement ID for the {@link AppOpenAd}. */
  public long getPlacementId() {
    if (ad == null) {
      return 0;
    }
    return ad.getPlacementId();
  }

  /**
   * Sets a placement ID for the {@link AppOpenAd}.
   *
   * <p>To ensure this placement ID is included in reporting, call this method before showing the
   * ad.
   *
   * @param placementId A long integer provided by the AdMob UI for the configured placement.
   */
  public void setPlacementId(long placementId) {
    if (ad == null) {
      return;
    }
    ad.setPlacementId(placementId);
  }

  /** Returns the request response info. */
  @Nullable
  public ResponseInfo getResponseInfo() {
    if (ad == null) {
      Log.e(PluginUtils.LOGTAG, "Tried to get response info before it was ready. Returning null.");
      return null;
    }
    return ad.getResponseInfo();
  }
}
