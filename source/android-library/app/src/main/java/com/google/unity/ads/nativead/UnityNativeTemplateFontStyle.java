/*
 * Copyright (C) 2024 Google, Inc.
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

package com.google.unity.ads.nativead;

import android.graphics.Typeface;
import android.util.Log;
import com.google.unity.ads.PluginUtils;

/** Enums to specify the styling of text for Native Templates. */
public enum UnityNativeTemplateFontStyle {
  NORMAL,
  BOLD,
  ITALIC,
  MONOSPACE;

  public static UnityNativeTemplateFontStyle fromIntValue(int value) {
    if (value >= 0 && value < UnityNativeTemplateFontStyle.values().length) {
      return UnityNativeTemplateFontStyle.values()[value];
    }
    Log.w(PluginUtils.LOGTAG, "Invalid index for NativeTemplateFontStyle: " + value);
    return NORMAL;
  }

  Typeface getTypeface() {
    switch (this) {
      case NORMAL:
        return Typeface.DEFAULT;
      case BOLD:
        return Typeface.DEFAULT_BOLD;
      case ITALIC:
        return Typeface.defaultFromStyle(Typeface.ITALIC);
      case MONOSPACE:
        return Typeface.MONOSPACE;
    }
    return Typeface.DEFAULT;
  }
}
