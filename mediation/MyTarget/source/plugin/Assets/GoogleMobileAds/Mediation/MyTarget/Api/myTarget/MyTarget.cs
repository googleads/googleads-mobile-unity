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

using UnityEngine;

using GoogleMobileAds.Mediation.MyTarget;
using GoogleMobileAds.Mediation.MyTarget.Common;

namespace GoogleMobileAds.Mediation.MyTarget.Api
{
    public class MyTarget
    {
        internal static readonly IMyTargetClient client =
                MyTargetClientFactory.CreateMyTargetClient();

        public static void SetUserConsent(bool userConsent)
        {
            client.SetUserConsent(userConsent);
        }

        public static bool GetUserConsent()
        {
            return client.GetUserConsent();
        }

        public static void SetUserAgeRestricted(bool userAgeRestricted)
        {
            client.SetUserAgeRestricted(userAgeRestricted);
        }

        public static bool IsUserAgeRestricted()
        {
            return client.IsUserAgeRestricted();
        }

        public static void SetCCPAUserConsent(bool ccpaUserConsent)
        {
            client.SetCCPAUserConsent(ccpaUserConsent);
        }

        public static bool GetCCPAUserConsent()
        {
            return client.GetCCPAUserConsent();
        }
    }
}

namespace GoogleMobileAds.Api.Mediation.MyTarget
{
    [System.Obsolete("Use `GoogleMobileAds.Mediation.MyTarget.Api.MyTarget` instead.")]
    public class MyTarget
    {
        public static void SetUserConsent(bool userConsent)
        {
            GoogleMobileAds.Mediation.MyTarget.Api.MyTarget.SetUserConsent(userConsent);
        }

        public static bool GetUserConsent()
        {
            return GoogleMobileAds.Mediation.MyTarget.Api.MyTarget.GetUserConsent();
        }

        public static void SetUserAgeRestricted(bool userAgeRestricted)
        {
            GoogleMobileAds.Mediation.MyTarget.Api.MyTarget
                    .SetUserAgeRestricted(userAgeRestricted);
        }

        public static bool IsUserAgeRestricted()
        {
            return GoogleMobileAds.Mediation.MyTarget.Api.MyTarget.IsUserAgeRestricted();
        }

        public static void SetCCPAUserConsent(bool ccpaUserConsent)
        {
            GoogleMobileAds.Mediation.MyTarget.Api.MyTarget.SetCCPAUserConsent(ccpaUserConsent);
        }

        public static bool GetCCPAUserConsent()
        {
            return GoogleMobileAds.Mediation.MyTarget.Api.MyTarget.GetCCPAUserConsent();
        }
    }
}
