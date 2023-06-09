
// Copyright (C) 2023 Google, Inc.
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
using System.Collections.Generic;

using GoogleMobileAds;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api.AdManager
{
    /// <summary>
    /// A full page ad experience at natural transition points,
    /// such as a page change or an app launch for Google Ad Manager publishers.
    /// Interstitials use a close button that removes the ad from the user's experience.
    /// </summary>
    public class AdManagerInterstitialAd : InterstitialAd
    {
        /// <summary>
        /// Raised when the app receives an event from the interstitial ad.
        /// </summary>
        public event Action<AppEvent> OnAppEventReceived;

        private AdManagerInterstitialAd(IAdManagerInterstitialClient client)
        {
            _client = client;
            _canShowAd = true;
            RegisterAdEvents();
        }

        /// <summary>
        /// Loads an AdManager interstitial ad.
        /// </summary>
        public static void Load(string adUnitId, AdRequest request,
                                Action<AdManagerInterstitialAd, LoadAdError> adLoadCallback)
        {
            if (adLoadCallback == null)
            {
                UnityEngine.Debug.LogError("adLoadCallback is null. No ad was loaded.");
                return;
            }

            var client = MobileAds.GetClientFactory().BuildAdManagerInterstitialClient();
            client.CreateInterstitialAd();
            client.OnAdLoaded += (sender, args) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    adLoadCallback(new AdManagerInterstitialAd(client), null);
                });
            };
            client.OnAdFailedToLoad += (sender, error) =>
            {
                var loadAdError = new LoadAdError(error.LoadAdErrorClient);
                MobileAds.RaiseAction(() =>
                {
                    adLoadCallback(null, loadAdError);
                });
            };
            client.LoadAd(adUnitId, request);
        }

        protected internal override void RegisterAdEvents()
        {
            base.RegisterAdEvents();

            ((IAdManagerInterstitialClient)_client).OnAppEvent += (appEvent) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAppEventReceived != null)
                    {
                        OnAppEventReceived(appEvent);
                    }
                });
            };
        }
    }
}
