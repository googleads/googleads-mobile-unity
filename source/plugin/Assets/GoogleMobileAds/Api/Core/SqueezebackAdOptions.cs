// Copyright 2026 Google LLC
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
using UnityEngine;

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// Options for presenting a Squeezeback ad.
    /// </summary>
    [Serializable]
    public class SqueezebackAdOptions
    {
        /// <summary>
        /// The fallback background color shown behind the ad content when the creative does not
        /// provide a background media asset. If null, the SDK uses a default color.
        /// </summary>
        public Color? FallbackBackgroundColor { get; set; }

        /// <summary>
        /// Creates a new default SqueezebackAdOptions object.
        /// </summary>
        public SqueezebackAdOptions() {}

        /// <summary>
        /// Creates a SqueezebackAdOptions object from an existing instance.
        /// </summary>
        /// <param name="options">The SqueezebackAdOptions parameter to copy.</param>
        public SqueezebackAdOptions(SqueezebackAdOptions options)
        {
            if (options != null)
            {
                FallbackBackgroundColor = options.FallbackBackgroundColor;
            }
        }
    }
}
