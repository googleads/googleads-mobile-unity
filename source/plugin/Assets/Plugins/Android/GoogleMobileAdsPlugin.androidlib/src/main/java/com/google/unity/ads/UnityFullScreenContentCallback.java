package com.google.unity.ads;

import com.google.android.gms.ads.AdError;

/** An interface form of {@code FullScreenContentCallback} that
 * can be implemented via {@code AndroidJavaProxy} in Unity to
 * receive ad events synchronously. */
public interface UnityFullScreenContentCallback {

  void onAdFailedToShowFullScreenContent(AdError error);

  void onAdShowedFullScreenContent();

  void onAdDismissedFullScreenContent();

  void onAdImpression();

  void onAdClicked();
}
