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

namespace GoogleMobileAds.Common {
    public interface IInterstitialClient
    {
        // Ad event fired when the interstitial ad has been received.
        event EventHandler<EventArgs> OnAdLoaded;
        // Ad event fired when the interstitial ad has failed to load.
        event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;
        // Ad event fired when the interstitial ad is opened.
        event EventHandler<EventArgs> OnAdOpening;
        // Ad event fired when the interstitial ad is closed.
        event EventHandler<EventArgs> OnAdClosed;
        // Ad event fired when the interstitial ad is leaving the application.
        event EventHandler<EventArgs> OnAdLeavingApplication;

        // Creates an InterstitialAd.
        void CreateInterstitialAd(string adUnitId);

        // Loads a new interstitial request.
        void LoadAd(AdRequest request);

        // Determines whether the interstitial has loaded.
        bool IsLoaded();

        // Shows the InterstitialAd.
        void ShowInterstitial();

        // Destroys an InterstitialAd.
        void DestroyInterstitial();

        // Returns the mediation adapter class name.
        string MediationAdapterClassName();
    }
}
