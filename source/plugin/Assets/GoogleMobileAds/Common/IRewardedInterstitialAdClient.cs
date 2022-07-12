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

using System;

using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
    /// <summary>
    /// Rewarded interstitial ads can serve without requiring the user to opt-in to viewing.
    /// At any point during the experience, the user can decide to skip the ad.
    /// </summary>
    public interface IRewardedInterstitialAdClient : IBaseFullScreenAd, IBaseRewardedAd
    {
        /// <summary>
        /// Loads an RewardedInterstitialAd.
        /// </summary>
        void LoadRewardedInterstitialAd(
            string adUnitId,
            AdRequest request,
            Action<IRewardedInterstitialAdClient, ILoadAdErrorClient> callback);
    }
}
