// Copyright 2019 Google LLC
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

#if UNITY_IOS

using System;
using System.Runtime.InteropServices;

using GoogleMobileAds.Mediation.AdColony.Api;

namespace GoogleMobileAds.Mediation.AdColony.iOS
{
    // Externs used by the iOS component.
    internal class Externs
    {
        [DllImport("__Internal")]
        internal static extern void GADUMAdColonyAppOptionsSetPrivacyFrameworkRequired(
                int privacyFramework, bool isRequired);

        [DllImport("__Internal")]
        internal static extern bool GADUMAdColonyAppOptionsGetPrivacyFrameworkRequired(
                int privacyFramework);

        [DllImport("__Internal")]
        internal static extern void GADUMAdColonyAppOptionsSetPrivacyConsentString(
                int privacyFramework, string consentString);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUMAdColonyAppOptionsGetPrivacyConsentString(
                int privacyFramework);

        [DllImport("__Internal")]
        internal static extern void GADUMAdColonyAppOptionsSetUserID(string userId);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUMAdColonyAppOptionsGetUserID();

        [DllImport("__Internal")]
        internal static extern void GADUMAdColonyAppOptionsSetTestMode(bool isTestMode);

        [DllImport("__Internal")]
        internal static extern bool GADUMAdColonyAppOptionsIsTestMode();
    }
}

#endif
