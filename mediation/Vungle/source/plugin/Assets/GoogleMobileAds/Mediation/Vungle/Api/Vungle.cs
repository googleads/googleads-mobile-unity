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

using GoogleMobileAds.Mediation.Vungle;
using GoogleMobileAds.Mediation.Vungle.Common;

namespace GoogleMobileAds.Mediation.Vungle.Api
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

    public class Vungle
    {
        public static readonly IVungleClient client = VungleClientFactory.CreateVungleClient();

        public static void UpdateConsentStatus(VungleConsentStatus consentStatus,
                                               String consentMessageVersion)
        {
            client.UpdateConsentStatus(consentStatus, consentMessageVersion);
        }

        public static void UpdateCCPAStatus(VungleCCPAStatus consentStatus)
        {
            client.UpdateCCPAStatus(consentStatus);
        }
    }
}


namespace GoogleMobileAds.Api.Mediation.Vungle
{
    [System.Obsolete("Use `GoogleMobileAds.Mediation.Vungle.Api.VungleConsentStatus` instead.")]
    public enum VungleConsentStatus
    {
        OPTED_IN = 0,
        OPTED_OUT = 1
    }

    [System.Obsolete("Use `GoogleMobileAds.Mediation.Vungle.Api.VungleCCPAStatus` instead.")]
    public enum VungleCCPAStatus
    {
        OPTED_IN = 0,
        OPTED_OUT = 1
    }

    [System.Obsolete("Use `GoogleMobileAds.Mediation.Vungle.Api.Vungle` instead.")]
    public class Vungle
    {
        public static void UpdateConsentStatus(VungleConsentStatus consentStatus,
                                               String consentMessageVersion)
        {
            GoogleMobileAds.Mediation.Vungle.Api.Vungle.UpdateConsentStatus(
                (GoogleMobileAds.Mediation.Vungle.Api.VungleConsentStatus)consentStatus,
                consentMessageVersion);
        }

        public static void UpdateCCPAStatus(VungleCCPAStatus consentStatus)
        {
            GoogleMobileAds.Mediation.Vungle.Api.Vungle.UpdateCCPAStatus(
                (GoogleMobileAds.Mediation.Vungle.Api.VungleCCPAStatus)consentStatus);
        }
    }
}
