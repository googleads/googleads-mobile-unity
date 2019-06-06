// Copyright 2019 Google LLC
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

#if UNITY_IOS

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using GoogleMobileAds.Common.Mediation.AdColony;

namespace GoogleMobileAds.iOS.Mediation.AdColony
{
    public class AdColonyAppOptionsClient : IAdColonyAppOptionsClient
    {
        private static AdColonyAppOptionsClient instance = new AdColonyAppOptionsClient();
        private AdColonyAppOptionsClient() { }

        public static AdColonyAppOptionsClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetGDPRConsentString(string consentString)
        {
            Externs.GADUMAdColonyAppOptionsSetGDPRConsentString(consentString);
        }

        public void SetGDPRRequired(bool gdprRequired)
        {
            Externs.GADUMAdColonyAppOptionsSetGDPRRequired(gdprRequired);
        }

        public void SetUserId(string userId)
        {
            Externs.GADUMAdColonyAppOptionsSetUserId(userId);
        }

        public void SetTestMode(bool isTestMode)
        {
            Externs.GADUMAdColonyAppOptionsSetTestMode(isTestMode);
        }

        public string GetGDPRConsentString()
        {
            return IOSStringToUnityString(Externs.GADUMAdColonyAppOptionsGetGDPRConsentString());
        }

        public bool IsGDPRRequired()
        {
            return Externs.GADUMAdColonyAppOptionsIsGDPRRequired();
        }

        public string GetUserId()
        {
            return IOSStringToUnityString(Externs.GADUMAdColonyAppOptionsGetUserId());
        }

        public bool IsTestMode()
        {
            return Externs.GADUMAdColonyAppOptionsIsTestMode();
        }

        // Utility Method to properly pass NSStrings to Unity Strings.
        private string IOSStringToUnityString(IntPtr iOSString)
        {
            string unityString = Marshal.PtrToStringAnsi(iOSString);
            Marshal.FreeHGlobal(iOSString);
            return unityString;
        }
    }
}

#endif
