#if UNITY_IOS
// Copyright (C) 2025 Google LLC
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
using System.Runtime.InteropServices;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    public class AppOpenAdPreloaderClient : IAppOpenAdPreloaderClient
    {
        private IntPtr _appOpenAdPreloaderPtr;
        private readonly IntPtr _appOpenAdPreloaderClientPtr;

        // This property should be used when setting the _appOpenAdPreloaderPtr.
        private IntPtr AppOpenAdPreloaderPtr
        {
            get
            {
                return _appOpenAdPreloaderPtr;
            }

            set
            {
                Externs.GADURelease(_appOpenAdPreloaderPtr);
                _appOpenAdPreloaderPtr = value;
            }
        }

        internal delegate void GADUAdAvailableForPreloadIdCallback(IntPtr appOpenAdPreloaderClient,
                                                                   string preloadId,
                                                                   IntPtr responseInfoClient);

        internal delegate void GADUAdFailedToPreloadForPreloadIdCallback(
                IntPtr appOpenAdPreloaderClient,
                string preloadId,
                IntPtr adErrorReference);

        internal delegate void GADUAdsExhaustedForPreloadIdCallback(IntPtr appOpenAdPreloaderClient,
                                                                    string preloadId);

        private Action<string, IAdErrorClient> _onAdFailedToPreload;
        private Action<string, IResponseInfoClient> _onAdPreloaded;
        private Action<string> _onAdsExhausted;

        public AppOpenAdPreloaderClient()
        {
            _appOpenAdPreloaderClientPtr = (IntPtr)GCHandle.Alloc(this);
            AppOpenAdPreloaderPtr = Externs.GADUCreateAppOpenAdPreloader(
                    _appOpenAdPreloaderClientPtr);

            Externs.GADUSetAppOpenAdPreloaderCallbacks(
                AppOpenAdPreloaderPtr,
                AdAvailableForPreloadIdCallback,
                AdFailedToPreloadForPreloadIdCallback,
                AdsExhaustedForPreloadIdCallback);
        }

        public bool Preload(string preloadId, PreloadConfiguration preloadConfiguration,
            Action<string, IResponseInfoClient> onAdPreloaded,
            Action<string, IAdErrorClient> onAdFailedToPreload,
            Action<string> onAdsExhausted)
        {
            _onAdFailedToPreload = onAdFailedToPreload;
            _onAdPreloaded = onAdPreloaded;
            _onAdsExhausted = onAdsExhausted;

            var preloadConfigRef = Externs.GADUCreatePreloadConfigurationV2();
            var preloadConfigurationClient =
                new PreloadConfigurationV2Client(preloadConfigRef)
                {
                    AdUnitId = preloadConfiguration.AdUnitId,
                    BufferSize = preloadConfiguration.BufferSize
                };
            if (preloadConfiguration.Request != null)
            {
                preloadConfigurationClient.Request = preloadConfiguration.Request;
            }
            return Externs.GADUAppOpenAdPreloaderPreload(AppOpenAdPreloaderPtr, preloadId,
                                                         preloadConfigRef);
        }

        public bool IsAdAvailable(string preloadId)
        {
            return Externs.GADUAppOpenAdPreloaderIsAdAvailable(AppOpenAdPreloaderPtr, preloadId);
        }

        public IAppOpenAdClient DequeueAd(string preloadId)
        {
            var appOpenAdClient = new AppOpenAdClient();
            var appOpenAdClientPtr = (IntPtr)GCHandle.Alloc(appOpenAdClient);
            var appOpenAd = Externs.GADUAppOpenAdPreloaderGetPreloadedAd(AppOpenAdPreloaderPtr,
                    preloadId, appOpenAdClientPtr);
            if (appOpenAd == IntPtr.Zero) return null;
            appOpenAdClient.CreateAppOpenAdWithReference(appOpenAdClientPtr, appOpenAd);
            return appOpenAdClient;
        }

        public int GetNumAdsAvailable(string preloadId)
        {
            return Externs.GADUAppOpenAdPreloaderGetNumAdsAvailable(AppOpenAdPreloaderPtr,
                                                                    preloadId);
        }

        public PreloadConfiguration GetConfiguration(string preloadId)
        {
            var config = Externs.GADUAppOpenAdPreloaderGetConfiguration(AppOpenAdPreloaderPtr,
                                                                        preloadId);
            if (config == IntPtr.Zero)
            {
                return null;
            }
            var preloadConfigClient = new PreloadConfigurationV2Client(config);
            return new PreloadConfiguration
            {
                AdUnitId = preloadConfigClient.AdUnitId,
#pragma warning disable 0612
                Format = AdFormat.APP_OPEN_AD,
#pragma warning restore 0612
                BufferSize = preloadConfigClient.BufferSize
            };
        }

        public Dictionary<string, PreloadConfiguration> GetConfigurations()
        {
            var configurations = new Dictionary<string, PreloadConfiguration>();
            var configurationsPtr = Externs.GADUAppOpenAdPreloaderGetConfigurations(
                    AppOpenAdPreloaderPtr);
            // Marshall the Dictionary from configurationsPtr
            var marshalledConfigurations =
                    Utils.PtrNsDictionaryToDictionary(configurationsPtr);
            foreach(var entry in marshalledConfigurations)
            {
                var preloadConfigClient = new PreloadConfigurationV2Client(entry.Value);
                configurations[entry.Key] = new PreloadConfiguration
                {
                    AdUnitId = preloadConfigClient.AdUnitId,
#pragma warning disable 0612
                    Format = AdFormat.APP_OPEN_AD,
#pragma warning restore 0612
                    BufferSize = preloadConfigClient.BufferSize
                };
            }
            return configurations;
        }

        public void Destroy(string preloadId)
        {
            Externs.GADUAppOpenAdPreloaderDestroy(AppOpenAdPreloaderPtr, preloadId);
        }

        public void DestroyAll()
        {
            Externs.GADUAppOpenAdPreloaderDestroyAll(AppOpenAdPreloaderPtr);
        }

        ~AppOpenAdPreloaderClient()
        {
            AppOpenAdPreloaderPtr = IntPtr.Zero;
            if (_appOpenAdPreloaderClientPtr == IntPtr.Zero)
            {
                return;
            }
            ((GCHandle)_appOpenAdPreloaderClientPtr).Free();
        }

        #region App open ad preloader callback methods

        [MonoPInvokeCallback(typeof(GADUAdAvailableForPreloadIdCallback))]
        private static void AdAvailableForPreloadIdCallback(IntPtr appOpenAdPreloaderClient,
                                                            string preloadId,
                                                            IntPtr responseInfoClient)
        {
            var client = IntPtrToAppOpenAdPreloaderClient(appOpenAdPreloaderClient);
            if (client._onAdPreloaded != null)
            {
                client._onAdPreloaded(preloadId,
                        new ResponseInfoClient(responseInfoClient));
            }
        }

        [MonoPInvokeCallback(typeof(GADUAdFailedToPreloadForPreloadIdCallback))]
        private static void AdFailedToPreloadForPreloadIdCallback(IntPtr appOpenAdPreloaderClient,
                                                                  string preloadId,
                                                                  IntPtr adErrorReference)
        {
            var client = IntPtrToAppOpenAdPreloaderClient(appOpenAdPreloaderClient);
            if (client._onAdFailedToPreload != null)
            {
                client._onAdFailedToPreload(preloadId, new AdErrorClient(adErrorReference));
            }
        }

        [MonoPInvokeCallback(typeof(GADUAdsExhaustedForPreloadIdCallback))]
        private static void AdsExhaustedForPreloadIdCallback(IntPtr appOpenAdPreloaderClient,
                                                             string preloadId)
        {
            var client = IntPtrToAppOpenAdPreloaderClient(appOpenAdPreloaderClient);
            if (client._onAdsExhausted != null)
            {
                client._onAdsExhausted(preloadId);
            }
        }

        #endregion

        private static AppOpenAdPreloaderClient IntPtrToAppOpenAdPreloaderClient(
                IntPtr appOpenAdPreloaderClient)
        {
            var handle = (GCHandle)appOpenAdPreloaderClient;
            return handle.Target as AppOpenAdPreloaderClient;
        }

    }
}

#endif  // UNITY_IOS
