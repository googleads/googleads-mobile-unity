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

using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    public class RewardBasedVideoAd
    {
        private IRewardBasedVideoAdClient client;
        private static RewardBasedVideoAd instance;

        // These are the ad callback events that can be hooked into.
        public event EventHandler<EventArgs> OnAdLoaded = delegate {};
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad = delegate {};
        public event EventHandler<EventArgs> OnAdOpening = delegate {};
        public event EventHandler<EventArgs> OnAdStarted = delegate {};
        public event EventHandler<EventArgs> OnAdClosed = delegate {};
        public event EventHandler<Reward> OnAdRewarded = delegate {};
        public event EventHandler<EventArgs> OnAdLeavingApplication = delegate {};

        public static RewardBasedVideoAd Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RewardBasedVideoAd();
                }
                return instance;
            }
        }

        // Creates a Singleton RewardBasedVideoAd.
        private RewardBasedVideoAd()
        {
            client = GoogleMobileAdsClientFactory.BuildRewardBasedVideoAdClient();
            client.CreateRewardBasedVideoAd();

            client.OnAdLoaded += delegate(object sender, EventArgs args)
            {
                OnAdLoaded(this, args);
            };

            client.OnAdFailedToLoad += delegate(object sender, AdFailedToLoadEventArgs args)
            {
                OnAdFailedToLoad(this, args);
            };

            client.OnAdOpening += delegate(object sender, EventArgs args)
            {
                OnAdOpening(this, args);
            };

            client.OnAdStarted += delegate(object sender, EventArgs args)
            {
                OnAdStarted(this, args);
            };

            client.OnAdRewarded += delegate(object sender, Reward args)
            {
                OnAdRewarded(this, args);
            };

            client.OnAdClosed += delegate(object sender, EventArgs args)
            {
                OnAdClosed(this, args);
            };

            client.OnAdLeavingApplication += delegate(object sender, EventArgs args)
            {
                OnAdLeavingApplication(this, args);
            };
        }

        // Loads a new reward based video ad request
        public void LoadAd(AdRequest request, string adUnitId)
        {
            client.LoadAd(request, adUnitId);
        }

        // Determines whether the reward based video has loaded.
        public bool IsLoaded()
        {
            return client.IsLoaded();
        }

        // Shows the reward based video.
        public void Show()
        {
            client.ShowRewardBasedVideoAd();
        }
    }
}
