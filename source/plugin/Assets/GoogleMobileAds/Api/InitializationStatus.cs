// Copyright (C) 2018 Google, Inc.
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
    /// An immutable snapshot of the Unity plugin's initialization status,
    /// categorized by mediation adapter.
    /// </summary>
    public class InitializationStatus
    {
        private IInitializationStatusClient client;

        internal InitializationStatus(IInitializationStatusClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Returns the status of a specific ad network.
        /// </summary>
        /// <param name="className">The ad network's adapter class name.</param>
        /// @param[in] className The ad network's adapter class name.
        public AdapterStatus getAdapterStatusForClassName(string className)
        {
            return client.getAdapterStatusForClassName(className);
        }

        /// <summary>
        /// Initialization status of each known ad network, keyed by its adapter's class name.
        /// </summary>
        public Dictionary<string, AdapterStatus> getAdapterStatusMap()
        {
            return client.getAdapterStatusMap();
        }
    }
}
