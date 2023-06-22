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

using GoogleMobileAds.Api;
using GoogleMobileAds.Api.AdManager;
using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Android
{
    public class AdManagerBannerClient : BannerClient, IAdManagerBannerClient
    {
        public event Action<AppEvent> OnAppEvent;

        public List<AdSize> ValidAdSizes
        {
            get { return this._validAdSizes; }
            set
            {
                try
                {
                    if (value != null)
                    {
                        List<AndroidJavaObject> adSizeList = value.ConvertAll<AndroidJavaObject>(
                                adSize => Utils.GetAdSizeJavaObject(adSize));
                        AndroidJavaObject adSizeArrayList = new AndroidJavaObject(
                                "java.util.ArrayList");
                        foreach (AndroidJavaObject adSize in adSizeList)
                        {
                            adSizeArrayList.Call<bool>("add", adSize);
                        }
                        this.bannerView.Call("setAdSizes", adSizeArrayList);
                    }
                    else
                    {
                        this.bannerView.Call("setAdSizes", value);
                    }
                    this._validAdSizes = value;
                }
                catch (AndroidJavaException e)
                {
                    Debug.LogError(
                            "ValidAdSizes has an invalid value (or null / empty): " + e.Message);
                }
            }
        }

        private List<AdSize> _validAdSizes;

        private bool refreshValidAdSizes = true;

        public AdManagerBannerClient() : base(Utils.UnityAdManagerAdListenerClassName)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity =
                    playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            this.bannerView = new AndroidJavaObject(
                Utils.UnityAdManagerBannerViewClassName, activity, this);
        }

        // Loads an ad.
        public override void LoadAd(AdRequest request)
        {
            this.bannerView.Call("loadAd", Utils.GetAdManagerAdRequestJavaObject(request));
        }

        #region Callbacks from UnityAdManagerAdListener.

        public void onAppEvent(string name, string data)
        {
            if (this.OnAppEvent != null)
            {
                this.OnAppEvent(new AppEvent()
                {
                    Name = name,
                    Data = data,
                });
            }
        }

        #endregion
    }
}
