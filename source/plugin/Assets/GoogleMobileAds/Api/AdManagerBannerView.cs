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
    public class AdManagerBannerView
    {
        /// <summary>
        /// Raised when an ad is loaded into the banner view.
        /// </summary>
        public event Action OnBannerAdLoaded;

        /// <summary>
        /// Raised when an ad fails to load into the banner view.
        /// </summary>
        public event Action<LoadAdError> OnBannerAdLoadFailed;

        /// <summary>
        /// Raised when the ad is estimated to have earned money.
        /// </summary>
        public event Action<AdValue> OnAdPaid;

        /// <summary>
        /// Raised when an ad is clicked.
        /// </summary>
        public event Action OnAdClicked;

        /// <summary>
        /// Raised when an impression is recorded for an ad.
        /// </summary>
        public event Action OnAdImpressionRecorded;

        /// <summary>
        /// Raised when an ad opened full-screen content.
        /// </summary>
        public event Action OnAdFullScreenContentOpened;

        /// <summary>
        /// Raised when the ad closed full-screen content.
        /// On iOS, this event is only raised when an ad opens an overlay, not when opening a new
        /// application such as Safari or the App Store.
        /// </summary>
        public event Action OnAdFullScreenContentClosed;

        /// <summary>
        /// Raised when the app receives an event from the banner ad.
        /// </summary>
        public event Action<AppEvent> OnAppEventReceived;

        /// <summary>
        /// Returns true if Destroy() has been called.
        /// </summary>
        public bool IsDestroyed { get { return _client == null; } }

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
            get { return _client.ValidAdSizes; }
            set { _client.ValidAdSizes = value; }
        }

        internal IAdManagerBannerClient _client;

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

        /// <summary>
        /// Destroys the banner view.
        /// </summary>
        public void Destroy()
        {
            if (_client != null)
            {
                _client.DestroyBannerView();
                _client = null;
            }
        }

        /// <summary>
        /// Returns the ad unit ID.
        /// </summary>
        public string GetAdUnitID()
        {
            return _client != null ? _client.GetAdUnitID() : null;
        }

        /// <summary>
        /// Returns the ad request response info or null if the ad is not loaded.
        /// </summary>
        public ResponseInfo GetResponseInfo()
        {
            return _client != null ? new ResponseInfo(_client.GetResponseInfoClient()) : null;
        }

        /// <summary>
        /// Returns the height of the banner view in pixels.
        /// </summary>
        public float GetHeightInPixels()
        {
            return _client != null ? _client.GetHeightInPixels() : 0;
        }

        /// <summary>
        /// Returns the width of the banner view in pixels.
        /// </summary>
        public float GetWidthInPixels()
        {
            return _client != null ? _client.GetWidthInPixels() : 0;
        }

        /// <summary>
        /// Loads an ad into the banner view.
        /// </summary>
        public void LoadAd(AdRequest request)
        {
            if (_client != null)
            {
                _client.LoadAd(request);
            }
        }

        /// <summary>
        /// Shows the banner view.
        /// </summary>
        public void Show()
        {
            if (_client != null)
            {
                _client.ShowBannerView();
            }
        }

        /// <summary>
        /// Hides the banner view.
        /// </summary>
        public void Hide()
        {
            if (_client != null)
            {
                _client.HideBannerView();
            }
        }

        /// <summary>
        /// Sets the position of the banner view using standard position.
        /// </summary>
        public void SetPosition(AdPosition adPosition)
        {
            if (_client != null)
            {
                _client.SetPosition(adPosition);
            }
        }

        /// <summary>
        /// Sets the position of the banner view using custom position.
        /// </summary>
        public void SetPosition(int x, int y)
        {
            if (_client != null)
            {
                _client.SetPosition(x, y);
            }
        }

        /// <summary>
        /// Indicates whether the last loaded ad is a collapsible banner.
        /// </summary>
        public bool IsCollapsible()
        {
            return _client == null ? false : _client.IsCollapsible();
        }

        private void ConfigureBannerEvents()
        {
            
            _client.OnAdLoaded += (sender, args) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnBannerAdLoaded != null)
                    {
                        OnBannerAdLoaded();
                    }
                });
            };

            _client.OnAdFailedToLoad += (sender, args) =>
            {
                LoadAdError loadAdError = new LoadAdError(args.LoadAdErrorClient);
                MobileAds.RaiseAction(() =>
                {
                    if (OnBannerAdLoadFailed != null)
                    {
                        OnBannerAdLoadFailed(loadAdError);
                    }
                });
            };

            _client.OnAdOpening += (sender, args) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdFullScreenContentOpened != null)
                    {
                        OnAdFullScreenContentOpened();
                    }
                });
            };

            _client.OnAdClosed += (sender, args) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdFullScreenContentClosed != null)
                    {
                        OnAdFullScreenContentClosed();
                    }
                });
            };

            _client.OnPaidEvent += (adValue) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdPaid != null)
                    {
                        OnAdPaid(adValue);
                    }
                });
            };

            _client.OnAdClicked += () =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdClicked != null)
                    {
                        OnAdClicked();
                    }
                });
            };

            _client.OnAdImpressionRecorded += () =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdImpressionRecorded != null)
                    {
                        OnAdImpressionRecorded();
                    }
                });
            };

            _client.OnAppEvent += (appEvent) =>
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
