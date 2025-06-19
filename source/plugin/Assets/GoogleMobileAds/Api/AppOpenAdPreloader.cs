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

using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// The preloader for AppOpen ads.
    /// </summary>
    public static class AppOpenAdPreloader
    {

        static AppOpenAdPreloader()
        {
            RegisterAdEvents();
        }

        private static readonly IAppOpenAdPreloaderClient _client =
            MobileAds.GetClientFactory().BuildAppOpenAdPreloaderClient();

        /// <summary>
        /// Raised with preload ID and error parameters when an ad fails to preload.
        /// </summary>
        public static event Action<string, AdError> OnAdFailedToPreload;

        /// <summary>
        /// Raised with preload ID and response info parameters when a new ad is available for the
        /// given preload ID.
        /// </summary>
        public static event Action<string, ResponseInfo> OnAdPreloaded;

        /// <summary>
        /// Raised with preload ID parameter when the last available ad is exhausted for the given
        /// preload ID.
        /// </summary>
        public static event Action<string> OnAdsExhausted;

        /// <summary>
        /// Starts preloading interstitial ads from the configuration for the given preload ID.
        /// </summary>
        /// <param name="preloadId">
        /// A string that uniquely identifies the <c>PreloadConfiguration</c>. Use this
        /// identifier when calling <c>GetPreloadedAd</c> to retrieve a preloaded ad for this
        /// configuration.
        /// </param>
        /// <param name="preloadConfiguration">
        /// The configuration that dictates how ads are preloaded.
        /// </param>
        /// <param name="onAdFailedToPreload">
        /// Called when an ad failed to load for a given preload ID.
        /// </param>
        /// <param name="onAdPreloaded">
        /// Called when a new ad is available for the given preload ID.
        /// </param>
        /// <param name="onAdsExhausted">
        /// Called when the last available ad is exhausted for the given preload ID.
        /// </param>
        /// <returns>
        /// False if preloading fails to start.
        /// </returns>
        public static bool Preload(string preloadId, PreloadConfiguration preloadConfiguration)
        {
            return _client.Preload(preloadId, preloadConfiguration);
        }

        /// <summary>
        /// Get the AppOpen ad PreloadConfiguration associated with the given preload ID, or null
        /// if one does not exist.
        /// </summary>
        public static PreloadConfiguration GetConfiguration(string preloadId)
        {
            return _client.GetConfiguration(preloadId);
        }

        /// <summary>
        /// Get a dictionary of all AppOpen ad PreloadConfigurations, keyed by their associated
        /// preload ID, or an empty dictionary if no preloaders exist.
        /// </summary>
        public static Dictionary<string, PreloadConfiguration> GetConfigurations()
        {
            return _client.GetConfigurations();
        }

        /// <summary>
        /// Returns a preloaded ad and removes it from the cache.
        /// </summary>
        /// <param name="preloadId">
        /// The ad's preload ID.
        /// </param>
        /// <returns>
        /// The preloaded ad for the given preload ID, or null if no ad is available.
        /// </returns>
        /// <remarks>
        /// The ad returned may be any of the ads preloaded for <c>preloadId</c>.
        /// The order returned is not guaranteed to match the order of <c>OnAdAvailable</c> events.
        /// </remarks>
        public static AppOpenAd GetPreloadedAd(string preloadId)
        {
            var client = _client.GetPreloadedAd(preloadId);
            if (client == null)
            {
                return null;
            }
            return new AppOpenAd(client);
        }

        /// <summary>
        /// Get the number of ads available for the given preload ID.
        /// </summary>
        /// <param name="preloadId">
        /// The preload ID to check for available ads.
        /// </param>
        /// <returns>
        /// The number of ads available for the given preload ID.
        /// </returns>
        public static int GetNumAdsAvailable(string preloadId)
        {
            return _client.GetNumAdsAvailable(preloadId);
        }

        /// <summary>
        /// Determines whether at least one ad is available for the given <c>preloadId</c>.
        /// </summary>
        /// <param name="preloadId">
        /// The preload ID to check for availability.
        /// </param>
        /// <returns>
        /// True if there is an ad available for the given preload ID.
        /// </returns>
        public static bool IsAdAvailable(string preloadId)
        {
            return _client.IsAdAvailable(preloadId);
        }

        /// <summary>
        /// Stops preloading for the given <c>preloadId</c> and destroys all associated preloaded
        /// ads.
        /// </summary>
        /// <param name="preloadId">
        /// The preload ID to stop preloading for.
        /// </param>
        /// <returns>
        /// True if an active preload configuration was destroyed; <c>false</c> if there is no
        /// active preload configuration for the given <c>preloadId</c>.
        /// </returns>
        public static bool Destroy(string preloadId)
        {
            return _client.Destroy(preloadId);
        }

        /// <summary>
        /// Stops preloading and destroys preloaded ads for all preload configurations.
        /// </summary>
        public static void DestroyAll()
        {
            _client.DestroyAll();
        }

        private static void RegisterAdEvents()
        {
            _client.OnAdPreloaded += (preloadId, responseInfo) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdPreloaded != null)
                    {
                        OnAdPreloaded(preloadId, new ResponseInfo(responseInfo));
                    }
                });
            };

            _client.OnAdFailedToPreload += (preloadId, error) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdFailedToPreload != null)
                    {
                        OnAdFailedToPreload(preloadId, new AdError(error));
                    }
                });
            };

            _client.OnAdsExhausted += (preloadId) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdsExhausted != null)
                    {
                        OnAdsExhausted(preloadId);
                    }
                });
            };

        }
    }
}
