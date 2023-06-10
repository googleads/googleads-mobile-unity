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
using GoogleMobileAds.Mediation.AdColony.Api;
using GoogleMobileAds.Mediation.AdColony.Common;

namespace GoogleMobileAds.Mediation.AdColony.iOS
{
    public class AdColonyAppOptionsClient : IAdColonyAppOptionsClient
    {
        private static AdColonyAppOptionsClient instance = new AdColonyAppOptionsClient();
        private AdColonyAppOptionsClient() { }

        public static AdColonyAppOptionsClient Instance {
            get {
                return instance;
            }
        }

        public void SetPrivacyFrameworkRequired(AdColonyPrivacyFramework privacyFramework,
                                                bool isRequired)
        {
            Externs.GADUMAdColonyAppOptionsSetPrivacyFrameworkRequired(
                    (int)privacyFramework, isRequired);
        }

        public bool GetPrivacyFrameworkRequired(AdColonyPrivacyFramework privacyFramework)
        {
            return Externs.GADUMAdColonyAppOptionsGetPrivacyFrameworkRequired(
                    (int)privacyFramework);
        }

        public void SetPrivacyConsentString(AdColonyPrivacyFramework privacyFramework,
                                            string consentString)
        {
            Externs.GADUMAdColonyAppOptionsSetPrivacyConsentString(
                    (int)privacyFramework, consentString);
        }

        public string GetPrivacyConsentString(AdColonyPrivacyFramework privacyFramework)
        {
            return IOSStringToUnityString(
                    Externs.GADUMAdColonyAppOptionsGetPrivacyConsentString(
                            (int)privacyFramework));
        }

        public void SetUserId(string userId)
        {
            Externs.GADUMAdColonyAppOptionsSetUserID(userId);
        }

        public string GetUserId()
        {
            return IOSStringToUnityString(Externs.GADUMAdColonyAppOptionsGetUserID());
        }

        public void SetTestMode(bool isTestMode)
        {
            Externs.GADUMAdColonyAppOptionsSetTestMode(isTestMode);
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
