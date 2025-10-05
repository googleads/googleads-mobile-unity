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
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// Interface for accessing and modifying application preference data (SharedPreferences on
    /// Android and NSUserDefaults on iOS). Values saved by this interface are saved in an
    /// application scope, and are shared by SDKs included in this application.
    /// </summary>
    public class ApplicationPreferences
    {
        static ApplicationPreferences()
        {
            try
            {
                _client = MobileAds.GetClientFactory().ApplicationPreferencesInstance();
            }
            catch (Exception ex)
            {
                // Catch any exception due to dll or platform issues.
                Debug.LogError("Cannot get an ApplicationPreferences instance: " + ex.Message);
            }
        }

        private static IApplicationPreferencesClient _client;

        /// <summary>
        /// Set an int value in the application preferences.
        /// <param name="key">The key with which to associate the value.</param>
        /// <param name="value">The value that needs to be associated to the key.</param>
        /// </summary>
        public static void SetInt(string key, int value)
        {
            _client.SetInt(key, value);
        }

        /// <summary>
        /// Set a string value in the application preferences.
        /// <param name="key">The key with which to associate the value.</param>
        /// <param name="value">The value that needs to be associated to the key.</param>
        /// </summary>
        public static void SetString(string key, string value)
        {
            _client.SetString(key, value);
        }

        /// <summary>
        /// Read an int value from the application preferences.
        /// <param name="key">The key with which to retrieve the value.</param>
        /// </summary>
        public static int GetInt(string key)
        {
            return _client.GetInt(key);
        }

        /// <summary>
        /// Read a string value from the application preferences.
        /// <param name="key">The key with which to retrieve the value.</param>
        /// </summary>
        public static string GetString(string key)
        {
            return _client.GetString(key);
        }
    }
}
