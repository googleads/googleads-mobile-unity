package com.google.unity.mediation.adcolony;

import android.os.Bundle;
import com.google.unity.ads.AdNetworkExtras;
import com.jirbo.adcolony.AdColonyAdapter;
import com.jirbo.adcolony.AdColonyBundleBuilder;
import java.util.HashMap;

/**
 * Mediation extras bundle class for the AdColony adapter.
 */
public class AdColonyUnityExtrasBuilder implements AdNetworkExtras {

  /**
   * Key to obtain the "show pre popup" option.
   */
  private static final String KEY_SHOW_PRE_POPUP = "show_pre_popup";

  /**
   * Key to obtain the "show post popup" option.
   */
  private static final String KEY_SHOW_POST_POPUP = "show_post_popup";

  @Override
  public Bundle buildExtras(HashMap<String, String> extras) {
    String showPrePopup = extras.get(KEY_SHOW_PRE_POPUP);
    if (showPrePopup != null) {
      AdColonyBundleBuilder.setShowPrePopup(Boolean.parseBoolean(showPrePopup));
    }

    String showPostPopup = extras.get(KEY_SHOW_POST_POPUP);
    if (showPostPopup != null) {
      AdColonyBundleBuilder.setShowPostPopup(Boolean.parseBoolean(showPostPopup));
    }

    return AdColonyBundleBuilder.build();
  }

  @Override
  public Class getAdapterClass() {
    return AdColonyAdapter.class;
  }
}
