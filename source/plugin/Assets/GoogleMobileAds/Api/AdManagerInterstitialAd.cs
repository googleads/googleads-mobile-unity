
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
using UnityEngine;

using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api.AdManager
{
    /// <summary>
    /// A full page ad experience at natural transition points,
    /// such as a page change or an app launch for Google Ad Manager publishers.
    /// Interstitials use a close button that removes the ad from the user's experience.
    /// </summary>
    public class AdManagerInterstitialAd
    {
        /// <summary>
        /// Raised when the ad is estimated to have earned money.
        /// </summary>
        public event Action<AdValue> OnAdPaid;

        /// <summary>
        /// Raised when an ad is clicked.
        /// </summary>
        public event Action OnAdClicked;

        /// <summary>
        /// Raised when an impression is recorded for an ad.
        /// </summary>
        public event Action OnAdImpressionRecorded;

        /// <summary>
        /// Raised when an ad opened full-screen content.
        /// </summary>
        public event Action OnAdFullScreenContentOpened;

        /// <summary>
        /// Raised when the ad closed full-screen content.
        /// On iOS, this event is only raised when an ad opens an overlay, not when opening a new
        /// application such as Safari or the App Store.
        /// </summary>
        public event Action OnAdFullScreenContentClosed;

        /// <summary>
        /// Raised when the ad failed to open full-screen content.
        /// </summary>
        public event Action<AdError> OnAdFullScreenContentFailed;

        /// <summary>
        /// Raised when the app receives an event from the interstitial ad.
        /// </summary>
        public event Action<AppEvent> OnAppEventReceived;

        internal IAdManagerInterstitialClient _client;
        internal bool _canShowAd;

        private AdManagerInterstitialAd(IAdManagerInterstitialClient client)
        {
            _client = client;
            _canShowAd = true;
            RegisterAdEvents();
        }

        /// <summary>
        /// Verify if an ad is preloaded and available to show.
        /// </summary>
        /// <param name="adUnitId">The ad Unit Id of the ad to verify. </param>
        public static bool IsAdAvailable(string adUnitId)
        {
            if (string.IsNullOrEmpty(adUnitId))
            {
                Debug.LogError("adUnitId cannot be null or empty.");
                return false;
            }
            var client = MobileAds.GetClientFactory().BuildAdManagerInterstitialClient();
            return client.IsAdAvailable(adUnitId);
        }

        /// <summary>
        /// Returns the next pre-loaded interstitial ad and null if no ad is available.
        /// </summary>
        /// <param name="adUnitId">The ad Unit ID of the ad to poll.</param>
        public static AdManagerInterstitialAd PollAd(string adUnitId)
        {
            if (string.IsNullOrEmpty(adUnitId))
            {
                Debug.LogError("adUnitId cannot be null or empty.");
                return null;
            }
            var client = MobileAds.GetClientFactory().BuildAdManagerInterstitialClient();
            if (client == null)
            {
                return null;
            }
            client.CreateInterstitialAd();
            return new AdManagerInterstitialAd(client.PollAd(adUnitId));
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

        /// <summary>
        /// Returns true if the ad is loaded and not shown.
        /// </summary>
        public bool CanShowAd()
        {
            return _client != null && _canShowAd;
        }

        /// <summary>
        /// Shows the ad.
        /// </summary>
        public void Show()
        {
            if (CanShowAd())
            {
                _canShowAd = false;
                _client.Show();
            }
        }

        /// <summary>
        /// Destroys the ad.
        /// </summary>
        public void Destroy()
        {
            if (_client != null)
            {
                _canShowAd = false;
                _client.DestroyInterstitial();
                _client = null;
            }
        }

        /// <summary>
        /// Returns the ad unit ID.
        /// </summary>
        public string GetAdUnitID()
        {
            return _client != null ? _client.GetAdUnitID() : null;
        }

        /// <summary>
        /// Returns the ad request response info.
        /// </summary>
        public ResponseInfo GetResponseInfo()
        {
            return _client != null ? new ResponseInfo(_client.GetResponseInfoClient()) : null;
        }

        private void RegisterAdEvents()
        {
            _client.OnAdClicked += () =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdClicked != null)
                    {
                        OnAdClicked();
                    }
                });
            };

            _client.OnAdDidDismissFullScreenContent += (sender, args) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdFullScreenContentClosed != null)
                    {
                        OnAdFullScreenContentClosed();
                    }
                });
            };

            _client.OnAdDidPresentFullScreenContent += (sender, args) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdFullScreenContentOpened != null)
                    {
                        OnAdFullScreenContentOpened();
                    }
                });
            };

            _client.OnAdDidRecordImpression += (sender, args) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdImpressionRecorded != null)
                    {
                        OnAdImpressionRecorded();
                    }
                });
            };

            _client.OnAdFailedToPresentFullScreenContent += (sender, error) =>
            {
                var adError = new AdError(error.AdErrorClient);
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdFullScreenContentFailed != null)
                    {
                        OnAdFullScreenContentFailed(adError);
                    }
                });
            };

            _client.OnPaidEvent += (adValue) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdPaid != null)
                    {
                        OnAdPaid(adValue);
                    }
                });
            };

            _client.OnAppEvent += (appEvent) =>
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
