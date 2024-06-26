// Copyright 2017 Google LLC
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
using GoogleMobileAds.Mediation.AppLovin.Common;

namespace GoogleMobileAds.Mediation.AppLovin.Android
{
    public class AppLovinClient : IAppLovinClient
    {
        private static readonly AppLovinClient instance = new AppLovinClient();
        private const string appLovinSdkClassName = "com.applovin.sdk.AppLovinSdk";
        private const string appLovinPrivacySettingsClassName =
                "com.applovin.sdk.AppLovinPrivacySettings";
        private const string UnityActivityClassName = "com.unity3d.player.UnityPlayer";

        public static AppLovinClient Instance
        {
            get
            {
                return instance;
            }
        }

        private AppLovinClient() { }

        public void Initialize()
        {
            AndroidJavaObject currentActivity = getCurrentActivityAndroidJavaObject();
            AndroidJavaClass appLovin = new AndroidJavaClass(appLovinSdkClassName);
            appLovin.CallStatic("initializeSdk", currentActivity);
        }

        public void SetHasUserConsent(bool hasUserConsent)
        {
            AndroidJavaObject currentActivity = getCurrentActivityAndroidJavaObject();
            AndroidJavaClass appLovinPrivacySettings =
                    new AndroidJavaClass(appLovinPrivacySettingsClassName);
            appLovinPrivacySettings.CallStatic("setHasUserConsent", hasUserConsent,
                                               currentActivity);
        }

        public void SetIsAgeRestrictedUser(bool isAgeRestrictedUser)
        {
            AndroidJavaObject currentActivity = getCurrentActivityAndroidJavaObject();
            AndroidJavaClass appLovinPrivacySettings =
                    new AndroidJavaClass(appLovinPrivacySettingsClassName);
            appLovinPrivacySettings.CallStatic("setIsAgeRestrictedUser", isAgeRestrictedUser,
                                               currentActivity);
        }

        public void SetDoNotSell(bool doNotSell)
        {
            AndroidJavaObject currentActivity = getCurrentActivityAndroidJavaObject();
            AndroidJavaClass appLovinPrivacySettings =
                    new AndroidJavaClass(appLovinPrivacySettingsClassName);
            appLovinPrivacySettings.CallStatic("setDoNotSell", doNotSell, currentActivity);
        }

        // Private utility methods

        private AndroidJavaObject getCurrentActivityAndroidJavaObject()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass(UnityActivityClassName);
            AndroidJavaObject currentActivity =
                    unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            return currentActivity;
        }
    }
}

#endif
