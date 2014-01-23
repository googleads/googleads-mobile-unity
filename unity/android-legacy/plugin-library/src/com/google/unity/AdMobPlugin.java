// Copyright 2013 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

package com.google.unity;

import com.google.ads.Ad;
import com.google.ads.AdListener;
import com.google.ads.AdRequest;
import com.google.ads.AdRequest.ErrorCode;
import com.google.ads.AdSize;
import com.google.ads.AdView;
import com.google.ads.mediation.admob.AdMobAdapterExtras;

import android.app.Activity;
import android.util.Log;
import android.view.Gravity;
import android.view.View;
import android.widget.FrameLayout;
import android.widget.LinearLayout;

import com.unity3d.player.UnityPlayer;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.Iterator;

/**
 * This class represents the native implementation for the AdMob Unity plugin. This class is used
 * to request AdMob ads natively via the Google AdMob SDK. The Google AdMob SDK is a dependency
 * for this plugin.
 *
 * @author api.eleichtenschl@gmail.com (Eric Leichtenschlag)
 */
public class AdMobPlugin implements AdListener {
  /** The plugin version. */
  public static final String VERSION = "1.0";

  /** Tag used for logging statements. */
  private static final String LOGTAG = "AdMobPlugin";

  /** A singleton for the plugin. */
  private static AdMobPlugin instance;

  /** The {@link AdView} to display to the user. */
  private AdView adView;

  /** The {@code Activity} that the banner will be displayed in. */
  private Activity activity;

  /** The Unity {@code GameObject} that should receive messages about ad events. */
  private String callbackHandlerName;

  /** Creates an instance of {@code AdMobPlugin}. */
  private AdMobPlugin() {
    // Prevent instantiation.
  }

  /**
   * Gets a singleton instance of {@code AdMobPlugin}.
   *
   * @return the {@code AdMobPlugin} singleton.
   */
  public static AdMobPlugin instance() {
    if (instance == null) {
      instance = new AdMobPlugin();
    }
    return instance;
  }

  /**
   * Creates an {@link AdView} to old ads.
   *
   * @param activity The activity to place the {@code AdView}.
   * @param publisherId Your publisher ID from the AdMob or DFP console
   * @param adSizeString A string ad size constant representing  the desired ad size.
   * @param positionAtTop True to position the ad at the top of the screen. False to position
   *     the ad at the bottom of the screen.
   */
  public static void createBannerView(final Activity activity, final String publisherId,
      final String adSizeString, final boolean positionAtTop) {
    Log.d(LOGTAG, "called createBannerView in Java code");
    final AdMobPlugin plugin = AdMobPlugin.instance();
    plugin.activity = activity;
    activity.runOnUiThread(new Runnable() {
      @Override
      public void run() {
        AdSize adSize = AdMobPlugin.adSizeFromSize(adSizeString);
        if (adSize == null) {
          Log.e(AdMobPlugin.LOGTAG, "AdSize is null. Did you use an AdSize constant?");
          return;
        }
        plugin.adView = new AdView(activity, adSize, publisherId);
        plugin.adView.setAdListener(plugin);
        LinearLayout layout = new LinearLayout(activity);
        FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(
            FrameLayout.LayoutParams.FILL_PARENT, FrameLayout.LayoutParams.WRAP_CONTENT);
        layoutParams.gravity = positionAtTop ? Gravity.TOP : Gravity.BOTTOM;
        activity.addContentView(layout, layoutParams);
        LinearLayout.LayoutParams adParams = new LinearLayout.LayoutParams(
            LinearLayout.LayoutParams.FILL_PARENT, LinearLayout.LayoutParams.FILL_PARENT);
        layout.addView(plugin.adView, adParams);
      }
    });
  }

  /**
   * Sets the name of the {@code GameComponent} that should listen for ad events.
   *
   * @param callbackHandlerName the {@code GameComponent} that should listen for ad events.
   */
  public static void setCallbackHandlerName(String callbackHandlerName) {
    final AdMobPlugin plugin = AdMobPlugin.instance();
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
    final AdMobPlugin plugin = AdMobPlugin.instance();
    if (plugin.activity == null) {
      Log.e(AdMobPlugin.LOGTAG, "Activity is null. Call createBannerView before requestBannerAd.");
      return;
    }

    plugin.activity.runOnUiThread(new Runnable() {
      @Override
      public void run() {
        if (plugin.adView == null) {
          Log.e(AdMobPlugin.LOGTAG, "AdView is null. Aborting requestBannerAd.");
        } else {
          AdRequest adRequest = new AdRequest();
          if (isTesting) {
            // This will request test ads on the emulator only. You can get your hashed device ID
            // from LogCat when making a live request. Pass this hashed device ID to addTestDevice
            // to get test ads on your device.
            adRequest.addTestDevice(AdRequest.TEST_EMULATOR);
          }
          AdMobAdapterExtras extras = new AdMobAdapterExtras();
          if (extrasJson != null) {
            try {
              JSONObject extrasObject = new JSONObject(extrasJson);
              Iterator<String> extrasIterator = extrasObject.keys();
              while (extrasIterator.hasNext()) {
                String key = extrasIterator.next();
                extras.addExtra(key, extrasObject.get(key));
              }
            } catch (JSONException exception) {
              Log.e(AdMobPlugin.LOGTAG, "Extras are malformed. Ignoring ad request.");
              return;
            }
          }
          extras.addExtra("unity", 1);
          adRequest.setNetworkExtras(extras);
          plugin.adView.loadAd(adRequest);
        }
      }
    });
  }

  /**
   * Sets the banner view to be visible.
   */
  public static void showBannerView() {
    final AdMobPlugin plugin = AdMobPlugin.instance();
    if (plugin.activity == null) {
      Log.e(AdMobPlugin.LOGTAG, "Activity is null. Call createBannerView before showBannerView.");
      return;
    }

    plugin.activity.runOnUiThread(new Runnable() {
      @Override
      public void run() {
        if (plugin.adView == null) {
          Log.e(AdMobPlugin.LOGTAG, "AdView is null. Aborting showBannerView.");
          return;
        } else {
          plugin.adView.setVisibility(View.VISIBLE);
        }
      }
    });
  }

  /**
   * Hides the banner view from the user.
   */
  public static void hideBannerView() {
    Log.d("AdMobPlugin", "called hideBannerView in Java code");
    final AdMobPlugin plugin = AdMobPlugin.instance();
    if (plugin.activity == null) {
      Log.e(AdMobPlugin.LOGTAG, "Activity is null. Call createBannerView before hideBannerView.");
      return;
    }

    plugin.activity.runOnUiThread(new Runnable() {
      @Override
      public void run() {
        if (plugin.adView == null) {
          Log.e(AdMobPlugin.LOGTAG, "AdView is null. Aborting hideBannerView.");
          return;
        } else {
          plugin.adView.setVisibility(View.GONE);
        }
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
      return AdSize.IAB_MRECT;
    } else if ("IAB_BANNER".equals(size)) {
      return AdSize.IAB_BANNER;
    } else if ("IAB_LEADERBOARD".equals(size)) {
      return AdSize.IAB_LEADERBOARD;
    } else if ("SMART_BANNER".equals(size)) {
      return AdSize.SMART_BANNER;
    } else {
      Log.w(LOGTAG, String.format("Unexpected ad size string: %s", size));
      return null;
    }
  }

  /** AdListener implementation. */
  @Override
  public void onReceiveAd(Ad ad) {
    if (callbackHandlerName != null) {
      UnityPlayer.UnitySendMessage(callbackHandlerName, "OnReceiveAd", "");
    }
  }

  @Override
  public void onFailedToReceiveAd(Ad ad, ErrorCode error) {
    if (callbackHandlerName != null) {
      UnityPlayer.UnitySendMessage(callbackHandlerName, "OnFailedToReceiveAd", error.toString());
    }
  }

  @Override
  public void onPresentScreen(Ad ad) {
    if (callbackHandlerName != null) {
      UnityPlayer.UnitySendMessage(callbackHandlerName, "OnPresentScreen", "");
    }
  }

  @Override
  public void onDismissScreen(Ad ad) {
    if (callbackHandlerName != null) {
      UnityPlayer.UnitySendMessage(callbackHandlerName, "OnDismissScreen", "");
    }
  }

  @Override
  public void onLeaveApplication(Ad ad) {
    if (callbackHandlerName != null) {
      UnityPlayer.UnitySendMessage(callbackHandlerName, "OnLeaveApplication", "");
    }
  }
}
