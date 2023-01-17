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
using GoogleMobileAds;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// A full page ad experience at natural transition points,
    /// such as a page change or an app launch.
    /// Interstitials use a close button that removes the ad from the user's experience.
    /// </summary>
    public class InterstitialAd
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
        /// Raised when an ad is loaded.
        /// </summary>
        [Obsolete("Use InterstitialAd.Load().")]
        public event EventHandler<EventArgs> OnAdLoaded;

        /// <summary>
        /// Raised when the ad failed to open full-screen content.
        /// </summary>
        [Obsolete("Use InterstitialAd.Load().")]
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

        /// <summary>
        /// Raised when an ad opened full-screen content.
        /// </summary>
        [Obsolete("Use OnAdFullScreenContentOpened.")]
        public event EventHandler<EventArgs> OnAdOpening;

        /// <summary>
        /// Raised when the ad closed full-screen content.
        /// On iOS, this event is only raised when an ad opens an overlay, not when opening a new
        /// application such as Safari or the App Store.
        /// </summary>
        [Obsolete("Use OnAdFullScreenContentClosed.")]
        public event EventHandler<EventArgs> OnAdClosed;

        /// <summary>
        /// Raised when the ad failed to open full-screen content.
        /// </summary>
        [Obsolete("Use OnAdFullScreenContentFailed.")]
        public event EventHandler<AdErrorEventArgs> OnAdFailedToShow;

        /// <summary>
        /// Raised when an impression is recorded for an ad.
        /// </summary>
        [Obsolete("Use OnAdImpressionRecorded.")]
        public event EventHandler<EventArgs> OnAdDidRecordImpression;

        /// <summary>
        /// Raised when the ad is estimated to have earned money.
        /// </summary>
        [Obsolete("Use OnAdPaid.")]
        public event EventHandler<AdValueEventArgs> OnPaidEvent;

        private IInterstitialClient _client;
        private string _adUnitId;
        private bool _canShowAd;

        // Creates an interstitial ad.
        [Obsolete("Use InterstitialAd.Load().")]
        public InterstitialAd(string adUnitId)
        {
            _adUnitId = adUnitId;
        }

        private InterstitialAd(IInterstitialClient client)
        {
            _client = client;
            _canShowAd = true;
            RegisterAdEvents();
        }

        /// <summary>
        /// Loads an interstitial ad.
        /// </summary>
        public static void Load(string adUnitId,
                                AdRequest request,
                                Action<InterstitialAd, LoadAdError> adLoadCallback)
        {
            if (adLoadCallback == null)
            {
                UnityEngine.Debug.LogError("adLoadCallback is null. No ad was loaded.");
                return;
            }

            var client = MobileAds.GetClientFactory().BuildInterstitialClient();
            client.CreateInterstitialAd();
            client.OnAdLoaded += (sender, args) =>
            {
                adLoadCallback(new InterstitialAd(client), null);
            };
            client.OnAdFailedToLoad += (sender, error) =>
            {
                var loadAdError = new LoadAdError(error.LoadAdErrorClient);
                adLoadCallback(null, loadAdError);
            };
            client.LoadAd(adUnitId, request);
        }

        // Loads an interstitial ad.
        [Obsolete("Use InterstitialAd.Load().")]
        public void LoadAd(AdRequest request)
        {
            _client = MobileAds.GetClientFactory().BuildInterstitialClient();
            _client.CreateInterstitialAd();
            _client.OnAdLoaded += (sender, args) =>
            {
                _canShowAd = true;
                RegisterAdEvents();
                if (OnAdLoaded != null)
                {
                    OnAdLoaded(this, EventArgs.Empty);
                }
            };
            _client.OnAdFailedToLoad += (sender, error) =>
            {
                var loadAdError = new LoadAdError(error.LoadAdErrorClient);
                if (OnAdFailedToLoad != null)
                {
                    OnAdFailedToLoad(this, new AdFailedToLoadEventArgs
                    {
                        LoadAdError = loadAdError
                    });
                }
            };
            _client.LoadAd(_adUnitId, request);
        }

        /// <summary>
        /// Returns true if the ad is loaded and not shown.
        /// </summary>
        public bool CanShowAd()
        {
            return _client != null && _canShowAd;
        }

        /// <summary>
        /// Returns true if the ad is loaded.
        /// </summary>
        [Obsolete("Use CanShowAd().")]
        public bool IsLoaded()
        {
            return CanShowAd();
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
            }
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
                if (OnAdClicked != null)
                {
                    OnAdClicked();
                }
            };

            _client.OnAdDidDismissFullScreenContent += (sender, args) =>
            {
                if (OnAdClosed != null)
                {
                    OnAdClosed(this, args);
                }
                if (OnAdFullScreenContentClosed != null)
                {
                    OnAdFullScreenContentClosed();
                }
            };

            _client.OnAdDidPresentFullScreenContent += (sender, args) =>
            {
                if (OnAdOpening != null)
                {
                    OnAdOpening(this, args);
                }
                if (OnAdFullScreenContentOpened != null)
                {
                    OnAdFullScreenContentOpened();
                }
            };

            _client.OnAdDidRecordImpression += (sender, args) =>
            {
                if (OnAdDidRecordImpression != null)
                {
                    OnAdDidRecordImpression(this, args);
                }
                if (OnAdImpressionRecorded != null)
                {
                    OnAdImpressionRecorded();
                }
            };
            _client.OnAdFailedToPresentFullScreenContent += (sender, error) =>
            {
                var adError = new AdError(error.AdErrorClient);
                if (OnAdFailedToShow != null)
                {
                    OnAdFailedToShow(this, new AdErrorEventArgs { AdError = adError });
                }
                if (OnAdFullScreenContentFailed != null)
                {
                    OnAdFullScreenContentFailed(adError);
                }
            };
            _client.OnPaidEvent += (sender, args) =>
            {
                if (OnPaidEvent != null)
                {
                    OnPaidEvent(this, args);
                }
                if (OnAdPaid != null)
                {
                    OnAdPaid(args.AdValue);
                }
            };
        }
    }
}
