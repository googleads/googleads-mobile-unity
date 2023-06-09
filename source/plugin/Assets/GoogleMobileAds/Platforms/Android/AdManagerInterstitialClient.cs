// Copyright (C) 2023 Google, Inc.
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
using GoogleMobileAds.Api.AdManager;
using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Android
{
    public class AdManagerInterstitialClient : AndroidJavaProxy, IAdManagerInterstitialClient
    {
        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;

        public event EventHandler<AdErrorClientEventArgs> OnAdFailedToPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidRecordImpression;

        public event EventHandler<AdValueEventArgs> OnPaidEvent;

        public event Action<AppEvent> OnAppEvent;

        public event Action OnAdClicked;

        private AndroidJavaObject _androidAdmanagerInterstitialAd;

        public AdManagerInterstitialClient() :
                base(Utils.UnityAdManagerInterstitialAdCallbackClassName)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity =
                    playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            this._androidAdmanagerInterstitialAd = new AndroidJavaObject(
                Utils.UnityAdManagerInterstitialAdClassName, activity, this);
        }

        #region IAdManagerInterstitialClient implementation

        // Creates an interstitial ad.
        public void CreateInterstitialAd()
        {
            // No op.
        }

        // Loads an AdManager Interstitial ad.
        public void LoadAd(string adUnitId, AdRequest request)
        {
            this._androidAdmanagerInterstitialAd.Call("loadAd", adUnitId,
                    Utils.GetAdManagerAdRequestJavaObject(request));
        }

        // Presents the interstitial ad on the screen.
        public void Show()
        {
            this._androidAdmanagerInterstitialAd.Call("show");
        }

        // Destroys the interstitial ad.
        public void DestroyInterstitial()
        {
            this._androidAdmanagerInterstitialAd.Call("destroy");
        }

        // Returns ad request response info
        public IResponseInfoClient GetResponseInfoClient()
        {
            return new ResponseInfoClient(ResponseInfoClientType.AdLoaded,
                                          this._androidAdmanagerInterstitialAd);
        }

        #endregion

        #region Callbacks from UnityAdManagerInterstitialAdCallback.

        public void onInterstitialAdLoaded()
        {
            if (this.OnAdLoaded != null)
            {
                this.OnAdLoaded(this, EventArgs.Empty);
            }
        }

        public void onInterstitialAdFailedToLoad(AndroidJavaObject error)
        {
            if (this.OnAdFailedToLoad != null)
            {
                LoadAdErrorClientEventArgs args = new LoadAdErrorClientEventArgs()
                {
                    LoadAdErrorClient = new LoadAdErrorClient(error)
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

        public void onPaidEvent(int precision, long valueInMicros, string currencyCode)
        {
            if (this.OnPaidEvent != null)
            {
                AdValue adValue = new AdValue()
                {
                    Precision = (AdValue.PrecisionType)precision,
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

        public void onAppEvent(string name, string data)
        {
            if (this.OnAppEvent != null)
            {
                this.OnAppEvent(new AppEvent()
                {
                    Name = name,
                    Data = data,
                });
            }
        }

        #endregion
    }
}
