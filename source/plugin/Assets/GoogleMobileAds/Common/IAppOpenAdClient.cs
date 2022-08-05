// Copyright (C) 2021 Google, LLC
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

namespace GoogleMobileAds.Common
{
    /// <summary>
    /// App open ads are used to display ads when users enter your app. An AppOpenAd object
    /// contains all the data necessary to display an ad. Unlike interstitial ads, app open ads
    /// make it easy to provide an app branding so that users understand the context in which they
    /// see the ad.
    /// </summary>
    public interface IAppOpenAdClient : IBaseFullScreenAd
    {
        /// <summary>
        /// Loads an IAppOpenAd.
        /// </summary>
        void LoadAd(string adUnitId,
                    ScreenOrientation orientation,
                    AdRequest request,
                    Action<IAppOpenAdClient, ILoadAdErrorClient> callback);

        /// <summary>
        /// Shows the AppOpenAd on the screen.
        /// </summary>
        void Show();
    }
}
