// Copyright 2014 Google Inc. All Rights Reserved.

package com.google.unity;

import com.google.android.gms.ads.AdListener;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.AdSize;
import com.google.android.gms.ads.AdView;
import com.google.android.gms.ads.mediation.admob.AdMobExtras;

import android.app.Activity;
import android.graphics.Color;
import android.os.Bundle;
import android.util.Log;
import android.view.Gravity;
import android.view.View;
import android.widget.FrameLayout;

import com.unity3d.player.UnityPlayer;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.Iterator;

/**
 * This class represents the native implementation for the Google Mobile Ads Unity plugin. This
 * class is used to request Google Mobile ads natively via the Google Mobile Ads library in Google
 * Play services. The Google Play services library is a dependency for this plugin.
 */
public class GoogleMobileAdsPlugin {
  /** The plugin version. */
  public static final String VERSION = "1.0";

  /** Tag used for logging statements. */
  private static final String LOGTAG = "GoogleMobileAdsPlugin";

  /** The {@code GoogleMobileAdsPlugin} singleton. */
  private static final GoogleMobileAdsPlugin instance = new GoogleMobileAdsPlugin();

  /** The {@link AdView} to display to the user. */
  private AdView adView;

  /** The {@code Activity} that the banner will be displayed in. */
  private Activity activity;

  /** The Unity {@code GameObject} that should receive messages about ad events. */
  private String callbackHandlerName;

  /** Creates an instance of {@code GoogleMobileAdsPlugin}. */
  private GoogleMobileAdsPlugin() {
    // Prevent instantiation.
  }

  /**
   * Gets a singleton instance of {@code GoogleMobileAdsPlugin}.
   *
   * @return the {@code GoogleMobileAdsPlugin} singleton.
   */
  public static GoogleMobileAdsPlugin instance() {
    return instance;
  }

  /**
   * Creates an {@link AdView} to old ads.
   *
   * @param activity The activity to place the {@code AdView}.
   * @param publisherId Your ad unit ID.
   * @param adSizeString A string ad size constant representing  the desired ad size.
   * @param positionAtTop True to position the ad at the top of the screen. False to position
   *     the ad at the bottom of the screen.
   */
  public static void createBannerView(final Activity activity, final String publisherId,
      final String adSizeString, final boolean positionAtTop) {
    final GoogleMobileAdsPlugin plugin = GoogleMobileAdsPlugin.instance();
    plugin.activity = activity;
    activity.runOnUiThread(new Runnable() {
      @Override
      public void run() {
        AdSize adSize = GoogleMobileAdsPlugin.adSizeFromSize(adSizeString);
        if (adSize == null) {
          Log.e(GoogleMobileAdsPlugin.LOGTAG, "AdSize is null. Did you use an AdSize constant?");
          return;
        }
        plugin.adView = new AdView(activity);
        // Setting the background color works around an issue where the first ad isn't visible.
        plugin.adView.setBackgroundColor(Color.TRANSPARENT);
        plugin.adView.setAdUnitId(publisherId);
        plugin.adView.setAdSize(adSize);
        plugin.adView.setAdListener(new AdListener() {
          @Override
          public void onAdLoaded() {
            if (plugin.callbackHandlerName != null && !plugin.callbackHandlerName.isEmpty()) {
              UnityPlayer.UnitySendMessage(plugin.callbackHandlerName, "OnReceiveAd", "");
            }
          }

          @Override
          public void onAdFailedToLoad(int errorCode) {
            if (plugin.callbackHandlerName != null && !plugin.callbackHandlerName.isEmpty()) {
              UnityPlayer.UnitySendMessage(plugin.callbackHandlerName, "OnFailedToReceiveAd",
                  getErrorReason(errorCode));
            }
          }

          @Override
          public void onAdOpened() {
            if (plugin.callbackHandlerName != null && !plugin.callbackHandlerName.isEmpty()) {
              UnityPlayer.UnitySendMessage(plugin.callbackHandlerName, "OnPresentScreen", "");
            }
          }

          @Override
          public void onAdClosed() {
            if (plugin.callbackHandlerName != null && !plugin.callbackHandlerName.isEmpty()) {
              UnityPlayer.UnitySendMessage(plugin.callbackHandlerName, "OnDismissScreen", "");
            }
          }

          @Override
          public void onAdLeftApplication() {
            if (plugin.callbackHandlerName != null && !plugin.callbackHandlerName.isEmpty()) {
              UnityPlayer.UnitySendMessage(plugin.callbackHandlerName, "OnLeaveApplication", "");
            }
          }
        });
        FrameLayout.LayoutParams adParams = new FrameLayout.LayoutParams(
            FrameLayout.LayoutParams.MATCH_PARENT, FrameLayout.LayoutParams.WRAP_CONTENT);
        adParams.gravity = positionAtTop ? Gravity.TOP : Gravity.BOTTOM;
        activity.addContentView(plugin.adView, adParams);
      }
    });
  }

  /**
   * Sets the name of the {@code GameComponent} that should listen for ad events.
   *
   * @param callbackHandlerName the {@code GameComponent} that should listen for ad events.
   */
  public static void setCallbackHandlerName(String callbackHandlerName) {
    final GoogleMobileAdsPlugin plugin = GoogleMobileAdsPlugin.instance();
    plugin.callbackHandlerName = callbackHandlerName;
  }

  /**
   * Requests a banner ad without any extras. This method should be called only after a banner
   * view has been created.
   *
   * @param isTesting True to enable test ads. False to get network ads.
   */
  public static void requestBannerAd(final boolean isTesting) {
    requestBannerAd(isTesting, null);
  }

  /**
   * Requests a banner ad. This method should be called only after a banner view has been created.
   *
   * @param isTesting True to enable test ads. False to get network ads.
   * @param extrasJson A json object with key/value pairs representing what extras to pass in the
   *     request.
   */
  public static void requestBannerAd(final boolean isTesting, final String extrasJson) {
    final GoogleMobileAdsPlugin plugin = GoogleMobileAdsPlugin.instance();
    if (plugin.activity == null) {
      Log.e(GoogleMobileAdsPlugin.LOGTAG,
          "Activity is null. Call createBannerView before requestBannerAd.");
      return;
    }

    plugin.activity.runOnUiThread(new Runnable() {
      @Override
      public void run() {
        if (plugin.adView == null) {
          Log.e(GoogleMobileAdsPlugin.LOGTAG, "AdView is null. Aborting requestBannerAd.");
        } else {
          AdRequest.Builder adRequestBuilder = new AdRequest.Builder();
          if (isTesting) {
            // This will request test ads on the emulator only. You can get your hashed device ID
            // from LogCat when making a live request. Pass this hashed device ID to addTestDevice
            // to get test ads on your device.
            adRequestBuilder.addTestDevice(AdRequest.DEVICE_ID_EMULATOR);
          }
          Bundle bundle = new Bundle();
          if (extrasJson != null) {
            try {
              JSONObject extrasObject = new JSONObject(extrasJson);
              Iterator<String> extrasIterator = extrasObject.keys();
              while (extrasIterator.hasNext()) {
                String key = extrasIterator.next();
                bundle.putString(key, extrasObject.getString(key));
              }
            } catch (JSONException exception) {
              Log.e(GoogleMobileAdsPlugin.LOGTAG,
                  "Extras are malformed. Aborting requestBannerAd.");
              return;
            }
          }
          bundle.putInt("unity", 1);
          AdMobExtras extras = new AdMobExtras(bundle);
          adRequestBuilder.addNetworkExtras(extras);
          plugin.adView.loadAd(adRequestBuilder.build());
        }
      }
    });
  }

  /**
   * Sets the banner view to be visible.
   */
  public static void showBannerView() {
    final GoogleMobileAdsPlugin plugin = GoogleMobileAdsPlugin.instance();
    if (plugin.activity == null) {
      Log.e(GoogleMobileAdsPlugin.LOGTAG,
          "Activity is null. Call createBannerView before showBannerView.");
      return;
    }

    plugin.activity.runOnUiThread(new Runnable() {
      @Override
      public void run() {
        if (plugin.adView == null) {
          Log.e(GoogleMobileAdsPlugin.LOGTAG, "AdView is null. Aborting showBannerView.");
          return;
        }
        plugin.adView.setVisibility(View.VISIBLE);
      }
    });
  }

  /**
   * Hides the banner view from the user.
   */
  public static void hideBannerView() {
    final GoogleMobileAdsPlugin plugin = GoogleMobileAdsPlugin.instance();
    if (plugin.activity == null) {
      Log.e(GoogleMobileAdsPlugin.LOGTAG,
          "Activity is null. Call createBannerView before hideBannerView.");
      return;
    }

    plugin.activity.runOnUiThread(new Runnable() {
      @Override
      public void run() {
        if (plugin.adView == null) {
          Log.e(GoogleMobileAdsPlugin.LOGTAG, "AdView is null. Aborting hideBannerView.");
          return;
        }
        plugin.adView.setVisibility(View.GONE);
      }
    });
  }

  /**
   * Gets an AdSize object from the string size passed in from JavaScript. Returns null if an
   * improper string is provided.
   *
   * @param size The string size representing an ad format constant.
   * @return An AdSize object used to create a banner.
   */
  private static AdSize adSizeFromSize(String size) {
    if ("BANNER".equals(size)) {
      return AdSize.BANNER;
    } else if ("IAB_MRECT".equals(size)) {
      return AdSize.MEDIUM_RECTANGLE;
    } else if ("IAB_BANNER".equals(size)) {
      return AdSize.FULL_BANNER;
    } else if ("IAB_LEADERBOARD".equals(size)) {
      return AdSize.LEADERBOARD;
    } else if ("SMART_BANNER".equals(size)) {
      return AdSize.SMART_BANNER;
    } else {
      Log.w(LOGTAG, String.format("Unexpected ad size string: %s", size));
      return null;
    }
  }

  /**
   * Gets a string error reason from an error code.
   *
   * @param errorCode The error code.
   * @return The reason for the error.
   */
  private static String getErrorReason(int errorCode) {
    switch(errorCode) {
      case AdRequest.ERROR_CODE_INTERNAL_ERROR:
        return "Internal error";
      case AdRequest.ERROR_CODE_INVALID_REQUEST:
        return "Invalid request";
      case AdRequest.ERROR_CODE_NETWORK_ERROR:
        return "Network Error";
      case AdRequest.ERROR_CODE_NO_FILL:
        return "No fill";
      default:
        Log.w(LOGTAG, String.format("Unexpected error code: %s", errorCode));
        return "";
    }
  }
}
