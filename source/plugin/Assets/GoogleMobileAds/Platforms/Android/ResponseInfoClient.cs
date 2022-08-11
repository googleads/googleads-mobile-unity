// Copyright (C) 2020 Google, LLC
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
    public class ResponseInfoClient : IResponseInfoClient
    {
        private AndroidJavaObject _androidResponseInfo;

        public ResponseInfoClient(ResponseInfoClientType type, AndroidJavaObject androidClient)
        {
            _androidResponseInfo = androidClient.Call<AndroidJavaObject>("getResponseInfo");
        }

        public ResponseInfoClient(AndroidJavaObject androidResponseInfo)
        {
            _androidResponseInfo = androidResponseInfo;
        }

        public string GetMediationAdapterClassName()
        {
            if (_androidResponseInfo != null)
            {
                return _androidResponseInfo.Call<string>("getMediationAdapterClassName");
            }
            return null;
        }

        public string GetResponseId()
        {
            if (_androidResponseInfo != null)
            {
                return _androidResponseInfo.Call<string>("getResponseId");
            }
            return null;
        }

        public override string ToString()
        {
            if (_androidResponseInfo != null)
            {
                return _androidResponseInfo.Call<string>("toString");
            }
            return null;
        }
    }
}
