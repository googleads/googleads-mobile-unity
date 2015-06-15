// Copyright 2014 Google Inc. All Rights Reserved.

package com.google.unity.ads;

/**
 * An interface form of {@link UnityAdListener} that can be implemented via
 * {@code AndroidJavaProxy} in Unity to receive ad events synchronously.
 */
public interface UnityAdListener {
  void onAdLoaded();
  void onAdFailedToLoad(String errorReason);
  void onAdOpened();
  void onAdClosed();
  void onAdLeftApplication();
}
