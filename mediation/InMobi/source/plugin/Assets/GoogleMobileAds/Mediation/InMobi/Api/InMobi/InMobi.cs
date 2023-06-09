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
using System.Collections.Generic;

using GoogleMobileAds.Mediation.InMobi;
using GoogleMobileAds.Mediation.InMobi.Common;

namespace GoogleMobileAds.Mediation.InMobi.Api
{
    public class InMobi
    {
        internal static readonly IInMobiClient client = InMobiClientFactory.InMobiInstance();

        public static void UpdateGDPRConsent(Dictionary<string, string> consentObject)
        {
            if (consentObject == null)
            {
                MonoBehaviour.print("Error: Consent object is null.");
                return;
            }

            client.UpdateGDPRConsent(consentObject);
        }
    }
}

namespace GoogleMobileAds.Api.Mediation.InMobi
{
    [System.Obsolete("Use `GoogleMobileAds.Mediation.InMobi.Api.InMobi` instead.")]
    public class InMobi
    {
        public static void UpdateGDPRConsent(Dictionary<string, string> consentObject)
        {
            GoogleMobileAds.Mediation.InMobi.Api.InMobi.UpdateGDPRConsent(consentObject);
        }
    }
}
