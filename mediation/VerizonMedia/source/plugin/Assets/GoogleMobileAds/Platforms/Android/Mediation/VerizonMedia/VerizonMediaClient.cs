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
        private const string VERIZON_PRIVACY_CLASS = "com.google.ads.mediation.verizon.VerizonPrivacy";

        private static VerizonMediaClient instance = new VerizonMediaClient();
        private VerizonMediaClient() { }

        public static VerizonMediaClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetPrivacyData(Dictionary<string, string> privacyData)
        {
            AndroidJavaObject privacyHashMap = new AndroidJavaObject("java.util.HashMap");
            foreach (KeyValuePair<string, string> entry in privacyData)
            {
                privacyHashMap.Call<AndroidJavaObject>("put", entry.Key, entry.Value);
            }

            AndroidJavaClass verizonPrivacyClass = new AndroidJavaClass(VERIZON_PRIVACY_CLASS);
            AndroidJavaObject verizonPrivacy = verizonPrivacyClass.CallStatic<AndroidJavaObject>("getInstance");
            verizonPrivacy.Call("setPrivacyData", privacyHashMap);
        }

        public string GetVerizonIABConsentKey()
        {
            AndroidJavaClass vasAds = new AndroidJavaClass("com.verizon.ads.VASAds");
            return vasAds.GetStatic<string>("IAB_CONSENT_KEY");
        }
    }
}

#endif
