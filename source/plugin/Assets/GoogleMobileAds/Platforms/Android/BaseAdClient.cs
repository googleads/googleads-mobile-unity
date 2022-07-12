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

        public IResponseInfoClient Response
        {
            get
            {
                if (_response == null)
                {
                    _response = GetResponse();
                }
                return _response;
            }
        }

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
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BaseAd()");
            //END_DEBUG_STRIP
            _ad = new AndroidJavaObject(adClassName, _activity, this);
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

        /// <summary>
        /// returns the ResponseInfo.
        /// </summary>
        protected ResponseInfo GetResponse()
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BaseAd GetResponse");
            //END_DEBUG_STRIP
            var responseObject = _ad.Call<AndroidJavaObject>("getResponseInfo");
            var responseInfo = new ResponseInfo(responseObject);
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BaseAd "+ responseInfo);
            //END_DEBUG_STRIP
            return responseInfo;
        }

        /// <summary>
        /// returns the Reward if applicable.
        /// </summary>
        protected Reward GetRewardItem()
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BaseAd GetRewardItem");
            //END_DEBUG_STRIP
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
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BaseAd "+ rewardItem);
            //END_DEBUG_STRIP
            return rewardItem;
        }

        #region Callbacks from IAndroidAdListener

        public void onAdClickRecorded()
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BaseAd onAdClickRecorded");
            //END_DEBUG_STRIP
            OnAdClickRecorded();
        }

        public void onAdImpressionRecorded()
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BaseAd onAdImpressionRecorded");
            //END_DEBUG_STRIP
            OnAdImpressionRecorded();
        }

        public void onAdFullScreenContentClosed()
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BaseAd onAdFullScreenContentClosed");
            //END_DEBUG_STRIP
            OnAdFullScreenContentClosed();
        }

        public void onAdFullScreenContentFailed(AndroidJavaObject error)
        {
            IAdErrorClient errorClient = new AdErrorClient(error);
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BaseAd onAdFullScreenContentFailed");
            UnityEngine.Debug.Log("AndroidClient "+ errorClient);
            //END_DEBUG_STRIP
            OnAdFullScreenContentFailed(errorClient);
        }

        public void onAdFullScreenContentOpened()
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BaseAd onAdFullScreenContentOpened");
            //END_DEBUG_STRIP
            OnAdFullScreenContentOpened();
        }

        public void onAdPaid(int precision, long valueInMicros, string currencyCode)
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BaseAd onAdPaid");
            //END_DEBUG_STRIP
            AdValue adValue = new AdValue()
            {
                Precision = (AdValue.PrecisionType)precision,
                Value = valueInMicros,
                CurrencyCode = currencyCode
            };
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BaseAd "+ adValue);
            //END_DEBUG_STRIP
            OnAdPaid(adValue);
        }

        public void onAdLoaded()
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BaseAd onAdPaid");
            //END_DEBUG_STRIP
            OnAdLoaded();
        }

        public void onAdLoadFailed(AndroidJavaObject error)
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BaseAd onAdPaid");
            //END_DEBUG_STRIP
            OnAdLoadFailed(error);
        }

        #endregion

        #region Callbacks from IAndroidRewardAdListener

        public void onUserEarnedReward(string type, float amount)
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BaseAd onUserEarnedReward");
            //END_DEBUG_STRIP
            Reward rewardItem = new Reward()
            {
                Type = type,
                Amount = (double)amount
            };
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BaseAd "+rewardItem);
            //END_DEBUG_STRIP
            OnUserEarnedReward(rewardItem);
        }

        #endregion
    }
}
