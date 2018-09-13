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

using GoogleMobileAds.Common.Mediation.DUAdPlatform;

namespace GoogleMobileAds.Android.Mediation.DUAdPlatform
{
    public class DUAdPlatformClient : IDUAdPlatformClient
    {
        private static DUAdPlatformClient instance = new DUAdPlatformClient();
        private DUAdPlatformClient() { }

        public static DUAdPlatformClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetConsentStatus(bool consentStatus)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass duAdNetwork = new AndroidJavaClass("com.duapps.ad.base.DuAdNetwork");

            string parameterString = (consentStatus == true ? "true" : "false");
            MonoBehaviour.print("Calling 'DuAdNetwork.setConsentStatus()' with argument: " + parameterString);
            duAdNetwork.CallStatic("setConsentStatus", currentActivity, consentStatus);
        }

        public bool GetConsentStatus()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass duAdNetwork = new AndroidJavaClass("com.duapps.ad.base.DuAdNetwork");

            MonoBehaviour.print("Calling 'DuAdNetwork.getConsentStatus()'");
            return duAdNetwork.CallStatic<bool>("getConsentStatus", currentActivity);
        }
    }
}

#endif
