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

using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
    public interface IRewardedInterstitialAdClient
    {
        // Ad event fired when the rewarded interstitial ad has been received.
        event EventHandler<EventArgs> OnAdLoaded;
        // Ad event fired when the rewarded interstitial ad has failed to load.
        event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;
        // Ad event fired when the rewarded interstitial ad is estimated to have earned money.
        event EventHandler<AdValueEventArgs> OnPaidEvent;
        // Ad event fired when the rewarded interstitial ad has rewarded the user.
        event EventHandler<Reward> OnUserEarnedReward;
        // Ad event fired when the rewarded interstitial ad has failed to present the full screen content.
        event EventHandler<AdErrorClientEventArgs> OnAdFailedToPresentFullScreenContent;
        // Ad event fired when the rewarded interstitial ad has presented the full screen content.
        event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;
        // Ad event fired when the rewarded interstitial ad has dismissed the full screen content.
        event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;
        // Ad event fired when the rewarded interstitial ad has recorded an impression.
        event EventHandler<EventArgs> OnAdDidRecordImpression;
        // Ad event fired when an ad is clicked.
        event Action OnAdClicked;

        // Creates a rewarded interstitial ad.
        void CreateRewardedInterstitialAd();

        // Loads a rewarded interstitial ad.
        void LoadAd(string adUnitID, AdRequest request);

        // Returns the reward item for the loaded rewarded interstitial ad.
        Reward GetRewardItem();

        // Shows the rewarded interstitial ad on the screen.
        void Show();

        // Sets the server side verification options
        void SetServerSideVerificationOptions(ServerSideVerificationOptions serverSideVerificationOptions);

        // Returns ad request Response info client.
        IResponseInfoClient GetResponseInfoClient();

        // Destroys the rewarded interstitial ad.
        void DestroyRewardedInterstitialAd();
    }
}
