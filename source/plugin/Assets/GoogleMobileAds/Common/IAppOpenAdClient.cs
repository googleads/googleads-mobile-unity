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
  public interface IAppOpenAdClient
  {
    // Ad event fired when the app open ad has been loaded.
    event EventHandler<EventArgs> OnAdLoaded;

    // Ad event fired when the app open ad has failed to load.
    event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;

    // Ad event fired when the app open ad is estimated to have earned money.
    event EventHandler<AdValueEventArgs> OnPaidEvent;

    // Ad event fired when the ad failed to present full screen content.
    event EventHandler<AdErrorClientEventArgs> OnAdFailedToPresentFullScreenContent;

    // Ad event fired when the ad presented the full screen content.
    event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;

    // Ad event fired when the ad dismissed full screen content.
    event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;

    // Ad event fired when an impression has been recorded for the ad.
    event EventHandler<EventArgs> OnAdDidRecordImpression;

    // Ad event fired when an ad is clicked.
    event Action OnAdClicked;

    // Creates an app open ad.
    void CreateAppOpenAd();

    // Loads an app open ad.
    void LoadAd(string adUnitID, AdRequest request);

    // Loads an app open ad.
    void LoadAd(string adUnitID, AdRequest request, ScreenOrientation orientation);

    // Shows the app open ad on the screen.
    void Show();

    // Returns ad request Response info client.
    IResponseInfoClient GetResponseInfoClient();

    // Destroys the app open ad.
    void DestroyAppOpenAd();
  }
}
