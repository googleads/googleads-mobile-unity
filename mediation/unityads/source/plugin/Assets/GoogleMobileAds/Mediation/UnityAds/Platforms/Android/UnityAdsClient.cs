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

using GoogleMobileAds.Mediation.UnityAds.Common;

namespace GoogleMobileAds.Mediation.UnityAds.Android
{
    public class UnityAdsClient : IUnityAdsClient
    {
        private const string UNITY_PLAYER_CLASS_NAME = "com.unity3d.player.UnityPlayer";
        private const string UNITY_ADS_METADATA_CLASS_NAME = "com.unity3d.ads.metadata.MetaData";

        private static UnityAdsClient instance = new UnityAdsClient();
        private UnityAdsClient() { }

        public static UnityAdsClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetConsentMetaData(string key, bool metaDataValue)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass(UNITY_PLAYER_CLASS_NAME);
            AndroidJavaObject currentActivity =
                    unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");

            AndroidJavaObject unityAdsMetaData =
                    new AndroidJavaObject(UNITY_ADS_METADATA_CLASS_NAME, currentActivity);
            bool success = unityAdsMetaData.Call<bool>("set", key, metaDataValue);
            if (success)
            {
                unityAdsMetaData.Call("commit");
            }
            else
            {
                MonoBehaviour.print("[UnityAds Plugin] Failed to update consent MetaData.");
            }
        }
    }
}

#endif
