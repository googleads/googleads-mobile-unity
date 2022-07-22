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
        public bool IsDestroyed { get; private set; }

        private AndroidJavaObject androidInterstitialAd;

        public InterstitialClient() : base(Utils.UnityInterstitialAdCallbackClassName)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity =
                    playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            this.androidInterstitialAd = new AndroidJavaObject(
                Utils.InterstitialClassName, activity, this);
        }

        public event Action OnAdFullScreenContentOpened = delegate { };
        public event Action OnAdFullScreenContentClosed = delegate { };
        public event Action<IAdErrorClient> OnAdFullScreenContentFailed = delegate { };
        public event Action<AdValue> OnAdPaid = delegate { };
        public event Action OnAdClickRecorded = delegate { };
        public event Action OnAdImpressionRecorded = delegate { };

        Action<IInterstitialClient, ILoadAdErrorClient> _callback;

        #region IGoogleMobileAdsInterstitialClient implementation

        // Loads an ad.

        public void LoadInterstitialAd(string adUnitId, AdRequest request,
            Action<IInterstitialClient, ILoadAdErrorClient> callback)
        {
            _callback = callback;
            this.androidInterstitialAd.Call("loadAd", adUnitId,
                Utils.GetAdRequestJavaObject(request));
        }

        // Presents the interstitial ad on the screen.
        public void Show()
        {
            this.androidInterstitialAd.Call("show");
        }

        // Destroys the interstitial ad.
        public void Destroy()
        {
            this.androidInterstitialAd.Call("destroy");
            IsDestroyed = true;
        }

        // Returns ad request response info
        public IResponseInfoClient GetResponseInfoClient()
        {
            return new ResponseInfoClient(ResponseInfoClientType.AdLoaded, this.androidInterstitialAd);
        }

        #endregion

        #region Callbacks from UnityInterstitialAdCallback.

        internal void onInterstitialAdLoaded()
        {
            if (_callback == null)
            {
                return;
            }

            _callback(this, null);
            _callback = null;
        }

        internal void onInterstitialAdFailedToLoad(AndroidJavaObject error)
        {
            if (_callback == null)
            {
                return;
            }

            _callback(null, new LoadAdErrorClient(error));
            _callback = null;
        }

        internal void onAdFailedToShowFullScreenContent(AndroidJavaObject error)
        {
            this.OnAdFullScreenContentFailed(new AdErrorClient(error));
        }

        internal void onAdShowedFullScreenContent()
        {
            this.OnAdFullScreenContentOpened();
        }

        internal void onAdDismissedFullScreenContent()
        {
            this.OnAdFullScreenContentClosed();
        }

        internal void onAdImpression()
        {
            this.OnAdImpressionRecorded();
        }

        internal void onAdClickRecorded()
        {
            this.OnAdClickRecorded();
        }

        public void onPaidEvent(int precision, long valueInMicros, string currencyCode)
        {
            AdValue adValue = new AdValue()
            {
                Precision = (AdValue.PrecisionType)precision,
                Value = valueInMicros,
                CurrencyCode = currencyCode
            };

            this.OnAdPaid(adValue);
        }

        #endregion
    }
}
