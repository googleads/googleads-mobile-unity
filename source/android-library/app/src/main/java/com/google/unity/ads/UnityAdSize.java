package com.google.unity.ads;

import android.app.Activity;
import android.util.DisplayMetrics;
import android.view.Display;
import android.view.WindowManager;
import com.google.android.gms.ads.AdSize;

/** Class to help provide Google Mobile Ads AdSizes for GMA unity plugin. */
public class UnityAdSize {

  private static final int FULL_WIDTH = -1;

  private UnityAdSize() {}

  public static AdSize getCurrentOrientationAnchoredAdaptiveBannerAdSize(
      Activity activity, int width) {
    if (width == FULL_WIDTH) {
      width = getScreenWidth(activity);
    }
    return AdSize.getCurrentOrientationAnchoredAdaptiveBannerAdSize(activity, width);
  }

  public static AdSize getLandscapeAnchoredAdaptiveBannerAdSize(Activity activity, int width) {
    if (width == FULL_WIDTH) {
      width = getScreenWidth(activity);
    }
    return AdSize.getLandscapeAnchoredAdaptiveBannerAdSize(activity, width);
  }

  public static AdSize getPortraitAnchoredAdaptiveBannerAdSize(Activity activity, int width) {
    if (width == FULL_WIDTH) {
      width = getScreenWidth(activity);
    }
    return AdSize.getPortraitAnchoredAdaptiveBannerAdSize(activity, width);
  }

  public static AdSize getSmartBannerAdSize() {
    return AdSize.SMART_BANNER;
  }

  private static int getScreenWidth(Activity activity) {
    DisplayMetrics displayMetrics = new DisplayMetrics();
    WindowManager windowManager = activity.getWindowManager();
    Display display = windowManager.getDefaultDisplay();
    display.getMetrics(displayMetrics);
    return (int) (displayMetrics.widthPixels / displayMetrics.density);
  }
}
