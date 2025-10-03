package com.google.unity.ads.decagon;

import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;

/**
 * An interface form of {@link AppOpenAdCallback} that can be implemented via {@code
 * AndroidJavaProxy} in Unity to receive ad events synchronously.
 */
public interface UnityAppOpenAdCallback
    extends UnityFullScreenContentCallback, UnityPaidEventListener {

  void onAppOpenAdLoaded();

  void onAppOpenAdFailedToLoad(LoadAdError error);
}
