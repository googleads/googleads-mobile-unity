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

using GoogleMobileAds;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    public class InterstitialAd
    {
        private IInterstitialClient client;
        private string adUnitId;
        private bool isLoaded;

        // Creates an InterstitialAd.
        public InterstitialAd(string adUnitId)
        {
            this.client = MobileAds.GetClientFactory().BuildInterstitialClient();
            this.adUnitId = adUnitId;
            this.isLoaded = false;
            client.CreateInterstitialAd();

            this.client.OnAdLoaded += (sender, args) =>
            {
                this.isLoaded = true;
                if (this.OnAdLoaded != null)
                {
                    this.OnAdLoaded(this, args);
                }
            };

            this.client.OnAdFailedToLoad += (sender, args) =>
            {
                if (this.OnAdFailedToLoad != null)
                {
                    LoadAdError loadAdError = new LoadAdError(args.LoadAdErrorClient);
                    this.OnAdFailedToLoad(this, new AdFailedToLoadEventArgs()
                    {
                        LoadAdError = loadAdError
                    });
                }
            };

            this.client.OnAdDidPresentFullScreenContent += (sender, args) =>
            {
                if (this.OnAdOpening != null)
                {
                    this.OnAdOpening(this, args);
                }
            };

            this.client.OnAdDidDismissFullScreenContent += (sender, args) =>
            {
                if (this.OnAdClosed != null)
                {
                    this.OnAdClosed(this, args);
                }
            };

            this.client.OnAdFailedToPresentFullScreenContent += (sender, args) =>
            {
                if (this.OnAdFailedToShow != null)
                {
                    AdError adError = new AdError(args.AdErrorClient);
                    this.OnAdFailedToShow(this, new AdErrorEventArgs()
                    {
                        AdError = adError
                    });
                }
            };

            this.client.OnAdDidRecordImpression += (sender, args) =>
            {
                if (this.OnAdDidRecordImpression != null)
                {
                    this.OnAdDidRecordImpression(this, args);
                }
            };

            this.client.OnPaidEvent += (sender, args) =>
            {
                if (this.OnPaidEvent != null)
                {
                    this.OnPaidEvent(this, args);
                }
            };

        }

        // These are the ad callback events that can be hooked into.
        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

        public event EventHandler<EventArgs> OnAdOpening;

        public event EventHandler<EventArgs> OnAdClosed;

        public event EventHandler<AdErrorEventArgs> OnAdFailedToShow;

        public event EventHandler<EventArgs> OnAdDidRecordImpression;

        // Called when the ad is estimated to have earned money.
        public event EventHandler<AdValueEventArgs> OnPaidEvent;

        // Loads an InterstitialAd.
        public void LoadAd(AdRequest request)
        {
            client.LoadAd(this.adUnitId, request);
        }

        // Determines whether the InterstitialAd has loaded.
        public bool IsLoaded()
        {
            return this.isLoaded;
        }

        // Displays the InterstitialAd.
        public void Show()
        {
            this.isLoaded = false;
            client.Show();
        }

        // Destroys the InterstitialAd.
        public void Destroy()
        {
            client.DestroyInterstitial();
        }

        // Returns ad request response info.
        public ResponseInfo GetResponseInfo()
        {
            return new ResponseInfo(this.client.GetResponseInfoClient());
        }
    }
}