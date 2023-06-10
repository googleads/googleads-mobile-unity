
// Copyright (C) 2023 Google, Inc.
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

namespace GoogleMobileAds.Api.AdManager
{
    /// <summary>
    /// An <see cref="AdManagerAdRequest"/> contains targeting information used to fetch an ad from
    /// Google Ad Manager.
    /// </summary>
    [Serializable]
    public class AdManagerAdRequest : AdRequest
    {
        /// <summary>
        /// The identifier used for frequency capping, audience segmentation and
        /// targeting, sequential ad rotation, and other audience-based ad delivery
        /// controls across devices.
        /// </summary>
        public string PublisherProvidedId;

        /// <summary>
        /// The custom targeting parameters.
        /// </summary>
        public Dictionary<string, string> CustomTargeting = new Dictionary<string, string>();

        /// <summary>
        /// Hashset of strings used to exclude specified categories in ad results.
        /// </summary>
        public HashSet<String> CategoryExclusions = new HashSet<string>();

        /// <summary>
        /// Create a new AdManagerAdRequest object.
        /// </summary>
        public AdManagerAdRequest() : base() {}

        /// <summary>
        /// Create a copy of AdManagerAdRequest object.
        /// </summary>
        public AdManagerAdRequest(AdManagerAdRequest request) : base(request)
        {
            PublisherProvidedId = request.PublisherProvidedId;
            CustomTargeting = request.CustomTargeting;
            CategoryExclusions = request.CategoryExclusions;
        }
    }
}
