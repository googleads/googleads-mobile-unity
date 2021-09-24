// Copyright (C) 2021 Google LLC
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
    internal class AdErrorClient : IAdErrorClient
    {
        AndroidJavaObject error;

        public AdErrorClient(AndroidJavaObject error)
        {
            this.error = error;
        }
        public int GetCode()
        {
           return error.Call<int>("getCode");
        }
        public string GetDomain()
        {
            return error.Call<string>("getDomain");
        }

        public string GetMessage()
        {
            return error.Call<string>("getMessage");
        }

        public IAdErrorClient GetCause()
        {
            return new AdErrorClient(error.Call<AndroidJavaObject>("getCause"));
        }

        public override string ToString()
        {
            return error.Call<string>("toString");
        }

    }
}
