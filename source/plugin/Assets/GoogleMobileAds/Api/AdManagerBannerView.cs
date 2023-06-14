// Copyright (C) 2023 Google, Inc.
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

using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api.AdManager
{
    /// <summary>
    /// Banner view that works with Google Ad Manager. Banner views occupy a spot within an app's
    /// layout. They stay on screen while users are interacting with the app.
    /// </summary>
    public class AdManagerBannerView : BannerView
    {
        /// <summary>
        /// Raised when the app receives an event from the banner ad.
        /// </summary>
        public event Action<AppEvent> OnAppEventReceived;

        /// <summary>
        /// Sets the supported sizes of the banner ad. In most cases, only one ad size will be
        /// specified. Use one of the predefined standard ad sizes (such as AdSize.MediumRectangle),
        /// or create one by specifying width and height.
        ///
        /// <para>Multiple ad sizes can be specified if your application can appropriately handle
        /// multiple ad sizes. For example, your application might read <see cref="ValidAdSizes"/>
        /// during the <see cref="AdManagerBannerView#OnBannerAdLoaded"/> callback and change the
        /// layout according to the size of the ad that was loaded. If multiple ad sizes are
        /// specified, the <see cref="AdManagerBannerView"/> will assume the size of the first ad
        /// size until an ad is loaded.</para>
        ///
        /// <para>This method also immediately resizes the currently displayed ad, so calling this
        /// method after an ad has been loaded is not recommended unless you know for certain that
        /// the content of the ad will render correctly in the new ad size. This can be used if an
        /// ad needs to be resized after it has been loaded. If more than one ad size is specified,
        /// the currently displayed ad will be resized to the first ad size.</para>
        /// </summary>
        public List<AdSize> ValidAdSizes
        {
            get { return ((IAdManagerBannerClient)_client).ValidAdSizes; }
            set { ((IAdManagerBannerClient)_client).ValidAdSizes = value; }
        }

        /// <summary>
        /// Creates an Ad Manager banner view with a standard position.
        /// </summary>
        public AdManagerBannerView(string adUnitId, AdSize adSize, AdPosition position)
        {
            _client = MobileAds.GetClientFactory().BuildAdManagerBannerClient();
            _client.CreateBannerView(adUnitId, adSize, position);
            ConfigureBannerEvents();
        }

        /// <summary>
        /// Creates an Ad Manager banner view with a custom position.
        /// </summary>
        public AdManagerBannerView(string adUnitId, AdSize adSize, int x, int y)
        {
            _client = MobileAds.GetClientFactory().BuildAdManagerBannerClient();
            _client.CreateBannerView(adUnitId, adSize, x, y);
            ConfigureBannerEvents();
        }

        protected internal override void ConfigureBannerEvents()
        {
            base.ConfigureBannerEvents();

            ((IAdManagerBannerClient)_client).OnAppEvent += (appEvent) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAppEventReceived != null)
                    {
                        OnAppEventReceived(appEvent);
                    }
                });
            };
        }
    }
}
