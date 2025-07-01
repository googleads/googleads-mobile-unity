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
    public class RewardedAdPreloaderClient : IRewardedAdPreloaderClient
    {
        private IntPtr _rewardedAdPreloaderPtr;
        private readonly IntPtr _rewardedAdPreloaderClientPtr;

        // This property should be used when setting the _rewardedAdPreloaderPtr.
        private IntPtr RewardedAdPreloaderPtr
        {
            get
            {
                return _rewardedAdPreloaderPtr;
            }

            set
            {
                Externs.GADURelease(_rewardedAdPreloaderPtr);
                _rewardedAdPreloaderPtr = value;
            }
        }

        internal delegate void GADUAdAvailableForPreloadIdCallback(IntPtr rewardedAdPreloaderClient,
                                                                   string preloadId,
                                                                   IntPtr responseInfoClient);

        internal delegate void GADUAdFailedToPreloadForPreloadIdCallback(
                IntPtr rewardedAdPreloaderClient,
                string preloadId,
                IntPtr adErrorReference);

        internal delegate void GADUAdsExhaustedForPreloadIdCallback(
                IntPtr rewardedAdPreloaderClient, string preloadId);

        private Action<string, IAdErrorClient> _onAdFailedToPreload;
        private Action<string, IResponseInfoClient> _onAdPreloaded;
        private Action<string> _onAdsExhausted;

        public RewardedAdPreloaderClient()
        {
            _rewardedAdPreloaderClientPtr = (IntPtr)GCHandle.Alloc(this);
            RewardedAdPreloaderPtr = Externs.GADUCreateRewardedAdPreloader(
                    _rewardedAdPreloaderClientPtr);

            Externs.GADUSetRewardedAdPreloaderCallbacks(
                RewardedAdPreloaderPtr,
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
            return Externs.GADURewardedAdPreloaderPreload(RewardedAdPreloaderPtr, preloadId,
                                                          preloadConfigRef);
        }

        public bool IsAdAvailable(string preloadId)
        {
            return Externs.GADURewardedAdPreloaderIsAdAvailable(RewardedAdPreloaderPtr, preloadId);
        }

        public IRewardedAdClient DequeueAd(string preloadId)
        {
            var rewardedAdClient = new RewardedAdClient();
            var rewardedAdClientPtr = (IntPtr)GCHandle.Alloc(rewardedAdClient);
            var rewardedAd = Externs.GADURewardedAdPreloaderGetPreloadedAd(RewardedAdPreloaderPtr,
                                                                           preloadId,
                                                                           rewardedAdClientPtr);
            if (rewardedAd == IntPtr.Zero) return null;
            rewardedAdClient.CreateRewardedAdWithReference(rewardedAdClientPtr, rewardedAd);
            return rewardedAdClient;
        }

        public int GetNumAdsAvailable(string preloadId)
        {
            return Externs.GADURewardedAdPreloaderGetNumAdsAvailable(RewardedAdPreloaderPtr,
                                                                     preloadId);
        }

        public PreloadConfiguration GetConfiguration(string preloadId)
        {
            var config = Externs.GADURewardedAdPreloaderGetConfiguration(RewardedAdPreloaderPtr,
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
                Format = AdFormat.REWARDED,
#pragma warning restore 0612
                BufferSize = preloadConfigClient.BufferSize
            };
        }

        public Dictionary<string, PreloadConfiguration> GetConfigurations()
        {
            var configurations = new Dictionary<string, PreloadConfiguration>();
            var configurationsPtr = Externs.GADURewardedAdPreloaderGetConfigurations(
                    RewardedAdPreloaderPtr);
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
                    Format = AdFormat.REWARDED,
#pragma warning restore 0612
                    BufferSize = preloadConfigClient.BufferSize
                };
            }
            return configurations;
        }

        public void Destroy(string preloadId)
        {
            Externs.GADURewardedAdPreloaderDestroy(RewardedAdPreloaderPtr, preloadId);
        }

        public void DestroyAll()
        {
            Externs.GADURewardedAdPreloaderDestroyAll(RewardedAdPreloaderPtr);
        }

        ~RewardedAdPreloaderClient()
        {
            RewardedAdPreloaderPtr = IntPtr.Zero;
            if (_rewardedAdPreloaderClientPtr == IntPtr.Zero)
            {
                return;
            }
            ((GCHandle)_rewardedAdPreloaderClientPtr).Free();
        }

        #region App open ad preloader callback methods

        [MonoPInvokeCallback(typeof(GADUAdAvailableForPreloadIdCallback))]
        private static void AdAvailableForPreloadIdCallback(IntPtr rewardedAdPreloaderClient, 
                                                            string preloadId,
                                                            IntPtr responseInfoClient)
        {
            var client = IntPtrToRewardedAdPreloaderClient(rewardedAdPreloaderClient);
            if (client._onAdPreloaded != null)
            {
                client._onAdPreloaded(preloadId,
                        new ResponseInfoClient(responseInfoClient));
            }
        }

        [MonoPInvokeCallback(typeof(GADUAdFailedToPreloadForPreloadIdCallback))]
        private static void AdFailedToPreloadForPreloadIdCallback(IntPtr rewardedAdPreloaderClient,
                                                                  string preloadId,
                                                                  IntPtr adErrorReference)
        {
            var client = IntPtrToRewardedAdPreloaderClient(rewardedAdPreloaderClient);
            if (client._onAdFailedToPreload != null)
            {
                client._onAdFailedToPreload(preloadId, new AdErrorClient(adErrorReference));
            }
        }

        [MonoPInvokeCallback(typeof(GADUAdsExhaustedForPreloadIdCallback))]
        private static void AdsExhaustedForPreloadIdCallback(IntPtr rewardedAdPreloaderClient,
                                                             string preloadId)
        {
            var client = IntPtrToRewardedAdPreloaderClient(rewardedAdPreloaderClient);
            if (client._onAdsExhausted != null)
            {
                client._onAdsExhausted(preloadId);
            }
        }

        #endregion

        private static RewardedAdPreloaderClient IntPtrToRewardedAdPreloaderClient(
                IntPtr rewardedAdPreloaderClient)
        {
            var handle = (GCHandle)rewardedAdPreloaderClient;
            return handle.Target as RewardedAdPreloaderClient;
        }

    }
}

#endif  // UNITY_IOS
