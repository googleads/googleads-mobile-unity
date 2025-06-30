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
    public class AppOpenAdPreloaderClient : AndroidJavaProxy, IAppOpenAdPreloaderClient
    {
        private readonly AndroidJavaObject _unityAppOpenAdPreloader;

        private Action<string, IResponseInfoClient> _onAdPreloaded;
        private Action<string, IAdErrorClient> _onAdFailedToPreload;
        private Action<string> _onAdsExhausted;

        public AppOpenAdPreloaderClient() : base(Utils.PreloadCallbackClassname)
        {
            var playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            var activity =
                    playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            _unityAppOpenAdPreloader = new AndroidJavaObject(
                    Utils.UnityAppOpenAdPreloaderClassName, activity, this);
        }

        public bool Preload(string preloadId, PreloadConfiguration preloadConfiguration,
            Action<string, IResponseInfoClient> onAdPreloaded,
            Action<string, IAdErrorClient> onAdFailedToPreload,
            Action<string> onAdsExhausted)
        {
            _onAdFailedToPreload = onAdFailedToPreload;
            _onAdPreloaded = onAdPreloaded;
            _onAdsExhausted = onAdsExhausted;
            return _unityAppOpenAdPreloader.Call<bool>("start", preloadId,
                Utils.GetPreloadConfigurationJavaObject(preloadConfiguration));
        }

        public bool IsAdAvailable(string preloadId)
        {
            return _unityAppOpenAdPreloader.Call<bool>("isAdAvailable", preloadId);
        }

        public IAppOpenAdClient DequeueAd(string preloadId)
        {
            var appOpenAdClient = new AppOpenAdClient();
            var unityAppOpenAd = _unityAppOpenAdPreloader.Call<AndroidJavaObject>("pollAd",
                    preloadId, appOpenAdClient);
            if (unityAppOpenAd == null)
            {
                return null;
            }
            appOpenAdClient.androidAppOpenAd = unityAppOpenAd;
            return appOpenAdClient;
        }

        public int GetNumAdsAvailable(string preloadId)
        {
            return _unityAppOpenAdPreloader.Call<int>("getNumAdsAvailable", preloadId);
        }

        public PreloadConfiguration GetConfiguration(string preloadId)
        {
            return Utils.GetPreloadConfiguration(
                _unityAppOpenAdPreloader.Call<AndroidJavaObject>("getConfiguration", preloadId));
        }

        public Dictionary<string, PreloadConfiguration> GetConfigurations()
        {
            var configurations = new Dictionary<string, PreloadConfiguration>();
            var androidConfigurations =
                _unityAppOpenAdPreloader.Call<AndroidJavaObject>("getConfigurations");
            var keySet = androidConfigurations.Call<AndroidJavaObject>("keySet");
            var iterator = keySet.Call<AndroidJavaObject>("iterator");

            while (iterator.Call<bool>("hasNext"))
            {
                var key = iterator.Call<AndroidJavaObject>("next");
                var keyString = key.Call<string>("toString");
                var androidPreloadConfiguration = androidConfigurations.Call<AndroidJavaObject>("get", key);
                configurations.Add(keyString, Utils.GetPreloadConfiguration(androidPreloadConfiguration));
            }

            return configurations;
        }

        public void Destroy(string preloadId)
        {
            _unityAppOpenAdPreloader.Call("destroy", preloadId);
        }

        public void DestroyAll()
        {
            _unityAppOpenAdPreloader.Call("destroyAll");
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
