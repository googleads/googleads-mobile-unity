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

using GoogleMobileAds;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    public class AdLoader
    {
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;
        public event EventHandler<CustomNativeEventArgs> OnCustomNativeTemplateAdLoaded;
        public Dictionary<string, Action<CustomNativeTemplateAd, string>> CustomNativeTemplateClickHandlers
        {
            get; private set;
        }
        public string AdUnitId { get; private set; }
        public HashSet<NativeAdType> AdTypes { get; private set; }
        public HashSet<string> TemplateIds { get; private set; }

        private IAdLoaderClient adLoaderClient;

        private AdLoader(Builder builder)
        {
            AdUnitId = string.Copy(builder.AdUnitId);
            CustomNativeTemplateClickHandlers =
                new Dictionary<string, Action<CustomNativeTemplateAd, string>>(builder.CustomNativeTemplateClickHandlers);
            TemplateIds = new HashSet<string>(builder.TemplateIds);
            AdTypes = new HashSet<NativeAdType>(builder.AdTypes);

            Dictionary<string, bool> templateIdsDictionary = new Dictionary<string, bool>();
            foreach(string templateId in TemplateIds)
                templateIdsDictionary[templateId] = false;
            foreach (var keyValuePair in CustomNativeTemplateClickHandlers)
                templateIdsDictionary[keyValuePair.Key] = true;
            AdLoaderClientArgs clientArgs = new AdLoaderClientArgs()
            {
                AdUnitId = AdUnitId,
                AdTypes = AdTypes,
                TemplateIds = templateIdsDictionary
            };
            adLoaderClient = MobileAds.GetClientFactory().BuildAdLoaderClient(clientArgs);

            Utils.CheckInitialization();

            adLoaderClient.OnCustomNativeTemplateAdLoaded += (object sender, CustomNativeClientEventArgs args) =>
            {
                CustomNativeTemplateAd nativeAd = new CustomNativeTemplateAd(args.nativeAdClient);
                CustomNativeEventArgs adEventArgs = new CustomNativeEventArgs() {
                    nativeAd = nativeAd
                };
                OnCustomNativeTemplateAdLoaded(this, adEventArgs);
            };
            adLoaderClient.OnCustomNativeTemplateAdClicked += (object sender, CustomNativeClientEventArgs args) =>
            {
                CustomNativeTemplateAd nativeAd = new CustomNativeTemplateAd(args.nativeAdClient);
                if (CustomNativeTemplateClickHandlers.ContainsKey(nativeAd.GetCustomTemplateId()))
                    CustomNativeTemplateClickHandlers[nativeAd.GetCustomTemplateId()](nativeAd, args.assetName);
            };
            adLoaderClient.OnAdFailedToLoad += (object sender, AdFailedToLoadEventArgs args) 
                => OnAdFailedToLoad?.Invoke(this, args);
        }

        public void LoadAd(AdRequest request) {
            adLoaderClient.LoadAd(request);
        }

        public class Builder
        {
            internal string AdUnitId { get; private set; }
            internal HashSet<NativeAdType> AdTypes { get; private set; }
            internal HashSet<string> TemplateIds { get; private set; }
            internal Dictionary<string, Action<CustomNativeTemplateAd, string>> CustomNativeTemplateClickHandlers
            {
                get; private set;
            }

            public Builder(string adUnitId)
            {
                AdUnitId = adUnitId;
                AdTypes = new HashSet<NativeAdType>();
                TemplateIds = new HashSet<string>();
                CustomNativeTemplateClickHandlers = new Dictionary<string, Action<CustomNativeTemplateAd, string>>();
            }

            public Builder ForCustomNativeAd(string templateId)
            {
                TemplateIds.Add(templateId);
                AdTypes.Add(NativeAdType.CustomTemplate);
                return this;
            }

            public Builder ForCustomNativeAd(string templateId, Action<CustomNativeTemplateAd, string> callback)
            {
                TemplateIds.Add(templateId);
                CustomNativeTemplateClickHandlers[templateId] = callback;
                AdTypes.Add(NativeAdType.CustomTemplate);
                return this;
            }

            public AdLoader Build()
            {
                return new AdLoader(this);
            }
        }
    }
}