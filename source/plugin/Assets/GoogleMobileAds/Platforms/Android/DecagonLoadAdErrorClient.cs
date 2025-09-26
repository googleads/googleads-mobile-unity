// Copyright (C) 2025 Google LLC
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

using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Android
{
    internal class DecagonLoadAdErrorClient : ILoadAdErrorClient
    {
        AndroidJavaObject _loadAdError;

        public DecagonLoadAdErrorClient(AndroidJavaObject loadAdError)
        {
            _loadAdError = loadAdError;
        }

        public int GetCode()
        {
           return _loadAdError.Call<int>("getCode");
        }

        public string GetDomain()
        {
            return "";
        }

        public string GetMessage()
        {
            return _loadAdError.Call<string>("getMessage");
        }

        public IAdErrorClient GetCause()
        {
            return null;
        }

        public IResponseInfoClient GetResponseInfoClient()
        {
            var responseInfoJavaObject = _loadAdError.Call<AndroidJavaObject>("getResponseInfo");
            return new DecagonResponseInfoClient(responseInfoJavaObject);
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}