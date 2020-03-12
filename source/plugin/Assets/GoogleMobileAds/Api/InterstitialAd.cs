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
using UnityEngine;

namespace GoogleMobileAds.Api
{
    public class InterstitialAd : AdvertisingBase
    {
        // These are the ad callback events that can be hooked into.
        public event EventHandler<EventArgs> OnAdLoaded;
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;
        public event EventHandler<EventArgs> OnAdOpening;
        public event EventHandler<EventArgs> OnAdClosed;
        public event EventHandler<EventArgs> OnAdLeavingApplication;
        public event EventHandler<AdValueEventArgs> OnPaidEvent;

        private IInterstitialClient client;

        /// <summary>
        /// Creates an InterstitialAd.
        /// </summary>
        /// <param name="adUnitId"></param>
        public InterstitialAd(string adUnitId)
        {
            client = GoogleMobileAdsClientFactory.BuildInterstitialClient();
            client.CreateInterstitialAd(adUnitId);

            client.OnAdLoaded += (sender, args) => ExecuteEvent(this, OnAdLoaded, args);
            client.OnAdFailedToLoad += (sender, args) => ExecuteEvent(this, OnAdFailedToLoad, args);
            client.OnAdOpening += (sender, args) => ExecuteEvent(this, OnAdOpening, args);
            client.OnAdClosed += (sender, args) => ExecuteEvent(this, OnAdClosed, args);
            client.OnAdLeavingApplication += (sender, args) => ExecuteEvent(this, OnAdLeavingApplication, args);
            client.OnPaidEvent += (sender, args) => ExecuteEvent(this, OnPaidEvent, args);
        }

        /// <summary>
        /// Loads an InterstitialAd.
        /// </summary>
        /// <param name="request"></param>
        public override void LoadAd(AdRequest request)
        {
            client.LoadAd(request);

            if (MobileAds.IsDebugging)
                Debug.Log("Loading interstitial ad...");
        }

        /// <summary>
        /// Determines whether the InterstitialAd has loaded.
        /// </summary>
        /// <returns></returns>
        public override bool IsLoaded()
        {
            return client.IsLoaded();
        }

        /// <summary>
        /// Displays the InterstitialAd.
        /// </summary>
        public override void Show()
        {
            client.ShowInterstitial();
        }

        /// <summary>
        /// Destroys the InterstitialAd.
        /// </summary>
        public void Destroy()
        {
            client.DestroyInterstitial();
        }

        /// <summary>
        /// Returns the mediation adapter class name.
        /// </summary>
        /// <returns></returns>
        public override string MediationAdapterClassName()
        {
            return client.MediationAdapterClassName();
        }
    }
}