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
    public class RewardedAdClient : AndroidJavaProxy, IRewardedAdClient
    {
        public bool IsDestroyed { get; private set; }

        private AndroidJavaObject androidRewardedAd;

        public RewardedAdClient() : base(Utils.UnityRewardedAdCallbackClassName)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity =
                playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            androidRewardedAd = new AndroidJavaObject(Utils.UnityRewardedAdClassName, activity, this);
        }

        #region IRewardedClient implementation

        public event Action OnAdFullScreenContentOpened = delegate { };
        public event Action OnAdFullScreenContentClosed = delegate { };
        public event Action<IAdErrorClient> OnAdFullScreenContentFailed = delegate { };
        public event Action<AdValue> OnAdPaid = delegate { };
        public event Action OnAdClickRecorded = delegate { };
        public event Action OnAdImpressionRecorded = delegate { };

        private Action<IRewardedAdClient, ILoadAdErrorClient> _loadCallback;
        private Action<Reward> _rewardCallback;

        public void LoadAd(string adUnitId,
                           AdRequest request,
                           Action<IRewardedAdClient, ILoadAdErrorClient> callback)
        {
            _loadCallback = callback;
            androidRewardedAd.Call("loadAd", adUnitId, Utils.GetAdRequestJavaObject(request));
        }

        public void ShowAd(Action<Reward> userRewardEarnedCallback)
        {
            _rewardCallback = userRewardEarnedCallback;
            androidRewardedAd.Call("show");
        }

        public void SetServerSideVerificationOptions(ServerSideVerificationOptions serverSideVerificationOptions)
        {
            androidRewardedAd.Call("setServerSideVerificationOptions", Utils.GetServerSideVerificationOptionsJavaObject(serverSideVerificationOptions));
        }

        // Returns the reward item for the loaded rewarded ad.
        public Reward GetRewardItem()
        {
            AndroidJavaObject rewardItem = this.androidRewardedAd.Call<AndroidJavaObject>("getRewardItem");
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

        // Returns ad request response info
        public IResponseInfoClient GetResponseInfoClient()
        {
            return new ResponseInfoClient(ResponseInfoClientType.AdLoaded, this.androidRewardedAd);
        }

        // Destroy the rewarded ad.
        public void Destroy()
        {
            this.androidRewardedAd.Call("destroy");
            IsDestroyed = true;
        }

        #endregion

        #region Callbacks from UnityRewardedAdCallback

        internal void onRewardedAdLoaded()
        {
            if (_loadCallback != null)
            {
                _loadCallback(this, null);
                _loadCallback = null;
            }
        }

        internal void onRewardedAdFailedToLoad(AndroidJavaObject error)
        {
            if (_loadCallback != null)
            {
                _loadCallback(null, new LoadAdErrorClient(error));
                _loadCallback = null;
            }
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

        internal void onUserEarnedReward(string type, float amount)
        {
            if (_rewardCallback != null)
            {
                Reward reward = new Reward()
                {
                    Type = type,
                    Amount = amount
                };
                _rewardCallback(reward);
                _rewardCallback = null;
            }
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
