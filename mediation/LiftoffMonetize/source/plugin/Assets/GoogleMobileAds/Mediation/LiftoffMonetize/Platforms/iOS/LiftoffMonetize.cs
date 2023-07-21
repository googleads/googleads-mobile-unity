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

using System;
using UnityEngine;
using System.Runtime.InteropServices;
using GoogleMobileAds.Mediation.LiftoffMonetize.Api;
using GoogleMobileAds.Mediation.LiftoffMonetize.Common;

namespace GoogleMobileAds.Mediation.LiftoffMonetize.iOS
{
    public class LiftoffMonetizeClient : ILiftoffMonetizeClient
    {
        private static LiftoffMonetizeClient instance = new LiftoffMonetizeClient();
        private LiftoffMonetizeClient() {}

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
            MonoBehaviour.print("[LiftoffMonetize Plugin] 'UpdateConsentStatus' is a no-op for " +
                    "iOS. Use 'SetGDPRStatus' or 'SetGDPRMessageVersion' instead.");
        }

        public void UpdateCCPAStatus(VungleCCPAStatus ccpaStatus)
        {
            MonoBehaviour.print("[LiftoffMonetize Plugin] 'UpdateCCPAStatus' is a no-op for " +
                    "iOS. Use 'SetCCPAStatus' instead.");
        }

        public void SetGDPRStatus(bool gdprStatus)
        {
            Externs.GADUMLiftoffMonetizeSetGDPRStatus(gdprStatus);
        }

        public void SetGDPRMessageVersion(String gdprMessageVersion)
        {
            Externs.GADUMLiftoffMonetizeSetGDPRMessageVersion(gdprMessageVersion);
        }

        public void SetCCPAStatus(bool ccpaStatus)
        {
            Externs.GADUMLiftoffMonetizeSetCCPAStatus(ccpaStatus);
        }
    }
}

#endif
