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

using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
    public interface IRewardedAdClient
    {
        // Ad event fired when the rewarded ad has been received.
        event EventHandler<EventArgs> OnAdLoaded;
        // Ad event fired when the rewarded ad has failed to load.
        event EventHandler<AdErrorEventArgs> OnAdFailedToLoad;
        // Ad event fired when the rewarded ad has failed to show.
        event EventHandler<AdErrorEventArgs> OnAdFailedToShow;
        // Ad event fired when the rewarded ad is opened.
        event EventHandler<EventArgs> OnAdOpening;
        // Ad event fired when the rewarded ad has rewarded the user.
        event EventHandler<Reward> OnUserEarnedReward;
        // Ad event fired when the rewarded ad is closed.
        event EventHandler<EventArgs> OnAdClosed;
        // Ad event fired when the rewarded ad is estimated to have earned money.
        event EventHandler<AdValueEventArgs> OnPaidEvent;

        // Creates a rewarded ad.
        void CreateRewardedAd(string adUnitId);

        // Load a rewarded ad.
        void LoadAd(AdRequest request);

        // Determines whether the rewarded ad has loaded.
        bool IsLoaded();

        // Returns the mediation adapter class name.
        string MediationAdapterClassName();

        // Returns the reward item for the loaded rewarded ad.
        Reward GetRewardItem();

        // Shows the rewarded ad on the screen.
        void Show();

        // Sets the server side verification options
        void SetServerSideVerificationOptions(ServerSideVerificationOptions serverSideVerificationOptions);
    }
}
