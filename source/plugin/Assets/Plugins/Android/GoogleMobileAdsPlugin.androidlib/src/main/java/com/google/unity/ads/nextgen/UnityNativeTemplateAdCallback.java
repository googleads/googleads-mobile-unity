package com.google.unity.ads.nextgen;

import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;

/**
 * An interface form of {@link UnityNativeTemplateAdCallback} that can be implemented via {@code
 * AndroidJavaProxy} in Unity to receive ad events synchronously.
 */
public interface UnityNativeTemplateAdCallback
    extends UnityPaidEventListener, UnityFullScreenContentCallback {

  void onNativeAdLoaded();

  void onNativeAdFailedToLoad(LoadAdError error);
}
