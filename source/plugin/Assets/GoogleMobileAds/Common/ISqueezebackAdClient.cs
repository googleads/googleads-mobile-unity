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
    public interface ISqueezebackAdClient
    {
        // Ad event fired when the squeezeback ad has loaded.
        event Action OnAdLoaded;
        // Ad event fired when the squeezeback ad has failed to load.
        event Action<LoadAdErrorClientEventArgs> OnAdFailedToLoad;
        // Ad event fired when the squeezeback ad is displayed on screen (after squeeze animation).
        event Action OnAdShown;
        // Ad event fired when the squeezeback ad is hidden from screen (after unsqueeze animation).
        event Action OnAdHidden;
        // Ad event fired when an ad impression has been recorded.
        event Action OnAdDidRecordImpression;
        // Ad event fired when an ad has been clicked.
        event Action OnAdClicked;
        // Ad event fired when the full screen content has failed to be presented.
        event Action<AdErrorClientEventArgs> OnAdFailedToPresentFullScreenContent;
        // Ad event fired when the ad opens an overlay covering the screen.
        event Action OnAdDidPresentFullScreenContent;
        // Ad event fired when the ad overlay is dismissed.
        event Action OnAdDidDismissFullScreenContent;
        // Ad event fired when the squeezeback ad is estimated to have earned money.
        event Action<AdValue> OnPaidEvent;

        // Loads a squeezeback ad.
        void LoadAd(string adUnitId, AdRequest request);

        // Shows the squeezeback ad on screen.
        void Show();

        // Hides the squeezeback ad and restores the app content.
        void Hide();

        // Destroys the squeezeback ad.
        void Destroy();

        // Returns the response info for the loaded ad.
        IResponseInfoClient GetResponseInfoClient();
    }
}
