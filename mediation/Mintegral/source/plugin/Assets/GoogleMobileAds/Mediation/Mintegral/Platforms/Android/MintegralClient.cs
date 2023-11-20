// Copyright 2023 Google LLC
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

using GoogleMobileAds.Mediation.Mintegral.Common;

namespace GoogleMobileAds.Mediation.Mintegral.Android
{
    public class MintegralClient : IMintegralClient
    {
        private static MintegralClient instance = new MintegralClient();
        private MintegralClient() { }

        private const string MintegralSDKClassName = "com.mbridge.msdk.MBridgeSDK";
        private const string MintegralSDKFactoryClassName =
                "com.mbridge.msdk.out.MBridgeSDKFactory";
        private const string MintegralConstantsClassName =
                "com.mbridge.msdk.MBridgeConstans";

        public static MintegralClient Instance
        {
            get
            {
                return instance;
            }
        }

        // Public methods

        public void SetConsentStatus(bool consentStatus)
        {
            AndroidJavaClass MintegralFactory =
                    new AndroidJavaClass(MintegralSDKFactoryClassName);
            AndroidJavaObject MintegralSDK =
                    MintegralFactory.CallStatic<AndroidJavaObject>("getMBridgeSDK");

            MintegralSDK.Call("setConsentStatus",
                    getAndroidCurrentActivity(),
                    getMintegralConstantFromConsentStatus(consentStatus));
        }

        public void SetDoNotTrackStatus(bool doNotTrack)
        {
            AndroidJavaClass MintegralFactory =
                    new AndroidJavaClass(MintegralSDKFactoryClassName);
            AndroidJavaObject MintegralSDK =
                    MintegralFactory.CallStatic<AndroidJavaObject>("getMBridgeSDK");

            MintegralSDK.Call("setDoNotTrackStatus", doNotTrack);
        }

        // Private utility methods

        private AndroidJavaObject getAndroidCurrentActivity()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity =
                    unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            return currentActivity;
        }

        private int getMintegralConstantFromConsentStatus(bool consentStatus)
        {
            AndroidJavaClass MintegralConstants =
                new AndroidJavaClass(MintegralConstantsClassName);

            if (consentStatus == true) {
                return MintegralConstants.GetStatic<int>("IS_SWITCH_ON");
            }

            return MintegralConstants.GetStatic<int>("IS_SWITCH_OFF");
        }
    }
}

#endif
