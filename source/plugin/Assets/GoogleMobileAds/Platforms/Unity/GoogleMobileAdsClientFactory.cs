// Copyright (C) 2020 Google LLC
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
using GoogleMobileAds;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.Scripting;

namespace GoogleMobileAds
{
    [Preserve]
    public class GoogleMobileAdsClientFactory: IClientFactory
    {
        public IAppStateEventClient BuildAppStateEventClient()
        {
            return GoogleMobileAds.Common.AppStateEventClient.Instance;
        }

        public IAppOpenAdClient BuildAppOpenAdClient()
        {
            return new GoogleMobileAds.Unity.AppOpenAdClient();
        }

        public IBannerClient BuildBannerClient()
        {
            return new GoogleMobileAds.Unity.BannerClient();
        }

        public IAdManagerBannerClient BuildAdManagerBannerClient()
        {
            return new GoogleMobileAds.Unity.AdManagerBannerClient();
        }

        public IInterstitialClient BuildInterstitialClient()
        {
            return new GoogleMobileAds.Unity.InterstitialClient();
        }

        public IAdManagerInterstitialClient BuildAdManagerInterstitialClient()
        {
            return new GoogleMobileAds.Unity.AdManagerInterstitialClient();
        }

        public IRewardedAdClient BuildRewardedAdClient()
        {
            return new GoogleMobileAds.Unity.RewardedAdClient();
        }

        public IRewardedInterstitialAdClient BuildRewardedInterstitialAdClient()
        {
            return new GoogleMobileAds.Unity.RewardedInterstitialAdClient();
        }

        public IApplicationPreferencesClient ApplicationPreferencesInstance()
        {
            return new GoogleMobileAds.Unity.ApplicationPreferencesClient();
        }

        public IMobileAdsClient MobileAdsInstance()
        {
            return new GoogleMobileAds.Unity.MobileAdsClient();
        }
    }
}
