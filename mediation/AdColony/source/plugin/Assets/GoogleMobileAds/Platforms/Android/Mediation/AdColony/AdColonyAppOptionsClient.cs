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

using GoogleMobileAds.Common.Mediation.AdColony;

namespace GoogleMobileAds.Android.Mediation.AdColony
{
    public class AdColonyAppOptionsClient : IAdColonyAppOptionsClient
    {
        private static readonly string adapterClassName = "com.google.ads.mediation.adcolony.AdColonyMediationAdapter";
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
            AndroidJavaClass adColonyAdapter = new AndroidJavaClass(adapterClassName);
            AndroidJavaObject appOptions = adColonyAdapter.CallStatic<AndroidJavaObject>("getAppOptions");

            appOptions.Call<AndroidJavaObject>("setGDPRConsentString", consentString);
        }

        public void SetGDPRRequired(bool gdprRequired)
        {
            AndroidJavaClass adColonyAdapter = new AndroidJavaClass(adapterClassName);
            AndroidJavaObject appOptions = adColonyAdapter.CallStatic<AndroidJavaObject>("getAppOptions");

            appOptions.Call<AndroidJavaObject>("setGDPRRequired", gdprRequired);
        }

        public void SetUserId(string userId)
        {
            AndroidJavaClass adColonyAdapter = new AndroidJavaClass(adapterClassName);
            AndroidJavaObject appOptions = adColonyAdapter.CallStatic<AndroidJavaObject>("getAppOptions");

            appOptions.Call<AndroidJavaObject>("setUserID", userId);
        }

        public void SetTestMode(bool isTestMode)
        {
            AndroidJavaClass adColonyAdapter = new AndroidJavaClass(adapterClassName);
            AndroidJavaObject appOptions = adColonyAdapter.CallStatic<AndroidJavaObject>("getAppOptions");

            appOptions.Call<AndroidJavaObject>("setTestModeEnabled", isTestMode);
        }

        public string GetGDPRConsentString()
        {
            AndroidJavaClass adColonyAdapter = new AndroidJavaClass(adapterClassName);
            AndroidJavaObject appOptions = adColonyAdapter.CallStatic<AndroidJavaObject>("getAppOptions");

            return appOptions.Call<string>("getGDPRConsentString");
        }

        public bool IsGDPRRequired()
        {
            AndroidJavaClass adColonyAdapter = new AndroidJavaClass(adapterClassName);
            AndroidJavaObject appOptions = adColonyAdapter.CallStatic<AndroidJavaObject>("getAppOptions");

            return appOptions.Call<bool>("getGDPRRequired");
        }

        public string GetUserId()
        {
            AndroidJavaClass adColonyAdapter = new AndroidJavaClass(adapterClassName);
            AndroidJavaObject appOptions = adColonyAdapter.CallStatic<AndroidJavaObject>("getAppOptions");

            return appOptions.Call<string>("getUserID");
        }

        public bool IsTestMode()
        {
            AndroidJavaClass adColonyAdapter = new AndroidJavaClass(adapterClassName);
            AndroidJavaObject appOptions = adColonyAdapter.CallStatic<AndroidJavaObject>("getAppOptions");

            return appOptions.Call<bool>("getTestModeEnabled");
        }
    }
}

#endif
