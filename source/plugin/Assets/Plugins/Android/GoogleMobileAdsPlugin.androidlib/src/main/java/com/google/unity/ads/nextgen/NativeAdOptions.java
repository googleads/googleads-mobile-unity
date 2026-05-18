package com.google.unity.ads.nextgen;

import com.google.android.libraries.ads.mobile.sdk.common.VideoOptions;

/**
 * DECAGON CONTEXT NOTE FOR PORTING
 *
 * <p>In legacy Nonagon (GMS), configuring Native Ads uses an `AdLoader.Builder` taking a Decagon
 * separate `NativeAdOptions` class. However, Decagon removes this Class completely!
 *
 * <p>All properties are now set directly onto `NativeAdRequest.Builder`.
 *
 * <p>To avoid breaking reflection signatures where the Unity C# layer still attempts to instantiate
 * and pass a POJO for options, we create this local Adapter Shim
 * (`com.google.unity.ads.nextgen.NativeAdOptions`).
 *
 * <p>We mimic the properties provided by `GoogleMobileAds/Api/Core/NativeAdOptions.cs` and pipe
 * them during the `loadAd` phase inside `UnityNativeTemplateAdNextgen`.
 */
public class NativeAdOptions {

  private int mediaAspectRatio;
  private int adChoicesPlacement;
  private VideoOptions videoOptions;

  public int getMediaAspectRatio() {
    return mediaAspectRatio;
  }

  public void setMediaAspectRatio(int mediaAspectRatio) {
    this.mediaAspectRatio = mediaAspectRatio;
  }

  public int getAdChoicesPlacement() {
    return adChoicesPlacement;
  }

  public void setAdChoicesPlacement(int adChoicesPlacement) {
    this.adChoicesPlacement = adChoicesPlacement;
  }

  public VideoOptions getVideoOptions() {
    return videoOptions;
  }

  public void setVideoOptions(VideoOptions videoOptions) {
    this.videoOptions = videoOptions;
  }
}
