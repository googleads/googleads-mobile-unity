// Copyright (C) 2017 Google, Inc.
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

using UnityEngine;

using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    public class MobileAds
    {
        public static class Utils {
            // Returns the device's scale.
            public static float GetDeviceScale() {
                return client.GetDeviceScale();
            }
        }
        private static readonly IMobileAdsClient client = GetMobileAdsClient();

        public static void Initialize(string appId)
        {
            client.Initialize(appId);
            MobileAdsEventExecutor.Initialize();
        }

        public static void Initialize(Action<InitializationStatus> initCompleteAction)
        {
            client.Initialize(initCompleteAction);
            MobileAdsEventExecutor.Initialize();
        }

        public static void SetApplicationMuted(bool muted)
        {
            client.SetApplicationMuted(muted);
        }

        public static void SetApplicationVolume(float volume)
        {
            client.SetApplicationVolume(volume);
        }

        public static void SetiOSAppPauseOnBackground(bool pause)
        {
            client.SetiOSAppPauseOnBackground(pause);
        }

        public static TAdPlacement FindAdPlacementByName<TAdPlacement>(string name)
        {
            if (typeof(TAdPlacement).IsSubclassOf(typeof(AdPlacement)))
            {
                GameObject go = GameObject.Find(name);
                if (go != null)
                {
                    return go.GetComponent<TAdPlacement>();
                }
            }
            return default(TAdPlacement);
        }

        public static void LoadBannerAd(string placementName)
        {
            BannerAdPlacement banner = FindAdPlacementByName<BannerAdPlacement>(placementName);
            if (banner != null)
            {
                banner.Load();
            }
            else
            {
                throw new ArgumentException("No Banner Ad Placement found with the name: " + placementName);
            }
        }

        public static void LoadInterstitialAd(string placementName)
        {
            InterstitialAdPlacement interstitial = FindAdPlacementByName<InterstitialAdPlacement>(placementName);
            if (interstitial != null)
            {
                interstitial.Load();
            }
            else
            {
                throw new ArgumentException("No Interstitial Ad Placement found with the name: " + placementName);
            }
        }

        public static void ShowInterstitialAd(string placementName)
        {
            InterstitialAdPlacement interstitial = FindAdPlacementByName<InterstitialAdPlacement>(placementName);
            if (interstitial != null)
            {
                if (interstitial.IsLoaded)
                {
                    interstitial.Show();
                }
            }
            else
            {
                throw new ArgumentException("No Interstitial Ad Placement found with the name: " + placementName);
            }
        }

        public static void LoadRewardedAd(string placementName)
        {
            RewardedAdPlacement rewarded = FindAdPlacementByName<RewardedAdPlacement>(placementName);
            if (rewarded != null)
            {
                rewarded.Load();
            }
            else
            {
                throw new ArgumentException("No Rewarded Ad Placement found with the name: " + placementName);
            }
        }

        public static bool IsRewardedAdLoaded(string placementName)
        {
            RewardedAdPlacement rewarded = FindAdPlacementByName<RewardedAdPlacement>(placementName);
            if (rewarded != null)
            {
                return rewarded.IsLoaded;
            }
            else
            {
                throw new ArgumentException("No Rewarded Ad Placement found with the name: " + placementName);
            }
        }

        public static void ShowRewardedAd(string placementName)
        {
            RewardedAdPlacement rewarded = FindAdPlacementByName<RewardedAdPlacement>(placementName);
            if (rewarded != null)
            {
                if (rewarded.IsLoaded)
                {
                    rewarded.Show();
                }
            }
            else
            {
                throw new ArgumentException("No Rewarded Ad Placement found with the name: " + placementName);
            }
        }

        private static IMobileAdsClient GetMobileAdsClient()
        {
          return GoogleMobileAdsClientFactory.MobileAdsInstance();
        }
    }
}
