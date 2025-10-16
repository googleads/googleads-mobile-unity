package com.google.unity.ads.decagon;

import android.app.Activity;
import android.util.Log;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.annotation.VisibleForTesting;
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.AdValue;
import com.google.android.libraries.ads.mobile.sdk.common.FullScreenContentError;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
import com.google.android.libraries.ads.mobile.sdk.rewarded.OnUserEarnedRewardListener;
import com.google.android.libraries.ads.mobile.sdk.rewarded.RewardItem;
import com.google.android.libraries.ads.mobile.sdk.rewarded.RewardedAd;
import com.google.android.libraries.ads.mobile.sdk.rewarded.RewardedAdEventCallback;
import com.google.unity.ads.PluginUtils;
import java.util.concurrent.Executor;
import java.util.concurrent.Executors;

/** Rewarded ad implementation for the Google Mobile Ads Unity plugin. */
public class UnityRewardedAd extends UnityAdBase<RewardedAd, UnityRewardedAdCallback> {

  private final AdWrapper<RewardedAd> adWrapper;

  public UnityRewardedAd(Activity activity, UnityRewardedAdCallback callback) {
    this(activity, callback, AdWrapper.forRewarded(), Executors.newSingleThreadExecutor());
  }

  @VisibleForTesting
  UnityRewardedAd(
      Activity activity,
      UnityRewardedAdCallback callback,
      AdWrapper<RewardedAd> adWrapper,
      Executor executor) {
    super(activity, callback, executor);
    this.adWrapper = adWrapper;
  }

  /**
   * Loads a rewarded ad.
   *
   * @param request The {@link AdRequest} object with targeting parameters.
   */
  public void load(final AdRequest request) {
    activity.runOnUiThread(
        () ->
            this.adWrapper.load(
                request,
                new AdLoadCallback<RewardedAd>() {
                  @Override
                  public void onAdLoaded(@NonNull RewardedAd rewardedAd) {
                    // Rewarded ad loaded.
                    UnityRewardedAd.this.ad = rewardedAd;
                    executor.execute(
                        () -> {
                          if (callback != null) {
                            callback.onRewardedAdLoaded();
                          }
                        });
                  }

                  @Override
                  public void onAdFailedToLoad(@NonNull LoadAdError adError) {
                    // Rewarded ad failed to load.
                    executor.execute(
                        () -> {
                          if (callback != null) {
                            callback.onRewardedAdFailedToLoad(adError);
                          }
                        });
                    ad = null;
                  }
                }));
  }

  /** Shows the rewarded ad if it has loaded. */
  @SuppressWarnings("EnumOrdinal")
  public void show() {
    if (ad == null) {
      Log.e(
          PluginUtils.LOGTAG,
          "Tried to show rewarded ad before it was ready. Please call load first and wait for"
              + " a successful onAdLoaded callback.");
      return;
    }

    // Listen for ad events.
    ad.setAdEventCallback(
        new RewardedAdEventCallback() {
          @Override
          public void onAdShowedFullScreenContent() {
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
            ad = null;
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
          ad.show(
              this.activity,
              new OnUserEarnedRewardListener() {
                @Override
                public void onUserEarnedReward(@NonNull RewardItem rewardItem) {
                  executor.execute(
                      () -> {
                        if (callback != null) {
                          callback.onUserEarnedReward(rewardItem.getType(), rewardItem.getAmount());
                        }
                      });
                }
              });
        });
  }

  /** Gets the placement ID for the {@link RewardedAd}. */
  public long getPlacementId() {
    if (ad == null) {
      return 0;
    }
    return ad.getPlacementId();
  }

  /**
   * Sets a placement ID for the {@link RewardedAd}.
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
