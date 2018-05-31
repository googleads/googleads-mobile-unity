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

using UnityEngine;

using GoogleMobileAds.Common.Mediation.Tapjoy;
using GoogleMobileAds.Mediation;

namespace GoogleMobileAds.Api.Mediation.Tapjoy
{
    public class Tapjoy
    {
        public static readonly ITapjoyClient client = GetTapjoyClient();

        private static ITapjoyClient GetTapjoyClient()
        {
            return TapjoyClientFactory.TapjoyInstance();
        }

        public static void SetUserConsent(string consentString)
        {
            client.SetUserConsent(consentString);
        }

        public static void SubjectToGDPR(bool gdprApplicability)
        {
            client.SubjectToGDPR(gdprApplicability);
        }
    }
}
