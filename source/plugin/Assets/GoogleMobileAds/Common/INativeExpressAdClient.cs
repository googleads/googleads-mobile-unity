// Copyright (C) 2016 Google, Inc.
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
    internal interface INativeExpressAdClient
    {
        // Fired when the native express ad has been received.
        event EventHandler<EventArgs> OnAdLoaded;

        // Fired when the native express ad has failed to load.
        event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

        // Fired when the native express ad is opened.
        event EventHandler<EventArgs> OnAdOpening;

        // Fired when the native express ad is closed.
        event EventHandler<EventArgs> OnAdClosed;

        // Fired when the native express ad is leaving the application.
        event EventHandler<EventArgs> OnAdLeavingApplication;

        // Creates a native express ad view and adds it to the view hierarchy.
        void CreateNativeExpressAdView(string adUnitId, AdSize adSize, AdPosition position);

        // Creates a native express ad view and adds it to the view hierarchy with a custom position.
        void CreateNativeExpressAdView(string adUnitId, AdSize adSize, int x, int y);

        // Requests a new ad for the native express ad view.
        void LoadAd(AdRequest request);

        // Shows the native express ad view on the screen.
        void ShowNativeExpressAdView();

        // Hides the native express ad view from the screen.
        void HideNativeExpressAdView();

        // Destroys a native express ad view.
        void DestroyNativeExpressAdView();
    }
}
