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
    public class InterstitialAdPreloaderClient : AndroidJavaProxy, IInterstitialAdPreloaderClient
    {
        private readonly AndroidJavaObject _unityInterstitialPreloader;

        private Action<string, IResponseInfoClient> _onAdPreloaded;
        private Action<string, IAdErrorClient> _onAdFailedToPreload;
        private Action<string> _onAdsExhausted;

        public InterstitialAdPreloaderClient() : base(Utils.PreloadCallbackClassname)
        {
            var playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            var activity =
                    playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            _unityInterstitialPreloader = new AndroidJavaObject(
                    Utils.UnityInterstitialAdPreloaderClassName, activity, this);
        }

        public bool Preload(string preloadId, PreloadConfiguration preloadConfiguration,
            Action<string, IResponseInfoClient> onAdPreloaded,
            Action<string, IAdErrorClient> onAdFailedToPreload,
            Action<string> onAdsExhausted)
        {
            _onAdFailedToPreload = onAdFailedToPreload;
            _onAdPreloaded = onAdPreloaded;
            _onAdsExhausted = onAdsExhausted;
            return _unityInterstitialPreloader.Call<bool>("start", preloadId,
                Utils.GetPreloadConfigurationJavaObject(preloadConfiguration));
        }

        public bool IsAdAvailable(string preloadId)
        {
            return _unityInterstitialPreloader.Call<bool>("isAdAvailable", preloadId);
        }

        public IInterstitialClient DequeueAd(string preloadId)
        {
            var interstitialClient = new InterstitialClient();
            var unityInterstitialAd = _unityInterstitialPreloader.Call<AndroidJavaObject>("pollAd",
                preloadId, interstitialClient);
            if (unityInterstitialAd == null)
            {
                return null;
            }
            interstitialClient.androidInterstitialAd = unityInterstitialAd;
            return interstitialClient;
        }

        public int GetNumAdsAvailable(string preloadId)
        {
            return _unityInterstitialPreloader.Call<int>("getNumAdsAvailable", preloadId);
        }

        public PreloadConfiguration GetConfiguration(string preloadId)
        {
            return Utils.GetPreloadConfiguration(
                _unityInterstitialPreloader.Call<AndroidJavaObject>("getConfiguration", preloadId));
        }

        public Dictionary<string, PreloadConfiguration> GetConfigurations()
        {
            var configurations = new Dictionary<string, PreloadConfiguration>();
            var androidConfigurations =
                _unityInterstitialPreloader.Call<AndroidJavaObject>("getConfigurations");
            var keySet = androidConfigurations.Call<AndroidJavaObject>("keySet");
            var iterator = keySet.Call<AndroidJavaObject>("iterator");

            while (iterator.Call<bool>("hasNext"))
            {
                var key = iterator.Call<AndroidJavaObject>("next");
                var keyString = key.Call<string>("toString");
                var androidPreloadConfiguration = androidConfigurations.Call<AndroidJavaObject>(
                        "get", key);
                configurations.Add(keyString, Utils.GetPreloadConfiguration(
                        androidPreloadConfiguration));
            }

            return configurations;
        }

        public void Destroy(string preloadId)
        {
            _unityInterstitialPreloader.Call("destroy", preloadId);
        }

        public void DestroyAll()
        {
            _unityInterstitialPreloader.Call("destroyAll");
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
