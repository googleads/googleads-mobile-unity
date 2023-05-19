// Copyright 2015-2021 Google LLC
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
using GoogleMobileAds.Api.AdManager;

namespace GoogleMobileAds.Common
{
    public interface IInterstitialClient
    {
        // Ad event fired when the interstitial ad has been received.
        event EventHandler<EventArgs> OnAdLoaded;
        // Ad event fired when the interstitial ad has failed to load.
        event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;
        // Ad event fired when the interstitial ad is estimated to have earned money.
        event EventHandler<AdValueEventArgs> OnPaidEvent;
        // Ad event fired when the full screen content has failed to be presented.
        event EventHandler<AdErrorClientEventArgs> OnAdFailedToPresentFullScreenContent;
        // Ad event fired when the full screen content has been presented.
        event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;
        // Ad event fired when the full screen content has been dismissed.
        event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;
        // Ad event fired when an ad impression has been recorded.
        event EventHandler<EventArgs> OnAdDidRecordImpression;
        // Ad event fired when an ad has been clicked.
        event Action OnAdClicked;

        // Creates an interstitial ad.
        void CreateInterstitialAd();

        // Loads an interstitial ad.
        void LoadAd(string adUnitID, AdRequest request);

        // Shows the interstitial ad on the screen.
        void Show();

        // Returns ad request Response info client.
        IResponseInfoClient GetResponseInfoClient();

        // Destroys the interstitial ad.
        void DestroyInterstitial();
    }
}
