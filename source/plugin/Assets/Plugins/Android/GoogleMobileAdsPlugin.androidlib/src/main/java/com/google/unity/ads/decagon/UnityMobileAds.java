/*
 * Copyright (C) 2025 Google, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.google.unity.ads.decagon;

import android.app.Activity;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.util.Log;
import androidx.annotation.Nullable;
import com.google.android.libraries.ads.mobile.sdk.MobileAds;
import com.google.android.libraries.ads.mobile.sdk.common.RequestConfiguration;
import com.google.android.libraries.ads.mobile.sdk.initialization.InitializationConfig;
import com.google.android.libraries.ads.mobile.sdk.initialization.OnAdapterInitializationCompleteListener;
import com.google.errorprone.annotations.concurrent.GuardedBy;

/** Mobile Ads implementation for the Google Mobile Ads Unity plugin. */
public final class UnityMobileAds {
  private static final String TAG = "UnityMobileAds";
  private static final String APPLICATION_ID_KEY = "com.google.android.gms.ads.APPLICATION_ID";

  private static final Object stateLock = new Object();

  @GuardedBy("stateLock")
  private static volatile boolean isMobileAdsInitialized = false;

  @GuardedBy("stateLock")
  private static volatile RequestConfiguration requestConfiguration;

  @GuardedBy("stateLock")
  private static volatile float userVolume = -1;

  @GuardedBy("stateLock")
  private static volatile boolean isMuted = false;

  @GuardedBy("stateLock")
  private static volatile boolean isPublisherFirstPartyIdEnabled = false;

  private UnityMobileAds() {}

  /**
   * Initializes the Google Mobile Ads SDK.
   *
   * @param activity The {@link Activity} to use for initializing Google Mobile Ads SDK.
   * @param callback The {@link OnAdapterInitializationCompleteListener} to be called when Google
   *     Mobile Ads SDK is initialized.
   */
  public static void initialize(
      Activity activity, OnAdapterInitializationCompleteListener callback) {
    String appId = getApplicationMetaData(activity, APPLICATION_ID_KEY);

    if (appId == null) {
      Log.e(TAG, "Application ID is null. Cannot initialize the Google Mobile Ads SDK.");
      return;
    }

    InitializationConfig.Builder configBuilder = new InitializationConfig.Builder(appId);
    synchronized (stateLock) {
      if (requestConfiguration != null) {
        configBuilder = configBuilder.setRequestConfiguration(requestConfiguration);
      }
    }
    InitializationConfig config = configBuilder.build();
    // Initialize the Google Mobile Ads SDK on a background thread.
    new Thread(
            () ->
                MobileAds.initialize(
                    activity,
                    config,
                    initializationStatus -> {
                      synchronized (stateLock) {
                        isMobileAdsInitialized = true;
                        requestConfiguration = null;
                        if (isPublisherFirstPartyIdEnabled) {
                          var unused = MobileAds.putPublisherFirstPartyIdEnabled(true);
                          isPublisherFirstPartyIdEnabled = false;
                        }
                        if (userVolume >= 0) {
                          MobileAds.setUserControlledAppVolume(userVolume);
                          userVolume = -1;
                        }
                        if (isMuted) {
                          MobileAds.setUserMutedApp(isMuted);
                          isMuted = false;
                        }
                      }
                      callback.onAdapterInitializationComplete(initializationStatus);
                    }))
        .start();
  }

  /**
   * Sets the global {@link RequestConfiguration} that will be used for every ad request.
   *
   * @param config The {@link RequestConfiguration} to used for future ad requests.
   */
  public static void setRequestConfiguration(RequestConfiguration config) {
    // Cannot set the configuration until after the MobileAds has been initialized.
    synchronized (stateLock) {
      if (!isMobileAdsInitialized) {
        requestConfiguration = config;
        return;
      }
    }
    MobileAds.setRequestConfiguration(config);
  }

  /**
   * Sets whether the publisher first party ID is enabled.
   *
   * @param enabled Whether the publisher first party ID is enabled.
   */
  public static boolean putPublisherFirstPartyIdEnabled(boolean enabled) {
    synchronized (stateLock) {
      if (!isMobileAdsInitialized) {
        isPublisherFirstPartyIdEnabled = enabled;
        return true;
      }
    }
    return MobileAds.putPublisherFirstPartyIdEnabled(enabled);
  }

  /**
   * Sets the user-controlled app volume.
   *
   * @param volume The user-controlled app volume.
   */
  public static void setUserControlledAppVolume(float volume) {
    synchronized (stateLock) {
      if (!isMobileAdsInitialized) {
        userVolume = volume;
        return;
      }
    }
    MobileAds.setUserControlledAppVolume(volume);
  }

  /**
   * Sets whether the app is muted.
   *
   * @param muted Whether the app is muted.
   */
  public static void setApplicationMuted(boolean muted) {
    synchronized (stateLock) {
      if (!isMobileAdsInitialized) {
        isMuted = muted;
        return;
      }
    }
    MobileAds.setUserMutedApp(muted);
  }

  /**
   * Retrieves the application metadata value from the given activity.
   *
   * @param activity The {@link Activity} to use for retrieving the application metadata.
   * @param key The key to use for retrieving the application metadata value.
   * @return The application metadata value, or null if not found.
   */
  @Nullable
  private static String getApplicationMetaData(Activity activity, String key) {
    if (activity == null) {
      Log.e(TAG, "Unity Activity is null. Cannot read Application ID.");
      return null;
    }

    try {
      ApplicationInfo appInfo =
          activity
              .getPackageManager()
              .getApplicationInfo(activity.getPackageName(), PackageManager.GET_META_DATA);
      Bundle bundle = appInfo.metaData;
      if (bundle != null && bundle.containsKey(key)) {
        return bundle.getString(key);
      } else {
        Log.e(TAG, "Application ID not found in manifest!");
      }
    } catch (Exception e) {
      Log.e(TAG, "Error reading application ID from manifest: " + e.getMessage());
    }
    return null;
  }
}
