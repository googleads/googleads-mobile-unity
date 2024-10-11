// Copyright 2024 Google LLC.
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

using System;
using UnityEngine;

using GoogleMobileAds.Api;

namespace GoogleMobileAds.Android {
  public class PreloadListener : AndroidJavaProxy {
    private Action<PreloadConfiguration> onAdsAvailableAction;

    private Action<PreloadConfiguration> onAdsExhaustedAction;

    public PreloadListener(Action<PreloadConfiguration> onAdsAvailable, Action<PreloadConfiguration> onAdsExhausted)
        : base(Utils.PreloadListenerClassname) {
      this.onAdsAvailableAction = onAdsAvailable;
      this.onAdsExhaustedAction = onAdsExhausted;
    }

    #region Callbacks from UnityPreloadListener

    void onAdsAvailable(AndroidJavaObject preloadConfiguration) {
        if (onAdsAvailableAction == null) {
        return;
        }
        PreloadConfigurationClient client = new PreloadConfigurationClient(preloadConfiguration);
        onAdsAvailableAction(new PreloadConfiguration
                {
                    AdUnitId =  client.GetAdUnitId(),
                    Format = client.GetAdFormat(),
                });
    }

    void onAdsExhausted(AndroidJavaObject preloadConfiguration) {
        if (onAdsExhaustedAction == null) {
            return;
        }
        PreloadConfigurationClient client = new PreloadConfigurationClient(preloadConfiguration);
        onAdsExhaustedAction(new PreloadConfiguration
                {
                    AdUnitId =  client.GetAdUnitId(),
                    Format = client.GetAdFormat(),
                });
    }

    #endregion
  }
}
