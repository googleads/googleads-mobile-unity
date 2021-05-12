package com.google.unity.ads;

import com.google.android.gms.ads.LoadAdError;

/**
 * An interface form of {@link InterstitialAdCallback} that can be implemented via {@code
 * AndroidJavaProxy} in Unity to receive ad events synchronously.
 */
public interface UnityInterstitialAdCallback extends UnityPaidEventListener,
    UnityFullScreenContentCallback {

  void onInterstitialAdLoaded();

  void onInterstitialAdFailedToLoad(LoadAdError error);
}
