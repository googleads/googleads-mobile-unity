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

#if UNITY_IOS

using UnityEngine;
using GoogleMobileAds.Mediation.AppLovin.Common;

namespace GoogleMobileAds.Mediation.AppLovin.iOS
{
    public class AppLovinClient : IAppLovinClient
    {
        private static readonly AppLovinClient instance = new AppLovinClient();
        private AppLovinClient() {}

        public static AppLovinClient Instance
        {
            get {
                return instance;
            }
        }

        public void Initialize()
        {
            Externs.GADUMInitializeAppLovin();
        }

        public void SetHasUserConsent(bool hasUserConsent)
        {
            Externs.GADUMAppLovinSetHasUserConsent(hasUserConsent);
        }

        public void SetIsAgeRestrictedUser(bool isAgeRestrictedUser)
        {
            Externs.GADUMAppLovinSetIsAgeRestrictedUser(isAgeRestrictedUser);
        }

        public void SetDoNotSell(bool doNotSell)
        {
            Externs.GADUMAppLovinSetDoNotSell(doNotSell);
        }
    }
}

#endif
