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
        private readonly IInsightsEmitter _insightsEmitter = InsightsEmitter.Instance;
        private const Insight.AdFormat InterstitialFormat = Insight.AdFormat.Interstitial;

        public event Action OnAdLoaded;

        public event Action<LoadAdErrorClientEventArgs> OnAdFailedToLoad;

        public event Action<AdErrorClientEventArgs> OnAdFailedToPresentFullScreenContent;

        public event Action OnAdDidPresentFullScreenContent;

        public event Action OnAdDidDismissFullScreenContent;

        public event Action OnAdDidRecordImpression;

        public event Action<AdValue> OnPaidEvent;

        public event Action<AppEvent> OnAppEvent;

        public event Action OnAdClicked;

        // Placement ID is not supported for AdManagerInterstitialAd.
        public long PlacementId { get; set; }

        private AndroidJavaObject _androidAdmanagerInterstitialAd;

        private string _adUnitId;

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

#if GMA_PREVIEW_FEATURES

        public bool IsAdAvailable(string adUnitId)
        {
            return this._androidAdmanagerInterstitialAd.Call<bool>("isAdAvailable", adUnitId);
        }

        public IInterstitialClient PollAd(string adUnitId)
        {
            return PollAdManagerAd(adUnitId);
        }

#endif

        public IAdManagerInterstitialClient PollAdManagerAd(string adUnitId)
        {
            this._adUnitId = adUnitId;
            this._androidAdmanagerInterstitialAd.Call("pollAd", adUnitId);
            return this;
        }

        // Loads an AdManager Interstitial ad.
        public void LoadAd(string adUnitId, AdRequest request)
        {
            this._adUnitId = adUnitId;
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

        // Returns the ad unit ID.
        public string GetAdUnitID()
        {
            return this._androidAdmanagerInterstitialAd.Call<string>("getAdUnitId");
        }

        // Returns ad request response info
        public IResponseInfoClient GetResponseInfoClient()
        {
            var responseInfoJavaObject = _androidAdmanagerInterstitialAd.Call<AndroidJavaObject>("getResponseInfo");
            return new ResponseInfoClient(ResponseInfoClientType.AdLoaded, responseInfoJavaObject);
        }

        #endregion

        #region Callbacks from UnityAdManagerInterstitialAdCallback.

        public void onInterstitialAdLoaded()
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdLoaded,
                Format = InterstitialFormat,
                AdUnitId = this._adUnitId,
            });

            if (this.OnAdLoaded != null)
            {
                this.OnAdLoaded();
            }
        }

        public void onInterstitialAdFailedToLoad(AndroidJavaObject error)
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdLoaded,
                Format = InterstitialFormat,
                AdUnitId = this._adUnitId,
                Success = false,
            });

            if (this.OnAdFailedToLoad != null)
            {
                LoadAdErrorClientEventArgs args = new LoadAdErrorClientEventArgs()
                {
                    LoadAdErrorClient = new LoadAdErrorClient(error)
                };
                this.OnAdFailedToLoad(args);
            }
        }

        void onAdFailedToShowFullScreenContent(AndroidJavaObject error)
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdShowedFullScreenContent,
                Format = InterstitialFormat,
                AdUnitId = this._adUnitId,
                Success = false,
            });

            if (this.OnAdFailedToPresentFullScreenContent != null)
            {
                AdErrorClientEventArgs args = new AdErrorClientEventArgs()
                {
                    AdErrorClient = new AdErrorClient(error),
                };
                this.OnAdFailedToPresentFullScreenContent(args);
            }
        }

        void onAdShowedFullScreenContent()
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdShowedFullScreenContent,
                Format = InterstitialFormat,
                AdUnitId = this._adUnitId,
            });

            if (this.OnAdDidPresentFullScreenContent != null)
            {
                this.OnAdDidPresentFullScreenContent();
            }
        }


        void onAdDismissedFullScreenContent()
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdDismissedFullScreenContent,
                Format = InterstitialFormat,
                AdUnitId = this._adUnitId,
            });

            if (this.OnAdDidDismissFullScreenContent != null)
            {
                this.OnAdDidDismissFullScreenContent();
            }
        }

        void onAdImpression()
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdShown,
                Format = InterstitialFormat,
                AdUnitId = this._adUnitId,
            });

            if (this.OnAdDidRecordImpression != null)
            {
                this.OnAdDidRecordImpression();
            }
        }

        internal void onAdClicked()
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdClicked,
                Format = InterstitialFormat,
                AdUnitId = this._adUnitId,
            });

            if (this.OnAdClicked != null)
            {
                this.OnAdClicked();
            }
        }

        public void onPaidEvent(int precision, long valueInMicros, string currencyCode)
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdPaid,
                Format = InterstitialFormat,
                AdUnitId = this._adUnitId,
            });

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
