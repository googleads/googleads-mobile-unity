package com.google.unity.mediation.vungle;

import com.vungle.mediation.VungleAdapter;

/**
 * Mediation extras bundle class for the Vungle adapter.
 */
public class VungleUnityRewardedVideoExtrasBuilder extends VungleUnityExtrasBuilder {

  @Override
  public Class getAdapterClass() {
    return VungleAdapter.class;
  }
}
