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

package com.google.unity.ads;

import android.app.Activity;
import com.google.android.gms.ads.MobileAds;
import com.google.android.gms.ads.RequestConfiguration;
import com.google.android.gms.ads.initialization.OnInitializationCompleteListener;
import com.google.android.gms.ads.preload.PreloadCallback;
import com.google.android.gms.ads.preload.PreloadConfiguration;
import java.lang.reflect.Method;
import java.util.List;

/** Mobile Ads implementation for the Google Mobile Ads Unity plugin. */
public final class UnityMobileAds {

  public static void initialize(Activity activity, OnInitializationCompleteListener listener) {
    if (activity == null) {
      return;
    }
    MobileAds.initialize(activity, listener);
  }

  public static void setAppVolume(float volume) {
    MobileAds.setAppVolume(volume);
  }

  public static void disableMediationAdapterInitialization(Activity activity) {
    if (activity == null) {
      return;
    }
    MobileAds.disableMediationAdapterInitialization(activity);
  }

  public static void setAppMuted(boolean muted) {
    MobileAds.setAppMuted(muted);
  }

  public static boolean putPublisherFirstPartyIdEnabled(boolean enabled) {
    return MobileAds.putPublisherFirstPartyIdEnabled(enabled);
  }

  public static void setRequestConfiguration(RequestConfiguration config) {
    if (config == null) {
      return;
    }
    MobileAds.setRequestConfiguration(config);
  }

  public static RequestConfiguration getRequestConfiguration() {
    return MobileAds.getRequestConfiguration();
  }

  public static void openAdInspector(Activity activity, UnityAdInspectorListener listener) {
    if (activity == null) {
      return;
    }
    UnityAdInspector.openAdInspector(activity, listener);
  }

  @SuppressWarnings("deprecation") // This API is still supported on Unity.
  public static void startPreload(
      Activity activity, List<PreloadConfiguration> configs, PreloadCallback callback) {
    if (activity == null) {
      return;
    }
    MobileAds.startPreload(activity, configs, callback);
  }

  public static void setPlugin(String version) {
    try {
      Method setPluginMethod = MobileAds.class.getDeclaredMethod("setPlugin", String.class);
      setPluginMethod.setAccessible(true);
      setPluginMethod.invoke(null, version);
    } catch (Exception e) {
      // ignoring this exception as the method is not available in all versions of the GMA SDK.
    }
  }

  public static String getSdkVersionString() {
    return MobileAds.getVersion().toString();
  }

  private UnityMobileAds() {}
}
