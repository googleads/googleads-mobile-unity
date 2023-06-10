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

#if UNITY_ANDROID

using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

using GoogleMobileAds.Mediation.InMobi.Common;

namespace GoogleMobileAds.Mediation.InMobi.Android
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
            AndroidJavaObject consentObjectJSON = new AndroidJavaObject("org.json.JSONObject");
            AndroidJavaClass inMobi =
                    new AndroidJavaClass("com.google.ads.mediation.inmobi.InMobiConsent");

            foreach (KeyValuePair<string, string> entry in consentObject) {
                consentObjectJSON.Call<AndroidJavaObject>("put", entry.Key, entry.Value);
            }

            inMobi.CallStatic("updateGDPRConsent", consentObjectJSON);
        }
    }
}

#endif
