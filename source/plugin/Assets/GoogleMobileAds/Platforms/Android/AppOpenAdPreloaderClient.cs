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
        private AndroidJavaObject androidAppOpenAdPreloader;

        public AppOpenAdPreloaderClient() : base(Utils.PreloadCallbackClassname)
        {
            var playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            var activity =
                    playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            androidAppOpenAdPreloader = new AndroidJavaObject(
                    Utils.UnityAppOpenAdPreloaderClassName, activity, this);
        }

        public event Action<string, IAdErrorClient> OnAdFailedToPreload;
        public event Action<string, IResponseInfoClient> OnAdAvailable;
        public event Action<string> OnAdsExhausted;

        public bool Preload(string preloadId, PreloadConfiguration preloadConfiguration)
        {
            return androidAppOpenAdPreloader.Call<bool>("start", preloadId, preloadConfiguration);
        }

        public PreloadConfiguration GetConfiguration(string preloadId)
        {
            return Utils.GetPreloadConfiguration(
                androidAppOpenAdPreloader.Call<AndroidJavaObject>("getConfiguration", preloadId));
        }

        public Dictionary<string, PreloadConfiguration> GetConfigurations()
        {
            var configurations = new Dictionary<string, PreloadConfiguration>();
            var androidConfigurations =
                androidAppOpenAdPreloader.Call<AndroidJavaObject>("getConfigurations");
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

        public IAppOpenAdClient GetPreloadedAd(string preloadId)
        {
            var appOpenAdClient = new AppOpenAdClient();
            androidAppOpenAdPreloader.Call<AndroidJavaObject>("pollAd", preloadId);
            return appOpenAdClient;
        }

        public int GetNumberOfAdsAvailable(string preloadId)
        {
            return androidAppOpenAdPreloader.Call<int>("getNumberOfAdsAvailable", preloadId);
        }

        public bool IsAdAvailable(string preloadId)
        {
            return androidAppOpenAdPreloader.Call<bool>("isAdAvailable", preloadId);
        }

        public void Destroy(string preloadId)
        {
            androidAppOpenAdPreloader.Call("destroy", preloadId);
        }

        public void DestroyAll()
        {
            androidAppOpenAdPreloader.Call("destroyAll");
        }
    }
}
