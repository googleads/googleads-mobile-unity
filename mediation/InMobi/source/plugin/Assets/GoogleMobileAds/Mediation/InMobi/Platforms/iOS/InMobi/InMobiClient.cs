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

using UnityEngine;
using System.Collections.Generic;

using GoogleMobileAds.Mediation.InMobi.Common;

namespace GoogleMobileAds.Mediation.InMobi.iOS
{
    public class InMobiClient : IInMobiClient
    {
        private static InMobiClient instance = new InMobiClient();
        private InMobiClient() {}

        public static InMobiClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void UpdateGDPRConsent(Dictionary<string, string> consentObject)
        {
            // Since a Dictionary cannot be converted to an NSDictionary object, we parse the
            // dictionary as a delimited string to be interpreted.
            string consentObjectString = "";
            foreach (KeyValuePair<string, string> entry in consentObject) {
                consentObjectString += entry.Key + "=" + entry.Value + ";";
            }

            Externs.GADMInMobiUpdateGDPRConsent(consentObjectString);
        }
    }
}

#endif
