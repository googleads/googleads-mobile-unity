package com.google.unity.mediation.vungle;

import com.vungle.mediation.VungleInterstitialAdapter;

/**
 * Mediation extras bundle class for the Vungle adapter.
 */
public class VungleUnityInterstitialExtrasBuilder extends VungleUnityExtrasBuilder {

  @Override
  public Class getAdapterClass() {
    return VungleInterstitialAdapter.class;
  }
}
