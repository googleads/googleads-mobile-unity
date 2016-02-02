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
using UnityEngine;

using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
    internal class DummyClient : IBannerClient, IInterstitialClient, IRewardBasedVideoAdClient
    {
        public event EventHandler<EventArgs> OnAdLoaded = delegate {};
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad = delegate {};
        public event EventHandler<EventArgs> OnAdOpening = delegate {};
        public event EventHandler<EventArgs> OnAdStarted = delegate {};
        public event EventHandler<EventArgs> OnAdClosed = delegate {};
        public event EventHandler<Reward> OnAdRewarded = delegate {};
        public event EventHandler<EventArgs> OnAdLeavingApplication = delegate {};

        public String UserId
        {
            get
            {
                Debug.Log("Get userId");
                return "UserId";
            }
            set { Debug.Log("Set userId"); }
        }

        public DummyClient()
        {
            Debug.Log("Created DummyClient");
        }

        public void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position)
        {
            Debug.Log("Dummy CreateBannerView");
        }

        public void LoadAd(AdRequest request)
        {
            Debug.Log("Dummy LoadAd");
        }

        public void ShowBannerView()
        {
            Debug.Log("Dummy ShowBannerView");
        }

        public void HideBannerView()
        {
            Debug.Log("Dummy HideBannerView");
        }

        public void DestroyBannerView()
        {
            Debug.Log("Dummy DestroyBannerView");
        }

        public void CreateInterstitialAd(string adUnitId)
        {
            Debug.Log("Dummy CreateIntersitialAd");
        }

        public bool IsLoaded()
        {
            Debug.Log("Dummy IsLoaded");
            return true;
        }

        public void ShowInterstitial()
        {
            Debug.Log("Dummy ShowInterstitial");
        }

        public void DestroyInterstitial()
        {
            Debug.Log("Dummy DestroyInterstitial");
        }

        public void CreateRewardBasedVideoAd()
        {
            Debug.Log("Dummy CreateRewardBasedVideoAd");
        }

        public void SetUserId(string userId)
        {
            Debug.Log("Dummy LoadAd");
        }

        public void LoadAd(AdRequest request, string adUnitId)
        {
            Debug.Log("Dummy LoadAd");
        }

        public void DestroyRewardBasedVideoAd()
        {
            Debug.Log("Dummy DestroyRewardBasedVideoAd");
        }

        public void ShowRewardBasedVideoAd()
        {
            Debug.Log("Dummy ShowRewardBasedVideoAd");
        }

        public void SetDefaultInAppPurchaseProcessor(IDefaultInAppPurchaseProcessor processor)
        {
            Debug.Log("Dummy SetDefaultInAppPurchaseProcessor");
        }

        public void SetCustomInAppPurchaseProcessor(ICustomInAppPurchaseProcessor processor)
        {
            Debug.Log("Dummy SetCustomInAppPurchaseProcessor");
        }
    }
}
