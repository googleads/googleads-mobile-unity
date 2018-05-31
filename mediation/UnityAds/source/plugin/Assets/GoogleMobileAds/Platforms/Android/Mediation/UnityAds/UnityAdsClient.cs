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

using System.Reflection;
using UnityEngine;

using GoogleMobileAds.Common.Mediation.UnityAds;

namespace GoogleMobileAds.Android.Mediation.UnityAds
{
    public class UnityAdsClient : IUnityAdsClient
    {
        private static UnityAdsClient instance = new UnityAdsClient();
        private UnityAdsClient() {}

        public static UnityAdsClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetGDPRConsentMetaData(bool consent)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");

            AndroidJavaObject consentObject = new AndroidJavaObject("java.lang.Boolean", consent);
            AndroidJavaObject unityAdsMetaData = new AndroidJavaObject ("com.unity3d.ads.metadata.MetaData", currentActivity);
            bool success = unityAdsMetaData.Call<bool> ("set", "gdpr.consent", consentObject);
            if (success) {
                string parameterString = (consent == true ? "true" : "false");
                MonoBehaviour.print ("Setting UnityAds metadata with key 'gdpr.consent' to value: " + parameterString);
                unityAdsMetaData.Call ("commit");
            }
        }
    }
}

#endif
