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

import android.util.Log;
import com.google.unity.ads.PluginUtils;
import com.google.unity.ads.R;

/** Enums to specify Type of Native Template to be used and its associated layout file */
public enum UnityNativeTemplateType {
  SMALL(R.layout.small_template_view_layout),
  MEDIUM(R.layout.medium_template_view_layout);

  private final int resourceId;

  UnityNativeTemplateType(int resourceId) {
    this.resourceId = resourceId;
  }

  public int resourceId() {
    return resourceId;
  }

  public static UnityNativeTemplateType fromIntValue(int value) {
    if (value >= 0 && value < UnityNativeTemplateType.values().length) {
      return UnityNativeTemplateType.values()[value];
    }
    Log.w(PluginUtils.LOGTAG, "Invalid template type index: " + value);
    return MEDIUM;
  }
}
