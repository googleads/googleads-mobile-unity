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

namespace GoogleMobileAds.Android
{
    public class ApplicationPreferencesClient : IApplicationPreferencesClient
    {
        private static AndroidJavaObject _androidApplicationPreferences;

        public ApplicationPreferencesClient()
        {
            if (_androidApplicationPreferences == null)
            {
                AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
                AndroidJavaObject activity =
                        playerClass.GetStatic<AndroidJavaObject>("currentActivity");
                _androidApplicationPreferences = new AndroidJavaObject(
                        Utils.UnityApplicationPreferencesClassName, activity);
            }
        }

        /// <summary>
        /// Set an int value in the Android default shared preferences.
        /// <param name="key">The key with which to associate the value.</param>
        /// <param name="value">The value that needs to be associated to the key.</param>
        /// </summary>
        public void SetInt(string key, int value)
        {
            _androidApplicationPreferences.Call("setInt", key, value);
        }

        /// <summary>
        /// Set a string value in the Android default shared preferences.
        /// <param name="key">The key with which to associate the value.</param>
        /// <param name="value">The value that needs to be associated to the key.</param>
        /// </summary>
        public void SetString(string key, string value)
        {
            _androidApplicationPreferences.Call("setString", key, value);
        }

        /// <summary>
        /// Read an int value from the Android default shared preferences.
        /// <param name="key">The key with which to retrieve the value.</param>
        /// </summary>
        public int GetInt(string key)
        {
            return _androidApplicationPreferences.Call<int>("getInt", key);
        }

        /// <summary>
        /// Read a string value from the Android default shared preferences.
        /// <param name="key">The key with which to retrieve the value.</param>
        /// </summary>
        public string GetString(string key)
        {
            return _androidApplicationPreferences.Call<string>("getString", key);
        }
    }
}
