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
    public class AppOpenAdClient : BaseAdClient,
                                   IAppOpenAdClient
    {

        private Action<IAppOpenAdClient, ILoadAdErrorClient> _loadCallback;

        public AppOpenAdClient() : base(Utils.UnityAppOpenAdClassName)
        {
        }

        public void LoadAd(string adUnitId,
                           ScreenOrientation orientation,
                           AdRequest request,
                           Action<IAppOpenAdClient, ILoadAdErrorClient> callback)
        {
            _loadCallback = callback;

            if (_ad != null)
            {
                _ad.Call("loadAd",
                    adUnitId,
                    Utils.GetAdRequestJavaObject(request),
                    Utils.GetAppOpenAdOrientation(orientation));
            }
        }

        #region Callbacks from IAppOpenAd

        public void Show()
        {
            if (_ad != null)
            {
                _ad.Call("show");
            }
        }

        public void Destroy()
        {
            _ad = null;
        }

        #endregion

        protected override void OnAdLoaded()
        {
            if (_loadCallback != null)
            {
                _loadCallback(this, null);
            }
            _loadCallback = null;
        }

        protected override void OnAdLoadFailed(AndroidJavaObject error)
        {
            if (_loadCallback != null)
            {
                _loadCallback(null, new LoadAdErrorClient(error));
            }
        }
    }
}
