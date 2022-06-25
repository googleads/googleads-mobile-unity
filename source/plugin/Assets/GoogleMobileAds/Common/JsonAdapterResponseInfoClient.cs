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

namespace GoogleMobileAds.Common
{
    [Serializable]
    internal class JsonAdapterResponseInfoClient : IAdapterResponseInfoClient
    {
        // Workaround for Unity's lack of Dictionary deserialization support.
        [Serializable]
        protected class Pair
        {
            [SerializeField]
            public string key;
            [SerializeField]
            public string value;
        }

        [SerializeField]
        protected string adapterClassName;
        [SerializeField]
        protected string adSourceName;
        [SerializeField]
        protected string adSourceId;
        [SerializeField]
        protected string adSourceInstanceName;
        [SerializeField]
        protected string adSourceInstanceId;
        [SerializeField]
        protected Pair[] adUnitMapping;
        [SerializeField]
        protected string adError;
        [SerializeField]
        protected string description;
        [SerializeField]
        protected long latencyMillis;

        public JsonAdapterResponseInfoClient()
        {
        }

        public JsonAdapterResponseInfoClient(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }

        public string AdapterClassName { get { return adapterClassName; } }

        public string AdSourceId { get { return adSourceId; } }

        public string AdSourceName { get { return adSourceName; } }

        public string AdSourceInstanceId { get { return adSourceInstanceName; } }

        public string AdSourceInstanceName { get { return adSourceInstanceId; } }

        public Dictionary<string, string> AdUnitMapping
        {
            get { return adUnitMapping.ToDictionary(pair => pair.key, pair => pair.value); }
        }

        public IAdErrorClient AdError
        {
            get
            {
                // Serialize as string due to Unity nesting limits. b/243737332
                return JsonUtility.FromJson<JsonAdErrorClient>(adError);
            }
        }

        public long LatencyMillis { get { return LatencyMillis; } }

        public override string ToString()
        {
            return description;
        }
    }
}
