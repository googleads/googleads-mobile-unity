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
        private static readonly IVerizonMediaClient client = VerizonMediaClientFactory.CreateVerizonMediaClient();

        public static void SetPrivacyData(Dictionary<string, string> privacyData)
        {
            if (privacyData == null) {
                Debug.LogError("Error: Privacy Data map is null");
                return;
            }

            client.SetPrivacyData(privacyData);
        }

        public static string GetVerizonIABConsentKey()
        {
            return client.GetVerizonIABConsentKey();
        }
   }
}
