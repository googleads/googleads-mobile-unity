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
using System.Reflection;

using GoogleMobileAds.Mediation.Tapjoy.Common;

namespace GoogleMobileAds.Mediation.Tapjoy.Android
{
    public class TapjoyClient : ITapjoyClient
    {
        private static TapjoyClient instance = new TapjoyClient();
        private TapjoyClient() { }

        private const string TAPJOY_CLASS_NAME = "com.tapjoy.Tapjoy";

        public static TapjoyClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetUserConsent(string consentString)
        {
            AndroidJavaClass tapjoy = new AndroidJavaClass(TAPJOY_CLASS_NAME);
            AndroidJavaObject tapjoyPrivacyPolicy =
                    tapjoy.CallStatic<AndroidJavaObject>("getPrivacyPolicy");
            tapjoyPrivacyPolicy.Call("setUserConsent", consentString);
        }

        public void SubjectToGDPR(bool gdprApplicability)
        {
            AndroidJavaClass tapjoy = new AndroidJavaClass(TAPJOY_CLASS_NAME);
            AndroidJavaObject tapjoyPrivacyPolicy =
                    tapjoy.CallStatic<AndroidJavaObject>("getPrivacyPolicy");
            tapjoyPrivacyPolicy.Call("setSubjectToGDPR", gdprApplicability);
        }

        public void SetUSPrivacy(string privacyString)
        {
            AndroidJavaClass tapjoy = new AndroidJavaClass(TAPJOY_CLASS_NAME);
            AndroidJavaObject tapjoyPrivacyPolicy =
                    tapjoy.CallStatic<AndroidJavaObject>("getPrivacyPolicy");
            tapjoyPrivacyPolicy.Call("setUSPrivacy", privacyString);
        }
    }
}

#endif
