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
    public class DecagonInterstitialAdClient : AndroidJavaProxy, IInterstitialClient
    {
        internal AndroidJavaObject _androidInterstitialAd;

        public DecagonInterstitialAdClient()
            : base(DecagonUtils.UnityInterstitialAdCallbackClassName)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity =
                    playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            _androidInterstitialAd =
                new AndroidJavaObject(DecagonUtils.UnityInterstitialAdClassName, activity, this);
        }

        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;

        public event EventHandler<AdErrorClientEventArgs> OnAdFailedToPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidRecordImpression;

        public event Action<AdValue> OnPaidEvent;

        public event Action OnAdClicked;

        #region IGoogleMobileAdsInterstitialClient implementation

        // Creates an interstitial ad.
        public void CreateInterstitialAd()
        {
            // No op.
        }

        // Loads an ad.
        public void LoadAd(string adUnitId, AdRequest request)
        {
            _androidInterstitialAd.Call("load",
                                        DecagonUtils.GetAdRequestJavaObject(request, adUnitId));
        }

        // Presents the interstitial ad on the screen.
        public void Show()
        {
            _androidInterstitialAd.Call("show");
        }

        // Destroys the interstitial ad.
        public void DestroyInterstitial()
        {
            // Currently we don't have to do anything on destroy.
        }

        // Returns the ad unit ID.
        public string GetAdUnitID()
        {
            // TODO(vkini): Implement GetAdUnitID for Decagon.
            return "";
        }

        public long PlacementId
        {
            get
            {
                return _androidInterstitialAd.Call<long>("getPlacementId");
            }
            set
            {
                _androidInterstitialAd.Call("setPlacementId", value);
            }
        }

        // Returns ad request response info
        public IResponseInfoClient GetResponseInfoClient()
        {
            var responseInfoJavaObject
                = _androidInterstitialAd.Call<AndroidJavaObject>("getResponseInfo");
            return new DecagonResponseInfoClient(responseInfoJavaObject);
        }

        #endregion

        #if GMA_PREVIEW_FEATURES

        // Ad Preloading v1 will not be supported in Decagon.
        public bool IsAdAvailable(string adUnitId)
        {
            return false;
        }

        public IInterstitialClient PollAd(string adUnitId)
        {
            return null;
        }

#endif

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
                    LoadAdErrorClient = new DecagonLoadAdErrorClient(error)
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
                    AdErrorClient = new DecagonFullScreenContentErrorClient(error),
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
                this.OnPaidEvent(adValue);
            }
        }

        #endregion
    }
}
