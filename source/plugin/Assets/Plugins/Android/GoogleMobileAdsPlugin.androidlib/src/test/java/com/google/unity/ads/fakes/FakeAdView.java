package com.google.unity.ads.fakes;

import android.content.Context;
import androidx.annotation.NonNull;
import com.google.android.gms.ads.BaseAdView;
import com.google.android.gms.ads.LoadAdError;
import com.google.unity.ads.UnityAdListener;

/** Fake AdView class for testing. */
public class FakeAdView extends BaseAdView {
  private UnityAdListener adListener;
  private boolean isCollapsible;
  private long placementId;

  /* Creates a new {@link FakeAdView}. */
  public FakeAdView(@NonNull Context context, int adViewType) {
    super(context, adViewType);
  }

  /* Sets the ad listener for the ad view. */
  public void setAdListener(UnityAdListener adListener) {
    this.adListener = adListener;
  }

  /*  Simulates an ad loaded event. */
  public void simulateAdLoaded() {
    if (adListener != null) {
      adListener.onAdLoaded();
    }
  }

  /*
   * Simulates an ad failed to load event.
   *
   * @param error The error that caused the ad to fail to load.
   */
  public void simulateAdFailedToLoad(LoadAdError error) {
    if (adListener != null) {
      adListener.onAdFailedToLoad(error);
    }
  }

  /** Returns the collapsible state of the ad view. */
  @Override
  public boolean isCollapsible() {
    return isCollapsible;
  }

  /*
   * Sets the collapsible state of the ad view.
   *
   * @param collapsible The collapsible state of the ad view.
   */
  public void setCollapsible(boolean collapsible) {
    this.isCollapsible = collapsible;
  }

  /** Gets the placement ID of the ad view. */
  @Override
  public long getPlacementId() {
    return placementId;
  }

  /**
   * Sets the placement ID for the ad view.
   *
   * <p>To ensure this placement ID is included in reporting, call this method before showing the
   * ad.
   *
   * @param placementId A long integer provided by the AdMob UI for the configured placement.
   */
  @Override
  public void setPlacementId(long placementId) {
    this.placementId = placementId;
  }
}
