// Copyright (C) 2018 Google, Inc.
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

using GoogleMobileAds;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    public class RewardedAd : AdvertisingBase
    {
        // These are the ad callback events that can be hooked into.
        public event EventHandler<EventArgs> OnAdLoaded;
        public event EventHandler<AdErrorEventArgs> OnAdFailedToLoad;
        public event EventHandler<AdErrorEventArgs> OnAdFailedToShow;
        public event EventHandler<EventArgs> OnAdOpening;
        public event EventHandler<EventArgs> OnAdClosed;
        public event EventHandler<Reward> OnUserEarnedReward;
        public event EventHandler<AdValueEventArgs> OnPaidEvent;

        private IRewardedAdClient client;

        public RewardedAd(string adUnitId)
        {
            client = GoogleMobileAdsClientFactory.BuildRewardedAdClient();
            client.CreateRewardedAd(adUnitId);

            client.OnAdLoaded += (sender, args) => ExecuteEvent(this, OnAdLoaded, args);
            client.OnAdFailedToLoad += (sender, args) => ExecuteEvent(this, OnAdFailedToLoad, args);
            client.OnAdFailedToShow += (sender, args) => ExecuteEvent(this, OnAdFailedToShow, args);
            client.OnAdOpening += (sender, args) => ExecuteEvent(this, OnAdOpening, args);
            client.OnUserEarnedReward += (sender, args) => ExecuteEvent(this, OnUserEarnedReward, args);
            client.OnAdClosed += (sender, args) => ExecuteEvent(this, OnAdClosed, args);
            client.OnPaidEvent += (sender, args) => ExecuteEvent(this, OnPaidEvent, args);
        }

        /// <summary>
        /// Loads a new rewarded ad.
        /// </summary>
        /// <param name="request"></param>
        public override void LoadAd(AdRequest request)
        {
            client.LoadAd(request);
        }

        /// <summary>
        /// Determines whether the rewarded ad has loaded.
        /// </summary>
        /// <returns></returns>
        public override bool IsLoaded()
        {
            return client.IsLoaded();
        }

        /// <summary>
        /// Shows the rewarded ad.
        /// </summary>
        public override void Show()
        {
            client.Show();
        }

        /// <summary>
        /// Sets the server side verification options
        /// </summary>
        /// <param name="serverSideVerificationOptions"></param>
        public void SetServerSideVerificationOptions(ServerSideVerificationOptions serverSideVerificationOptions)
        {
            client.SetServerSideVerificationOptions(serverSideVerificationOptions);
        }

        /// <summary>
        /// Returns the reward item for the loaded rewarded ad.
        /// </summary>
        /// <returns></returns>
        public Reward GetRewardItem()
        {
            if (client.IsLoaded()) {
              return client.GetRewardItem();
            }
            return null;
        }

        /// <summary>
        /// Returns the mediation adapter class name.
        /// </summary>
        /// <returns></returns>
        public string MediationAdapterClassName()
        {
            return this.client.MediationAdapterClassName();
        }
    }
}