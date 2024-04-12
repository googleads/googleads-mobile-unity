// Copyright 2024 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

package com.google.unity.mediation.line;

import android.os.Bundle;
import com.google.ads.mediation.line.LineExtras;
import com.google.ads.mediation.line.LineMediationAdapter;
import com.google.unity.ads.AdNetworkExtras;
import java.util.HashMap;

/** Mediation extras bundle class for the Line adapter. */
public class LineUnityExtrasBuilder implements AdNetworkExtras {

  /** Key to obtain the "enable ad sound" option. */
  private static final String KEY_ENABLE_AD_SOUND = "enable_ad_sound";

  @Override
  public Bundle buildExtras(HashMap<String, String> extras) {
    LineExtras lineExtras = new LineExtras();

    String enableAdSound = extras.get(KEY_ENABLE_AD_SOUND);
    if (enableAdSound != null) {
      lineExtras = new LineExtras(Boolean.parseBoolean(enableAdSound));
    }

    return lineExtras.build();
  }

  @Override
  public Class getAdapterClass() {
    return LineMediationAdapter.class;
  }
}
