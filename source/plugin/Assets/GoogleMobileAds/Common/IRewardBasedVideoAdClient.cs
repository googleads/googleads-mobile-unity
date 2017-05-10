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

using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
    public interface IRewardBasedVideoAdClient
    {
        // Ad event fired when the reward based video ad has been received.
        event EventHandler<EventArgs> OnAdLoaded;
        // Ad event fired when the reward based video ad has failed to load.
        event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;
        // Ad event fired when the reward based video ad is opened.
        event EventHandler<EventArgs> OnAdOpening;
        // Ad event fired when the reward based video ad has started playing.
        event EventHandler<EventArgs> OnAdStarted;
        // Ad event fired when the reward based video ad has rewarded the user.
        event EventHandler<Reward> OnAdRewarded;
        // Ad event fired when the reward based video ad is closed.
        event EventHandler<EventArgs> OnAdClosed;
        // Ad event fired when the reward based video ad is leaving the application.
        event EventHandler<EventArgs> OnAdLeavingApplication;

        // Creates a reward based video ad and adds it to the view hierarchy.
        void CreateRewardBasedVideoAd();

        // Requests a new ad for the reward based video ad.
        void LoadAd(AdRequest request, string adUnitId);

        // Determines whether the reward based video has loaded.
        bool IsLoaded();

        // Shows the reward based video ad on the screen.
        void ShowRewardBasedVideoAd();
    }
}
