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
    public interface IBannerClient
    {
        // Ad event fired when the banner ad has been received.
        event EventHandler<EventArgs> OnAdLoaded;
        // Ad event fired when the banner ad has failed to load.
        event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;
        // Ad event fired when the banner ad is opened.
        event EventHandler<EventArgs> OnAdOpening;
        // Ad event fired when the banner ad is closed.
        event EventHandler<EventArgs> OnAdClosed;
        // Ad event fired when the banner ad is estimated to have earned money.
        event EventHandler<AdValueEventArgs> OnPaidEvent;
        // Ad event fired when the banner ad is clicked.
        event Action OnAdClicked;
        // Ad event fired when the banner ad records an impression.
        event Action OnAdImpressionRecorded;

        // Creates a banner view and adds it to the view hierarchy.
        void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position);

        // Creates a banner view and adds it to the view hierarchy with a custom position.
        void CreateBannerView(string adUnitId, AdSize adSize, int x, int y);

        // Requests a new ad for the banner view.
        void LoadAd(AdRequest request);

        // Shows the banner view on the screen.
        void ShowBannerView();

        // Hides the banner view from the screen.
        void HideBannerView();

        // Destroys a banner view.
        void DestroyBannerView();

        // Returns the height of the BannerView in pixels.
        float GetHeightInPixels();

        // Returns the width of the BannerView in pixels.
        float GetWidthInPixels();

        // Set the position of the banner view using standard position.
        void SetPosition(AdPosition adPosition);

        // Set the position of the banner view using custom position.
        void SetPosition(int x, int y);

        // Returns ad request Response info client.
        IResponseInfoClient GetResponseInfoClient();


    }
}
