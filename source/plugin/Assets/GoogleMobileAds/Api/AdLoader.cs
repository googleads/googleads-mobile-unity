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
        private IAdLoaderClient adLoaderClient;

        private AdLoader(Builder builder)
        {
            this.AdUnitId = string.Copy(builder.AdUnitId);
            this.CustomNativeTemplateClickHandlers =
                    new Dictionary<string, Action<CustomNativeTemplateAd, string>>(
                    builder.CustomNativeTemplateClickHandlers);
            this.TemplateIds = new HashSet<string>(builder.TemplateIds);
            this.AdTypes = new HashSet<NativeAdType>(builder.AdTypes);

            Dictionary<string, bool> templateIdsDictionary = new Dictionary<string, bool>();
            foreach(string templateId in TemplateIds)
            {
              templateIdsDictionary[templateId] = false;
            }
            foreach (var keyValuePair in this.CustomNativeTemplateClickHandlers)
            {
              templateIdsDictionary[keyValuePair.Key] = true;
            }
            AdLoaderClientArgs clientArgs = new AdLoaderClientArgs(){
                  AdUnitId = this.AdUnitId,
                  AdTypes = this.AdTypes,
                  TemplateIds = templateIdsDictionary
              };
            this.adLoaderClient = GoogleMobileAdsClientFactory.BuildAdLoaderClient(clientArgs);

            Utils.CheckInitialization();

            this.adLoaderClient.OnCustomNativeTemplateAdLoaded +=
                    delegate (object sender, CustomNativeClientEventArgs args)
            {
                CustomNativeTemplateAd nativeAd = new CustomNativeTemplateAd(args.nativeAdClient);
                CustomNativeEventArgs adEventArgs = new CustomNativeEventArgs()
                {
                    nativeAd = nativeAd
                };
                this.OnCustomNativeTemplateAdLoaded(this, adEventArgs);
            };
            this.adLoaderClient.OnCustomNativeTemplateAdClicked +=
                     delegate (object sender, CustomNativeClientEventArgs args)
            {
                CustomNativeTemplateAd nativeAd = new CustomNativeTemplateAd(args.nativeAdClient);
                if (this.CustomNativeTemplateClickHandlers.ContainsKey(nativeAd.GetCustomTemplateId()))
                {
                    this.CustomNativeTemplateClickHandlers[nativeAd.GetCustomTemplateId()](nativeAd, args.assetName);
                }
            };
            this.adLoaderClient.OnAdFailedToLoad += delegate (
                object sender, AdFailedToLoadEventArgs args)
            {
                if (this.OnAdFailedToLoad != null)
                {
                    this.OnAdFailedToLoad(this, args);
                }
            };
        }

        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

        public event EventHandler<CustomNativeEventArgs> OnCustomNativeTemplateAdLoaded;

        public Dictionary<string, Action<CustomNativeTemplateAd, string>>
                CustomNativeTemplateClickHandlers
        {
            get; private set;
        }

        public string AdUnitId { get; private set; }

        public HashSet<NativeAdType> AdTypes { get; private set; }

        public HashSet<string> TemplateIds { get; private set; }

        public void LoadAd(AdRequest request)
        {
            this.adLoaderClient.LoadAd(request);
        }

        public class Builder
        {
            public Builder(string adUnitId)
            {
                this.AdUnitId = adUnitId;
                this.AdTypes = new HashSet<NativeAdType>();
                this.TemplateIds = new HashSet<string>();
                this.CustomNativeTemplateClickHandlers =
                        new Dictionary<string, Action<CustomNativeTemplateAd, string>>();
            }

            internal string AdUnitId { get; private set; }

            internal HashSet<NativeAdType> AdTypes { get; private set; }

            internal HashSet<string> TemplateIds { get; private set; }

            internal Dictionary<string, Action<CustomNativeTemplateAd, string>>
                    CustomNativeTemplateClickHandlers
            {
                get; private set;
            }

            public Builder ForCustomNativeAd(string templateId)
            {
                this.TemplateIds.Add(templateId);
                this.AdTypes.Add(NativeAdType.CustomTemplate);
                return this;
            }

            public Builder ForCustomNativeAd(
                    string templateId,
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
    }
}
