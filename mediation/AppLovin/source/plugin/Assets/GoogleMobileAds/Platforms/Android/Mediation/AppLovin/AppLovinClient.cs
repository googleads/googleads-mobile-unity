// Copyright (C) 2017 Google, Inc.
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

using System.Reflection;
using UnityEngine;

using GoogleMobileAds.Common.Mediation.AppLovin;

namespace GoogleMobileAds.Android.Mediation.AppLovin
{
    public class AppLovinClient : IAppLovinClient
    {
        private static AppLovinClient instance = new AppLovinClient();
        private AppLovinClient() { }

        public static AppLovinClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void Initialize()
        {
            MonoBehaviour.print("AppLovin intialize received");
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass appLovin = new AndroidJavaClass("com.applovin.sdk.AppLovinSdk");
            appLovin.CallStatic("initializeSdk", currentActivity);
        }

        public void SetHasUserConsent(bool hasUserConsent)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass appLovinPrivacySettings = new AndroidJavaClass("com.applovin.sdk.AppLovinPrivacySettings");

            string parameterString = (hasUserConsent == true ? "true" : "false");
            MonoBehaviour.print("Calling 'AppLovinPrivacySettings.setHasUserConsent()' with argument: " + parameterString);
            appLovinPrivacySettings.CallStatic("setHasUserConsent", hasUserConsent, currentActivity);
        }

        public void SetIsAgeRestrictedUser(bool isAgeRestrictedUser)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass appLovinPrivacySettings = new AndroidJavaClass("com.applovin.sdk.AppLovinPrivacySettings");

            string parameterString = (isAgeRestrictedUser == true ? "true" : "false");
            MonoBehaviour.print("Calling 'AppLovinPrivacySettings.setIsAgeRestrictedUser()' with argument: " + parameterString);
            appLovinPrivacySettings.CallStatic("setIsAgeRestrictedUser", isAgeRestrictedUser, currentActivity);
        }
    }
}

#endif
