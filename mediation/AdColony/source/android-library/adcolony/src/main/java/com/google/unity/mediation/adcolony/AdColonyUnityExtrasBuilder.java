package com.google.unity.mediation.adcolony;

import android.os.Bundle;

import com.google.unity.ads.AdNetworkExtras;
import com.jirbo.adcolony.AdColonyAdapter;
import com.jirbo.adcolony.AdColonyBundleBuilder;

import java.util.HashMap;

/**
 * Mediation extras bundle class for the AdColony adapter.
 */
public abstract class AdColonyUnityExtrasBuilder implements AdNetworkExtras {

    /**
     * Key to obtain Zone ID.
     */
    private static final String KEY_ZONE_ID = "zone_id";

    /**
     * Key to obtain User ID.
     */
    private static final String KEY_USER_ID = "user_id";

    /**
     * Key to obtain the "show pre popup" option.
     */
    private static final String KEY_SHOW_PRE_POPUP = "show_pre_popup";

    /**
     * Key to obtain the "show post popup" option.
     */
    private static final String KEY_SHOW_POST_POPUP = "show_post_popup";

    /**
     * Key to obtain test mode option.
     */
    private static final String KEY_TEST_MODE = "test_mode";

    @Override
    public Bundle buildExtras(HashMap<String, String> extras) {
        String zoneId = extras.get(KEY_ZONE_ID);
        if (zoneId != null) {
            AdColonyBundleBuilder.setZoneId(zoneId);
        }

        String userId = extras.get(KEY_USER_ID);
        if (userId != null) {
            AdColonyBundleBuilder.setUserId(userId);
        }

        String showPrePopup = extras.get(KEY_SHOW_PRE_POPUP);
        if (showPrePopup != null) {
            AdColonyBundleBuilder.setShowPostPopup(Boolean.valueOf(showPrePopup));
        }

        String showPostPopup = extras.get(KEY_SHOW_POST_POPUP);
        if (showPostPopup != null) {
            AdColonyBundleBuilder.setShowPostPopup(Boolean.valueOf(showPostPopup));
        }

        String testMode = extras.get(KEY_TEST_MODE);
        if (testMode != null) {
            AdColonyBundleBuilder.setTestModeEnabled(Boolean.valueOf(testMode));
        }

        return AdColonyBundleBuilder.build();
    }

    @Override
    public Class getAdapterClass() {
        return AdColonyAdapter.class;
    }
}

