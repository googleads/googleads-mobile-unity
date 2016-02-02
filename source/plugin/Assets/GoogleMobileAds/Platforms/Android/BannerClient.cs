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

#if UNITY_ANDROID

using System;
using System.Collections.Generic;
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    internal class BannerClient : AndroidJavaProxy, IBannerClient
    {
        private AndroidJavaObject bannerView;

        public event EventHandler<EventArgs> OnAdLoaded = delegate {};
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad = delegate {};
        public event EventHandler<EventArgs> OnAdOpening = delegate {};
        public event EventHandler<EventArgs> OnAdClosed = delegate {};
        public event EventHandler<EventArgs> OnAdLeavingApplication = delegate {};

        public BannerClient() : base(Utils.UnityBannerAdListenerClassName)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity =
                    playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            bannerView = new AndroidJavaObject(
                Utils.BannerViewClassName, activity, this);
        }

        // Creates a banner view.
        public void CreateBannerView(String adUnitId, AdSize adSize, AdPosition position) {
            bannerView.Call("create",
                    new object[3] { adUnitId, Utils.GetAdSizeJavaObject(adSize), (int)position });
        }

        // Loads an ad.
        public void LoadAd(AdRequest request)
        {
            bannerView.Call("loadAd", Utils.GetAdRequestJavaObject(request));
        }

        // Displays the banner view on the screen.
        public void ShowBannerView() {
            bannerView.Call("show");
        }

        // Hides the banner view from the screen.
        public void HideBannerView()
        {
            bannerView.Call("hide");
        }

        // Destroys the banner view.
        public void DestroyBannerView()
        {
            bannerView.Call("destroy");
        }

        #region Callbacks from UnityBannerAdListener.

        void onAdLoaded()
        {
            OnAdLoaded(this, EventArgs.Empty);
        }

        void onAdFailedToLoad(string errorReason)
        {
            AdFailedToLoadEventArgs args = new AdFailedToLoadEventArgs() {
                Message = errorReason
            };
            OnAdFailedToLoad(this, args);
        }

        void onAdOpened()
        {
            OnAdOpening(this, EventArgs.Empty);
        }

        void onAdClosed()
        {
            OnAdClosed(this, EventArgs.Empty);
        }

        void onAdLeftApplication()
        {
            OnAdLeavingApplication(this, EventArgs.Empty);
        }

        #endregion
    }
}

#endif
