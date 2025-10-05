package com.google.unity.ads;

import android.app.Activity;
import com.google.android.gms.ads.AdInspectorError;
import com.google.android.gms.ads.MobileAds;
import com.google.android.gms.ads.OnAdInspectorClosedListener;

/** Ad inspector implementation for the Google Mobile Ads Unity plugin. */
public final class UnityAdInspector {

  private UnityAdInspector() {}

  public static void openAdInspector(
      final Activity activity, final UnityAdInspectorListener adInspectorListener) {

    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            MobileAds.openAdInspector(
                activity,
                new OnAdInspectorClosedListener() {
                  @Override
                  public void onAdInspectorClosed(AdInspectorError adInspectorError) {
                    if (adInspectorListener != null) {
                      adInspectorListener.onAdInspectorClosed(adInspectorError);
                    }
                  }
                });
          }
        });
  }
}
