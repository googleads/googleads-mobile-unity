#if UNITY_IOS
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
        private IntPtr _adapterResponseInfoPtr;

        internal AdapterResponseInfoClient(IntPtr adapterResponseInfoPtr)
        {
            _adapterResponseInfoPtr = adapterResponseInfoPtr;
        }

        public string AdapterClassName
        {
            get
            {
                return Externs.GADUAdapterResponseInfoAdNetworkClassName(_adapterResponseInfoPtr);
            }
        }

        public string AdSourceId
        {
            get
            {
                return Externs.GADUAdapterResponseInfoAdSourceID(_adapterResponseInfoPtr);
            }
        }

        public string AdSourceName
        {
            get
            {
                return Externs.GADUAdapterResponseInfoAdSourceName(_adapterResponseInfoPtr);
            }
        }

        public string AdSourceInstanceId
        {
            get
            {
                return Externs.GADUAdapterResponseInfoAdSourceInstanceID(_adapterResponseInfoPtr);
            }
        }

        public string AdSourceInstanceName
        {
            get
            {
                return Externs.GADUAdapterResponseInfoAdSourceInstanceName(_adapterResponseInfoPtr);
            }
        }

        public long LatencyMillis
        {
            get
            {
                long latency = Externs.GADUAdapterResponseInfoLatency(_adapterResponseInfoPtr);
                return Convert.ToInt64(latency * 1000);
            }
        }

        public Dictionary<string, string> AdUnitMapping
        {
            get
            {
                int count = Externs.GADUAdapterResponseInfoAdUnitMappingCount(
                                        _adapterResponseInfoPtr);

                Dictionary<string, string> dict = new Dictionary<string, string>();
                for (int i = 0; i < count; i++)
                {
                    string key = Externs.GADUAdapterResponseInfoAdUnitMappingKey(
                                             _adapterResponseInfoPtr, i);
                    string val = Externs.GADUAdapterResponseInfoAdUnitMappingValue(
                                             _adapterResponseInfoPtr, key);
                    dict.Add(key, val);
                }
                return dict;
            }
        }

        public IAdErrorClient AdError
        {
            get
            {
                IntPtr adError = Externs.GADUAdapterResponseInfoAdError(_adapterResponseInfoPtr);
                return adError == IntPtr.Zero ? null : new AdErrorClient(adError);
            }
        }

        public override string ToString()
        {
            return Externs.GADUAdapterResponseInfoDescription(_adapterResponseInfoPtr);
        }
    }
}
#endif