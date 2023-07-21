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

using System;

using GoogleMobileAds.Mediation.LiftoffMonetize;
using GoogleMobileAds.Mediation.LiftoffMonetize.Common;

namespace GoogleMobileAds.Mediation.LiftoffMonetize.Api
{
    public enum VungleConsentStatus
    {
        OPTED_IN = 0,
        OPTED_OUT = 1
    }

    public enum VungleCCPAStatus
    {
        OPTED_IN = 0,
        OPTED_OUT = 1
    }

    public class LiftoffMonetize
    {
        public static readonly ILiftoffMonetizeClient client =
                LiftoffMonetizeClientFactory.CreateLiftoffMonetizeClient();

        public static void UpdateConsentStatus(VungleConsentStatus consentStatus,
                                               String consentMessageVersion)
        {
            client.UpdateConsentStatus(consentStatus, consentMessageVersion);
        }

        public static void UpdateCCPAStatus(VungleCCPAStatus consentStatus)
        {
            client.UpdateCCPAStatus(consentStatus);
        }

        public static void SetGDPRStatus(bool gdprStatus)
        {
            client.SetGDPRStatus(gdprStatus);
        }

        public static void SetGDPRMessageVersion(String gdprMessageVersion)
        {
            client.SetGDPRMessageVersion(gdprMessageVersion);
        }

        public static void SetCCPAStatus(bool ccpaStatus)
        {
            client.SetCCPAStatus(ccpaStatus);
        }
    }
}


namespace GoogleMobileAds.Api.Mediation.LiftoffMonetize
{
    [System.Obsolete("Use `GoogleMobileAds.Mediation.LiftoffMonetize.Api.VungleConsentStatus` instead.")]
    public enum VungleConsentStatus
    {
        OPTED_IN = 0,
        OPTED_OUT = 1
    }

    [System.Obsolete("Use `GoogleMobileAds.Mediation.LiftoffMonetize.Api.VungleCCPAStatus` instead.")]
    public enum VungleCCPAStatus
    {
        OPTED_IN = 0,
        OPTED_OUT = 1
    }

    [System.Obsolete("Use `GoogleMobileAds.Mediation.LiftoffMonetize.Api.LiftoffMonetize` instead.")]
    public class LiftoffMonetize
    {
        public static void UpdateConsentStatus(VungleConsentStatus consentStatus,
                                               String consentMessageVersion)
        {
            GoogleMobileAds.Mediation.LiftoffMonetize.Api.LiftoffMonetize.UpdateConsentStatus(
                (GoogleMobileAds.Mediation.LiftoffMonetize.Api.VungleConsentStatus)consentStatus,
                consentMessageVersion);
        }

        public static void UpdateCCPAStatus(VungleCCPAStatus consentStatus)
        {
            GoogleMobileAds.Mediation.LiftoffMonetize.Api.LiftoffMonetize.UpdateCCPAStatus(
                (GoogleMobileAds.Mediation.LiftoffMonetize.Api.VungleCCPAStatus)consentStatus);
        }
    }
}
