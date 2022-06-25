package com.google.unity.ads;

/**
 * An interface form of {@link InterstitialAdCallback} that can be implemented via {@code
 * AndroidJavaProxy} in Unity to receive ad events synchronously.
 */
public interface UnityInterstitialAdCallback
    extends UnityPaidEventListener, UnityFullScreenContentCallback {

  void onInterstitialAdLoaded();

  void onInterstitialAdFailedToLoad(String error);
}
