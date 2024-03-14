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
        /// <summary>
        /// Set an int value in the iOS Core Foundation preferences.
        /// <param name="key">The key with which to associate the value.</param>
        /// <param name="value">The value that needs to be associated to the key.</param>
        /// </summary>
        public void SetInt(string key, int value)
        {
            Externs.GADUSetIntegerPreference(key, value);
        }

        /// <summary>
        /// Set a string value in the iOS Core Foundation preferences.
        /// <param name="key">The key with which to associate the value.</param>
        /// <param name="value">The value that needs to be associated to the key.</param>
        /// </summary>
        public void SetString(string key, string value)
        {
            Externs.GADUSetStringPreference(key, value);
        }

        /// <summary>
        /// Read an int value from the iOS Core Foundation preferences.
        /// <param name="key">The key with which to retrieve the value.</param>
        /// </summary>
        public int GetInt(string key)
        {
            return Externs.GADUGetIntegerPreference(key);
        }

        /// <summary>
        /// Read a string value from the iOS Core Foundation preferences.
        /// <param name="key">The key with which to retrieve the value.</param>
        /// </summary>
        public string GetString(string key)
        {
            return Externs.GADUGetStringPreference(key);
        }
    }
}
#endif
