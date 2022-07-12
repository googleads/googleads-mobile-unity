// Copyright (C) 2022 Google LLC
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
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    public class BannerAdClient : BaseAdClient,
                                  IBannerAdClient
    {

        public event Action OnBannerAdLoaded = delegate { };
        public event Action<ILoadAdErrorClient> OnBannerAdLoadFailed = delegate { };

        public BannerAdClient() : base(Utils.BannerViewClassName)
        {
        }

        public void LoadBannerAd(string adUnitId, AdSize adSize, AdPosition position)
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BannerAd LoadBannerAd");
            //END_DEBUG_STRIP
            if (_ad != null)
            {
                _ad.Call("create",
                          new object[3]
                          {
                              adUnitId,
                              Utils.GetAdSizeJavaObject(adSize),
                              (int)position
                          });
            }
        }

        public void LoadBannerAd(string adUnitId, AdSize adSize, int x, int y)
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BannerAd LoadBannerAd");
            //END_DEBUG_STRIP
            if (_ad != null)
            {
                _ad.Call("create",
                          new object[4]
                          {
                              adUnitId,
                              Utils.GetAdSizeJavaObject(adSize),
                              x,
                              y
                          });
            }
        }

        public void LoadAd(AdRequest request)
        {
            if (_ad != null)
            {
                _ad.Call("loadAd", Utils.GetAdRequestJavaObject(request));
            }
        }

        #region Callbacks from IBannerAd

        public void Show()
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BannerAd Show");
            //END_DEBUG_STRIP
            if (_ad != null)
            {
                _ad.Call("show");
            }
        }

        public void Hide()
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BannerAd Hide");
            //END_DEBUG_STRIP
            if (_ad != null)
            {
                _ad.Call("hide");
            }
        }

        public void Destroy()
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.BannerAd Destroy");
            //END_DEBUG_STRIP
            if (_ad != null)
            {
                _ad.Call("destroy");
                _ad = null;
            }
        }

        public float GetHeightInPixels()
        {
            return _ad == null ? 0 : _ad.Call<float>("getHeightInPixels");
        }

        public float GetWidthInPixels()
        {
            return _ad == null ? 0 : _ad.Call<float>("getWidthInPixels");
        }

        public void SetPosition(AdPosition adPosition)
        {
            if (_ad != null)
            {
                _ad.Call("setPosition", (int)adPosition);
            }
        }

        public void SetPosition(int x, int y)
        {
            if (_ad != null)
            {
                _ad.Call("setPosition", x, y);
            }
        }

        #endregion

        protected override void OnAdLoaded()
        {
            OnBannerAdLoaded();
        }

        protected override void OnAdLoadFailed(AndroidJavaObject error)
        {
            OnBannerAdLoadFailed(new  LoadAdErrorClient(error));
        }
    }
}
