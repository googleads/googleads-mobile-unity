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

using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Unity
{
    public class AppOpenAdPreloaderClient : IAppOpenAdPreloaderClient
    {
        private Action<string, IResponseInfoClient> _onAdPreloaded;
        private Action<string, IAdErrorClient> _onAdFailedToPreload;
        private Action<string> _onAdsExhausted;

        private Dictionary<string, PreloadConfiguration> _preloadConfigurations =
            new Dictionary<string, PreloadConfiguration>();
        private Dictionary<string, Queue<AppOpenAdClient>> _bufferedAds =
            new Dictionary<string, Queue<AppOpenAdClient>>();

        public bool Preload(string preloadId, PreloadConfiguration preloadConfiguration,
                            Action<string, IResponseInfoClient> onAdPreloaded,
                            Action<string, IAdErrorClient> onAdFailedToPreload,
                            Action<string> onAdsExhausted)
        {
            _onAdFailedToPreload = onAdFailedToPreload;
            _onAdPreloaded = onAdPreloaded;
            _onAdsExhausted = onAdsExhausted;

            if (preloadId == null)
            {
                throw new ArgumentException("preloadId cannot be null.");
            }
            if (preloadConfiguration == null)
            {
                throw new ArgumentException("preloadConfiguration cannot be null.");
            }

            preloadConfiguration.Format = AdFormat.APP_OPEN_AD;
            if (preloadConfiguration.BufferSize <= 0)
            {
                preloadConfiguration.BufferSize = 2;
            }
            // Store a copy of the configuration keyed by the preload ID.
            _preloadConfigurations[preloadId] = new PreloadConfiguration(preloadConfiguration);

            // Clear existing buffer for this ID if it exists, otherwise create a new one.
            if (!_bufferedAds.ContainsKey(preloadId))
            {
                _bufferedAds[preloadId] = new Queue<AppOpenAdClient>();
            }
            else
            {
                _bufferedAds[preloadId].Clear(); // Clear existing buffer for this ID
            }

            SimulatePreloadBuffer(preloadId, new PreloadConfiguration(preloadConfiguration));
            return true;
        }

        private void SimulatePreloadBuffer(string preloadId, PreloadConfiguration config)
        {
            Queue<AppOpenAdClient> currentQueue;
            _bufferedAds.TryGetValue(preloadId, out currentQueue);
            while (currentQueue.Count < config.BufferSize)
            {
                AppOpenAdClient adClient = new AppOpenAdClient();
                adClient.OnAdLoaded += (sender, args) =>
                {
                    if (_onAdPreloaded != null)
                    {
                        _onAdPreloaded(preloadId, new ResponseInfoClient());
                    }
                };
                adClient.LoadAd(config.AdUnitId, config.Request);
                currentQueue.Enqueue(adClient);
            }
        }

        // Returns the preload configuration for the given preload ID. Returns null if not found.
        public PreloadConfiguration GetConfiguration(string preloadId)
        {
            PreloadConfiguration config;
            _preloadConfigurations.TryGetValue(preloadId, out config);
            return config;
        }

        // Returns a copy of the preload configurations.
        public Dictionary<string, PreloadConfiguration> GetConfigurations()
        {
            return new Dictionary<string, PreloadConfiguration>(_preloadConfigurations);
        }

        public IAppOpenAdClient DequeueAd(string preloadId)
        {
            Queue<AppOpenAdClient> queue;
            if (_bufferedAds.TryGetValue(preloadId, out queue) && queue.Count > 0)
            {
                AppOpenAdClient adClient = queue.Dequeue();
                PreloadConfiguration currentPreloadConfig;
                _preloadConfigurations.TryGetValue(preloadId, out currentPreloadConfig);
                if (queue.Count == 0 && currentPreloadConfig != null && _onAdsExhausted != null)
                {
                    _onAdsExhausted(preloadId);
                }
                SimulatePreloadBuffer(preloadId, new PreloadConfiguration(currentPreloadConfig));
                return adClient;
            }
            return null;
        }

        public int GetNumAdsAvailable(string preloadId)
        {
            Queue<AppOpenAdClient> queue;
            return _bufferedAds.TryGetValue(preloadId, out queue) ? queue.Count : 0;
        }

        public bool IsAdAvailable(string preloadId)
        {
            return GetNumAdsAvailable(preloadId) > 0;
        }

        public void Destroy(string preloadId)
        {
            _preloadConfigurations.Remove(preloadId);
            _bufferedAds.Remove(preloadId);
        }

        public void DestroyAll()
        {
            _preloadConfigurations.Clear();
            _bufferedAds.Clear();
        }
    }
}
