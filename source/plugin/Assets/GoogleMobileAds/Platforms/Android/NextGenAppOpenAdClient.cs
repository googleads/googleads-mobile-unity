// Copyright (C) 2025 Google LLC
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
    public class NextGenAppOpenAdClient : AndroidJavaProxy, IAppOpenAdClient
    {
        private readonly IInsightsEmitter _insightsEmitter = InsightsEmitter.Instance;
        private const Insight.AdFormat AppOpenFormat = Insight.AdFormat.AppOpen;

        internal AndroidJavaObject androidAppOpenAd;
        private string _adUnitId;

        public NextGenAppOpenAdClient() : base(NextGenUtils.UnityAppOpenAdCallbackClassName)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity =
                    playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            androidAppOpenAd =
                new AndroidJavaObject(NextGenUtils.UnityAppOpenAdClassName, activity, this);
        }

        #region IAppOpenClient implementation

        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;

        public event Action<AdValue> OnPaidEvent;

        public event EventHandler<AdErrorClientEventArgs> OnAdFailedToPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidRecordImpression;

        public event Action OnAdClicked;

        public long PlacementId
        {
            get
            {
                return androidAppOpenAd.Call<long>("getPlacementId");
            }
            set
            {
                androidAppOpenAd.Call("setPlacementId", value);
            }
        }

        public void CreateAppOpenAd()
        {
            // Do nothing.
        }

        public void LoadAd(string adUnitID, AdRequest request)
        {
            _adUnitId = adUnitID;
            androidAppOpenAd.Call("load", NextGenUtils.GetAdRequestJavaObject(request, adUnitID));
        }

        public void Show()
        {
            androidAppOpenAd.Call("show");
        }

        public IResponseInfoClient GetResponseInfoClient()
        {
            var responseInfoJavaObject
                = androidAppOpenAd.Call<AndroidJavaObject>("getResponseInfo");
            return new NextGenResponseInfoClient(responseInfoJavaObject);
        }

        // Returns the ad unit ID.
        public string GetAdUnitID()
        {
            // TODO(vkini): Implement GetAdUnitID for NextGen.
            return "";
        }

#if GMA_PREVIEW_FEATURES

        // Do not support this method in NextGen.
        public bool IsAdAvailable(string adUnitId)
        {
            return false;
        }


        public IAppOpenAdClient PollAd(string adUnitId)
        {
            return null;
        }

#endif

        public void DestroyAppOpenAd()
        {
            // Do nothing.
        }

        #endregion

        #region Callbacks from UnityAppOpenAdListener

        void onAppOpenAdLoaded()
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdLoaded,
                Format = AppOpenFormat,
                AdUnitId = _adUnitId,
            });

            if (this.OnAdLoaded != null)
            {
                this.OnAdLoaded(this, EventArgs.Empty);
            }
        }

        void onAppOpenAdFailedToLoad(AndroidJavaObject error)
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdLoaded,
                Format = AppOpenFormat,
                AdUnitId = _adUnitId,
                Success = false,
            });

            if (this.OnAdFailedToLoad != null)
            {
                LoadAdErrorClientEventArgs args = new LoadAdErrorClientEventArgs()
                {
                    LoadAdErrorClient = new NextGenLoadAdErrorClient(error),
                };
                this.OnAdFailedToLoad(this, args);
            }
        }

        void onAdFailedToShowFullScreenContent(AndroidJavaObject error)
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdShowedFullScreenContent,
                Format = AppOpenFormat,
                AdUnitId = _adUnitId,
                Success = false,
            });

            if (this.OnAdFailedToPresentFullScreenContent != null)
            {
                AdErrorClientEventArgs args = new AdErrorClientEventArgs()
                {
                    AdErrorClient = new NextGenFullScreenContentErrorClient(error),
                };
                this.OnAdFailedToPresentFullScreenContent(this, args);
            }
        }

        void onAdShowedFullScreenContent()
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdShowedFullScreenContent,
                Format = AppOpenFormat,
                AdUnitId = _adUnitId,
            });

            if (this.OnAdDidPresentFullScreenContent != null)
            {
                this.OnAdDidPresentFullScreenContent(this, EventArgs.Empty);
            }
        }

        void onAdDismissedFullScreenContent()
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdDismissedFullScreenContent,
                Format = AppOpenFormat,
                AdUnitId = _adUnitId,
            });

            if (this.OnAdDidDismissFullScreenContent != null)
            {
                this.OnAdDidDismissFullScreenContent(this, EventArgs.Empty);
            }
        }

        void onAdImpression()
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdShown,
                Format = AppOpenFormat,
                AdUnitId = _adUnitId,
            });

            if (this.OnAdDidRecordImpression != null)
            {
                this.OnAdDidRecordImpression(this, EventArgs.Empty);
            }
        }

        internal void onAdClicked()
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdClicked,
                Format = AppOpenFormat,
                AdUnitId = _adUnitId,
            });

            if (this.OnAdClicked != null)
            {
                this.OnAdClicked();
            }
        }

        void onPaidEvent(int precision, long valueInMicros, string currencyCode)
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdPaid,
                Format = AppOpenFormat,
                AdUnitId = _adUnitId,
            });

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
