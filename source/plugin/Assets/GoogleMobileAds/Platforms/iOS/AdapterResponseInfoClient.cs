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
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
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
                return _jsonObject.GetValue<string>("Network");
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
                var value = _jsonObject.GetValue<double>("Latency") * 100;
                return Convert.ToInt64(value);
            }
        }

        public Dictionary<string, string> AdUnitMapping
        {
            get
            {
                return _jsonObject.GetDictionary<string>("AdUnitMapping");
            }
        }

        public IAdErrorClient AdError
        {
            get
            {
                var errorDescription = _jsonObject.GetValue<string>("Error");
                if (string.IsNullOrEmpty(errorDescription))
                {
                    return null;
                }
                if (errorDescription.StartsWith("{"))
                {
                    return new JsonAdErrorClient(new JsonObject(errorDescription));
                }
                var errorJson = String.Format("{ \"description\": \"{0}\" }", errorDescription);
                var errorJsonObject = new JsonObject(errorJson);
                return new JsonAdErrorClient(errorJsonObject);
            }
        }

        public override string ToString()
        {
            return _jsonObject.ToString();
        }
    }
}
