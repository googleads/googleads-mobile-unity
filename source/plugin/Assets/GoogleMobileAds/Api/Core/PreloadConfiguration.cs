// Copyright (C) 2024 Google, Inc.
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

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// Configuration for preloading ads.
    /// </summary>
    [Serializable]
    public sealed class PreloadConfiguration
    {
        /// <summary>
        /// The ad unit ID of the ad to preload.
        /// </summary>
        public string AdUnitId;

        /// <summary>
        /// The ad format of the ad to preload.
        /// </summary>
        [Obsolete]
        public AdFormat Format;

        /// <summary>
        /// The ad request to preload.
        /// </summary>
        public AdRequest Request;

        /// <summary>
        /// The maximum amount of ads buffered for this configuration.
        /// </summary>
        public uint BufferSize;

        public PreloadConfiguration() { }

        public PreloadConfiguration(PreloadConfiguration configuration)
        {
            AdUnitId = configuration.AdUnitId;
#pragma warning disable CS0612
            Format = configuration.Format;
#pragma warning restore CS0612
            Request = configuration.Request;
            BufferSize = configuration.BufferSize;
        }
    }
}
