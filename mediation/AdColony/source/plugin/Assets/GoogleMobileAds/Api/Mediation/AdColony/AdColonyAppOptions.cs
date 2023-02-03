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

using GoogleMobileAds.Common.Mediation.AdColony;
using GoogleMobileAds.Mediation;

namespace GoogleMobileAds.Api.Mediation.AdColony
{
    public enum AdColonyPrivacyFramework
    {
        GDPR = 0,
        CCPA = 1,
    }

    public class AdColonyAppOptions
    {
        public static readonly IAdColonyAppOptionsClient client = GetAdColonyAppOptionsClient();

        private static IAdColonyAppOptionsClient GetAdColonyAppOptionsClient()
        {
            return AdColonyAppOptionsClientFactory.getAdColonyAppOptionsInstance();
        }

        public static void SetPrivacyFrameworkRequired(AdColonyPrivacyFramework privacyFramework,
                                                       bool isRequired)
        {
            client.SetPrivacyFrameworkRequired(privacyFramework, isRequired);
        }

        public static bool GetPrivacyFrameworkRequired(AdColonyPrivacyFramework privacyFramework)
        {
            return client.GetPrivacyFrameworkRequired(privacyFramework);
        }

        public static void SetPrivacyConsentString(AdColonyPrivacyFramework privacyFramework,
                                                   string consentString)
        {
            client.SetPrivacyConsentString(privacyFramework, consentString);
        }

        public static string GetPrivacyConsentString(AdColonyPrivacyFramework privacyFramework)
        {
            return client.GetPrivacyConsentString(privacyFramework);
        }

        public static void SetUserId(string userId)
        {
            client.SetUserId(userId);
        }

        public static string GetUserId()
        {
            return client.GetUserId();
        }

        public static void SetTestMode(bool isTestMode)
        {
            client.SetTestMode(isTestMode);
        }

        public static bool IsTestMode()
        {
            return client.IsTestMode();
        }
    }
}
