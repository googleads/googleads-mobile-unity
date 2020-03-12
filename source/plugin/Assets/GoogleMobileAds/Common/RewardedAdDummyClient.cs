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

using GoogleMobileAds.Api;
using UnityEngine;

namespace GoogleMobileAds.Common
{
    public class RewardedAdDummyClient : IRewardedAdClient
    {
        public RewardedAdDummyClient()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        // Disable warnings for unused dummy ad events.
#pragma warning disable 67

        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<AdErrorEventArgs> OnAdFailedToLoad;

        public event EventHandler<AdErrorEventArgs> OnAdFailedToShow;

        public event EventHandler<EventArgs> OnAdOpening;

        public event EventHandler<EventArgs> OnAdClosed;

        public event EventHandler<Reward> OnUserEarnedReward;

        public event EventHandler<AdValueEventArgs> OnPaidEvent;


#pragma warning restore 67

        public void CreateRewardedAd(string adUnitId)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void LoadAd(AdRequest request)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);

            if (OnAdLoaded != null)
            {
                OnAdLoaded(this, EventArgs.Empty);
            }
        }

        public bool IsLoaded()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            return true;
        }

        public void Show()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public string MediationAdapterClassName()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            return null;
        }

        public Reward GetRewardItem()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            return null;
        }

        public void SetServerSideVerificationOptions(ServerSideVerificationOptions serverSideVerificationOptions)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }
    }
}
