#if UNITY_IOS
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
using System.Reflection;
using System.Runtime.InteropServices;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    public class MobileAdsClient : IMobileAdsClient
    {
        private static MobileAdsClient instance = new MobileAdsClient();
        private Action<PreloadConfiguration> adAvailableAction;
        private Action<PreloadConfiguration> adsExhaustedAction;
        private Action<AdInspectorErrorClientEventArgs> adInspectorClosedAction;
        private Action<IInitializationStatusClient> initCompleteAction;
        private IntPtr mobileAdsClientPtr;
        internal delegate void GADUAdInspectorClosedCallback(IntPtr mobileAdsClient,
                                                             IntPtr errorRef);
        internal delegate void GADUInitializationCompleteCallback(IntPtr mobileAdsClient,
                                                                  IntPtr initStatusClient);
        internal delegate void GADUAdAvailableCallback(IntPtr mobileAdsClient,
                                                       IntPtr preloadConfigClient);
        internal delegate void GADUAdsExhaustedCallback(IntPtr mobileAdsClient,
                                                        IntPtr preloadConfigClient);

        private MobileAdsClient()
        {
            this.mobileAdsClientPtr = (IntPtr)GCHandle.Alloc(this);
        }

        public static MobileAdsClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void Initialize(Action<IInitializationStatusClient> initCompleteAction)
        {
            this.initCompleteAction = initCompleteAction;
            Externs.GADUInitializeWithCallback(this.mobileAdsClientPtr, InitializationCompleteCallback);
        }

        public void DisableMediationInitialization()
        {
            Externs.GADUDisableMediationInitialization();
        }

        public void SetApplicationVolume(float volume)
        {
            Externs.GADUSetApplicationVolume(volume);
        }

        public void SetApplicationMuted(bool muted)
        {
            Externs.GADUSetApplicationMuted(muted);
        }

        public void SetRequestConfiguration(RequestConfiguration requestConfiguration)
        {
            RequestConfigurationClient.SetRequestConfiguration(requestConfiguration);

        }

        public RequestConfiguration GetRequestConfiguration()
        {
            return RequestConfigurationClient.GetRequestConfiguration();
        }

        public void SetiOSAppPauseOnBackground(bool pause)
        {
            Externs.GADUSetiOSAppPauseOnBackground(pause);
        }

        public void DisableSDKCrashReporting()
        {
            Externs.GADUDisableSDKCrashReporting();
        }

        public float GetDeviceScale()
        {
            return Externs.GADUDeviceScale();
        }

        public int GetDeviceSafeWidth()
        {
            return Externs.GADUDeviceSafeWidth();
        }

        public Version GetSDKVersion()
        {
            string iOSVersion = Externs.GADUMobileAdsVersion();
            return new Version(iOSVersion);
        }

        public void Preload(List<PreloadConfiguration> configurations,
                            Action<PreloadConfiguration> onAdAvailable,
                            Action<PreloadConfiguration> onAdsExhausted)
        {
            this.adAvailableAction = onAdAvailable;
            this.adsExhaustedAction = onAdsExhausted;
            IntPtr[] configurationsArray = new IntPtr[configurations.Count];
            for (int configIndex = 0; configIndex < configurations.Count; configIndex++)
            {
                PreloadConfiguration preloadConfig = configurations[configIndex];
                IntPtr preloadConfigRef = Externs.GADUCreatePreloadConfiguration();
                PreloadConfigurationClient preloadConfigurationClient =
                        new PreloadConfigurationClient(preloadConfigRef)
                        {
                            AdUnitId = preloadConfig.AdUnitId,
                            Format = preloadConfig.Format,
                            BufferSize = preloadConfig.BufferSize
                        };
                if (preloadConfig.Request != null)
                {
                    preloadConfigurationClient.Request = preloadConfig.Request;
                }
                configurationsArray[configIndex] = preloadConfigurationClient.preloadConfigurationPtr;
            }
            Externs.GADUPreloadWithCallback(this.mobileAdsClientPtr, configurationsArray,
                    configurations.Count, AdAvailableCallback, AdsExhaustedCallback);
        }

        public void OpenAdInspector(Action<AdInspectorErrorClientEventArgs> onAdInspectorClosed)
        {
            adInspectorClosedAction = onAdInspectorClosed;
            Externs.GADUPresentAdInspector(this.mobileAdsClientPtr, AdInspectorClosedCallback);
        }

        public void HelloWorld()
        {
            Externs.HelloWorld();
        }

#if GMA_PREVIEW_FEATURES
        public void HelloWorldPreview()
        {
            Externs.HelloWorldPreview();
        }
#endif

        [MonoPInvokeCallback(typeof(GADUAdAvailableCallback))]
        private static void AdAvailableCallback(IntPtr mobileAdsClient, IntPtr config)
        {
            MobileAdsClient client = IntPtrToMobileAdsClient(mobileAdsClient);
            if (client.adAvailableAction != null)
            {
                PreloadConfigurationClient preloadConfigClient =
                        new PreloadConfigurationClient(config);
                client.adAvailableAction(new PreloadConfiguration()
                {
                    AdUnitId = preloadConfigClient.AdUnitId,
                    Format = preloadConfigClient.Format,
                    BufferSize = preloadConfigClient.BufferSize
                });
            }
        }

        [MonoPInvokeCallback(typeof(GADUAdsExhaustedCallback))]
        private static void AdsExhaustedCallback(IntPtr mobileAdsClient, IntPtr config)
        {
            MobileAdsClient client = IntPtrToMobileAdsClient(mobileAdsClient);
            if (client.adsExhaustedAction != null)
            {
                PreloadConfigurationClient preloadConfigClient =
                        new PreloadConfigurationClient(config);
                client.adsExhaustedAction(new PreloadConfiguration()
                {
                    AdUnitId = preloadConfigClient.AdUnitId,
                    Format = preloadConfigClient.Format,
                    BufferSize = preloadConfigClient.BufferSize
                });
            }
        }

        [MonoPInvokeCallback(typeof(GADUAdInspectorClosedCallback))]
        private static void AdInspectorClosedCallback(IntPtr mobileAdsClient, IntPtr errorRef)
        {
            MobileAdsClient client = IntPtrToMobileAdsClient(mobileAdsClient);
            if (client.adInspectorClosedAction == null)
            {
                return;
            }

            AdInspectorErrorClientEventArgs args = (errorRef == IntPtr.Zero)
                ? null
                : new AdInspectorErrorClientEventArgs
                {
                    AdErrorClient = new AdInspectorErrorClient(errorRef)
                };

            client.adInspectorClosedAction(args);
            client.adInspectorClosedAction = null;
        }

        [MonoPInvokeCallback(typeof(GADUInitializationCompleteCallback))]
        private static void InitializationCompleteCallback(IntPtr mobileAdsClient, IntPtr initStatus)
        {
            MobileAdsClient client = IntPtrToMobileAdsClient(mobileAdsClient);
            if (client.initCompleteAction != null)
            {
                IInitializationStatusClient statusClient = new InitializationStatusClient(initStatus);
                client.initCompleteAction(statusClient);
            }
            string nativePluginVersion = "";
            try
            {
                Assembly assembly = Assembly.Load("GoogleMobileAdsNative.Common");
                Version assemblyVersion = assembly.GetName().Version;
                nativePluginVersion = string.Format("{0}.{1}.{2}",
                        assemblyVersion.Major,
                        assemblyVersion.Minor,
                        assemblyVersion.Revision);
            }
            catch (Exception) {}
            string versionString = AdRequest.BuildVersionString(nativePluginVersion);
            Externs.GADUSetPlugin(versionString);
        }

        private static MobileAdsClient IntPtrToMobileAdsClient(IntPtr mobileAdsClient)
        {
            GCHandle handle = (GCHandle)mobileAdsClient;
            return handle.Target as MobileAdsClient;
        }

        public void Dispose()
        {
            ((GCHandle)this.mobileAdsClientPtr).Free();
        }

        ~MobileAdsClient()
        {
            this.Dispose();
        }
    }
}
#endif
