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

using UnityEngine;
using System.Collections.Generic;

using GoogleMobileAds.Common.Mediation.VerizonMedia;
using GoogleMobileAds.Mediation;

namespace GoogleMobileAds.Api.Mediation.VerizonMedia
{
    public class VerizonMedia
    {
        public static readonly IVerizonMediaClient client = GetVerizonMediaClient();

        private static IVerizonMediaClient GetVerizonMediaClient()
        {
            return VerizonMediaClientFactory.VerizonMediaInstance();
        }

        public static void SetConsentData(Dictionary<string, string> consentMap, bool restricted)
        {
            if (consentMap == null) {
                Debug.Log("Error: Consent map is null");
                return;
            }

            client.SetConsentData(consentMap, restricted);
        }

        public static string GetVerizonIABConsentKey()
        {
            return client.GetVerizonIABConsentKey();
        }
   }
}
