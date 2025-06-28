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
using System.Collections.Generic;
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    public class RewardedAdPreloaderClient : AndroidJavaProxy, IRewardedAdPreloaderClient
    {
        private readonly AndroidJavaObject _unityRewardedAdPreloader;

        private Action<string, IAdErrorClient> _onAdFailedToPreload;
        private Action<string, IResponseInfoClient> _onAdPreloaded;
        private Action<string> _onAdsExhausted;

        public RewardedAdPreloaderClient() : base(Utils.PreloadCallbackClassname)
        {
            var playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            var activity =
                    playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            _unityRewardedAdPreloader = new AndroidJavaObject(
                    Utils.UnityRewardedAdPreloaderClassName, activity, this);
        }

        public bool Preload(string preloadId, PreloadConfiguration preloadConfiguration,
            Action<string, IResponseInfoClient> onAdPreloaded,
            Action<string, IAdErrorClient> onAdFailedToPreload,
            Action<string> onAdsExhausted)
        {
            _onAdFailedToPreload = onAdFailedToPreload;
            _onAdPreloaded = onAdPreloaded;
            _onAdsExhausted = onAdsExhausted;
            return _unityRewardedAdPreloader.Call<bool>("start", preloadId,
                Utils.GetPreloadConfigurationJavaObject(preloadConfiguration));
        }

        public bool IsAdAvailable(string preloadId)
        {
            return _unityRewardedAdPreloader.Call<bool>("isAdAvailable", preloadId);
        }

        public IRewardedAdClient DequeueAd(string preloadId)
        {
            var rewardedAdClient = new RewardedAdClient();
            var unityRewardedAd = _unityRewardedAdPreloader.Call<AndroidJavaObject>("pollAd",
                                preloadId, rewardedAdClient);
            if (unityRewardedAd == null)
            {
                return null;
            }
            rewardedAdClient.androidRewardedAd = unityRewardedAd;
            return rewardedAdClient;
        }

        public int GetNumAdsAvailable(string preloadId)
        {
            return _unityRewardedAdPreloader.Call<int>("getNumAdsAvailable", preloadId);
        }

        public PreloadConfiguration GetConfiguration(string preloadId)
        {
            return Utils.GetPreloadConfiguration(
                _unityRewardedAdPreloader.Call<AndroidJavaObject>("getConfiguration", preloadId));
        }

        public Dictionary<string, PreloadConfiguration> GetConfigurations()
        {
            var configurations = new Dictionary<string, PreloadConfiguration>();
            var androidConfigurations =
                _unityRewardedAdPreloader.Call<AndroidJavaObject>("getConfigurations");
            var keySet = androidConfigurations.Call<AndroidJavaObject>("keySet");
            var iterator = keySet.Call<AndroidJavaObject>("iterator");

            while (iterator.Call<bool>("hasNext"))
            {
                var key = iterator.Call<AndroidJavaObject>("next");
                var keyString = key.Call<string>("toString");
                var androidPreloadConfiguration =
                    androidConfigurations.Call<AndroidJavaObject>("get", key);
                configurations.Add(keyString,
                                   Utils.GetPreloadConfiguration(androidPreloadConfiguration));
            }

            return configurations;
        }

        public void Destroy(string preloadId)
        {
            _unityRewardedAdPreloader.Call("destroy", preloadId);
        }

        public void DestroyAll()
        {
            _unityRewardedAdPreloader.Call("destroyAll");
        }

        #region Callbacks from UnityPreloadCallback

        void onAdPreloaded(string preloadId, AndroidJavaObject responseInfo)
        {
            if (_onAdPreloaded != null)
            {
                _onAdPreloaded(preloadId, new ResponseInfoClient(ResponseInfoClientType.AdLoaded, responseInfo));
            }
        }

        void onAdFailedToPreload(string preloadId, AndroidJavaObject error)
        {
            if (_onAdFailedToPreload != null)
            {
                _onAdFailedToPreload(preloadId, new AdErrorClient(error));
            }
        }

        void onAdsExhausted(string preloadId)
        {
            if (_onAdsExhausted != null)
            {
                _onAdsExhausted(preloadId);
            }
        }

        #endregion

    }
}
