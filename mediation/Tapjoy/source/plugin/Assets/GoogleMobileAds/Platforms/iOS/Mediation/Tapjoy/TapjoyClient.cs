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

#if UNITY_IPHONE || UNITY_IOS

using UnityEngine;

using GoogleMobileAds.Common.Mediation.Tapjoy;

namespace GoogleMobileAds.iOS.Mediation.Tapjoy
{
    public class TapjoyClient : ITapjoyClient
    {
        private static TapjoyClient instance = new TapjoyClient();
        private TapjoyClient() {}

        public static TapjoyClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetUserConsent(string consentString)
        {
            MonoBehaviour.print ("Calling '[Tapjoy setUserConsent:]' with argument: '" + consentString + "'");
            Externs.GADUMTapjoySetUserConsent (consentString);
        }

        public void SubjectToGDPR(bool gdprApplicability)
        {
            string parameterString = (gdprApplicability == true ? "YES" : "NO");
            MonoBehaviour.print ("Calling '[Tapjoy subjectToGDPR:]' with argument: " + parameterString);
            Externs.GADUMTapjoySubjectToGDPR (gdprApplicability);
        }
    }
}

#endif
