package com.google.unity.mediation.vungle;

import android.os.Bundle;
import com.google.unity.ads.AdNetworkExtras;
import com.vungle.mediation.VungleExtrasBuilder;
import java.util.HashMap;

/**
 * Mediation extras bundle class for the Vungle adapter.
 */
abstract class VungleUnityExtrasBuilder implements AdNetworkExtras {

  /**
   * Key to add and obtain all placements.
   */
  private static final String ALL_PLACEMENTS_KEY = "all_placements";

  private static final String USER_ID_KEY = "user_id";

  private static final String SOUND_ENABLED_KEY = "sound_enabled";

  @Override
  public Bundle buildExtras(HashMap<String, String> extras) {
    String placements = extras.get(ALL_PLACEMENTS_KEY);
    if (placements == null) {
      return null;
    }

    VungleExtrasBuilder extrasBuilder = new VungleExtrasBuilder(placements.split(","));

    String soundEnabled = extras.get(SOUND_ENABLED_KEY);
    if (soundEnabled != null) {
      extrasBuilder.setStartMuted(!Boolean.parseBoolean(soundEnabled));
    }

    String userId = extras.get(USER_ID_KEY);
    if (userId != null) {
      extrasBuilder.setUserId(userId);
    }

    return extrasBuilder.build();
  }
}
