#if UNITY_IOS
// Copyright (C) 2023 Google LLC.
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

namespace GoogleMobileAds.iOS
{
    public class ApplicationPreferencesClient : IApplicationPreferencesClient
    {
        private static ApplicationPreferencesClient instance = new ApplicationPreferencesClient();

        public static ApplicationPreferencesClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetInt(string key, int value)
        {
            Externs.GADUSetUserDefaultsInteger(key, value);
        }

        public void SetString(string key, string value)
        {
            Externs.GADUSetUserDefaultsString(key, value);
        }
    }
}
#endif
