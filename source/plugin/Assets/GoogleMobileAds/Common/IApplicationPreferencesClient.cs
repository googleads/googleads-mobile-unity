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

namespace GoogleMobileAds.Common
{
    /// <summary>
    /// Interface for accessing and modifying platform preference data (SharedPreferences on
    /// Android and NSUserDefaults on iOS). Values saved by this interface are saved on an
    /// application scope, and are shared by SDKs included in this application.
    /// </summary>
    public interface IApplicationPreferencesClient
    {
        /// <summary>
        /// Set an int value in the platform preferences.
        /// <param name="key">The key with which to associate the value.</param>
        /// <param name="value">The value that needs to be associated to the key.</param>
        /// </summary>
        void SetInt(string key, int value);

        /// <summary>
        /// Set a string value in the platform preferences.
        /// <param name="key">The key with which to associate the value.</param>
        /// <param name="value">The value that needs to be associated to the key.</param>
        /// </summary>
        void SetString(string key, string value);
    }
}
