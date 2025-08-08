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
import com.google.android.libraries.ads.mobile.sdk.MobileAds;
import com.google.android.libraries.ads.mobile.sdk.common.AdInspectorError;
import com.google.android.libraries.ads.mobile.sdk.common.OnAdInspectorClosedListener;

/** Ad inspector implementation for the Google Mobile Ads Unity plugin. */
public final class UnityAdInspector {

  private UnityAdInspector() {}

  public static void openAdInspector(
      final Activity activity, final UnityAdInspectorListener adInspectorListener) {
    MobileAds.openAdInspector(
        new OnAdInspectorClosedListener() {
          @Override
          public void onAdInspectorClosed(AdInspectorError adInspectorError) {
            if (adInspectorListener != null) {
              adInspectorListener.onAdInspectorClosed(adInspectorError);
            }
          }
        });
  }
}
