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

#if UNITY_ANDROID

using System.Collections.Generic;
using UnityEngine;

using GoogleMobileAds.Common.Mediation.VerizonMedia;

namespace GoogleMobileAds.Android.Mediation.VerizonMedia
{
    public class VerizonMediaClient : IVerizonMediaClient
    {
        private static VerizonMediaClient instance = new VerizonMediaClient();
        private VerizonMediaClient() { }

        public static VerizonMediaClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetConsentData(Dictionary<string, string> consentMap, bool restricted)
        {
            AndroidJavaObject consentHashMap = new AndroidJavaObject("java.util.HashMap");
            AndroidJavaClass verizon = new AndroidJavaClass("com.google.ads.mediation.verizon.VerizonConsent");

            foreach (KeyValuePair<string, string> entry in consentMap)
            {
                consentHashMap.Call<AndroidJavaObject>("put", entry.Key, entry.Value);
            }

            AndroidJavaObject consentInstance = verizon.CallStatic<AndroidJavaObject>("getInstance");
            consentInstance.Call("setConsentData", consentHashMap, restricted);
        }

        public string GetVerizonIABConsentKey()
        {
            AndroidJavaClass vasAds = new AndroidJavaClass("com.verizon.ads.VASAds");
            return vasAds.GetStatic<string>("IAB_CONSENT_KEY");
        }
    }
}

#endif
