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

using GoogleMobileAds.Common.Mediation.MyTarget;

namespace GoogleMobileAds.Android.Mediation.MyTarget
{
    public class MyTargetClient : IMyTargetClient
    {
        private static MyTargetClient instance = new MyTargetClient();
        private MyTargetClient() {}

        public static MyTargetClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetUserConsent(bool userConsent)
        {
            AndroidJavaClass myTarget = new AndroidJavaClass ("com.my.target.common.MyTargetPrivacy");

            string parameterString = (userConsent == true ? "true" : "false");
            MonoBehaviour.print ("Calling 'MyTargetPrivacy.setUserConsent()' with argument: " + parameterString);
            myTarget.CallStatic ("setUserConsent", userConsent);
        }

        public void SetUserAgeRestricted(bool userAgeRestricted)
        {
            AndroidJavaClass myTarget = new AndroidJavaClass ("com.my.target.common.MyTargetPrivacy");

            string parameterString = (userAgeRestricted == true ? "true" : "false");
            MonoBehaviour.print ("Calling 'MyTargetPrivacy.setUserAgeRestricted()' with argument: " + parameterString);
            myTarget.CallStatic ("setUserAgeRestricted", userAgeRestricted);
        }

        public bool IsUserConsent()
        {
            AndroidJavaClass myTarget = new AndroidJavaClass ("com.my.target.common.MyTargetPrivacy");
            return myTarget.CallStatic<bool> ("isUserConsent");
        }

        public bool IsUserAgeRestricted()
        {
            AndroidJavaClass myTarget = new AndroidJavaClass ("com.my.target.common.MyTargetPrivacy");
            return myTarget.CallStatic<bool> ("isUserAgeRestricted");
        }
    }
}

#endif
