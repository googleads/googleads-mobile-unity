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

using System;
using UnityEngine;

using GoogleMobileAds.Api.Mediation.Vungle;
using GoogleMobileAds.Common.Mediation.Vungle;

namespace GoogleMobileAds.Android.Mediation.Vungle
{
    public class VungleClient : IVungleClient
    {
        private static VungleClient instance = new VungleClient();
        private VungleClient() {}

        public static VungleClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void UpdateConsentStatus(VungleConsent consentStatus)
        {
            UpdateConsentStatus(consentStatus, "");
        }

        public void UpdateConsentStatus(VungleConsent consentStatus,
                                        String consentMessageVersion)
        {
            if (consentStatus == VungleConsent.UNKNOWN) {
                MonoBehaviour.print("Cannot call 'VungleConsent.updateConsentStatus()' with unknown consent status.");
                return;
            }
            if (consentMessageVersion == null) {
                consentMessageVersion = "";
            }

            AndroidJavaObject vungleConsentObject;
            AndroidJavaClass vungleConsentEnum = new AndroidJavaClass("com.vungle.warren.Vungle$Consent");
            if (consentStatus == VungleConsent.ACCEPTED) {
                vungleConsentObject = vungleConsentEnum.GetStatic<AndroidJavaObject>("OPTED_IN");
            } else {
                vungleConsentObject = vungleConsentEnum.GetStatic<AndroidJavaObject>("OPTED_OUT");
            }

            AndroidJavaClass vungleConsentClass = new AndroidJavaClass("com.vungle.mediation.VungleConsent");
            vungleConsentClass.CallStatic("updateConsentStatus", vungleConsentObject, consentMessageVersion);
        }

        public VungleConsent GetCurrentConsentStatus()
        {
            AndroidJavaClass vungleConsentClass = new AndroidJavaClass("com.vungle.mediation.VungleConsent");
            AndroidJavaClass vungleConsentEnum = new AndroidJavaClass("com.vungle.warren.Vungle$Consent");

            AndroidJavaObject vungleConsent = vungleConsentClass.CallStatic<AndroidJavaObject>("getCurrentVungleConsent");

            if (vungleConsent == null) {
                return VungleConsent.UNKNOWN;
            }

            int vungleConsentValue = vungleConsent.Call<int> ("ordinal");
            int optedInConsentValue = vungleConsentEnum.GetStatic<AndroidJavaObject>("OPTED_IN").Call<int>("ordinal");
            int optedOutConsentValue = vungleConsentEnum.GetStatic<AndroidJavaObject>("OPTED_OUT").Call<int>("ordinal");

            if (vungleConsentValue == optedInConsentValue) {
                return VungleConsent.ACCEPTED;
            } else if (vungleConsentValue == optedOutConsentValue) {
                return VungleConsent.DENIED;
            }

            return VungleConsent.UNKNOWN;
        }

        public String GetCurrentConsentMessageVersion()
        {
            AndroidJavaClass vungleConsentClass = new AndroidJavaClass("com.vungle.mediation.VungleConsent");
            return vungleConsentClass.CallStatic<String>("getCurrentVungleConsentMessageVersion");
        }
    }
}

#endif
