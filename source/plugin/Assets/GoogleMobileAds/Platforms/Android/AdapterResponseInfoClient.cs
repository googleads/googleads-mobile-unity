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

namespace GoogleMobileAds.Android
{
    internal class AdapterResponseInfoClient : IAdapterResponseInfoClient
    {
        private JsonObject _jsonObject;

        internal AdapterResponseInfoClient(JsonObject jsonObject)
        {
            _jsonObject = jsonObject;
        }

        public string AdapterClassName
        {
            get
            {
                return _jsonObject.GetValue<string>("Adapter");
            }
        }

        public string AdSourceId
        {
            get
            {
                return _jsonObject.GetValue<string>("Ad Source ID");
            }
        }

        public string AdSourceName
        {
            get
            {
                return _jsonObject.GetValue<string>("Ad Source Name");
            }
        }

        public string AdSourceInstanceId
        {
            get
            {
                return _jsonObject.GetValue<string>("Ad Source Instance ID");
            }
        }

        public string AdSourceInstanceName
        {
            get
            {
                return _jsonObject.GetValue<string>("Ad Source Instance Name");
            }
        }

        public long LatencyMillis
        {
            get
            {
                return _jsonObject.GetValue<long>("Latency");
            }
        }

        public Dictionary<string, string> AdUnitMapping
        {
            get
            {
                return _jsonObject.GetDictionary<string>("Credentials");
            }
        }

        public IAdErrorClient AdError
        {
            get
            {
                var error = _jsonObject.GetJsonObject("Ad Error");
                return error == null ? null : new JsonAdErrorClient(error);
            }
        }

        public override string ToString()
        {
            return _jsonObject.ToString();
        }
    }
}
