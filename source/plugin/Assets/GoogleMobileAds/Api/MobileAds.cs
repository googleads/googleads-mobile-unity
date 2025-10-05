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
using System.Threading;
using UnityEngine;

using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// Contains logic that applies to the Google Mobile Ads SDK as a whole.
    /// </summary>
    public class MobileAds
    {
        /// <summary>
        /// Contains methods for returning the device scale and safe width.
        /// </summary>
        public static class Utils
        {
            /// <summary>
            /// The scale of the device in density independent pixels.
            /// </summary>
            public static float GetDeviceScale()
            {
                return Instance.client.GetDeviceScale();
            }

            /// <summary>
            /// The safe width of the device.
            /// </summary>
            /// <returns></returns>
            public static int GetDeviceSafeWidth()
            {
                return Instance.client.GetDeviceSafeWidth();
            }
        }

        private readonly IMobileAdsClient client = GetMobileAdsClient();

        private static IClientFactory clientFactory;

        private static MobileAds instance;

        /// <summary>
        /// The <c>MobileAds</c> static instance.
        /// </summary>
        public static MobileAds Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MobileAds();
                    MobileAdsEventExecutor.Initialize();
                }
                return instance;
            }
        }

        /// <summary>
        /// Determines whether ad events raised by the Google Mobile Ads Unity plugin should be
        /// invoked on the Unity main thread. The default value is false.
        /// </summary>
        public static bool RaiseAdEventsOnUnityMainThread { get; set; }

        /// <summary>
        /// Initializes the Google Mobile Ads SDK.
        /// </summary>
        /// <remarks>
        /// Call this method before loading an ad and before interacting with
        /// the rest of the Google Mobile Ads SDK.
        /// </remarks>
        /// <param name="initCompleteAction">
        /// An action which is invoked after initialization is complete. Includes
        /// the <c>InitializationStatus</c> of the SDK as a parameter.
        /// </param>
        public static void Initialize(Action<InitializationStatus> initCompleteAction)
        {
            Instance.client.Initialize((initStatusClient) =>
            {
                RaiseAction(() =>
                {
                    if (initCompleteAction != null)
                    {
                        initCompleteAction.Invoke(new InitializationStatus(initStatusClient));
                    }
                });
            });
        }

        /// <summary>
        /// Call before <see cref="MobileAds.Initialize(Action{InitializationStatus})"/> to
        /// disable mediation adapter initialization.
        /// </summary>
        /// <remarks>
        /// Warning: Calling this method might negatively impact your mediation performance.
        /// This method should only be called if you include mediation adapters in your app, but
        /// you don't plan on using Google mediation during this app session (for example, you are
        /// running an A/B test).
        /// </remarks>
        public static void DisableMediationInitialization()
        {
            Instance.client.DisableMediationInitialization();
        }

        /// <summary>
        /// Sets whether the app's audio is muted. Affects initial mute state for all ads.
        /// </summary>
        /// <remarks>
        /// Warning: Muting your app reduces video ad eligibility and might reduce your app's
        /// ad revenue. Use this API only if your app provides a custom mute control and reflects
        /// the user's mute decision through the API.
        /// </remarks>
        /// <param name="muted">
        /// True if the app is muted, false otherwise. Defaults to false.
        /// </param>
        public static void SetApplicationMuted(bool muted)
        {
            Instance.client.SetApplicationMuted(muted);
        }
        /// <summary>
        /// Sets the global <see cref="RequestConfiguration"/> that is used for every
        /// <see cref="AdRequest"/> during the app's session.
        /// </summary>
        /// <param name="requestConfiguration">
        /// The global configuration that is used for every <see cref="AdRequest"/>.
        /// </param>
        public static void SetRequestConfiguration(RequestConfiguration requestConfiguration)
        {
            Instance.client.SetRequestConfiguration(requestConfiguration);
        }

        /// <summary>
        /// Gets the global <see cref="RequestConfiguration"/>.
        /// </summary>
        /// <returns>
        /// The global configuration that is used for every <see cref="AdRequest"/>.
        /// </returns>
        public static RequestConfiguration GetRequestConfiguration()
        {

            return Instance.client.GetRequestConfiguration();
        }

        /// <summary>
        /// Sets the app's audio volume. Affects audio volumes of all ads relative
        /// to other audio output.
        /// </summary>
        /// <remarks>
        /// Warning: Lowering your app's audio volume reduces video ad eligibility and might reduce
        /// your app's ad revenue. Use this API only if your app provides custom volume control and
        /// reflects the user's volume choice through the API.
        /// </remarks>
        /// <param name="volume">
        /// The volume as a float from 0 (muted) to 1.0 (full media volume). Defaults to 1.0.
        /// </param>
        public static void SetApplicationVolume(float volume)
        {
            Instance.client.SetApplicationVolume(volume);
        }

        /// <summary>
        /// Indicates if the Unity app should be paused when a full-screen ad is displayed.
        /// </summary>
        /// <remarks>
        /// On Android, Unity is paused when displaying full-screen ads. Calling this method
        /// with <c>true</c> duplicates this behavior on iOS.
        /// </remarks>
        /// <param name="pause">
        /// True if iOS should pause the app when backgrounded. Default is false.
        /// </param>
        public static void SetiOSAppPauseOnBackground(bool pause)
        {
            Instance.client.SetiOSAppPauseOnBackground(pause);
        }

        /// <summary>
        /// Disables automated SDK crash reporting on iOS. Call this method closer to the app start.
        /// </summary>
        public static void DisableSDKCrashReporting()
        {
            Instance.client.DisableSDKCrashReporting();
        }

        /// <summary>
        /// Gets the underlying Google Mobile Ads Android or iOS SDK version for the active
        /// platform.
        /// </summary>
        /// <remarks>
        /// When running on the Unity editor, this method returns the Google Mobile Ads Unity SDK
        /// version.
        /// </remarks>
        /// <returns>
        /// The Google Mobile Ads Android or iOS SDK version for the platform the app is running on.
        /// </returns>
        public static Version GetPlatformVersion()
        {
            return Instance.client.GetSDKVersion();
        }

        /// <summary>
        /// Gets the Google Mobile Ads Unity SDK version.
        /// </summary>
        /// <returns>
        /// The Google Mobile Ads Unity SDK version.
        /// </returns>
        public static Version GetVersion()
        {
            Version assemblyVersion = typeof(MobileAds).Assembly.GetName().Version;
            return new Version(assemblyVersion.Major, assemblyVersion.Minor, assemblyVersion.Build);
        }

#if GMA_PREVIEW_FEATURES

        /// <summary>
        /// Preloads ads for the given configurations.
        /// </summary>
        /// <param name="configurations">The configurations to preload ads.</param>
        /// <param name="onAdAvailable">Called when an ad becomes available for the configuration.
        /// </param>
        /// <param name="onAdsExhausted">Called when the last available ad is exhausted for the
        /// configuration.</param>
        [Obsolete("Use the Preloader class for respective ad format instead.")]
        public static void Preload(List<PreloadConfiguration> configurations,
                                   Action<PreloadConfiguration> onAdAvailable,
                                   Action<PreloadConfiguration> onAdsExhausted)
        {
            Action<PreloadConfiguration> onAdAvailableAction = (preloadConfiguration) =>
                    {
                        MobileAds.RaiseAction(() =>
                                {
                                    if (onAdAvailable != null)
                                    {
                                        onAdAvailable(preloadConfiguration);
                                    }
                                });
                    };
            Action<PreloadConfiguration> onAdsExhaustedAction = (preloadConfiguration) =>
                    {
                        MobileAds.RaiseAction(() =>
                                {
                                    if (onAdsExhausted != null)
                                    {
                                        onAdsExhausted(preloadConfiguration);
                                    }
                                });
                    };
            Instance.client.Preload(configurations, onAdAvailableAction, onAdsExhaustedAction);
        }

#endif

        /// <summary>
        /// Opens ad inspector UI.
        /// </summary>
        /// <param name="adInspectorClosedAction">Called when ad inspector UI closes.</param>
        public static void OpenAdInspector(Action<AdInspectorError> adInspectorClosedAction)
        {
            Instance.client.OpenAdInspector(args =>
            {
                RaiseAction(() =>
                {
                    if(adInspectorClosedAction != null)
                    {
                        AdInspectorError error = null;
                        if (args != null && args.AdErrorClient != null)
                        {
                            error = new AdInspectorError(args.AdErrorClient);
                        }
                        adInspectorClosedAction(error);
                    }
                });
            });
        }

        internal static IClientFactory GetClientFactory() {
          if (clientFactory == null) {
            String typeName = null;
            if (Application.platform == RuntimePlatform.IPhonePlayer) {
              typeName = "GoogleMobileAds.GoogleMobileAdsClientFactory,GoogleMobileAds.iOS";
            } else if (Application.platform == RuntimePlatform.Android) {
              typeName = "GoogleMobileAds.GoogleMobileAdsClientFactory,GoogleMobileAds.Android";
            } else {
              typeName = "GoogleMobileAds.GoogleMobileAdsClientFactory,GoogleMobileAds.Unity";
            }
            Type type = Type.GetType(typeName);
            clientFactory = (IClientFactory)System.Activator.CreateInstance(type);
          }
          return clientFactory;
        }

        /// <summary>
        /// Used to provide a mock clientFactory - for unit testing only.
        /// </summary>
        /// <param name="clientFactory">
        /// <see cref="IClientFactory"/> implementation for the underlying platform.
        /// </param>
        internal static void SetClientFactory(IClientFactory clientFactory)
        {
            MobileAds.clientFactory = clientFactory;
        }

        /// <summary>
        /// Raises the action on the Unity main thread if RaiseAdEventsOnUnityMainThread is true.
        /// Raises the action on the current thread if RaiseAdEventsOnUnityMainThread is false.
        /// </summary>
        /// <param name="action">
        /// The action to raise on the Unity main thread.
        /// </param>
        internal static void RaiseAction(Action action)
        {
            if (action == null)
            {
                return;
            }
            // If we are already on the main thread or RaiseAdEventsOnUnityMainThread is false,
            // execute the action immediately.
            if (!RaiseAdEventsOnUnityMainThread || MobileAdsEventExecutor.IsOnMainThread())
            {
                action();
                return;
            }
            MobileAdsEventExecutor.ExecuteInUpdate(action);
        }

        private static IMobileAdsClient GetMobileAdsClient()
        {
            return GetClientFactory().MobileAdsInstance();
        }
    }
}
