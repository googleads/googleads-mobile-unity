// Copyright (C) 2025 Google, LLC
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
    public interface IRewardedAdPreloaderClient
    {
        /// <summary>
        /// Raised when an ad fails to preload with preload ID string and error parameters.
        /// </summary>
        event Action<string, IAdErrorClient> OnAdFailedToPreload;

        /// <summary>
        /// Raised when a new ad is available for the given preload ID with preload ID string and
        /// response info parameters.
        /// </summary>
        event Action<string, IResponseInfoClient> OnAdPreloaded;

        /// <summary>
        /// Raised when the last available ad is exhausted for the given preload ID with preload ID
        /// string parameter.
        /// </summary>
        event Action<string> OnAdsExhausted;

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
        /// <returns>
        /// False if preloading fails to start.
        /// </returns>
        bool Preload(string preloadId, PreloadConfiguration preloadConfiguration);

        /// <summary>
        /// Determines whether at least one ad is available for the given <c>preloadId</c>.
        /// </summary>
        /// <param name="preloadId">
        /// The preload ID to check for availability.
        /// </param>
        /// <returns>
        /// True if there is an ad available for the given preload ID; otherwise, false.
        /// </returns>
        bool IsAdAvailable(string preloadId);

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
        IRewardedAdClient DequeueAd(string preloadId);

        /// <summary>
        /// Get the number of ads available for the given preload ID.
        /// </summary>
        /// <param name="preloadId">
        /// The preload ID to check for available ads.
        /// </param>
        /// <returns>
        /// The number of ads available for the given preload ID.
        /// </returns>
        int GetNumAdsAvailable(string preloadId);

        /// <summary>
        /// Get the Rewarded ad PreloadConfiguration associated with the given preload ID, or null
        /// if one does not exist.
        /// </summary>
        PreloadConfiguration GetConfiguration(string preloadId);

        /// <summary>
        /// Get a dictionary of all Rewarded ad PreloadConfigurations, keyed by their associated
        /// preload ID, or an empty dictionary if no preloaders exist.
        /// </summary>
        Dictionary<string, PreloadConfiguration> GetConfigurations();

        /// <summary>
        /// Stops preloading for the given <c>preloadId</c> and destroys all associated preloaded
        /// ads.
        /// </summary>
        /// <param name="preloadId">
        /// The preload ID to stop preloading for.
        /// </param>
        void Destroy(string preloadId);

        /// <summary>
        /// Stops preloading and destroys preloaded ads for all preload configurations.
        /// </summary>
        void DestroyAll();
    }
}
