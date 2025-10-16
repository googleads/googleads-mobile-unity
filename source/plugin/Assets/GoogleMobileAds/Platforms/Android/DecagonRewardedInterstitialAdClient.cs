// Copyright (C) 2018 Google, Inc.
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
    public class DecagonRewardedInterstitialAdClient : AndroidJavaProxy,
                                                        IRewardedInterstitialAdClient
    {
        private AndroidJavaObject _androidRewardedInterstitialAd;

        public DecagonRewardedInterstitialAdClient()
            : base(DecagonUtils.UnityRewardedInterstitialAdCallbackClassName)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity =
                playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            _androidRewardedInterstitialAd = new AndroidJavaObject(
                DecagonUtils.UnityRewardedInterstitialAdClassName, activity, this);
        }

        #region IRewardedClient implementation

        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;

        public event EventHandler<Reward> OnUserEarnedReward;

        public event Action<AdValue> OnPaidEvent;

        public event EventHandler<AdErrorClientEventArgs> OnAdFailedToPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidRecordImpression;

        public event Action OnAdClicked;

        public void CreateRewardedInterstitialAd()
        {
            // No op.
        }

        public void LoadAd(string adUnitId, AdRequest request)
        {
            _androidRewardedInterstitialAd
                .Call("load", DecagonUtils.GetAdRequestJavaObject(request, adUnitId));
        }

        public void Show()
        {
            _androidRewardedInterstitialAd.Call("show");
        }

        public void SetServerSideVerificationOptions(
            ServerSideVerificationOptions serverSideVerificationOptions)
        {
            // TODO(vkini): Implement SetServerSideVerificationOptions for Decagon.
        }

        public long PlacementId
        {
            get
            {
                return _androidRewardedInterstitialAd.Call<long>("getPlacementId");
            }
            set
            {
                _androidRewardedInterstitialAd.Call("setPlacementId", value);
            }
        }

        // Returns the reward item for the loaded rewarded ad.
        public Reward GetRewardItem()
        {
            AndroidJavaObject rewardItem =
                _androidRewardedInterstitialAd.Call<AndroidJavaObject>("getRewardItem");
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

        // Returns the ad unit ID.
        public string GetAdUnitID()
        {
            // TODO(vkini): Implement GetAdUnitID for Decagon.
            return "";
        }

        // Ad Preloading v1 will not be supported in Decagon.
        public bool IsAdAvailable(string adUnitId)
        {
            return false;
        }

        public IRewardedAdClient PollAd(string adUnitId)
        {
            return null;
        }

        // Returns ad request response info
        public IResponseInfoClient GetResponseInfoClient()
        {
            var responseInfoJavaObject = _androidRewardedInterstitialAd.Call<AndroidJavaObject>(
                    "getResponseInfo");
            return new DecagonResponseInfoClient(responseInfoJavaObject);
        }

        // Destroy the rewarded interstitial ad.
        public void DestroyRewardedInterstitialAd()
        {
            // Currently we don't have to do anything on destroy.
        }

        #endregion

        #region Callbacks from UnityRewardedAdCallback
        void onRewardedInterstitialAdLoaded()
        {
            if (this.OnAdLoaded != null)
            {
                this.OnAdLoaded(this, EventArgs.Empty);
            }
        }

        void onRewardedInterstitialAdFailedToLoad(AndroidJavaObject error)
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
                    AdErrorClient = new DecagonFullScreenContentErrorClient(error)
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

        void onAdClicked()
        {
            if (this.OnAdClicked != null)
            {
                this.OnAdClicked();
            }
        }

        void onUserEarnedReward(string type, float amount)
        {
            if (this.OnUserEarnedReward != null)
            {
                Reward args = new Reward()
                {
                    Type = type,
                    Amount = amount
                };
                this.OnUserEarnedReward(this, args);
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
