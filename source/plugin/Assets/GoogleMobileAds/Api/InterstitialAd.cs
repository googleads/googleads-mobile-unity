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
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// A full page ad experience at natural transition points such as a page change, an app launch.
    /// Interstitials use a close button that removes the ad from the user's experience.
    /// </summary>
    public class InterstitialAd : FullScreenAd
    {
        /// <summary>
        /// Loads an interstitial ad.
        /// </summary>
        public static void LoadAd(string adUnitId,
                                  AdRequest request,
                                  Action<InterstitialAd, LoadAdError> callback)
        {
            Action<IInterstitialAdClient, ILoadAdErrorClient> apiCallback = (ad, error) =>
            {
                callback(
                    ad == null ? null : new InterstitialAd(ad),
                    error == null ? null : new LoadAdError(error));
            };
            var loader = MobileAds.GetClientFactory().BuildInterstitialAdClient();
            loader.LoadAd(adUnitId, request, apiCallback);
        }

        [Obsolete("Use InterstitialAd.LoadAd().")]
        public event EventHandler<EventArgs> OnAdLoaded = delegate{};
        [Obsolete("Use InterstitialAd.LoadAd().")]
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad = delegate{};
        [Obsolete("Use OnFullScreenAdOpened.")]
        public event EventHandler<EventArgs> OnAdOpening = delegate{};
        [Obsolete("Use OnFullScreenAdClosed.")]
        public event EventHandler<EventArgs> OnAdClosed = delegate{};
        [Obsolete("Use OnFullScreenAdFailed.")]
        public event EventHandler<AdErrorEventArgs> OnAdFailedToShow = delegate{};
        [Obsolete("Use OnAdImpressionRecorded.")]
        public event EventHandler<EventArgs> OnAdDidRecordImpression = delegate { };
        [Obsolete("Use OnAdPaid.")]
        public event EventHandler<AdValueEventArgs> OnPaidEvent = delegate{};

        private IInterstitialAdClient _client;
        private string _adUnitId;

        [Obsolete("Use InterstitialAd.LoadAd().")]
        public InterstitialAd(string adUnitId)
        {
            _adUnitId = adUnitId;
        }

        private InterstitialAd(IInterstitialAdClient client)
        {
            Init(client);
        }

        /// <summary>
        /// Loads an interstitial ad.
        /// </summary>
        [Obsolete("Use InterstitialAd.LoadAd().")]
        public void LoadAd(AdRequest request)
        {
            var loader = MobileAds.GetClientFactory().BuildInterstitialAdClient();
            loader.LoadAd(_adUnitId, request, OnLoadAd);
        }

        /// <summary>
        /// Determines whether the interstitial ad has loaded.
        /// </summary>
        [Obsolete("Use InterstitialAd.LoadAd().")]
        public bool IsLoaded()
        {
            return _client != null;
        }

        /// <summary>
        /// Shows the interstitial ad.
        /// </summary>
        public void Show()
        {
            if (_client != null)
            {
                _client.ShowAd();
            }
        }

        private void OnLoadAd(IInterstitialAdClient client, ILoadAdErrorClient error)
        {
            if (client == null || error != null)
            {
                OnAdFailedToLoad(this, new AdFailedToLoadEventArgs
                {
                    LoadAdError = new LoadAdError(error)
                });
            }
            else
            {
                Init(client);
                OnAdLoaded(this, EventArgs.Empty);
            }
        }

        private void Init(IInterstitialAdClient client)
        {
            base.Init(client);
            _client = client;
            OnAdPaid += (adValue) =>
            {
                OnPaidEvent(this, new AdValueEventArgs
                {
                    AdValue = adValue
                });
            };
            OnAdImpressionRecorded += () =>
            {
                OnAdDidRecordImpression(this, EventArgs.Empty);
            };
            OnAdFullScreenContentFailed += (error) =>
            {
                OnAdFailedToShow(this, new AdErrorEventArgs{ AdError = error });
            };
            OnAdFullScreenContentClosed += () =>
            {
                OnAdClosed(this, EventArgs.Empty);
            };
            OnAdFullScreenContentOpened += () =>
            {
                OnAdOpening(this, EventArgs.Empty);
            };
        }
    }
}
