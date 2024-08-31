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

using GoogleMobileAds.Mediation.LiftoffMonetize.Common;

namespace GoogleMobileAds.Mediation.LiftoffMonetize.Api
{
    public class LiftoffMonetize
    {
        public static readonly ILiftoffMonetizeClient client =
                LiftoffMonetizeClientFactory.CreateLiftoffMonetizeClient();

        public static void SetGDPRStatus(bool gdprStatus, string consentMessageVersion)
        {
            client.SetGDPRStatus(gdprStatus, consentMessageVersion);
        }

        public static void SetGDPRMessageVersion(string gdprMessageVersion)
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

    [System.Obsolete("Use `GoogleMobileAds.Mediation.LiftoffMonetize.Api.LiftoffMonetize` instead.")]
    public class LiftoffMonetize
    {
        public static void SetGDPRStatus(bool gdprStatus, string consentMessageVersion)
        {
            GoogleMobileAds.Mediation.LiftoffMonetize.Api.LiftoffMonetize.SetGDPRStatus(
                gdprStatus, consentMessageVersion);
        }

        public static void SetGDPRMessageVersion(string gdprMessageVersion)
        {
            GoogleMobileAds.Mediation.LiftoffMonetize.Api.LiftoffMonetize.SetGDPRMessageVersion(
                gdprMessageVersion);
        }

        public static void SetCCPAStatus(bool ccpaStatus)
        {
            GoogleMobileAds.Mediation.LiftoffMonetize.Api.LiftoffMonetize.SetCCPAStatus(
                ccpaStatus);
        }
    }
}
