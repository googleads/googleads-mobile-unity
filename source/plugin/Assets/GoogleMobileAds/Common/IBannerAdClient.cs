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

using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
    /// <summary>
    /// Banner views occupy a spot within an app's layout. They stay on screen while users are
    /// interacting with the app, and can refresh automatically after a certain period of time.
    /// </summary>
    public interface IBannerAdClient : IBaseAdClient
    {
        /// <summary>
        /// Raised when an ad is loaded into the banner view.
        /// </summary>
        event Action OnBannerAdLoaded;

        /// <summary>
        /// Raised when an ad fails to load into the banner view.
        /// </summary>
        event Action<ILoadAdErrorClient> OnBannerAdLoadFailed;

        /// <summary>
        /// Raised when an ad opened full screen content.
        /// </summary>
        event Action OnAdFullScreenContentOpened;

        /// <summary>
        /// Raised when the ad closed full screen content.
        /// On iOS this does not include ads which open (safari) web browser links.
        /// </summary>
        event Action OnAdFullScreenContentClosed;

        /// <summary>
        /// Loads an ad into the banner view.
        /// </summary>
        void LoadAd(AdRequest request);

        /// <summary>
        /// Shows the banner view on the screen.
        /// </summary>
        void ShowAd();

        /// <summary>
        /// Hides the banner view from the screen.
        /// </summary>
        void HideAd();

        /// <summary>
        /// Returns the height of the banner view in pixels.
        /// </summary>
        float GetHeightInPixels();

        /// <summary>
        /// Returns the width of the banner view in pixels.
        /// </summary>
        float GetWidthInPixels();

        /// <summary>
        /// Set the position of the banner view using standard position.
        /// </summary>
        void SetPosition(AdPosition adPosition);

        /// <summary>
        /// Set the position of the banner view using custom position.
        /// </summary>
        void SetPosition(int x, int y);

        /// <summary>
        /// Creates a banner view and adds it to the view hierarchy.
        /// </summary>
        void CreateBannerAd(string adUnitId, AdSize adSize, AdPosition position);

        /// <summary>
        /// Creates a banner view with a custom position.
        /// </summary>
        void CreateBannerAd(string adUnitId, AdSize adSize, int x, int y);
    }
}
