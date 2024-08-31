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

using UnityEngine;

using GoogleMobileAds.Mediation.LiftoffMonetize.Common;

namespace GoogleMobileAds.Mediation.LiftoffMonetize.Android
{
    public class LiftoffMonetizeClient : ILiftoffMonetizeClient
    {
        private static LiftoffMonetizeClient instance = new LiftoffMonetizeClient();
        private LiftoffMonetizeClient() {}

        private const string VUNGLE_PRIVACY_SETTINGS_CLASS_NAME =
                "com.vungle.ads.VunglePrivacySettings";

        public static LiftoffMonetizeClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetGDPRStatus(bool gdprStatus, string consentMessageVersion)
        {
            AndroidJavaClass vunglePrivacySettings =
                    new AndroidJavaClass(VUNGLE_PRIVACY_SETTINGS_CLASS_NAME);
            vunglePrivacySettings.CallStatic("setGDPRStatus", gdprStatus, consentMessageVersion);
        }

        public void SetGDPRMessageVersion(string gdprMessageVersion)
        {
            MonoBehaviour.print("[LiftoffMonetize Plugin] 'SetGDPRMessageVersion' is a no-op " +
                    "for Android. Use 'SetGDPRStatus' instead.");
        }

        public void SetCCPAStatus(bool ccpaStatus)
        {
            AndroidJavaClass vunglePrivacySettings =
                    new AndroidJavaClass(VUNGLE_PRIVACY_SETTINGS_CLASS_NAME);
            vunglePrivacySettings.CallStatic("setCCPAStatus", ccpaStatus);
        }
    }
}

#endif
