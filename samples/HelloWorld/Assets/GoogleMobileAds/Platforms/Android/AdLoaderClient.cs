// Copyright (C) 2015 Google, Inc.
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
    public class AdLoaderClient : AndroidJavaProxy, IAdLoaderClient
    {
        private AndroidJavaObject adLoader;
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;
        public event EventHandler<CustomNativeClientEventArgs> OnCustomNativeTemplateAdLoaded;
        public event EventHandler<CustomNativeClientEventArgs> OnCustomNativeTemplateAdClicked;

        public AdLoaderClient(AdLoaderClientArgs args) : base(Utils.UnityAdLoaderListenerClassName)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity =
                playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            adLoader = new AndroidJavaObject(Utils.NativeAdLoaderClassName, activity,
                args.AdUnitId, this);

            bool supportsRequestImageAssetUrls = false;

            if (args.AdTypes.Contains(NativeAdType.CustomTemplate))
            {
                supportsRequestImageAssetUrls = false;
                foreach (var keyValuePair in args.TemplateIds)
                {
                    string templateID = keyValuePair.Key;
                    bool hasHandler = keyValuePair.Value;
                    adLoader.Call("configureCustomNativeTemplateAd", templateID,
                        hasHandler);
                }
            }
            if (supportsRequestImageAssetUrls) {
                adLoader.Call("configureReturnUrlsForImageAssets");
            }
            adLoader.Call("create");
        }

        public void LoadAd(AdRequest request)
        {
            adLoader.Call("loadAd", Utils.GetAdRequestJavaObject(request));
        }

        public void onCustomTemplateAdLoaded(AndroidJavaObject ad)
        {
            if (this.OnCustomNativeTemplateAdLoaded != null)
            {
                CustomNativeClientEventArgs args = new CustomNativeClientEventArgs()
                {
                    nativeAdClient = new CustomNativeTemplateClient(ad),
                    assetName = null
                };
                this.OnCustomNativeTemplateAdLoaded(this, args);
            }
        }

        void onAdFailedToLoad(string errorReason)
        {
            AdFailedToLoadEventArgs args = new AdFailedToLoadEventArgs()
            {
                Message = errorReason
            };
            OnAdFailedToLoad(this, args);
        }

        public void onCustomClick(AndroidJavaObject ad, string assetName)
        {
          if (this.OnCustomNativeTemplateAdClicked != null)
          {
              CustomNativeClientEventArgs args = new CustomNativeClientEventArgs()
              {
                  nativeAdClient = new CustomNativeTemplateClient(ad),
                  assetName = assetName
              };
              this.OnCustomNativeTemplateAdClicked(this, args);
          }
        }
    }
}
