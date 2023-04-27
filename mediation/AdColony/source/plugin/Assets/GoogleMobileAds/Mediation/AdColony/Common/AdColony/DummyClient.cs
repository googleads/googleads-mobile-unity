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

using System.Reflection;
using UnityEngine;
using GoogleMobileAds.Mediation.AdColony.Api;

namespace GoogleMobileAds.Mediation.AdColony.Common
{
    public class DummyClient : IAdColonyAppOptionsClient
    {
        public DummyClient()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void SetPrivacyFrameworkRequired(AdColonyPrivacyFramework privacyFramework,
                                                bool isRequired)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public bool GetPrivacyFrameworkRequired(AdColonyPrivacyFramework privacyFramework)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            return false;
        }

        public void SetPrivacyConsentString(AdColonyPrivacyFramework privacyFramework,
                                            string consentString)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public string GetPrivacyConsentString(AdColonyPrivacyFramework privacyFramework)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            return "";
        }

        public void SetUserId(string userId)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public string GetUserId()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            return "";
        }

        public void SetTestMode(bool isTestMode)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public bool IsTestMode()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            return false;
        }
    }
}
