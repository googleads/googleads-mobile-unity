/*
 * Copyright (C) 2026 Google LLC
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

package com.google.unity.ads.nextgen;

import com.google.android.libraries.ads.mobile.sdk.common.PrecisionType;

class Util {
  private Util() {}

  // LINT.IfChange
  public static int getAdValuePrecisionType(PrecisionType precisionType) {
    switch (precisionType) {
      case ESTIMATED:
        return 1;
      case PUBLISHER_PROVIDED:
        return 2;
      case PRECISE:
        return 3;
      case UNKNOWN:
        return 0;
    }
    return 0;
  }
  // LINT.ThenChange(//depot/google3/java/com/google/android/libraries/admob/demo/unity/googlemobileads/source/plugin/Assets/GoogleMobileAds/Api/Core/AdValue.cs)
}
