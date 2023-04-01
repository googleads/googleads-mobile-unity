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
using System.Text;
using System.Collections.Generic;

namespace GoogleMobileAds.Api.AdManager
{
    /// <summary>
    /// An <see cref="AdManagerAdRequest"/> contains targeting information used to fetch an ad.
    /// Ad requests are created using <see cref="AdManagerAdRequest.Builder"/>.
    /// </summary>
    public class AdManagerAdRequest : AdRequest
    {
        /// <summary>
        /// The identifier used for frequency capping, audience segmentation and 
        /// targeting, sequential ad rotation, and other audience-based ad delivery
        /// controls across devices.
        /// </summary>
        public string PublisherProvidedId { get; private set; }

        /// <summary>
        /// The custom targeting parameters.
        /// </summary>
        public Dictionary<string, string> CustomTargeting { get; private set; }

        /// <summary>
        /// Slot-level ad category exclusion labels.
        /// </summary>
        public HashSet<String> CategoryExclusions { get; private set; }

        private AdManagerAdRequest(Builder builder)
        {
            this.Keywords = new HashSet<string>(builder.Keywords);
            this.Extras = new Dictionary<string, string>(builder.Extras);
            this.MediationExtras = builder.MediationExtras;
            this.PublisherProvidedId = builder.PublisherProvidedId;
            this.CustomTargeting = new Dictionary<string, string>(builder.CustomTargeting);
            this.CategoryExclusions = new HashSet<string>(builder.CategoryExclusions);
        }

        /// <summary>
        /// Constructs a <see cref="Builder"/>.
        /// </summary>
        public new class Builder : AdRequest.Builder
        {
            public Builder() : base()
            {
                this.CustomTargeting = new Dictionary<string, string>();
                this.CategoryExclusions = new HashSet<string>();
            }

            internal string PublisherProvidedId;
            
            internal HashSet<string> CategoryExclusions { get; private set; }

            internal Dictionary<string, string> CustomTargeting { get; private set; }

            /// <summary>
            /// Sets an identifier for use in frequency capping, audience segmentation and
            /// targeting, sequential ad rotation, and other audience-based ad delivery controls
            /// across devices.
            /// <param name="publisherProvidedId">
            /// The publisher provided identifier (PPID).
            /// </param>
            /// </summary>
            public Builder SetPublisherProvidedId(string publisherProvidedId)
            {
                this.PublisherProvidedId = publisherProvidedId;
                return this;
            }

            /// <summary>
            /// Sets a slot-level ad category exclusion label.
            /// </summary>
            /// <param name="categoryExclusion">
            /// Word or phrase describing the current activity of the user for targeting purposes.
            /// </param>
            public Builder AddCategoryExclusion(string categoryExclusion)
            {
                this.CategoryExclusions.Add(categoryExclusion);
                return this;
            }

            /// <summary>
            /// Custom key-value pairs to target Google Ad Manager campaigns (line items).
            /// </summary>
            /// <param name="key">The extra key to add.</param>
            /// <param name="value">The extra value to add.</param>
            public Builder AddCustomTargeting(string key, string value)
            {
                this.CustomTargeting.Add(key, value);
                return this;
            }

            /// <summary>
            /// Constructs an <see cref="AdManagerAdRequest"/> with the specified attributes.
            /// </summary>
            public new AdManagerAdRequest Build()
            {
                return new AdManagerAdRequest(this);
            }
        }
    }
}
