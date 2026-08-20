// Copyright 2026 Google LLC
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
    public interface IPictureInPictureAdClient
    {
        // Ad event fired when the picture in picture ad has loaded.
        event EventHandler<EventArgs> OnAdLoaded;
        // Ad event fired when the picture in picture ad has failed to load.
        event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;
        // Ad event fired when the picture in picture ad is displayed on screen.
        event EventHandler<EventArgs> OnAdShown;
        // Ad event fired when the picture in picture ad is hidden from screen.
        event EventHandler<EventArgs> OnAdHidden;
        // Ad event fired when an ad impression has been recorded.
        event EventHandler<EventArgs> OnAdDidRecordImpression;
        // Ad event fired when an ad has been clicked.
        event Action OnAdClicked;
        // Ad event fired when the ad opens an overlay covering the screen.
        event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;
        // Ad event fired when the ad overlay is dismissed.
        event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;
        // Ad event fired when the picture in picture ad is estimated to have earned money.
        event Action<AdValue> OnPaidEvent;

        // Creates a picture in picture ad wrapper.
        void CreatePictureInPictureAd();

        // Loads a picture in picture ad.
        void LoadAd(string adUnitId, AdRequest request);

        // Shows the picture in picture ad on screen.
        void Show(PictureInPictureAdPosition position);

        // Hides the picture in picture ad.
        void Hide();

        // Destroys the picture in picture ad.
        void Destroy();

        // Returns the current or last known position of the picture in picture ad.
        PictureInPictureAdPosition GetAdPosition();

        // Returns the response info for the loaded ad.
        IResponseInfoClient GetResponseInfoClient();
    }
}
