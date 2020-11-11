// Copyright (C) 2020 Google LLC.
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

using System.Reflection;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Unity
{
    public class RewardedAdClient : RewardingAdBaseClient, IRewardedAdClient
    {
        // Ad event fired when the rewarded ad is opened.
        public event EventHandler<EventArgs> OnAdOpening;
        // Ad event fired when the rewarded ad is closed.
        public event EventHandler<EventArgs> OnAdClosed;
        // Ad event fired when the rewarded ad has failed to load.
        public new event EventHandler<AdErrorEventArgs> OnAdFailedToLoad;
        // Ad event fired when the rewarded ad has failed to show.
        public event EventHandler<AdErrorEventArgs> OnAdFailedToShow;

        // Load a rewarded ad.
        public override void LoadAd(AdRequest request)
        {
            base.LoadAd(request);
            if (OnAdOpening != null)
            {
                OnAdDidPresentFullScreenContent += OnAdOpening;
            }
            if (OnAdClosed != null)
            {
                OnAdDidDismissFullScreenContent += OnAdClosed;
            }
            if (OnAdFailedToShow != null)
            {
                OnAdFailedToPresentFullScreenContent += OnAdFailedToShow;
            }
        }

        // Creates a rewarded ad.
        public void CreateRewardedAd(string adUnitId)
        {

        }


    }
}
