// Copyright 2015-2021 Google LLC
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

using GoogleMobileAds.Api.AdManager;

namespace GoogleMobileAds.Common
{
    public interface IAdManagerInterstitialClient : IInterstitialClient
    {
        // Ad event fired when the ad sends a message to the application.
        event Action<AppEvent> OnAppEvent;

        // Returns the next pre-loaded ad manager interstitial ad and null if no ad is available.
        IAdManagerInterstitialClient PollAdManagerAd(string adUnitId);
    }
}
