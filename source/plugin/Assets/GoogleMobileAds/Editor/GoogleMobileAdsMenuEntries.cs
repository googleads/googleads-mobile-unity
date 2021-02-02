// Copyright (C) 2019 Google LLC
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

using UnityEditor;
using UnityEngine;
using GoogleMobileAds.Placement;

namespace GoogleMobileAds.Editor
{
    public class GoogleMobileAdsMenuEntries
    {

        [MenuItem("GameObject/Google Mobile Ads/Banner Ad", false, 11)]
        public static void NewBannerAdGameObject()
        {
            GameObject banner = new GameObject("Banner Ad");
            banner.AddComponent<BannerAdGameObject>();
        }

        [MenuItem("GameObject/Google Mobile Ads/Interstitial Ad", false, 11)]
        public static void NewInterstitialAdGameObject()
        {
            GameObject interstitial = new GameObject("Interstitial Ad");
            interstitial.AddComponent<InterstitialAdGameObject>();
        }

        [MenuItem("GameObject/Google Mobile Ads/Rewarded Ad", false, 11)]
        public static void NewRewardedAdGameObject()
        {
            GameObject rewardedAd = new GameObject("Rewarded Ad");
            rewardedAd.AddComponent<RewardedAdGameObject>();
        }

        [MenuItem("GameObject/Google Mobile Ads/Rewarded Interstitial Ad", false, 11)]
        public static void NewRewardedInterstitialAdGameObject()
        {
            GameObject rewardedAd = new GameObject("Rewarded Interstitial Ad");
            rewardedAd.AddComponent<RewardedInterstitialAdGameObject>();
        }
    }
}
