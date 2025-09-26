// Copyright (C) 2025 Google, LLC
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

using System.Collections.Generic;
using UnityEngine;

using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    internal class DecagonResponseInfoClient : IResponseInfoClient
    {
        private AndroidJavaObject _androidResponseInfo;

        public DecagonResponseInfoClient(AndroidJavaObject androidJavaObject)
        {
            _androidResponseInfo = androidJavaObject;
        }

        public List<IAdapterResponseInfoClient> GetAdapterResponses()
        {
            List<IAdapterResponseInfoClient> adapterList = new List<IAdapterResponseInfoClient>();
            if (_androidResponseInfo == null)
            {
                return adapterList;
            }

            var androidAdapterList =
                _androidResponseInfo.Call<AndroidJavaObject>("getAdSourceResponses");
            if (androidAdapterList == null)
            {
                return adapterList;
            }

            var size = androidAdapterList.Call<int>("size");
            for (int i = 0; i < size; i++)
            {
                var androidAdapterResponseInfo =
                        androidAdapterList.Call<AndroidJavaObject>("get", i);
                if (androidAdapterResponseInfo != null)
                {
                    var client = new AdapterResponseInfoClient(androidAdapterResponseInfo);
                    adapterList.Add(client);
                }
            }
            return adapterList;
        }

        public IAdapterResponseInfoClient GetLoadedAdapterResponseInfo()
        {
            if (_androidResponseInfo == null)
            {
                return null;
            }

            var androidAdSourceResponseInfo =
                    _androidResponseInfo.Call<AndroidJavaObject>("getLoadedAdSourceResponseInfo");

            return androidAdSourceResponseInfo == null
                    ? null
                    : new DecagonAdSourceResponseInfoClient(androidAdSourceResponseInfo);
        }

        public string GetMediationAdapterClassName()
        {
            if (_androidResponseInfo == null)
            {
                return string.Empty;
            }

            return _androidResponseInfo.Call<string>("getAdapterClassName");
        }

        public Dictionary<string, string> GetResponseExtras()
        {
            if (_androidResponseInfo == null)
            {
                return new Dictionary<string, string>();
            }

            var androidBundle = _androidResponseInfo.Call<AndroidJavaObject>("getResponseExtras");
            if (androidBundle == null)
            {
                return new Dictionary<string, string>();
            }

            return Utils.GetDictionary(androidBundle);
        }

        public string GetResponseId()
        {
            if (_androidResponseInfo == null)
            {
                return string.Empty;
            }

            return _androidResponseInfo.Call<string>("getResponseId");
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
