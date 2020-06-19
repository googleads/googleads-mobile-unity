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

using GoogleMobileAds;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    public class MobileAds
    {
        public static class Utils
        {
            // Returns the device's scale.
            public static float GetDeviceScale()
            {
                return Instance.client.GetDeviceScale();
            }

            // Returns the safe width for the current device.
            public static int GetDeviceSafeWidth()
            {
                return Instance.client.GetDeviceSafeWidth();

            }
        }

        private readonly IMobileAdsClient client = GetMobileAdsClient();

        private static IClientFactory clientFactory;

        private static MobileAds instance;

        public static MobileAds Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MobileAds();
                }
                return instance;
            }
        }
        [Obsolete("Initialize(string appId) is deprecated, use Initialize(Action<InitializationStatus> initCompleteAction) instead.")]
        public static void Initialize(string appId)
        {
            Instance.client.Initialize(appId);
            MobileAdsEventExecutor.Initialize();
        }

        public static void Initialize(Action<InitializationStatus> initCompleteAction)
        {
            Instance.client.Initialize((initStatusClient) =>
            {

                if (initCompleteAction != null)
                {
                    initCompleteAction.Invoke(new InitializationStatus(initStatusClient));
                }
            });
            MobileAdsEventExecutor.Initialize();
        }

        public static void DisableMediationInitialization()
        {
            Instance.client.DisableMediationInitialization();
        }

        public static void SetApplicationMuted(bool muted)
        {
            Instance.client.SetApplicationMuted(muted);
        }

        public static void SetRequestConfiguration(RequestConfiguration requestConfiguration)
        {
            Instance.client.SetRequestConfiguration(requestConfiguration);
        }

        public static RequestConfiguration GetRequestConfiguration()
        {

            return Instance.client.GetRequestConfiguration();
        }

        public static void SetApplicationVolume(float volume)
        {
            Instance.client.SetApplicationVolume(volume);
        }

        public static void SetiOSAppPauseOnBackground(bool pause)
        {
            Instance.client.SetiOSAppPauseOnBackground(pause);
        }

        internal static IClientFactory GetClientFactory() {
          if (clientFactory == null) {
            clientFactory = new GoogleMobileAdsClientFactory();
          }
          return clientFactory;
        }

        private static IMobileAdsClient GetMobileAdsClient()
        {
            return GetClientFactory().MobileAdsInstance();
        }
    }
}
