package com.google.unity.ads.nextgen;

import com.google.android.libraries.ads.mobile.sdk.common.AdChoicesPlacement;
import com.google.android.libraries.ads.mobile.sdk.common.VideoOptions;
import com.google.android.libraries.ads.mobile.sdk.nativead.NativeAd;

/** NativeAdOptions is a wrapper around VideoOptions, AdChoicesPlacement and MediaAspectRatio. */
public class NativeAdOptions {

  private NativeAd.NativeMediaAspectRatio mediaAspectRatio;
  private AdChoicesPlacement adChoicesPlacement;
  private VideoOptions videoOptions;

  public NativeAdOptions() {
    this.mediaAspectRatio = NativeAd.NativeMediaAspectRatio.UNKNOWN;
    this.adChoicesPlacement = AdChoicesPlacement.TOP_RIGHT;
  }

  public NativeAd.NativeMediaAspectRatio getMediaAspectRatio() {
    return mediaAspectRatio;
  }

  public void setMediaAspectRatio(int mediaAspectRatio) {
    switch (mediaAspectRatio) {
      case 1:
        this.mediaAspectRatio = NativeAd.NativeMediaAspectRatio.ANY;
        break;
      case 2:
        this.mediaAspectRatio = NativeAd.NativeMediaAspectRatio.LANDSCAPE;
        break;
      case 3:
        this.mediaAspectRatio = NativeAd.NativeMediaAspectRatio.PORTRAIT;
        break;
      case 4:
        this.mediaAspectRatio = NativeAd.NativeMediaAspectRatio.SQUARE;
        break;
      default:
        this.mediaAspectRatio = NativeAd.NativeMediaAspectRatio.UNKNOWN;
        break;
    }
  }

  public AdChoicesPlacement getAdChoicesPlacement() {
    return adChoicesPlacement;
  }

  public void setAdChoicesPlacement(int adChoicesPlacement) {
    switch (adChoicesPlacement) {
      case 0:
        this.adChoicesPlacement = AdChoicesPlacement.TOP_LEFT;
        break;
      case 1:
        this.adChoicesPlacement = AdChoicesPlacement.TOP_RIGHT;
        break;
      case 2:
        this.adChoicesPlacement = AdChoicesPlacement.BOTTOM_RIGHT;
        break;
      case 3:
        this.adChoicesPlacement = AdChoicesPlacement.BOTTOM_LEFT;
        break;
      default:
        this.adChoicesPlacement = AdChoicesPlacement.TOP_RIGHT;
        break;
    }
  }

  public VideoOptions getVideoOptions() {
    return videoOptions;
  }

  public void setVideoOptions(VideoOptions videoOptions) {
    this.videoOptions = videoOptions;
  }
}
