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
using GoogleMobileAds.Common;

namespace GoogleMobileAds
{
    public interface IClientFactory
    {
        IAppStateEventClient BuildAppStateEventClient();

        IAppOpenAdClient BuildAppOpenAdClient();

        IAppOpenAdPreloaderClient BuildAppOpenAdPreloaderClient();

        IBannerClient BuildBannerClient();

        IAdManagerBannerClient BuildAdManagerBannerClient();

        IInterstitialClient BuildInterstitialClient();

        IInterstitialAdPreloaderClient BuildInterstitialAdPreloaderClient();

        IAdManagerInterstitialClient BuildAdManagerInterstitialClient();

        IRewardedAdClient BuildRewardedAdClient();

        IRewardedAdPreloaderClient BuildRewardedAdPreloaderClient();

        IRewardedInterstitialAdClient BuildRewardedInterstitialAdClient();

        INativeOverlayAdClient BuildNativeOverlayAdClient();

        IApplicationPreferencesClient ApplicationPreferencesInstance();

        IMobileAdsClient MobileAdsInstance();
    }
}
