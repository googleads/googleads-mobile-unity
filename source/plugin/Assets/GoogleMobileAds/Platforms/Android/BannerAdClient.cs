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
using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Android
{
    public class BannerAdClient : AndroidJavaProxy, IBannerAdClient
    {
        private AndroidJavaObject bannerView;

        public bool IsDestroyed { get; private set; }

        public BannerAdClient() : base(Utils.UnityAdListenerClassName)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity =
                    playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            this.bannerView = new AndroidJavaObject(
                Utils.BannerViewClassName, activity, this);
        }

        public event Action OnBannerAdLoaded = delegate { };
        public event Action<ILoadAdErrorClient> OnBannerAdLoadFailed = delegate { };
        public event Action OnAdFullScreenContentOpened = delegate { };
        public event Action OnAdFullScreenContentClosed = delegate { };
        public event Action<AdValue> OnAdPaid = delegate { };
        public event Action OnAdClickRecorded = delegate { };
        public event Action OnAdImpressionRecorded = delegate { };

        // Creates a banner view.
        public void CreateBannerAd(string adUnitId, AdSize adSize, AdPosition position)
        {
            this.bannerView.Call(
                    "create",
                    new object[3] { adUnitId, Utils.GetAdSizeJavaObject(adSize), (int)position });
        }

        // Creates a banner view with a custom position.
        public void CreateBannerAd(string adUnitId, AdSize adSize, int x, int y)
        {
            this.bannerView.Call(
                "create",
                new object[4] { adUnitId, Utils.GetAdSizeJavaObject(adSize), x, y });
        }

        // Loads an ad.
        public void LoadAd(AdRequest request)
        {
            this.bannerView.Call("loadAd", Utils.GetAdRequestJavaObject(request));
        }

        // Displays the banner view on the screen.
        public void ShowAd()
        {
            this.bannerView.Call("show");
        }

        // Hides the banner view from the screen.
        public void HideAd()
        {
            this.bannerView.Call("hide");
        }

        // Destroys the banner view.
        public void Destroy()
        {
            IsDestroyed = true;
            this.bannerView.Call("destroy");
        }

        // Returns the height of the BannerView in pixels.
        public float GetHeightInPixels()
        {
            return this.bannerView.Call<float>("getHeightInPixels");
        }

        // Returns the width of the BannerView in pixels.
        public float GetWidthInPixels()
        {
            return this.bannerView.Call<float>("getWidthInPixels");
        }

        // Set the position of the banner view using standard position.
        public void SetPosition(AdPosition adPosition)
        {
            this.bannerView.Call("setPosition", (int)adPosition);
        }

        // Set the position of the banner view using custom position.
        public void SetPosition(int x, int y)
        {
            this.bannerView.Call("setPosition", x, y);
        }

        public IResponseInfoClient GetResponseInfoClient()
        {
            return new ResponseInfoClient(ResponseInfoClientType.AdLoaded, this.bannerView);
        }

        #region Callbacks from UnityBannerAdListener.

        internal void onAdLoaded()
        {
            this.OnBannerAdLoaded();
        }

        internal void onAdFailedToLoad(AndroidJavaObject error)
        {
            this.OnBannerAdLoadFailed(new LoadAdErrorClient(error));
        }

        internal void onAdOpened()
        {
            this.OnAdFullScreenContentOpened();
        }

        internal void onAdClosed()
        {
            this.OnAdFullScreenContentClosed();
        }

        internal void onPaidEvent(int precision, long valueInMicros, string currencyCode)
        {
            AdValue adValue = new AdValue()
            {
                Precision = (AdValue.PrecisionType)precision,
                Value = valueInMicros,
                CurrencyCode = currencyCode
            };

            this.OnAdPaid(adValue);
        }

        internal void onAdClickRecorded()
        {
            this.OnAdClickRecorded();
        }

        internal void onAdImpressionRecorded()
        {
            this.OnAdImpressionRecorded();
        }

        #endregion
    }
}
