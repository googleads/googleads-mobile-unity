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
    public class BannerView
    {
        // These are the ad callback events that can be hooked into.
        public event EventHandler<EventArgs> OnAdLoaded;
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;
        public event EventHandler<EventArgs> OnAdOpening;
        public event EventHandler<EventArgs> OnAdClosed;
        public event EventHandler<EventArgs> OnAdLeavingApplication;
        public event EventHandler<AdValueEventArgs> OnPaidEvent;

        private IBannerClient client;

        /// <summary>
        /// Creates a BannerView and adds it to the view hierarchy.
        /// </summary>
        /// <param name="adUnitId"></param>
        /// <param name="adSize"></param>
        /// <param name="position"></param>
        public BannerView(string adUnitId, AdSize adSize, AdPosition position)
        {
            client = GoogleMobileAdsClientFactory.BuildBannerClient();
            client.CreateBannerView(adUnitId, adSize, position);

            ConfigureBannerEvents();
        }

        /// <summary>
        /// Creates a BannerView with a custom position.
        /// </summary>
        /// <param name="adUnitId"></param>
        /// <param name="adSize"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public BannerView(string adUnitId, AdSize adSize, int x, int y)
        {
            client = GoogleMobileAdsClientFactory.BuildBannerClient();
            client.CreateBannerView(adUnitId, adSize, x, y);

            ConfigureBannerEvents();
        }

        /// <summary>
        /// Loads an ad into the BannerView.
        /// </summary>
        /// <param name="request"></param>
        public void LoadAd(AdRequest request)
        {
            client.LoadAd(request);
        }

        /// <summary>
        /// Hides the BannerView from the screen.
        /// </summary>
        public void Hide()
        {
            client.HideBannerView();
        }

        /// <summary>
        /// Shows the BannerView on the screen.
        /// </summary>
        public void Show()
        {
            client.ShowBannerView();
        }

        /// <summary>
        /// Destroys the BannerView.
        /// </summary>
        public void Destroy()
        {
            client.DestroyBannerView();
        }

        /// <summary>
        /// Returns the height of the BannerView in pixels.
        /// </summary>
        /// <returns></returns>
        public float GetHeightInPixels()
        {
            return client.GetHeightInPixels();
        }

        /// <summary>
        /// Returns the width of the BannerView in pixels.
        /// </summary>
        /// <returns></returns>
        public float GetWidthInPixels()
        {
            return client.GetWidthInPixels();
        }

        /// <summary>
        /// Set the position of the BannerView using standard position.
        /// </summary>
        /// <param name="adPosition"></param>
        public void SetPosition(AdPosition adPosition)
        {
            client.SetPosition(adPosition);
        }

        /// <summary>
        /// Set the position of the BannerView using custom position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetPosition(int x, int y)
        {
            client.SetPosition(x, y);
        }

        private void ConfigureBannerEvents()
        {
            client.OnAdLoaded += (sender, args) => OnAdLoaded?.Invoke(this, args);
            client.OnAdFailedToLoad += (sender, args) => OnAdFailedToLoad?.Invoke(this, args);
            client.OnAdOpening += (sender, args) => OnAdOpening?.Invoke(this, args);
            client.OnAdClosed += (sender, args) => OnAdClosed?.Invoke(this, args);
            client.OnAdLeavingApplication += (sender, args) => OnAdLeavingApplication?.Invoke(this, args);
            client.OnPaidEvent += (sender, args) => OnPaidEvent?.Invoke(this, args);
        }

        /// <summary>
        /// Returns the mediation adapter class name.
        /// </summary>
        /// <returns></returns>
        public string MediationAdapterClassName()
        {
            return client.MediationAdapterClassName();
        }
    }
}