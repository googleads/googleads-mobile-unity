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
    public class InterstitialAdPreloaderClient : IInterstitialAdPreloaderClient
    {
        private IntPtr _interstitialAdPreloaderPtr;
        private readonly IntPtr _interstitialAdPreloaderClientPtr;

        // This property should be used when setting the _interstitialAdPreloaderPtr.
        private IntPtr InterstitialAdPreloaderPtr
        {
            get
            {
                return _interstitialAdPreloaderPtr;
            }

            set
            {
                Externs.GADURelease(_interstitialAdPreloaderPtr);
                _interstitialAdPreloaderPtr = value;
            }
        }

        internal delegate void GADUAdAvailableForPreloadIdCallback(IntPtr interstitialAdPreloaderClient,
                                                                   string preloadId,
                                                                   IntPtr responseInfoClient);

        internal delegate void GADUAdFailedToPreloadForPreloadIdCallback(
                IntPtr interstitialAdPreloaderClient,
                string preloadId,
                IntPtr adErrorReference);

        internal delegate void GADUAdsExhaustedForPreloadIdCallback(IntPtr interstitialAdPreloaderClient,
                                                                    string preloadId);

        private Action<string, IAdErrorClient> _onAdFailedToPreload;
        private Action<string, IResponseInfoClient> _onAdPreloaded;
        private Action<string> _onAdsExhausted;

        public InterstitialAdPreloaderClient()
        {
            _interstitialAdPreloaderClientPtr = (IntPtr)GCHandle.Alloc(this);
            InterstitialAdPreloaderPtr = Externs.GADUCreateInterstitialAdPreloader(_interstitialAdPreloaderClientPtr);

            Externs.GADUSetInterstitialAdPreloaderCallbacks(
                InterstitialAdPreloaderPtr,
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
            return Externs.GADUInterstitialAdPreloaderPreload(InterstitialAdPreloaderPtr, preloadId,
                                                              preloadConfigRef);
        }

        public bool IsAdAvailable(string preloadId)
        {
            return Externs.GADUInterstitialAdPreloaderIsAdAvailable(InterstitialAdPreloaderPtr,
                                                                    preloadId);
        }

        public IInterstitialClient DequeueAd(string preloadId)
        {
            var interstitialAdClient = new InterstitialClient();
            var interstitialAdClientPtr = (IntPtr)GCHandle.Alloc(interstitialAdClient);
            var interstitialAd = Externs.GADUInterstitialAdPreloaderGetPreloadedAd(
                    InterstitialAdPreloaderPtr, preloadId, interstitialAdClientPtr);
            if (interstitialAd == IntPtr.Zero) return null;
            interstitialAdClient.CreateInterstitialAdWithReference(interstitialAdClientPtr,
                                                                   interstitialAd);
            return interstitialAdClient;
        }

        public int GetNumAdsAvailable(string preloadId)
        {
            return Externs.GADUInterstitialAdPreloaderGetNumAdsAvailable(InterstitialAdPreloaderPtr,
                                                                         preloadId);
        }

        public PreloadConfiguration GetConfiguration(string preloadId)
        {
            var config = Externs.GADUInterstitialAdPreloaderGetConfiguration(
                    InterstitialAdPreloaderPtr, preloadId);
            if (config == IntPtr.Zero)
            {
                return null;
            }
            var preloadConfigClient = new PreloadConfigurationV2Client(config);
            return new PreloadConfiguration
            {
                AdUnitId = preloadConfigClient.AdUnitId,
#pragma warning disable 0612
                Format = AdFormat.INTERSTITIAL,
#pragma warning restore 0612
                BufferSize = preloadConfigClient.BufferSize
            };
        }

        public Dictionary<string, PreloadConfiguration> GetConfigurations()
        {
            var configurations = new Dictionary<string, PreloadConfiguration>();
            var configurationsPtr = Externs.GADUInterstitialAdPreloaderGetConfigurations(InterstitialAdPreloaderPtr);
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
                    Format = AdFormat.INTERSTITIAL,
#pragma warning restore 0612
                    BufferSize = preloadConfigClient.BufferSize
                };
            }
            return configurations;
        }

        public void Destroy(string preloadId)
        {
            Externs.GADUInterstitialAdPreloaderDestroy(InterstitialAdPreloaderPtr, preloadId);
        }

        public void DestroyAll()
        {
            Externs.GADUInterstitialAdPreloaderDestroyAll(InterstitialAdPreloaderPtr);
        }

        ~InterstitialAdPreloaderClient()
        {
            InterstitialAdPreloaderPtr = IntPtr.Zero;
            if (_interstitialAdPreloaderClientPtr == IntPtr.Zero)
            {
                return;
            }
            ((GCHandle)_interstitialAdPreloaderClientPtr).Free();
        }

        #region App open ad preloader callback methods

        [MonoPInvokeCallback(typeof(GADUAdAvailableForPreloadIdCallback))]
        private static void AdAvailableForPreloadIdCallback(IntPtr interstitialAdPreloaderClient,
                                                            string preloadId,
                                                            IntPtr responseInfoClient)
        {
            var client = IntPtrToInterstitialAdPreloaderClient(interstitialAdPreloaderClient);
            if (client._onAdPreloaded != null)
            {
                client._onAdPreloaded(preloadId,
                        new ResponseInfoClient(responseInfoClient));
            }
        }

        [MonoPInvokeCallback(typeof(GADUAdFailedToPreloadForPreloadIdCallback))]
        private static void AdFailedToPreloadForPreloadIdCallback(
                IntPtr interstitialAdPreloaderClient, string preloadId, IntPtr adErrorReference)
        {
            var client = IntPtrToInterstitialAdPreloaderClient(interstitialAdPreloaderClient);
            if (client._onAdFailedToPreload != null)
            {
                client._onAdFailedToPreload(preloadId, new AdErrorClient(adErrorReference));
            }
        }

        [MonoPInvokeCallback(typeof(GADUAdsExhaustedForPreloadIdCallback))]
        private static void AdsExhaustedForPreloadIdCallback(IntPtr interstitialAdPreloaderClient,
                                                             string preloadId)
        {
            var client = IntPtrToInterstitialAdPreloaderClient(interstitialAdPreloaderClient);
            if (client._onAdsExhausted != null)
            {
                client._onAdsExhausted(preloadId);
            }
        }

        #endregion

        private static InterstitialAdPreloaderClient IntPtrToInterstitialAdPreloaderClient(
                IntPtr interstitialAdPreloaderClient)
        {
            var handle = (GCHandle)interstitialAdPreloaderClient;
            return handle.Target as InterstitialAdPreloaderClient;
        }

    }
}

#endif  // UNITY_IOS
