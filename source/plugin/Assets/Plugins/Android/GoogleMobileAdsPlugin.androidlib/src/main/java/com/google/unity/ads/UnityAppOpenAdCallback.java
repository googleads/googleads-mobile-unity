package com.google.unity.ads;

import com.google.android.gms.ads.LoadAdError;

/**
 * An interface form of {@link AppOpenAdCallback} that can be implemented via {@code
 * AndroidJavaProxy} in Unity to receive ad events synchronously.
 */
public interface UnityAppOpenAdCallback
    extends UnityFullScreenContentCallback, UnityPaidEventListener {

  void onAppOpenAdLoaded();

  void onAppOpenAdFailedToLoad(LoadAdError error);
}
