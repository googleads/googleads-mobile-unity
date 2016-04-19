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
    internal class InterstitialClient : AndroidJavaProxy, IInterstitialClient
    {
        private AndroidJavaObject interstitial;

        public event EventHandler<EventArgs> OnAdLoaded = delegate {};
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad = delegate {};
        public event EventHandler<EventArgs> OnAdOpening = delegate {};
        public event EventHandler<EventArgs> OnAdClosed = delegate {};
        public event EventHandler<EventArgs> OnAdLeavingApplication = delegate {};

        public InterstitialClient()
            : base(Utils.UnityInterstitialAdListenerClassName)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity =
                    playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            interstitial = new AndroidJavaObject(
                Utils.InterstitialClassName, activity, this);
        }

        #region IGoogleMobileAdsInterstitialClient implementation

        // Creates an interstitial ad.
        public void CreateInterstitialAd(string adUnitId) {
            interstitial.Call("create", adUnitId);
        }

        // Loads an ad.
        public void LoadAd(AdRequest request) {
            interstitial.Call("loadAd", Utils.GetAdRequestJavaObject(request));
        }

        // Checks if interstitial has loaded.
        public bool IsLoaded() {
            return interstitial.Call<bool>("isLoaded");
        }

        // Presents the interstitial ad on the screen.
        public void ShowInterstitial() {
            interstitial.Call("show");
        }

        // Destroys the interstitial ad.
        public void DestroyInterstitial() {
            interstitial.Call("destroy");
        }

        // Sets IDefaultInAppPurchaseProcessor as PlayStorePurchaseListener on interstital ad.
        public void SetDefaultInAppPurchaseProcessor(IDefaultInAppPurchaseProcessor processor) {
            DefaultInAppPurchaseListener listener = new DefaultInAppPurchaseListener(processor);
            interstitial.Call("setPlayStorePurchaseParams", listener, processor.AndroidPublicKey);
        }

        // Sets ICustomInAppPurchaseProcessor as PlayStorePurchaseListener on interstital ad.
        public void SetCustomInAppPurchaseProcessor(ICustomInAppPurchaseProcessor processor) {
            CustomInAppPurchaseListener listener = new CustomInAppPurchaseListener(processor);
            interstitial.Call("setInAppPurchaseListener", listener);
        }

        #endregion

        #region Callbacks from UnityInterstitialAdListener.

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

        # endregion
    }
}

#endif
