// Copyright 2017 Google LLC
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

using System;
using GoogleMobileAds.Mediation.AppLovin.Common;
using GoogleMobileAds.Mediation.AppLovin;

namespace GoogleMobileAds.Mediation.AppLovin.Api
{
    public class AppLovin
    {
        internal static readonly IAppLovinClient client = GetAppLovinClient();

        public static void Initialize()
        {
            client.Initialize();
        }

        public static void SetHasUserConsent(bool hasUserConsent)
        {
            client.SetHasUserConsent(hasUserConsent);
        }

        public static void SetIsAgeRestrictedUser(bool isAgeRestrictedUser)
        {
            client.SetIsAgeRestrictedUser(isAgeRestrictedUser);
        }

        public static void SetDoNotSell(bool doNotSell)
        {
            client.SetDoNotSell(doNotSell);
        }

        internal static IAppLovinClient GetAppLovinClient()
        {
            return AppLovinClientFactory.AppLovinInstance();
        }
    }
}


namespace GoogleMobileAds.Api.Mediation.AppLovin
{
    [System.Obsolete("Use `GoogleMobileAds.Mediation.AppLovin.Api.AppLovin` instead.")]
    public class AppLovin
    {
        public static void Initialize()
        {
            GoogleMobileAds.Mediation.AppLovin.Api.AppLovin.Initialize();
        }

        public static void SetHasUserConsent(bool hasUserConsent)
        {
            GoogleMobileAds.Mediation.AppLovin.Api.AppLovin.SetHasUserConsent(hasUserConsent);
        }

        public static void SetIsAgeRestrictedUser(bool isAgeRestrictedUser)
        {
            GoogleMobileAds.Mediation.AppLovin.Api.
                AppLovin.SetIsAgeRestrictedUser(isAgeRestrictedUser);
        }

        public static void SetDoNotSell(bool doNotSell)
        {
            GoogleMobileAds.Mediation.AppLovin.Api.AppLovin.SetDoNotSell(doNotSell);
        }

        private static IAppLovinClient GetAppLovinClient()
        {
            return GoogleMobileAds.Mediation.AppLovin.Api.AppLovin.GetAppLovinClient();
        }
    }
}
