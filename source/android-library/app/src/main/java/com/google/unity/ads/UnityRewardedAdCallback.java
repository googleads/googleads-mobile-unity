package com.google.unity.ads;

/**
 * An interface form of {@link UnityRewardedAdCallback} that can be implemented via {@code
 * AndroidJavaProxy} in Unity to receive ad events synchronously.
 */
public interface UnityRewardedAdCallback extends UnityPaidEventListener {

    void onRewardedAdLoaded();

    void onRewardedAdFailedToLoad(String errorReason);

    void onRewardedAdFailedToShow(String errorReason);

    void onRewardedAdOpened();

    void onRewardedAdClosed();

    void onUserEarnedReward(String type, float amount);
}
