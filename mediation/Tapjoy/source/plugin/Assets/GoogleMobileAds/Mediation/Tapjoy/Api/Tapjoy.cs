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

using GoogleMobileAds.Mediation.Tapjoy.Common;
using GoogleMobileAds.Mediation.Tapjoy;

namespace GoogleMobileAds.Mediation.Tapjoy.Api
{
    public class Tapjoy
    {
        public static readonly ITapjoyClient client = TapjoyClientFactory.CreateTapjoyClient();

        public static void SetUserConsent(string consentString)
        {
            client.SetUserConsent(consentString);
        }

        public static void SubjectToGDPR(bool gdprApplicability)
        {
            client.SubjectToGDPR(gdprApplicability);
        }

        public static void SetUSPrivacy(string privacyString)
        {
            client.SetUSPrivacy(privacyString);
        }
    }
}

namespace GoogleMobileAds.Api.Mediation.Tapjoy
{
    [System.Obsolete("Use `GoogleMobileAds.Mediation.Tapjoy.Api.Tapjoy` instead.")]
    public class Tapjoy
    {
        public static void SetUserConsent(string consentString)
        {
            GoogleMobileAds.Mediation.Tapjoy.Api.Tapjoy.SetUserConsent(consentString);
        }

        public static void SubjectToGDPR(bool gdprApplicability)
        {
            GoogleMobileAds.Mediation.Tapjoy.Api.Tapjoy.SubjectToGDPR(gdprApplicability);
        }

        public static void SetUSPrivacy(string privacyString)
        {
            GoogleMobileAds.Mediation.Tapjoy.Api.Tapjoy.SetUSPrivacy(privacyString);
        }
    }
}
