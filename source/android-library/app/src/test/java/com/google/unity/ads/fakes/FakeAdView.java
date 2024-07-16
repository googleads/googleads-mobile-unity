package com.google.unity.ads.fakes;

import android.content.Context;
import androidx.annotation.NonNull;
import com.google.android.gms.ads.BaseAdView;

/** Fake AdView class for testing. */
public class FakeAdView extends BaseAdView {
  private boolean isCollapsible;

  /* Creates a new {@link FakeAdView}. */
  public FakeAdView(@NonNull Context context, int adViewType) {
    super(context, adViewType);
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
}
