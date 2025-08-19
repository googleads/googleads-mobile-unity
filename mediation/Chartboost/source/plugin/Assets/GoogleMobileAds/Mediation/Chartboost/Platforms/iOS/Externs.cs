// Copyright 2018 Google LLC
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

namespace GoogleMobileAds.Mediation.Chartboost.iOS
{
    // Externs used by the iOS component.
    internal class Externs
    {
        [DllImport("__Internal")]
        internal static extern void GADUMChartboostAddGDPRDataUseConsent(int gdprConsent);

        [DllImport("__Internal")]
        internal static extern void GADUMChartboostAddCCPADataUseConsent(int ccpaConsent);

        [DllImport("__Internal")]
        internal static extern void GADUMChartboostAddCustomDataUseConsent(string customConsentName,
                string customConsentValue);
    }
}

#endif
