package com.google.unity.ads;

import com.google.android.gms.ads.AdError;
import com.google.android.gms.ads.LoadAdError;

/**
 * An interface form of {@link UnityRewardedAdCallback} that can be implemented via {@code
 * AndroidJavaProxy} in Unity to receive ad events synchronously.
 */
public interface UnityRewardedAdCallback extends UnityPaidEventListener {

    void onRewardedAdLoaded();

    void onRewardedAdFailedToLoad(LoadAdError error);

    void onRewardedAdFailedToShow(AdError error);

    void onRewardedAdOpened();

    void onRewardedAdClosed();

    void onUserEarnedReward(String type, float amount);
}
