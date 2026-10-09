package com.google.unity.ads;

import com.google.android.gms.ads.LoadAdError;

/**
 * An interface form of {@link RewardedAdCallback} that can be implemented via {@code
 * AndroidJavaProxy} in Unity to receive ad events synchronously.
 */
public interface UnityRewardedAdCallback extends UnityPaidEventListener,
    UnityFullScreenContentCallback {

  void onRewardedAdLoaded();

  void onRewardedAdFailedToLoad(LoadAdError error);

  void onUserEarnedReward(String type, float amount);
}
