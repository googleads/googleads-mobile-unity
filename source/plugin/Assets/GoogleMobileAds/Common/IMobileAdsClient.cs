// Copyright (C) 2017 Google, Inc.
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
using System.Collections.Generic;

using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
    public interface IMobileAdsClient
    {

        // Initialize the Mobile Ads SDK and mediation adapters.
        void Initialize(Action<IInitializationStatusClient> initCompleteAction);

        /// <summary>
        /// Preload ads for the given configurations.
        /// </summary>
        /// <param name="configurations">The configurations to preload ads.</param>
        /// <param name="onAdAvailable">Called when an ad becomes available for the configuration.
        /// </param>
        /// <param name="onAdsExhausted">Called when the last available ad is exhausted for the
        /// configuration.</param>
        void Preload(List<PreloadConfiguration> configurations,
                Action<PreloadConfiguration> onAdAvailable,
                Action<PreloadConfiguration> onAdsExhausted);

        // Disable initialization of mediation adapters by the Mobile Ads SDK.
        void DisableMediationInitialization();

        // The application’s audio volume. Affects audio volumes of all ads relative
        // to other audio output. Valid ad volume values range from 0.0 (silent) to 1.0
        // (current device volume). Use this method only if your application has its own
        // volume controls (e.g., custom music or sound effect volumes). Defaults to 1.0.
        void SetApplicationVolume(float volume);

        // Indicates if the application’s audio is muted. Affects initial mute state for
        // all ads. Use this method only if your application has its own volume controls
        // (e.g., custom music or sound effect muting). Defaults to false.
        void SetApplicationMuted(bool muted);

        // Set whether an iOS app should pause when a full screen ad is displayed.
        void SetiOSAppPauseOnBackground(bool pause);

        // Disables automated SDK crash reporting.
        void DisableSDKCrashReporting();

        // Returns the scale for the current device.
        float GetDeviceScale();

        // Returns the safe width for the current device.
        int GetDeviceSafeWidth();

        // Set Global Request Configuration to Mobile Ads SDK
        void SetRequestConfiguration(RequestConfiguration requestConfiguration);

        // Returns the Mobile Ads SDK version.
        Version GetSDKVersion();

        // Get Mobile Ads SDK's Global Request Configuration
        RequestConfiguration GetRequestConfiguration();

        /// <summary>
        /// Opens the ad inspector UI.
        /// </summary>
        /// <param name="onAdInspectorClosed">Called when ad inspector UI closes.</param>
        void OpenAdInspector(Action<AdInspectorErrorClientEventArgs> adInspectorClosedAction);

        void HelloWorld();

#if GMA_PREVIEW_FEATURES
        void HelloWorldPreview();
#endif
    }
}
