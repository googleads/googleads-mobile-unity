package com.google.unity.ads;

import androidx.annotation.Nullable;
import com.google.android.gms.ads.AdError;
import com.google.android.gms.ads.ResponseInfo;

/** Callback for receiving preloading lifecycle events. */
public interface UnityPreloadCallback {

  /**
   * Called when the last available ad is exhausted for the given preload ID.
   *
   * @param preloadId The ads's preload ID.
   */
  void onAdsExhausted(String preloadId);

  /**
   * Called when a new ad is available for the given preload ID.
   *
   * @param preloadId The ads's preload ID.
   * @param responseInfo The ad's response info.
   */
  void onAdPreloaded(String preloadId, @Nullable ResponseInfo responseInfo);

  /**
   * Called when an ad failed to load for a given preload ID.
   *
   * @param preloadId The ads's preload ID.
   * @param adError The error that occurred while loading the ad.
   */
  void onAdFailedToPreload(String preloadId, AdError adError);
}
