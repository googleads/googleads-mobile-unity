package com.google.unity.ads.nextgen;

import android.app.Activity;
import com.google.android.libraries.ads.mobile.sdk.MobileAds;
import com.google.android.libraries.ads.mobile.sdk.common.RequestConfiguration;
import com.google.android.libraries.ads.mobile.sdk.initialization.InitializationConfig;
import com.google.android.libraries.ads.mobile.sdk.initialization.OnAdapterInitializationCompleteListener;

/** A wrapper for static calls to {@link MobileAds} to improve testability. */
class MobileAdsWrapper {

  RequestConfiguration getRequestConfiguration() {
    return MobileAds.getRequestConfiguration();
  }

  String getVersionString() {
    return MobileAds.getVersion().toString();
  }

  void initialize(
      Activity activity,
      InitializationConfig config,
      OnAdapterInitializationCompleteListener callback) {
    MobileAds.initialize(activity, config, callback);
  }

  boolean putPublisherFirstPartyIdEnabled(boolean enabled) {
    return MobileAds.putPublisherFirstPartyIdEnabled(enabled);
  }

  void setUserControlledAppVolume(float volume) {
    MobileAds.setUserControlledAppVolume(volume);
  }

  void setUserMutedApp(boolean muted) {
    MobileAds.setUserMutedApp(muted);
  }

  void setRequestConfiguration(RequestConfiguration config) {
    MobileAds.setRequestConfiguration(config);
  }
}
