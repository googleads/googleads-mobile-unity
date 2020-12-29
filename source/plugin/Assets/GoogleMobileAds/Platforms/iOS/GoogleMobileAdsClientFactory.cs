#if UNITY_IOS
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
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds {
  [Preserve]
  public class GoogleMobileAdsClientFactory : IClientFactory {
    public IBannerClient BuildBannerClient()
    {
      if (Application.platform == RuntimePlatform.IPhonePlayer)
      {
        return new GoogleMobileAds.iOS.BannerClient();
      }
      throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
                                          " on non-iOS runtime");
    }

    public IInterstitialClient BuildInterstitialClient()
    {
      if (Application.platform == RuntimePlatform.IPhonePlayer)
      {
        return new GoogleMobileAds.iOS.InterstitialClient();
      }
      throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
                                          " on non-iOS runtime");
    }

    public IRewardBasedVideoAdClient BuildRewardBasedVideoAdClient()
    {
      if (Application.platform == RuntimePlatform.IPhonePlayer)
      {
        return new GoogleMobileAds.iOS.RewardBasedVideoAdClient();
      }
      throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
                                          " on non-iOS runtime");
    }

    public IRewardedAdClient BuildRewardedAdClient()
    {
      if (Application.platform == RuntimePlatform.IPhonePlayer)
      {
        return new GoogleMobileAds.iOS.RewardedAdClient();
      }
      throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
                                          " on non-iOS runtime");
    }

    public IRewardedInterstitialAdClient BuildRewardedInterstitialAdClient()
    {
      if (Application.platform == RuntimePlatform.IPhonePlayer)
      {
        return new GoogleMobileAds.iOS.RewardedInterstitialAdClient();
      }
      throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
                                          " on non-iOS runtime");
    }

    public IMobileAdsClient MobileAdsInstance()
    {
      if (Application.platform == RuntimePlatform.IPhonePlayer)
      {
        return GoogleMobileAds.iOS.MobileAdsClient.Instance;
      }
      throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
                                          " on non-iOS runtime");
    }
  }
}
#endif
