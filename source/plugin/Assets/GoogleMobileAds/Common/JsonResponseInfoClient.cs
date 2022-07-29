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
    internal class JsonResponseInfoClient : IResponseInfoClient
    {
        // Workaround for Unity's lacks Dictionary support.
        [Serializable]
        protected class Pair
        {
            [SerializeField]
            public string key;
            [SerializeField]
            public string value;
        }

        [SerializeField]
        protected string adNetworkName;
        [SerializeField]
        protected JsonAdapterResponseInfoClient adapterResponseInfo;
        [SerializeField]
        protected JsonAdapterResponseInfoClient[] adapterResponses;
        [SerializeField]
        protected Pair[] responseExtras;
        [SerializeField]
        protected string responseId;
        [SerializeField]
        protected string description;

        public JsonResponseInfoClient()
        {
        }

        public JsonResponseInfoClient(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }

        public IAdapterResponseInfoClient GetLoadedAdapterResponseInfo()
        {
            return adapterResponseInfo;
        }

        public IAdapterResponseInfoClient[] GetAdapterResponses()
        {
            return adapterResponses;
        }

        public Dictionary<string, string> GetResponseExtras()
        {
            return responseExtras.ToDictionary(pair => pair.key, pair => pair.value);
        }

        public string GetMediationAdapterClassName()
        {
            return adNetworkName;
        }

        public string GetResponseId()
        {
            return responseId;
        }

        public override string ToString()
        {
            return description;
        }
    }
}
