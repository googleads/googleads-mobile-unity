// Copyright 2024 Google LLC
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
    public interface INativeOverlayAdClient
    {
        // Ad event fired when the native ad has been received.
        event EventHandler<EventArgs> OnAdLoaded;
        // Ad event fired when the native ad has failed to load.
        event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;
        // Ad event fired when an ad impression has been recorded.
        event EventHandler<EventArgs> OnAdDidRecordImpression;
        // Ad event fired when the full screen content has been presented.
        event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;
        // Ad event fired when the full screen content has been dismissed.
        event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;
        // Ad event fired when the native ad is estimated to have earned money.
        event Action<AdValue> OnPaidEvent;
        // Ad event fired when an ad has been clicked.
        event Action OnAdClicked;

        // Loads a native ad using the provided adUnitId
        void Load(string adUnitId, AdRequest request, NativeAdOptions nativeOptions);

        // Render the native ad on the screen using templateViewStyle, adsize and adposition.
        void Render(NativeTemplateStyle templateViewStyle, AdSize adSize, AdPosition adPosition);

        // Render the native ad on the screen using templateViewStyle, adsize and x,y coordinates.
        void Render(NativeTemplateStyle templateViewStyle, AdSize adSize, int x, int y);

        // Render the native ad on the screen using templateViewStyle and adposition using default
        // sizing.
        void Render(NativeTemplateStyle templateViewStyle, AdPosition adPosition);

        // Render the native ad on the screen using templateViewStyle and x,y coordinates while
        // using default sizing..
        void Render(NativeTemplateStyle templateViewStyle, int x, int y);

        // Hides the native overlay from the screen.
        void Hide();

        // Shows the native overlay on the screen.
        void Show();

        // Set the position of the native overlay using standard position.
        void SetPosition(AdPosition adPosition);

        // Set the position of the native overlay using custom position.
        void SetPosition(int x, int y);

        // Destroys the native overlay ad.
        void DestroyAd();

        // Returns the height of the overlay in pixels.
        float GetHeightInPixels();

        // Returns the width of the overlay in pixels.
        float GetWidthInPixels();

        // Returns ad request Response info client.
        IResponseInfoClient GetResponseInfoClient();
    }
}
