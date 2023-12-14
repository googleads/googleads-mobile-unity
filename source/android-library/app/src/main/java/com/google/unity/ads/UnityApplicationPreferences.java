/*
 * Copyright (C) 2023 Google LLC
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

// TODO (b/316225785): Use androidx.preference.PreferenceManager

import android.app.Activity;
import android.content.SharedPreferences;
import android.preference.PreferenceManager;

/** Application Preferences implementation for the Google Mobile Ads Unity plugin. */
public final class UnityApplicationPreferences {

  private final SharedPreferences defaultSharedPreferences;

  public UnityApplicationPreferences(final Activity activity) {
    defaultSharedPreferences = PreferenceManager.getDefaultSharedPreferences(activity);
  }

  /**
   * Set an int value in the Android default shared preferences.
   *
   * @param key The key with which to associate the value.
   * @param value The value that needs to be associated to the key.
   */
  public void setInt(String key, int value) {
    SharedPreferences.Editor editor = defaultSharedPreferences.edit();
    editor.putInt(key, value);
    editor.apply();
  }

  /**
   * Set a string value in the Android default shared preferences.
   *
   * @param key The key with which to associate the value.
   * @param value The value that needs to be associated to the key.
   */
  public void setString(String key, String value) {
    SharedPreferences.Editor editor = defaultSharedPreferences.edit();
    editor.putString(key, value);
    editor.apply();
  }

  /**
   * Read an int value from the Android default shared preferences.
   *
   * @param key The key with which to retrieve the value.
   */
  public int getInt(String key) {
    return defaultSharedPreferences.getInt(key, 0);
  }

  /**
   * Read a string value from the Android default shared preferences.
   *
   * @param key The key with which to retrieve the value.
   */
  public String getString(String key) {
    return defaultSharedPreferences.getString(key, null);
  }
}
