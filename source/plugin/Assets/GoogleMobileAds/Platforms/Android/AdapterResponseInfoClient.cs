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
        private AndroidJavaObject _adapterResponseInfo;

        public AdapterResponseInfoClient(AndroidJavaObject adapterResponseInfo)
        {
            _adapterResponseInfo = adapterResponseInfo;
        }

        public string AdapterClassName
        {
            get
            {
                return _adapterResponseInfo.Call<string>("getAdapterClassName");
            }
        }

        public string AdSourceId
        {
            get
            {
                return _adapterResponseInfo.Call<string>("getAdSourceId");
            }
        }

        public string AdSourceName
        {
            get
            {
                return _adapterResponseInfo.Call<string>("getAdSourceName");
            }
        }

        public string AdSourceInstanceId
        {
            get
            {
                return _adapterResponseInfo.Call<string>("getAdSourceInstanceId");
            }
        }

        public string AdSourceInstanceName
        {
            get
            {
                return _adapterResponseInfo.Call<string>("getAdSourceInstanceName");
            }
        }

        public long LatencyMillis
        {
            get
            {
                return _adapterResponseInfo.Call<long>("getLatencyMillis");
            }
        }

        public Dictionary<string, string> AdUnitMapping
        {
            get
            {
                var androidBundle =
                        _adapterResponseInfo.Call<AndroidJavaObject>("getCredentials");

                if (androidBundle == null)
                {
                    return new Dictionary<string, string>();
                }
                return Utils.GetDictionary(androidBundle);
            }
        }

        public IAdErrorClient AdError
        {
            get
            {
                var androidError =
                        _adapterResponseInfo.Call<AndroidJavaObject>("getAdError");
                return androidError == null ? null : new AdErrorClient(androidError);
            }
        }

        public override string ToString()
        {
            return _adapterResponseInfo.Call<string>("toString");
        }
    }
}
