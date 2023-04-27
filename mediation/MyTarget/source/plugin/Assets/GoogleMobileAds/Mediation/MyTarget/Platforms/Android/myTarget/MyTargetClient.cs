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

using GoogleMobileAds.Mediation.MyTarget.Common;

namespace GoogleMobileAds.Mediation.MyTarget.Android
{
    public class MyTargetClient : IMyTargetClient
    {
        private static MyTargetClient instance = new MyTargetClient();
        private MyTargetClient() {}

        private const string MYTARGET_PRIVACY_CLASS_NAME = "com.my.target.common.MyTargetPrivacy";

        public static MyTargetClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetUserConsent(bool userConsent)
        {
            AndroidJavaClass myTargetPrivacy = new AndroidJavaClass(MYTARGET_PRIVACY_CLASS_NAME);
            myTargetPrivacy.CallStatic("setUserConsent", userConsent);
        }

        public bool GetUserConsent()
        {
            AndroidJavaClass myTargetPrivacy = new AndroidJavaClass(MYTARGET_PRIVACY_CLASS_NAME);
            AndroidJavaObject myTargetCurrentPrivacy =
                    myTargetPrivacy.CallStatic<AndroidJavaObject>("currentPrivacy");
            AndroidJavaObject userConsent =
                    myTargetCurrentPrivacy.Get<AndroidJavaObject>("userConsent");
            return userConsent.Call<bool>("booleanValue");
        }

        public void SetUserAgeRestricted(bool userAgeRestricted)
        {
            AndroidJavaClass myTargetPrivacy = new AndroidJavaClass(MYTARGET_PRIVACY_CLASS_NAME);
            myTargetPrivacy.CallStatic("setUserAgeRestricted", userAgeRestricted);
        }

        public bool IsUserAgeRestricted()
        {
            AndroidJavaClass myTargetPrivacy = new AndroidJavaClass(MYTARGET_PRIVACY_CLASS_NAME);
            AndroidJavaObject myTargetCurrentPrivacy =
                    myTargetPrivacy.CallStatic<AndroidJavaObject>("currentPrivacy");
            return myTargetCurrentPrivacy.Get<bool>("userAgeRestricted");
        }

        public void SetCCPAUserConsent(bool ccpaUserConsent)
        {
            AndroidJavaClass myTargetPrivacy = new AndroidJavaClass(MYTARGET_PRIVACY_CLASS_NAME);
            myTargetPrivacy.CallStatic("setCcpaUserConsent", ccpaUserConsent);
        }

        public bool GetCCPAUserConsent()
        {
            AndroidJavaClass myTargetPrivacy = new AndroidJavaClass(MYTARGET_PRIVACY_CLASS_NAME);
            AndroidJavaObject myTargetCurrentPrivacy =
                    myTargetPrivacy.CallStatic<AndroidJavaObject>("currentPrivacy");
            AndroidJavaObject ccpaUserConsent =
                    myTargetCurrentPrivacy.Get<AndroidJavaObject>("ccpaUserConsent");
            return ccpaUserConsent.Call<bool>("booleanValue");
        }
    }
}

#endif
