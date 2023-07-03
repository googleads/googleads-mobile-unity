#if !UNITY_IOS
// Copyright (C) 2015 Google, Inc.
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
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds
{
    [Preserve]
    public class GoogleMobileAdsClientFactory : IClientFactory
    {
        public IAppStateEventClient BuildAppStateEventClient()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return new GoogleMobileAds.Android.AppStateEventClient();
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
            " on non-Android runtime");
        }

        public IAppOpenAdClient BuildAppOpenAdClient()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return new GoogleMobileAds.Android.AppOpenAdClient();
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
            " on non-Android runtime");
        }

        public IBannerClient BuildBannerClient()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return new GoogleMobileAds.Android.BannerClient();
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
            " on non-Android runtime");
        }

        public IAdManagerBannerClient BuildAdManagerBannerClient()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return new GoogleMobileAds.Android.AdManagerBannerClient();
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
            " on non-Android runtime");
        }

        public IInterstitialClient BuildInterstitialClient()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return new GoogleMobileAds.Android.InterstitialClient();
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
            " on non-Android runtime");
        }

        public IAdManagerInterstitialClient BuildAdManagerInterstitialClient()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return new GoogleMobileAds.Android.AdManagerInterstitialClient();
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
            " on non-Android runtime");
        }

        public IRewardedAdClient BuildRewardedAdClient()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return new GoogleMobileAds.Android.RewardedAdClient();
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
            " on non-Android runtime");
        }

        public IRewardedInterstitialAdClient BuildRewardedInterstitialAdClient()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return new GoogleMobileAds.Android.RewardedInterstitialAdClient();
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
            " on non-Android runtime");
        }

        public IApplicationPreferencesClient ApplicationPreferencesInstance()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return GoogleMobileAds.Android.ApplicationPreferencesClient.Instance;
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
            " on non-Android runtime");
        }

        public IMobileAdsClient MobileAdsInstance()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return GoogleMobileAds.Android.MobileAdsClient.Instance;
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
            " on non-Android runtime");
        }
    }
}
#endif
