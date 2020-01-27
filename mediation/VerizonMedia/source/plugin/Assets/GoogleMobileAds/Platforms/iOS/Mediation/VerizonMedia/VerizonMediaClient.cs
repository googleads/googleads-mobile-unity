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

#if UNITY_IPHONE || UNITY_IOS

using System;
using System.Collections.Generic;

using GoogleMobileAds.Common.Mediation.VerizonMedia;

namespace GoogleMobileAds.iOS.Mediation.VerizonMedia
{
    public class VerizonMediaClient : IVerizonMediaClient
    {
        private static VerizonMediaClient instance = new VerizonMediaClient();
        private VerizonMediaClient() {}

        public static VerizonMediaClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetPrivacyData(Dictionary<string, string> privacyData)
        {
            IntPtr mutableDictionary = Externs.GADUMVerizonMediaCreateMutableDictionary();
            if (mutableDictionary != IntPtr.Zero)
            {
                foreach (KeyValuePair<string, string> entry in privacyData)
                {
                    Externs.GADUMVerizonMediaDictionaryAddObject(mutableDictionary, entry.Key, entry.Value);
                }

                Externs.GADUMVerizonMediaSetPrivacyData(mutableDictionary);
            }
        }

        public string GetVerizonIABConsentKey()
        {
            return Externs.GADUMVerizonMediaGetVerizonIABConsentKey();
        }
    }
}

#endif
