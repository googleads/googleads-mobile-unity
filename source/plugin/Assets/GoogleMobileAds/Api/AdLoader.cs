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
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    public enum NativeAdType
    {
        CustomTemplate = 0,
    }

    public class AdLoader
    {
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;
        public event EventHandler<CustomNativeEventArgs> onCustomNativeTemplateAdLoaded;

        private IAdLoaderClient adLoaderClient;

        public string AdUnitId { get; private set; }
        public HashSet<NativeAdType> AdTypes { get; private set; }
        public HashSet<string> TemplateIds { get; private set; }
        public Dictionary<string, Action<CustomNativeTemplateAd, string>>
                CustomNativeTemplateClickHandlers { get; private set; }

        public class Builder
        {
            internal string AdUnitId { get; private set; }
            internal HashSet<NativeAdType> AdTypes { get; private set; }
            internal HashSet<string> TemplateIds { get; private set; }
            internal Dictionary<string, Action<CustomNativeTemplateAd, string>>
                    CustomNativeTemplateClickHandlers { get; private set; }

            public Builder(string adUnitId)
            {
                this.AdUnitId = adUnitId;
                this.AdTypes = new HashSet<NativeAdType>();
                this.TemplateIds = new HashSet<string>();
                this.CustomNativeTemplateClickHandlers = new Dictionary<string,
                        Action<CustomNativeTemplateAd, string>>();
            }

            public Builder forCustomNativeAd(string templateId)
            {
                this.TemplateIds.Add(templateId);
                this.AdTypes.Add(NativeAdType.CustomTemplate);
                return this;
            }

            public Builder forCustomNativeAd(string templateId,
                        Action<CustomNativeTemplateAd, string> callback)
            {
                this.TemplateIds.Add(templateId);
                this.CustomNativeTemplateClickHandlers[templateId] = callback;
                this.AdTypes.Add(NativeAdType.CustomTemplate);
                return this;
            }

            public AdLoader Build()
            {
                return new AdLoader(this);
            }
        }

        private AdLoader(Builder builder)
        {
            AdUnitId = String.Copy(builder.AdUnitId);
            CustomNativeTemplateClickHandlers = new Dictionary<string,
                Action<CustomNativeTemplateAd, string>>(builder.CustomNativeTemplateClickHandlers);
            TemplateIds = new HashSet<string>(builder.TemplateIds);
            AdTypes = new HashSet<NativeAdType>(builder.AdTypes);

            adLoaderClient = GoogleMobileAdsClientFactory.BuildAdLoaderClient(this);

            adLoaderClient.onCustomNativeTemplateAdLoaded +=
                    delegate(object sender, CustomNativeEventArgs args) {
                onCustomNativeTemplateAdLoaded(this, args);
            };

            adLoaderClient.OnAdFailedToLoad += delegate(object sender, AdFailedToLoadEventArgs args)
            {
                OnAdFailedToLoad(this, args);
            };
        }

        public void LoadAd(AdRequest request)
        {
            adLoaderClient.LoadAd(request);
        }
    }
}
