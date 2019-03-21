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

#if UNITY_ANDROID

using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    internal class InitializationStatusClient : IInitializationStatusClient
    {
        private AndroidJavaObject status;

        public InitializationStatusClient(AndroidJavaObject status)
        {
            this.status = status;
        }

        public AdapterStatus getAdapterStatusForClassName(string className)
        {
            AndroidJavaObject map = this.status.Call<AndroidJavaObject>("getAdapterStatusMap");
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
    }
}

#endif
