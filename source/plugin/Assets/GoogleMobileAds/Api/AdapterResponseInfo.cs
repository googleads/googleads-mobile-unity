// Copyright (C) 2022 Google, LLC
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

using GoogleMobileAds.Common;
using System.Collections.Generic;

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// Response information for an individual ad network in an ad response.
    /// </summary>
    public class AdapterResponseInfo
    {
        private IAdapterResponseInfoClient client;

        public AdapterResponseInfo(IAdapterResponseInfoClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Gets a class name that identifies the ad network adapter.
        /// </summary>
        public string AdapterClassName { get { return client.AdapterClassName; } }

        /// <summary>
        /// Gets the ad source ID associated with this adapter response.
        /// Returns an empty string if the ad server doesn't populate this field.
        /// </summary>
        public string AdSourceId { get { return client.AdSourceId; } }

        /// <summary>
        /// Gets the ad source name associated with this adapter response.
        /// Returns an empty string if the ad server doesn't populate this field.
        /// </summary>
        public string AdSourceName { get { return client.AdSourceName; } }

        /// <summary>
        /// Gets the ad source instance ID associated with this adapter response.
        /// Returns an empty string if the ad server doesn't populate this field.
        /// </summary>
        public string AdSourceInstanceId { get { return client.AdSourceInstanceId; } }

        /// <summary>
        /// Gets the ad source instance name associated with this adapter response.
        /// Returns an empty string if the ad server doesn't populate this field.
        /// </summary>
        public string AdSourceInstanceName { get { return client.AdSourceInstanceName; } }

        /// <summary>
        /// Gets a Dictionary containing the ad unit mapping.
        /// </summary>
        public Dictionary<string, string> AdUnitMapping { get { return client.AdUnitMapping; } }

        /// <summary>
        /// Gets the error that occurred while rendering the ad.
        /// Returns null if no error occurred or the adapter was not attempted.
        /// </summary>
        public AdError AdError { get { return new AdError(client.AdError); } }

        /// <summary>
        /// Gets the amount of time the ad network adapter spent loading an ad in ms.
        /// Returns 0 if the adapter was not attempted.
        /// </summary>
        public long LatencyMillis { get { return client.LatencyMillis; } }

        /// <summary>
        /// Returns a log friendly string version of this object.
        /// </summary>
        public override string ToString()
        {
            return client.ToString();
        }
    }
}
