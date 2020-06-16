// Copyright (C) 2020 Google, LLC.
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
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    internal class ResponseInfoClient : IResponseInfoClient
    {
        private AndroidJavaObject adFormat;
        private AndroidJavaObject androidResponseInfo;
        public ResponseInfoClient(AndroidJavaObject adFormat)
        {
            this.adFormat = adFormat;
            androidResponseInfo = this.adFormat.Call<AndroidJavaObject>("getResponseInfo");
        }

        public string GetMediationAdapterClassName()
        {
            if (androidResponseInfo != null)
            {
                return androidResponseInfo.Call<string>("getMediationAdapterClassName");
            }
            return null;
        }

        public string GetResponseId()
        {
            if (androidResponseInfo != null)
            {
                return androidResponseInfo.Call<string>("getResponseId");
            }
            return null;
        }

        public override string ToString()
        {
            return androidResponseInfo.Call<string>("toString");
        }
    }
}
