// Copyright (C) 2021 Google LLC
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

using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    public class AppOpenAdClient : AndroidJavaProxy, IAppOpenAdClient
    {
        private AndroidJavaObject androidAppOpenAd;

        public AppOpenAdClient() : base(Utils.UnityAppOpenAdCallbackClassName)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity =
                    playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            androidAppOpenAd = new AndroidJavaObject(Utils.UnityAppOpenAdClassName, activity, this);
        }

        #region IAppOpenClient implementation

        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;

        public event EventHandler<AdValueEventArgs> OnPaidEvent;

        public event EventHandler<AdErrorClientEventArgs> OnAdFailedToPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidRecordImpression;

        public event Action OnAdClicked;

        public void CreateAppOpenAd()
        {
            // Do nothing.
        }

        public void LoadAd(string adUnitID, AdRequest request)
        {
            androidAppOpenAd.Call("loadAd", adUnitID, Utils.GetAdRequestJavaObject(request));
        }

        public void LoadAd(string adUnitID, AdRequest request, ScreenOrientation orientation)
        {
            androidAppOpenAd.Call("loadAd", adUnitID,
                Utils.GetAdRequestJavaObject(request),
                Utils.GetAppOpenAdOrientation(orientation));
        }

        public void Show()
        {
            androidAppOpenAd.Call("show");
        }

        public IResponseInfoClient GetResponseInfoClient()
        {
            return new ResponseInfoClient(ResponseInfoClientType.AdLoaded, this.androidAppOpenAd);
        }

        public void DestroyAppOpenAd()
        {
            this.androidAppOpenAd.Call("destroy");
        }

        #endregion

        #region Callbacks from UnityAppOpenAdListener

        void onAppOpenAdLoaded()
        {
            if (this.OnAdLoaded != null)
            {
                this.OnAdLoaded(this, EventArgs.Empty);
            }
        }

        void onAppOpenAdFailedToLoad(AndroidJavaObject error)
        {
            if (this.OnAdFailedToLoad != null)
            {
                LoadAdErrorClientEventArgs args = new LoadAdErrorClientEventArgs()
                {
                    LoadAdErrorClient = new LoadAdErrorClient(error),
                };
                this.OnAdFailedToLoad(this, args);
            }
        }

        void onAdFailedToShowFullScreenContent(AndroidJavaObject error)
        {
            if (this.OnAdFailedToPresentFullScreenContent != null)
            {
                AdErrorClientEventArgs args = new AdErrorClientEventArgs()
                {
                    AdErrorClient = new AdErrorClient(error),
                };
                this.OnAdFailedToPresentFullScreenContent(this, args);
            }
        }

        void onAdShowedFullScreenContent()
        {
            if (this.OnAdDidPresentFullScreenContent != null)
            {
                this.OnAdDidPresentFullScreenContent(this, EventArgs.Empty);
            }
        }

        void onAdDismissedFullScreenContent()
        {
            if (this.OnAdDidDismissFullScreenContent != null)
            {
                this.OnAdDidDismissFullScreenContent(this, EventArgs.Empty);
            }
        }

        void onAdImpression()
        {
            if (this.OnAdDidRecordImpression != null)
            {
                this.OnAdDidRecordImpression(this, EventArgs.Empty);
            }
        }

        internal void onAdClicked()
        {
            if (this.OnAdClicked != null)
            {
                this.OnAdClicked();
            }
        }

        void onPaidEvent(int precision, long valueInMicros, string currencyCode)
        {
            if (this.OnPaidEvent != null)
            {
                AdValue adValue = new AdValue()
                {
                    Precision = (AdValue.PrecisionType) precision,
                    Value = valueInMicros,
                    CurrencyCode = currencyCode
                };
                AdValueEventArgs args = new AdValueEventArgs()
                {
                    AdValue = adValue
                };

                this.OnPaidEvent(this, args);
            }
        }

        #endregion
    }
}
