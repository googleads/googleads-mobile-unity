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
    public class InterstitialClient : AndroidJavaProxy, IInterstitialClient
    {
        internal AndroidJavaObject androidInterstitialAd;

        public InterstitialClient() : base(Utils.UnityInterstitialAdCallbackClassName)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity =
                    playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            this.androidInterstitialAd = new AndroidJavaObject(
                Utils.InterstitialClassName, activity, this);
        }

        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;

        public event EventHandler<AdErrorClientEventArgs> OnAdFailedToPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidRecordImpression;

        public event Action<AdValue> OnPaidEvent;

        public event Action OnAdClicked;

        // A long integer provided by the AdMob UI for the configured placement.
        public long PlacementId
        {
            get
            {
                return this.androidInterstitialAd.Call<long>("getPlacementId");
            }
            set
            {
                this.androidInterstitialAd.Call("setPlacementId", value);
            }
        }

        #region IGoogleMobileAdsInterstitialClient implementation

        // Creates an interstitial ad.
        public void CreateInterstitialAd()
        {
            // No op.
        }

        // Loads an ad.
        public void LoadAd(string adUnitId, AdRequest request)
        {
            this.androidInterstitialAd.Call("loadAd", adUnitId, Utils.GetAdRequestJavaObject(request));
        }

        // Presents the interstitial ad on the screen.
        public void Show()
        {
            this.androidInterstitialAd.Call("show");
        }

        // Destroys the interstitial ad.
        public void DestroyInterstitial()
        {
            this.androidInterstitialAd.Call("destroy");
        }

        // Returns the ad unit ID.
        public string GetAdUnitID()
        {
            return this.androidInterstitialAd.Call<string>("getAdUnitId");
        }

#if GMA_PREVIEW_FEATURES

        public bool IsAdAvailable(string adUnitId)
        {
            return this.androidInterstitialAd.Call<bool>("isAdAvailable", adUnitId);
        }

        public IInterstitialClient PollAd(string adUnitId)
        {
            this.androidInterstitialAd.Call("pollAd", adUnitId);
            return this;
        }

#endif

        // Returns ad request response info
        public IResponseInfoClient GetResponseInfoClient()
        {
            var responseInfoJavaObject = androidInterstitialAd.Call<AndroidJavaObject>(
                "getResponseInfo");
            return new ResponseInfoClient(ResponseInfoClientType.AdLoaded, responseInfoJavaObject);
        }

        #endregion

        #region Callbacks from UnityInterstitialAdCallback.

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
                this.OnPaidEvent(adValue);
            }
        }

        #endregion
    }
}
