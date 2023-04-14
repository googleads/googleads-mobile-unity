// Copyright 2018 Google LLC
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

package com.google.unity.mediation.adcolony;

import android.os.Bundle;
import com.google.unity.ads.AdNetworkExtras;
import com.jirbo.adcolony.AdColonyAdapter;
import com.jirbo.adcolony.AdColonyBundleBuilder;
import java.util.HashMap;

/**
 * Mediation extras bundle class for the AdColony adapter.
 */
public class AdColonyUnityExtrasBuilder implements AdNetworkExtras {

  /**
   * Key to obtain the "show pre popup" option.
   */
  private static final String KEY_SHOW_PRE_POPUP = "show_pre_popup";

  /**
   * Key to obtain the "show post popup" option.
   */
  private static final String KEY_SHOW_POST_POPUP = "show_post_popup";

  @Override
  public Bundle buildExtras(HashMap<String, String> extras) {
    String showPrePopup = extras.get(KEY_SHOW_PRE_POPUP);
    if (showPrePopup != null) {
      AdColonyBundleBuilder.setShowPrePopup(Boolean.parseBoolean(showPrePopup));
    }

    String showPostPopup = extras.get(KEY_SHOW_POST_POPUP);
    if (showPostPopup != null) {
      AdColonyBundleBuilder.setShowPostPopup(Boolean.parseBoolean(showPostPopup));
    }

    return AdColonyBundleBuilder.build();
  }

  @Override
  public Class getAdapterClass() {
    return AdColonyAdapter.class;
  }
}
