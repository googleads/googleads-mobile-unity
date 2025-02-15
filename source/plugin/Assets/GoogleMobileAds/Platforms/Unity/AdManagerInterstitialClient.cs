// Copyright (C) 2023 Google LLC.
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
using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Unity
{
    public class AdManagerInterstitialClient : InterstitialClient, IAdManagerInterstitialClient
    {
        public event Action<AppEvent> OnAppEvent;

        public IAdManagerInterstitialClient PollAdManagerAd(string adUnitId)
        {
            Debug.Log("Preloaded ads are not supported on the Unity editor platform.");
            return new AdManagerInterstitialClient();
        }
    }
}
