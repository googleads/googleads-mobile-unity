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
    public class InterstitialAd
    {
        private IInterstitialClient client;

        // These are the ad callback events that can be hooked into.
        public event EventHandler<EventArgs> OnAdLoaded = delegate {};
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad = delegate {};
        public event EventHandler<EventArgs> OnAdOpening = delegate {};
        public event EventHandler<EventArgs> OnAdClosed = delegate {};
        public event EventHandler<EventArgs> OnAdLeavingApplication = delegate {};

        // Creates an InterstitialAd.
        public InterstitialAd(string adUnitId)
        {
            client = GoogleMobileAdsClientFactory.BuildInterstitialClient();
            client.CreateInterstitialAd(adUnitId);

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

            client.OnAdClosed += delegate(object sender, EventArgs args)
            {
                OnAdClosed(this, args);
            };

            client.OnAdLeavingApplication += delegate(object sender, EventArgs args)
            {
                OnAdLeavingApplication(this, args);
            };
        }

        // Loads an InterstitialAd.
        public void LoadAd(AdRequest request)
        {
            client.LoadAd(request);
        }

        // Determines whether the InterstitialAd has loaded.
        public bool IsLoaded()
        {
            return client.IsLoaded();
        }

        // Displays the InterstitialAd.
        public void Show()
        {
            client.ShowInterstitial();
        }

        // Destroys the InterstitialAd.
        public void Destroy()
        {
            client.DestroyInterstitial();
        }

        // Set IDefaultInAppPurchaseProcessor for InterstitialAd.
        public void SetInAppPurchaseProcessor(IDefaultInAppPurchaseProcessor processor)
        {
            client.SetDefaultInAppPurchaseProcessor(processor);
        }

        // Set ICustomInAppPurchaseProcessor for InterstitialAd.
        public void SetInAppPurchaseProcessor(ICustomInAppPurchaseProcessor processor)
        {
            client.SetCustomInAppPurchaseProcessor(processor);
        }
    }
}
