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

#if UNITY_ANDROID

using UnityEngine;
using GoogleMobileAds.Mediation.AdColony.Api;
using GoogleMobileAds.Mediation.AdColony.Common;

namespace GoogleMobileAds.Mediation.AdColony.Android
{
    public class AdColonyAppOptionsClient : IAdColonyAppOptionsClient
    {
        private static AdColonyAppOptionsClient instance = new AdColonyAppOptionsClient();
        private AdColonyAppOptionsClient() { }

        private const string adapterClassName =
                "com.google.ads.mediation.adcolony.AdColonyMediationAdapter";
        private const string adColonyAppOptionsClassName = "com.adcolony.sdk.AdColonyAppOptions";

        public static AdColonyAppOptionsClient Instance
        {
            get {
                return instance;
            }
        }

        public void SetPrivacyFrameworkRequired(AdColonyPrivacyFramework privacyFramework,
                                                bool isRequired) {
            string adColonyPrivacyFrameworkString =
                    GetAdColonyPrivacyFrameworkString(privacyFramework);
            if (string.IsNullOrEmpty(adColonyPrivacyFrameworkString)) {
                Debug.Log("[AdColony Plugin] Error: Invalid AdColonyPrivacyFramework value " +
                        "provided to the AdColony adapter: " + privacyFramework);
                return;
            }

            AndroidJavaClass adColonyAdapter = new AndroidJavaClass(adapterClassName);
            AndroidJavaObject appOptions =
                    adColonyAdapter.CallStatic<AndroidJavaObject>("getAppOptions");
            appOptions.Call<AndroidJavaObject>("setPrivacyFrameworkRequired",
                    adColonyPrivacyFrameworkString, isRequired);
        }

        public bool GetPrivacyFrameworkRequired(AdColonyPrivacyFramework privacyFramework)
        {
            string adColonyPrivacyFrameworkString =
                    GetAdColonyPrivacyFrameworkString(privacyFramework);
            if (string.IsNullOrEmpty(adColonyPrivacyFrameworkString)) {
                Debug.Log("[AdColony Plugin] Error: Invalid AdColonyPrivacyFramework value " +
                        "provided to the AdColony adapter: " + privacyFramework);
                return false;
            }

            AndroidJavaClass adColonyAdapter = new AndroidJavaClass(adapterClassName);
            AndroidJavaObject appOptions =
                    adColonyAdapter.CallStatic<AndroidJavaObject>("getAppOptions");
            return appOptions.Call<bool>("getPrivacyFrameworkRequired",
                    adColonyPrivacyFrameworkString);
        }

        public void SetPrivacyConsentString(AdColonyPrivacyFramework privacyFramework,
                                            string consentString)
        {
            string adColonyPrivacyFrameworkString =
                    GetAdColonyPrivacyFrameworkString(privacyFramework);
            if (string.IsNullOrEmpty(adColonyPrivacyFrameworkString)) {
                Debug.Log("[AdColony Plugin] Error: Invalid AdColonyPrivacyFramework value " +
                        "provided to the AdColony adapter: " + privacyFramework);
                return;
            }

            AndroidJavaClass adColonyAdapter = new AndroidJavaClass(adapterClassName);
            AndroidJavaObject appOptions =
                    adColonyAdapter.CallStatic<AndroidJavaObject>("getAppOptions");
            appOptions.Call<AndroidJavaObject>("setPrivacyConsentString",
                    adColonyPrivacyFrameworkString, consentString);
        }

        public string GetPrivacyConsentString(AdColonyPrivacyFramework privacyFramework)
        {
            string adColonyPrivacyFrameworkString =
                    GetAdColonyPrivacyFrameworkString(privacyFramework);
            if (string.IsNullOrEmpty(adColonyPrivacyFrameworkString)) {
                Debug.Log("[AdColony Plugin] Error: Invalid AdColonyPrivacyFramework value " +
                        "provided to the AdColony adapter: " + privacyFramework);
                return "";
            }

            AndroidJavaClass adColonyAdapter = new AndroidJavaClass(adapterClassName);
            AndroidJavaObject appOptions =
                    adColonyAdapter.CallStatic<AndroidJavaObject>("getAppOptions");
            return appOptions.Call<string>("getPrivacyConsentString",
                    adColonyPrivacyFrameworkString);
        }

        public void SetUserId(string userId)
        {
            AndroidJavaClass adColonyAdapter = new AndroidJavaClass(adapterClassName);
            AndroidJavaObject appOptions =
                    adColonyAdapter.CallStatic<AndroidJavaObject>("getAppOptions");

            appOptions.Call<AndroidJavaObject>("setUserID", userId);
        }

        public string GetUserId()
        {
            AndroidJavaClass adColonyAdapter = new AndroidJavaClass(adapterClassName);
            AndroidJavaObject appOptions =
                    adColonyAdapter.CallStatic<AndroidJavaObject>("getAppOptions");

            return appOptions.Call<string>("getUserID");
        }

        public void SetTestMode(bool isTestMode)
        {
            AndroidJavaClass adColonyAdapter = new AndroidJavaClass(adapterClassName);
            AndroidJavaObject appOptions =
                    adColonyAdapter.CallStatic<AndroidJavaObject>("getAppOptions");

            appOptions.Call<AndroidJavaObject>("setTestModeEnabled", isTestMode);
        }

        public bool IsTestMode()
        {
            AndroidJavaClass adColonyAdapter = new AndroidJavaClass(adapterClassName);
            AndroidJavaObject appOptions =
                    adColonyAdapter.CallStatic<AndroidJavaObject>("getAppOptions");

            return appOptions.Call<bool>("getTestModeEnabled");
        }

        private string GetAdColonyPrivacyFrameworkString(AdColonyPrivacyFramework privacyFramework)
        {
            AndroidJavaClass adColonyAppOptions =
                    new AndroidJavaClass(adColonyAppOptionsClassName);

            switch (privacyFramework)
            {
                case AdColonyPrivacyFramework.GDPR:
                    return adColonyAppOptions.GetStatic<string>("GDPR");
                case AdColonyPrivacyFramework.CCPA:
                    return adColonyAppOptions.GetStatic<string>("CCPA");
                default:
                    return "";
            }
        }
    }
}

#endif
