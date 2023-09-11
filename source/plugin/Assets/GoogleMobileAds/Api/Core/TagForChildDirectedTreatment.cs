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
    /// This enum lets you specify whether you would like your app to be treated as
    /// child-directed for purposes of the Children's Online Privacy Protection Act (COPPA) -
    /// <seealso href="http://business.ftc.gov/privacy-and-security/childrens-privacy">
    /// http://business.ftc.gov/privacy-and-security/childrens-privacy</seealso>
    ///
    /// If you set this enum to True, you are indicating that your app should be treated as
    /// child-directed for purposes of the Children's Online Privacy Protection Act (COPPA).
    ///
    /// If you set this enum to False, you are indicating that your app should not be treated as
    /// child-directed for purposes of the Children's Online Privacy Protection Act (COPPA).
    ///
    /// If you don't set this enum, or set this enum to Unspecified, ad requests will include no
    /// indication of how you would like your app treated with respect to COPPA.
    ///
    /// By using this enum, you certify that this notification is accurate and you are
    /// authorized to act on behalf of the owner of the app. You understand that abuse of this
    /// setting can result in termination of your Google account.
    /// </summary>
    public enum TagForChildDirectedTreatment
    {
        /// Unspecified
        Unspecified = -1,
        /// False
        False = 0,
        /// True
        True = 1,
    }
}
