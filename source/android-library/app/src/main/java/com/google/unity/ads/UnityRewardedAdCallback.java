package com.google.unity.ads;

/**
 * An interface form of {@link RewardedAdCallback} that can be implemented via {@code
 * AndroidJavaProxy} in Unity to receive ad events synchronously.
 */
public interface UnityRewardedAdCallback
    extends UnityPaidEventListener, UnityFullScreenContentCallback {

  void onRewardedAdLoaded();

  void onRewardedAdFailedToLoad(String error);

  void onUserEarnedReward(String type, float amount);
}
