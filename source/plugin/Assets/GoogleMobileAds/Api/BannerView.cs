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
    public class BannerView
    {
        private IBannerClient client;

        // These are the ad callback events that can be hooked into.
        public event EventHandler<EventArgs> OnAdLoaded = delegate {};
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad = delegate {};
        public event EventHandler<EventArgs> OnAdOpening = delegate {};
        public event EventHandler<EventArgs> OnAdClosed = delegate {};
        public event EventHandler<EventArgs> OnAdLeavingApplication = delegate {};

        // Creates a BannerView and adds it to the view hierarchy.
        public BannerView(string adUnitId, AdSize adSize, AdPosition position)
        {
            client = GoogleMobileAdsClientFactory.BuildBannerClient();
            client.CreateBannerView(adUnitId, adSize, position);

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

        // Loads an ad into the BannerView.
        public void LoadAd(AdRequest request)
        {
            client.LoadAd(request);
        }

        // Hides the BannerView from the screen.
        public void Hide()
        {
            client.HideBannerView();
        }

        // Shows the BannerView on the screen.
        public void Show()
        {
            client.ShowBannerView();
        }

        // Destroys the BannerView.
        public void Destroy()
        {
            client.DestroyBannerView();
        }
    }
}
