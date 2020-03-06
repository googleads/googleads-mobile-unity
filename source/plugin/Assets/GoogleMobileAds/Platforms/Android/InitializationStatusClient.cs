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

using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System.Collections.Generic;

namespace GoogleMobileAds.Android
{
    internal class InitializationStatusClient : IInitializationStatusClient
    {
        private AndroidJavaObject status;
        private AndroidJavaObject statusMap;

        public InitializationStatusClient(AndroidJavaObject status)
        {
            this.status = status;
            this.statusMap = status.Call<AndroidJavaObject>("getAdapterStatusMap");
        }

        public AdapterStatus getAdapterStatusForClassName(string className)
        {
            AndroidJavaObject map = this.statusMap;
            AndroidJavaObject adapterStatus = map.Call<AndroidJavaObject>("get", className);

            if (adapterStatus == null)
            {
                return null;
            }

            string description = adapterStatus.Call<string>("getDescription");
            int latency = adapterStatus.Call<int>("getLatency");
            AndroidJavaClass state = new AndroidJavaClass(Utils.UnityAdapterStatusEnumName);
            AndroidJavaObject readyEnum = state.GetStatic<AndroidJavaObject>("READY");
            AndroidJavaObject adapterLoadState = adapterStatus.Call<AndroidJavaObject>("getInitializationState");
            AdapterState adapterState =
                adapterLoadState.Call<bool>("equals", readyEnum) ? AdapterState.Ready : AdapterState.NotReady;
            return new AdapterStatus(adapterState, description, latency);
        }

        public Dictionary<string, AdapterStatus> getAdapterStatusMap()
        {
            Dictionary<string, AdapterStatus> map = new Dictionary<string, AdapterStatus>();
            string[] keys = getKeys();
            foreach(string key in keys)
            {
                map.Add(key, getAdapterStatusForClassName(key));
            }
            return map;
        }

        private string[] getKeys()
        {
            AndroidJavaObject map = this.statusMap;
            AndroidJavaObject keySet = map.Call<AndroidJavaObject>("keySet");
            AndroidJavaClass arrayClass = new AndroidJavaClass("java.lang.reflect.Array");
            AndroidJavaObject arrayObject = arrayClass.CallStatic<AndroidJavaObject>("newInstance",
                                            new AndroidJavaClass("java.lang.String"),
                                            map.Call<int>("size"));
            return keySet.Call<string[]>("toArray", arrayObject);
        }
    }
}


