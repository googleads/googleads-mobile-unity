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

namespace GoogleMobileAds.Mediation.DTExchange.Common
{
    public class DummyClient : IDTExchangeClient
    {
        public DummyClient()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void SetGDPRConsent(bool consent)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void SetGDPRConsentString(string consentString)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void ClearGDPRConsentData()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void SetCCPAString(string ccpaString)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void ClearCCPAString()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }
    }
}
