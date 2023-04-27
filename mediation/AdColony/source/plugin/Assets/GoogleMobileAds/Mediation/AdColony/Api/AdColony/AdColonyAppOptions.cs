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

using GoogleMobileAds.Mediation.AdColony;
using GoogleMobileAds.Mediation.AdColony.Common;

namespace GoogleMobileAds.Mediation.AdColony.Api
{
    public enum AdColonyPrivacyFramework
    {
        GDPR = 0,
        CCPA = 1,
    }

    public class AdColonyAppOptions
    {
        internal static readonly IAdColonyAppOptionsClient client = GetAdColonyAppOptionsClient();

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

namespace GoogleMobileAds.Api.Mediation.AdColony
{
    [System.Obsolete("Use `GoogleMobileAds.Mediation.AdColony.Api.AdColonyAppOptions` instead.")]
    public class AdColonyAppOptions
    {
        public static void SetPrivacyFrameworkRequired(
                GoogleMobileAds.Mediation.AdColony.Api.AdColonyPrivacyFramework privacyFramework,
                bool isRequired)
        {
            GoogleMobileAds.Mediation.AdColony.Api.AdColonyAppOptions
                    .SetPrivacyFrameworkRequired(privacyFramework, isRequired);
        }

        public static bool GetPrivacyFrameworkRequired(
                GoogleMobileAds.Mediation.AdColony.Api.AdColonyPrivacyFramework privacyFramework)
        {
            return GoogleMobileAds.Mediation.AdColony.Api.AdColonyAppOptions
                    .GetPrivacyFrameworkRequired(privacyFramework);
        }

        public static void SetPrivacyConsentString(
                GoogleMobileAds.Mediation.AdColony.Api.AdColonyPrivacyFramework privacyFramework,
                string consentString)
        {
            GoogleMobileAds.Mediation.AdColony.Api.AdColonyAppOptions
                    .SetPrivacyConsentString(privacyFramework, consentString);
        }

        public static string GetPrivacyConsentString(
                GoogleMobileAds.Mediation.AdColony.Api.AdColonyPrivacyFramework privacyFramework)
        {
            return GoogleMobileAds.Mediation.AdColony.Api.AdColonyAppOptions
                    .GetPrivacyConsentString(privacyFramework);
        }

        public static void SetUserId(string userId)
        {
            GoogleMobileAds.Mediation.AdColony.Api.AdColonyAppOptions.SetUserId(userId);
        }

        public static string GetUserId()
        {
            return GoogleMobileAds.Mediation.AdColony.Api.AdColonyAppOptions.GetUserId();
        }

        public static void SetTestMode(bool isTestMode)
        {
            GoogleMobileAds.Mediation.AdColony.Api.AdColonyAppOptions.SetTestMode(isTestMode);
        }

        public static bool IsTestMode()
        {
            return GoogleMobileAds.Mediation.AdColony.Api.AdColonyAppOptions.IsTestMode();
        }
    }
}
