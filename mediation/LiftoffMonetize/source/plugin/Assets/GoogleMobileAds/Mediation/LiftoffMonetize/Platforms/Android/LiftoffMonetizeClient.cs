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

using GoogleMobileAds.Mediation.LiftoffMonetize.Api;
using GoogleMobileAds.Mediation.LiftoffMonetize.Common;

namespace GoogleMobileAds.Mediation.LiftoffMonetize.Android
{
    public class LiftoffMonetizeClient : ILiftoffMonetizeClient
    {
        private static LiftoffMonetizeClient instance = new LiftoffMonetizeClient();
        private LiftoffMonetizeClient() {}

        private const string VUNGLE_CLASS_NAME = "com.vungle.warren.Vungle";
        private const string VUNGLE_CONSENT_ENUM_NAME = "com.vungle.warren.Vungle$Consent";

        public static LiftoffMonetizeClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void UpdateConsentStatus(VungleConsentStatus consentStatus,
                                        String consentMessageVersion)
        {
            AndroidJavaObject LiftoffMonetizeConsentStatusObject =
                    GetConsentStatusAndroidJavaObject(consentStatus);
            if (LiftoffMonetizeConsentStatusObject == null)
            {
                MonoBehaviour.print("[LiftoffMonetize Plugin] Received invalid consent status. " +
                        "Status will not be updated.");
                return;
            }

            AndroidJavaClass vungle = new AndroidJavaClass(VUNGLE_CLASS_NAME);
            vungle.CallStatic("updateConsentStatus",
                    LiftoffMonetizeConsentStatusObject, consentMessageVersion);
        }

        public void UpdateCCPAStatus(VungleCCPAStatus ccpaStatus)
        {
            AndroidJavaObject LiftoffMonetizeCCPAStatusObject =
                    GetCCPAStatusAndroidJavaObject(ccpaStatus);
            if (LiftoffMonetizeCCPAStatusObject == null)
            {
                MonoBehaviour.print("[LiftoffMonetize Plugin] Received invalid CCPA status. " +
                        "Status will not be updated.");
                return;
            }

            AndroidJavaClass vungle = new AndroidJavaClass(VUNGLE_CLASS_NAME);
            vungle.CallStatic("updateCCPAStatus", LiftoffMonetizeCCPAStatusObject);
        }

        public void SetGDPRStatus(bool gdprStatus)
        {
            MonoBehaviour.print("[LiftoffMonetize Plugin] 'SetGDPRStatus' is a no-op for " +
                    "Android. Use 'UpdateConsentStatus' instead.");
        }

        public void SetGDPRMessageVersion(String gdprMessageVersion)
        {
            MonoBehaviour.print("[LiftoffMonetize Plugin] 'SetGDPRMessageVersion' is a no-op " +
                    "for Android. Use 'UpdateConsentStatus' instead.");
        }

        public void SetCCPAStatus(bool ccpaStatus)
        {
            MonoBehaviour.print("[LiftoffMonetize Plugin] 'SetCCPAStatus' is a no-op for " +
                    "Android. Use 'UpdateCCPAStatus' instead.");
        }

        // Private utility methods.
        private AndroidJavaObject GetConsentStatusAndroidJavaObject(
                VungleConsentStatus vungleConsent)
        {
            AndroidJavaClass vungleConsentEnum = new AndroidJavaClass(VUNGLE_CONSENT_ENUM_NAME);
            switch (vungleConsent)
            {
                case VungleConsentStatus.OPTED_IN:
                    return vungleConsentEnum.GetStatic<AndroidJavaObject>("OPTED_IN");
                case VungleConsentStatus.OPTED_OUT:
                    return vungleConsentEnum.GetStatic<AndroidJavaObject>("OPTED_OUT");
                default:
                    return null;
            }
        }

        private AndroidJavaObject GetCCPAStatusAndroidJavaObject(
                VungleCCPAStatus ccpaStatus)
        {
            AndroidJavaClass LiftoffMonetizeCCPAStatusEnum =
                    new AndroidJavaClass(VUNGLE_CONSENT_ENUM_NAME);
            switch (ccpaStatus)
            {
                case VungleCCPAStatus.OPTED_IN:
                    return LiftoffMonetizeCCPAStatusEnum.GetStatic<AndroidJavaObject>("OPTED_IN");
                case VungleCCPAStatus.OPTED_OUT:
                    return LiftoffMonetizeCCPAStatusEnum.GetStatic<AndroidJavaObject>("OPTED_OUT");
                default:
                    return null;
            }
        }
    }
}

#endif
