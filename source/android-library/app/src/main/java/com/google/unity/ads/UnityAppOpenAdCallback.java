package com.google.unity.ads;

/**
 * An interface form of {@link AppOpenAdCallback} that can be implemented via {@code
 * AndroidJavaProxy} in Unity to receive ad events synchronously.
 */
public interface UnityAppOpenAdCallback
    extends UnityFullScreenContentCallback, UnityPaidEventListener {

  void onAppOpenAdLoaded();

  void onAppOpenAdFailedToLoad(String error);
}
