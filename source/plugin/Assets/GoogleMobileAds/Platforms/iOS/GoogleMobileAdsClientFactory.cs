#if UNITY_IOS
// Copyright (C) 2020 Google, Inc.
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

namespace GoogleMobileAds
{
    public class GoogleMobileAdsClientFactory
    {
        public static IBannerClient BuildBannerClient()
        {
            return new GoogleMobileAds.iOS.BannerClient();
        }

        public static IInterstitialClient BuildInterstitialClient()
        {
          return new GoogleMobileAds.iOS.InterstitialClient();
        }

        public static IRewardBasedVideoAdClient BuildRewardBasedVideoAdClient()
        {
          return new GoogleMobileAds.iOS.RewardBasedVideoAdClient();
        }

        public static IRewardedAdClient BuildRewardedAdClient()
        {
          return new GoogleMobileAds.iOS.RewardedAdClient();
        }

        public static IAdLoaderClient BuildAdLoaderClient(AdLoaderClientArgs args)
        {
          return new GoogleMobileAds.iOS.AdLoaderClient(args);
        }

        public static IMobileAdsClient MobileAdsInstance()
        {
          return GoogleMobileAds.iOS.MobileAdsClient.Instance;
        }
    }
}
#endif
