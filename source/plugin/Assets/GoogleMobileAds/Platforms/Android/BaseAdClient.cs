// Copyright (C) 2022 Google LLC.
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
using System.Threading;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    public class BaseAdClient : AndroidJavaProxy, IAndroidAdListener, IAndroidRewardAdListener
    {
        public event Action OnAdFullScreenContentOpened = delegate { };
        public event Action OnAdFullScreenContentClosed = delegate { };
        public event Action<IAdErrorClient> OnAdFullScreenContentFailed = delegate { };
        public event Action<AdValue> OnAdPaid = delegate { };
        public event Action OnAdClickRecorded = delegate { };
        public event Action OnAdImpressionRecorded = delegate { };

        public bool IsDestroyed { get { return _ad == null; } }

        protected static AndroidJavaClass _playerClass;
        protected static AndroidJavaObject _activity;

        static BaseAdClient()
        {
            _playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            _activity = _playerClass.GetStatic<AndroidJavaObject>("currentActivity");
        }

        protected AndroidJavaObject _ad;
        protected IResponseInfoClient _response;

        public BaseAdClient(string adClassName)
            : base(Utils.UnityAdListenerClassName)
        {
            _ad = new AndroidJavaObject(adClassName, _activity, this);
        }

        public IResponseInfoClient GetResponseInfoClient()
        {
            if (_response == null)
            {
                var responseObject = _ad.Call<AndroidJavaObject>("getResponseInfo");
                _response = new ResponseInfoClient(responseObject);
            }
            return _response;
        }

        public Reward GetRewardItem()
        {
            AndroidJavaObject rewardObject = _ad.Call<AndroidJavaObject>("getRewardItem");
            if (rewardObject == null)
            {
                return null;
            }
            string type = rewardObject.Call<string>("getType");
            int amount = rewardObject.Call<int>("getAmount");
            Reward rewardItem = new Reward()
            {
                Type = type,
                Amount = (double)amount
            };
            return rewardItem;
        }

        /// <summary>
        /// Called after ad load. Please override and call back to the API.
        /// </summary>
        protected virtual void OnAdLoaded()
        {
        }

        /// <summary>
        /// Called after a failed ad load. Please override and call back to the API.
        /// </summary>
        protected virtual void OnAdLoadFailed(AndroidJavaObject error)
        {
        }

        /// <summary>
        /// Called after user earns a reward. Please override and call back to the API.
        /// </summary>
        protected virtual void OnUserEarnedReward(Reward item)
        {
        }

        #region Callbacks from IAndroidAdListener

        public void onAdClickRecorded()
        {
            OnAdClickRecorded();
        }

        public void onAdImpressionRecorded()
        {
            OnAdImpressionRecorded();
        }

        public void onAdFullScreenContentClosed()
        {
            OnAdFullScreenContentClosed();
        }

        public void onAdFullScreenContentFailed(AndroidJavaObject error)
        {
            IAdErrorClient errorClient = new AdErrorClient(error);
            OnAdFullScreenContentFailed(errorClient);
        }

        public void onAdFullScreenContentOpened()
        {
            OnAdFullScreenContentOpened();
        }

        public void onAdPaid(int precision, long valueInMicros, string currencyCode)
        {
            AdValue adValue = new AdValue()
            {
                Precision = (AdValue.PrecisionType)precision,
                Value = valueInMicros,
                CurrencyCode = currencyCode
            };
            OnAdPaid(adValue);
        }

        public void onAdLoaded()
        {
            OnAdLoaded();
        }

        public void onAdLoadFailed(AndroidJavaObject error)
        {
            OnAdLoadFailed(error);
        }

        #endregion

        #region Callbacks from IAndroidRewardAdListener

        public void onUserEarnedReward(string type, float amount)
        {
            Reward rewardItem = new Reward()
            {
                Type = type,
                Amount = (double)amount
            };
            OnUserEarnedReward(rewardItem);
        }

        #endregion
    }
}
