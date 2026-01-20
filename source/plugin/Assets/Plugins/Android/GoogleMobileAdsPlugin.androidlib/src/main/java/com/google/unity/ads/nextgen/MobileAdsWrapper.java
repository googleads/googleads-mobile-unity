package com.google.unity.ads.nextgen;

import com.google.android.libraries.ads.mobile.sdk.MobileAds;
import com.google.android.libraries.ads.mobile.sdk.common.RequestConfiguration;

/** A wrapper for static calls to {@link MobileAds} to improve testability. */
class MobileAdsWrapper {

  RequestConfiguration getRequestConfiguration() {
    return MobileAds.getRequestConfiguration();
  }

  String getVersionString() {
    return MobileAds.getVersion().toString();
  }
}
