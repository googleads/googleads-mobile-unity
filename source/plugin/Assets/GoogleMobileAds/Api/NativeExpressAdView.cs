// Copyright (C) 2016 Google, Inc.
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
using System.Reflection;

using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    public class NativeExpressAdView
    {
        private INativeExpressAdClient client;

        // Creates a NativeExpressAd and adds it to the view hierarchy.
        public NativeExpressAdView(string adUnitId, AdSize adSize, AdPosition position)
        {
            Type googleMobileAdsClientFactory = Type.GetType(
                "GoogleMobileAds.GoogleMobileAdsClientFactory,Assembly-CSharp");
            MethodInfo method = googleMobileAdsClientFactory.GetMethod(
                "BuildNativeExpressAdClient",
                BindingFlags.Static | BindingFlags.Public);
            this.client = (INativeExpressAdClient)method.Invoke(null, null);
            this.client.CreateNativeExpressAdView(adUnitId, adSize, position);

            ConfigureNativeExpressAdEvents();
        }

        // Creates a NativeExpressAd with a custom position.
        public NativeExpressAdView(string adUnitId, AdSize adSize, int x, int y)
        {
            Type googleMobileAdsClientFactory = Type.GetType(
                "GoogleMobileAds.GoogleMobileAdsClientFactory,Assembly-CSharp");
            MethodInfo method = googleMobileAdsClientFactory.GetMethod(
                "BuildNativeExpressAdClient",
                BindingFlags.Static | BindingFlags.Public);
            this.client = (INativeExpressAdClient)method.Invoke(null, null);
            this.client.CreateNativeExpressAdView(adUnitId, adSize, x, y);

            ConfigureNativeExpressAdEvents();
        }

        // These are the ad callback events that can be hooked into.
        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

        public event EventHandler<EventArgs> OnAdOpening;

        public event EventHandler<EventArgs> OnAdClosed;

        public event EventHandler<EventArgs> OnAdLeavingApplication;

        // Loads a native express ad.
        public void LoadAd(AdRequest request)
        {
            this.client.LoadAd(request);
        }

        // Hides the NativeExpressAdView from the screen.
        public void Hide()
        {
            this.client.HideNativeExpressAdView();
        }

        // Shows the NativeExpressAdView on the screen.
        public void Show()
        {
            this.client.ShowNativeExpressAdView();
        }

        // Destroys the NativeExpressAdView.
        public void Destroy()
        {
            this.client.DestroyNativeExpressAdView();
        }

        private void ConfigureNativeExpressAdEvents()
        {
            this.client.OnAdLoaded += (sender, args) =>
            {
                if(this.OnAdLoaded != null)
                {
                    this.OnAdLoaded(this, args);
                }
            };

            this.client.OnAdFailedToLoad += (sender, args) =>
            {
                if(this.OnAdFailedToLoad != null)
                {
                    this.OnAdFailedToLoad(this, args);
                }
            };

            this.client.OnAdOpening += (sender, args) =>
            {
                if(this.OnAdOpening != null)
                {
                    this.OnAdOpening(this, args);
                }
            };

            this.client.OnAdClosed += (sender, args) =>
            {
                if(this.OnAdClosed != null)
                {
                    this.OnAdClosed(this, args);
                }
            };

            this.client.OnAdLeavingApplication += (sender, args) =>
            {
                if(this.OnAdLeavingApplication != null)
                {
                    this.OnAdLeavingApplication(this, args);
                }
            };
        }
    }
}
