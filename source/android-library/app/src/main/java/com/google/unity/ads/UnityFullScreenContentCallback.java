package com.google.unity.ads;

/**
 * An interface form of {@code FullScreenContentCallback} that can be implemented via {@code
 * AndroidJavaProxy} in Unity to receive ad events synchronously.
 */
public interface UnityFullScreenContentCallback {

  void onAdFailedToShowFullScreenContent(String error);

  void onAdShowedFullScreenContent();

  void onAdImpression();

  void onAdDismissedFullScreenContent();
}
