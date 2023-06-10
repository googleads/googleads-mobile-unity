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

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Unity
{
    internal class AdapterResponseInfoClient : IAdapterResponseInfoClient
    {
        public string AdapterClassName { get { return "Placeholder AdapterClassName"; } }

        public string AdSourceId { get { return "Placeholder AdSourceId"; } }

        public string AdSourceName { get { return "Placeholder AdSourceName"; } }

        public string AdSourceInstanceId { get { return "Placeholder AdSourceInstanceId"; } }

        public string AdSourceInstanceName { get { return "Placeholder AdSourceInstanceName"; } }

        public Dictionary<string, string> AdUnitMapping
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "Placeholder Key", "Placeholder Value" }
                };
            }
        }

        public IAdErrorClient AdError { get { return new AdErrorClient(); } }

        public long LatencyMillis { get { return 0; } }
    }
}
