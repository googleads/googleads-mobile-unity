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
    /// Consent status values.
    /// </summary>
    public enum ConsentStatus
    {
        /// <summary>
        /// Unknown consent status.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Consent not required.
        /// </summary>
        NotRequired = 1,

        /// <summary>
        /// User consent required but not yet obtained.
        /// </summary>
        Required = 2,

        /// <summary>
        /// User consent obtained, personalized vs non-personalized undefined.
        /// </summary>
        Obtained = 3,
    }
}
