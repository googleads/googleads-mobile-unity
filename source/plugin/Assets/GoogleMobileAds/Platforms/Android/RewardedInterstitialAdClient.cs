// Copyright (C) 2020 Google LLC
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
    public class RewardedInterstitialAdClient : AndroidJavaProxy, IRewardedInterstitialAdClient
    {
        private readonly IInsightsEmitter _insightsEmitter = InsightsEmitter.Instance;
        private const Insight.AdFormat RewardedInterstitialFormat =
            Insight.AdFormat.RewardedInterstitial;

        private AndroidJavaObject androidRewardedInterstitialAd;

        private string _adUnitId;

        public RewardedInterstitialAdClient() : base(Utils.UnityRewardedInterstitialAdCallbackClassName)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            androidRewardedInterstitialAd = new AndroidJavaObject(Utils.UnityRewardedInterstitialAdClassName, activity, this);
        }

        public event Action OnAdLoaded;

        public event Action<LoadAdErrorClientEventArgs> OnAdFailedToLoad;

        public event Action<Reward> OnUserEarnedReward;

        public event Action<AdValue> OnPaidEvent;

        public event Action<AdErrorClientEventArgs> OnAdFailedToPresentFullScreenContent;

        public event Action OnAdDidPresentFullScreenContent;

        public event Action OnAdDidDismissFullScreenContent;

        public event Action OnAdDidRecordImpression;

        public event Action OnAdClicked;

        public long PlacementId
        {
            get
            {
                return androidRewardedInterstitialAd.Call<long>("getPlacementId");
            }
            set
            {
                androidRewardedInterstitialAd.Call("setPlacementId", value);
            }
        }

        public void CreateRewardedInterstitialAd()
        {
            // Do nothing.
        }

        public void LoadAd(string adUnitId, AdRequest request)
        {
            this._adUnitId = adUnitId;
            androidRewardedInterstitialAd.Call("loadAd", adUnitId, Utils.GetAdManagerAdRequestJavaObject(request));
        }

        public void Show()
        {
            androidRewardedInterstitialAd.Call("show");
        }

        public Reward GetRewardItem()
        {
            AndroidJavaObject rewardItem = this.androidRewardedInterstitialAd.Call<AndroidJavaObject>("getRewardItem");
            if (rewardItem == null)
            {
                return null;
            }
            string type = rewardItem.Call<string>("getType");
            int amount = rewardItem.Call<int>("getAmount");
            return new Reward()
            {
                Type = type,
                Amount = (double)amount
            };
        }

        public void SetServerSideVerificationOptions(ServerSideVerificationOptions serverSideVerificationOptions)
        {
            androidRewardedInterstitialAd.Call("setServerSideVerificationOptions", Utils.GetServerSideVerificationOptionsJavaObject(serverSideVerificationOptions));
        }

        // Returns the ad unit ID.
        public string GetAdUnitID()
        {
            return this.androidRewardedInterstitialAd.Call<string>("getAdUnitId");
        }

        public IResponseInfoClient GetResponseInfoClient()
        {
            var responseInfoJavaObject =
                    androidRewardedInterstitialAd.Call<AndroidJavaObject>("getResponseInfo");
            return new ResponseInfoClient(ResponseInfoClientType.AdLoaded, responseInfoJavaObject);
        }

        public void DestroyRewardedInterstitialAd()
        {
            this.androidRewardedInterstitialAd.Call("destroy");
        }

        #region Callbacks from RewardedInterstitialAdClientCallback.

        void onRewardedInterstitialAdLoaded()
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdLoaded,
                Format = RewardedInterstitialFormat,
                AdUnitId = this._adUnitId,
            });

            if (this.OnAdLoaded != null)
            {
                this.OnAdLoaded();
            }
        }

        void onRewardedInterstitialAdFailedToLoad(AndroidJavaObject error)
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdLoaded,
                Format = RewardedInterstitialFormat,
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
                Format = RewardedInterstitialFormat,
                AdUnitId = this._adUnitId,
                Success = false,
            });

            if (this.OnAdFailedToPresentFullScreenContent != null)
            {
                AdErrorClientEventArgs args = new AdErrorClientEventArgs()
                {
                    AdErrorClient = new AdErrorClient(error)
                };
                this.OnAdFailedToPresentFullScreenContent(args);
            }
        }

        void onAdShowedFullScreenContent()
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdShowedFullScreenContent,
                Format = RewardedInterstitialFormat,
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
                Format = RewardedInterstitialFormat,
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
                Format = RewardedInterstitialFormat,
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
                Format = RewardedInterstitialFormat,
                AdUnitId = this._adUnitId,
            });

            if (this.OnAdClicked != null)
            {
                this.OnAdClicked();
            }
        }

        void onUserEarnedReward(string type, float amount)
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.UserEarnedReward,
                Format = RewardedInterstitialFormat,
                AdUnitId = this._adUnitId,
            });

            if (this.OnUserEarnedReward != null)
            {
                Reward args = new Reward()
                {
                    Type = type,
                    Amount = amount
                };
                this.OnUserEarnedReward(args);
            }
        }

        public void onPaidEvent(int precision, long valueInMicros, string currencyCode)
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdPaid,
                Format = RewardedInterstitialFormat,
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

        #endregion
    }
}
