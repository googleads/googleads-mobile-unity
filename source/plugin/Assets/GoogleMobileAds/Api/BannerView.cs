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
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// Banner views occupy a spot within an app's layout. They stay on screen while users are
    /// interacting with the app, and can refresh automatically after a certain period of time
    /// </summary>
    public class BannerView : BaseAd
    {
        /// <summary>
        /// Raised when an ad is loaded into the banner view.
        /// </summary>
        public event Action OnBannerAdLoaded = delegate{};

        /// <summary>
        /// Raised when an ad fails to load into the banner view.
        /// </summary>
        public event Action<LoadAdError> OnBannerAdLoadFailed = delegate{};

        /// <summary>
        /// Raised when an ad opened full screen content.
        /// </summary>
        public event Action OnAdFullScreenContentOpened = delegate{};

        /// <summary>
        /// Raised when the ad closed full screen content.
        /// On iOS this does not include ads which open (safari) web browser links.
        /// </summary>
        public event Action OnAdFullScreenContentClosed = delegate{};

        // These are the ad callback events that can be hooked into.
        [Obsolete("Use OnBannerAdLoaded.")]
        public event EventHandler<EventArgs> OnAdLoaded = delegate{};
        [Obsolete("Use OnBannerAdLoadFailed.")]
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad = delegate{};
        [Obsolete("Use OnFullScreenAdOpened.")]
        public event EventHandler<EventArgs> OnAdOpening = delegate{};
        [Obsolete("Use OnFullScreenAdClosed.")]
        public event EventHandler<EventArgs> OnAdClosed = delegate{};
        // Called when an ad is estimated to have earned money.
        [Obsolete("Use OnAdPaid.")]
        public event EventHandler<AdValueEventArgs> OnPaidEvent = delegate{};

        private readonly IBannerAdClient _client;

        /// <summary>
        /// Creates a banner view and adds it to the view hierarchy.
        /// </summary>
        public BannerView(string adUnitId, AdSize adSize, AdPosition position)
        {
            _client = MobileAds.GetClientFactory().BuildBannerAdClient();
            _client.CreateBannerAd(adUnitId, adSize, position);
            Init();
        }

        /// <summary>
        /// Creates a banner view with a custom position.
        /// </summary>
        public BannerView(string adUnitId, AdSize adSize, int x, int y)
        {
            _client = MobileAds.GetClientFactory().BuildBannerAdClient();
            _client.CreateBannerAd(adUnitId, adSize, x, y);
            Init();
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
        /// Loads an into the banner view.
        /// </summary>
        public void LoadAd(AdRequest request)
        {
            if (_client != null)
            {
                _client.LoadAd(request);
            }
        }

        /// <summary>
        /// Shows the banner view from the screen.
        /// </summary>
        public void Show()
        {
            if (_client != null)
            {
                _client.ShowAd();
            }
        }

        /// <summary>
        /// Hides the banner view from the screen.
        /// </summary>
        public void Hide()
        {
            if (_client != null)
            {
                _client.HideAd();
            }
        }

        /// <summary>
        /// Set the position of the BannerView using standard position.
        /// </summary>
        public void SetPosition(AdPosition adPosition)
        {
            if (_client != null)
            {
                _client.SetPosition(adPosition);
            }
        }

        /// <summary>
        /// Set the position of the banner view using custom position.
        /// </summary>
        public void SetPosition(int x, int y)
        {
            if (_client != null)
            {
                _client.SetPosition(x, y);
            }
        }

        private void Init()
        {
            base.Init(_client);
            _client.OnBannerAdLoaded += () =>
            {
                OnAdLoaded(this, EventArgs.Empty);
                OnBannerAdLoaded();
            };
            _client.OnBannerAdLoadFailed += (error) =>
            {
                var loadError = new LoadAdError(error);
                OnAdFailedToLoad(this, new AdFailedToLoadEventArgs
                {
                    LoadAdError = loadError
                });
                OnBannerAdLoadFailed(loadError);
            };
            OnAdPaid += (adValue) =>
            {
                OnPaidEvent(this, new AdValueEventArgs
                {
                    AdValue = adValue
                });
            };
            _client.OnAdFullScreenContentClosed += () =>
            {
                OnAdClosed(this, EventArgs.Empty);
                OnAdFullScreenContentClosed();
            };
            _client.OnAdFullScreenContentOpened += () =>
            {
                OnAdOpening(this, EventArgs.Empty);
                OnAdFullScreenContentOpened();
            };
        }
    }
}
