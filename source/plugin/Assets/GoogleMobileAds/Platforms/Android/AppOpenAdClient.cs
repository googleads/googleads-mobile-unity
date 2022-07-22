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
        public bool IsDestroyed { get; private set; }

        private AndroidJavaObject androidAppOpenAd;

        public AppOpenAdClient() : base(Utils.UnityAppOpenAdCallbackClassName)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity =
                    playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            androidAppOpenAd = new AndroidJavaObject(Utils.UnityAppOpenAdClassName, activity, this);
        }

        #region IAppOpenClient implementation

        public event Action OnAdFullScreenContentOpened = delegate { };
        public event Action OnAdFullScreenContentClosed = delegate { };
        public event Action<IAdErrorClient> OnAdFullScreenContentFailed = delegate { };
        public event Action<AdValue> OnAdPaid = delegate { };
        public event Action OnAdClickRecorded = delegate { };
        public event Action OnAdImpressionRecorded = delegate { };

        Action<IAppOpenAdClient, ILoadAdErrorClient> _loadCallback;

        public void LoadAppOpenAd(string adUnitId, ScreenOrientation orientation,
            AdRequest request, Action<IAppOpenAdClient, ILoadAdErrorClient> callback)
        {
            _loadCallback = callback;
            androidAppOpenAd.Call("loadAd", adUnitId,
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

        public void Destroy()
        {
            this.androidAppOpenAd.Call("destroy");
            IsDestroyed = true;
        }

        #endregion

        #region Callbacks from UnityAppOpenAdListener

        internal void onAppOpenAdLoaded()
        {
            if (_loadCallback == null)
            {
                return;
            }

            _loadCallback(this, null);
            _loadCallback = null;
        }

        internal void onAppOpenAdFailedToLoad(AndroidJavaObject error)
        {
            if (_loadCallback == null)
            {
                return;
            }

            _loadCallback(null, new LoadAdErrorClient(error));
            _loadCallback = null;
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
        #endregion
    }
}
