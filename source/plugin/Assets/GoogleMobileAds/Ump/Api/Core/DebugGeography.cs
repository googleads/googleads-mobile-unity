// Copyright (C) 2022 Google LLC.
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

namespace GoogleMobileAds.Ump.Api
{
    /// <summary>
    /// Debug values for testing geography.
    /// </summary>
    public enum DebugGeography
    {
        /// <summary>
        /// Debug geography disabled.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Geography appears as in EEA for debug devices.
        /// </summary>
        EEA = 1,

        /// <summary>
        ///  Geography appears as not in EEA for debug devices.
        /// </summary>
        NotEEA = 2,
    }
}
