// Copyright (C) 2022 Google LLC
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

namespace GoogleMobileAds.Android
{
    public class AdErrorClient : IAdErrorClient
    {
        public int Code
        {
            get { return _error.Call<int>("getCode"); }
        }

        public string Domain
        {
            get { return _error.Call<string>("getDomain"); }
        }

        public string Message
        {
            get { return _error.Call<string>("getMessage"); }
        }

        public string Description
        {
            get { return _error.Call<string>("toString"); }
        }

        public IAdErrorClient Cause
        {
            get
            {
                var error = _error.Call<AndroidJavaObject>("getCause");
                return error == null ? null : new AdErrorClient(error);
            }
        }

        private AndroidJavaObject _error;

        public AdErrorClient(AndroidJavaObject error)
        {
            _error = error;
        }
    }
}
