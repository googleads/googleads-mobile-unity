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

namespace GoogleMobileAds.Ump.Api
{
    /// <summary>
    /// Privacy options requirement status.
    /// </summary>
    public enum PrivacyOptionsRequirementStatus
    {
        /// <summary>
        /// Privacy options requirement status is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Privacy options entry point is not required.
        /// </summary>
        NotRequired = 1,

        /// <summary>
        /// Privacy options entry point is required.
        /// </summary>
        Required = 2,
    }
}
