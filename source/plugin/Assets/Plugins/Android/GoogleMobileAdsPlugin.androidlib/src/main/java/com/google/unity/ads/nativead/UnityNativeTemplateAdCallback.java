package com.google.unity.ads.nativead;

import com.google.android.gms.ads.LoadAdError;
import com.google.unity.ads.UnityFullScreenContentCallback;
import com.google.unity.ads.UnityPaidEventListener;

/**
 * An interface form of {@link UnityNativeTemplateAdCallback} that can be implemented via {@code
 * AndroidJavaProxy} in Unity to receive ad events synchronously.
 */
public interface UnityNativeTemplateAdCallback
    extends UnityPaidEventListener, UnityFullScreenContentCallback {

  void onNativeAdLoaded();

  void onNativeAdFailedToLoad(LoadAdError error);
}
