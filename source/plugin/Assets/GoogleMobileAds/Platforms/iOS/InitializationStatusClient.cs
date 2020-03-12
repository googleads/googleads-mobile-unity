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

using System;
using System.Collections.Generic;
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    internal class InitializationStatusClient : IInitializationStatusClient
    {
        private IntPtr status;

        public InitializationStatusClient(IntPtr status)
        {
            this.status = status;
        }

        public AdapterStatus getAdapterStatusForClassName(string className)
        {
            string description = Utils.PtrToString(
                   Externs.GADUGetInitDescription(this.status, className));
            int latency = Externs.GADUGetInitLatency(this.status, className);
            AdapterState state = (AdapterState)Externs.GADUGetInitState(this.status, className);
            return new AdapterStatus(state, description, latency);
        }

        public Dictionary<string, AdapterStatus> getAdapterStatusMap()
        {
            Dictionary<string, AdapterStatus> map = new Dictionary<string, AdapterStatus>();
            List<string> keys = GetAdapterClassNames();
            foreach(string key in keys)
            {
                map.Add(key, getAdapterStatusForClassName(key));
            }
            return map;
        }

        public List<string> GetAdapterClassNames()
        {
            IntPtr unmanagedAssetArray =
                    Externs.GADUGetInitAdapterClasses(this.status);
            int numOfAssets =
                    Externs.GADUGetInitNumberOfAdapterClasses(
                            this.status);

            return Utils.PtrArrayToManagedList(unmanagedAssetArray, numOfAssets);
        }

        public void Dispose()
        {
            Externs.GADURelease(status);
        }

        ~InitializationStatusClient()
        {
            this.Dispose();
        }
    }
}


