// Copyright (C) 2020 Google LLC
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

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// Indicates the publisher specified that the ad request should receive treatment for
    /// users in the European Economic Area (EEA) under the age of consent.
    /// </summary>
    public enum TagForUnderAgeOfConsent
    {
        /// Unspecified
        Unspecified = -1,
        /// False
        False = 0,
        /// True
        True = 1,
    }
}
