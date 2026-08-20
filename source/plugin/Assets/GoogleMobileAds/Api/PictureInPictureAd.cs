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
using UnityEngine;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// A Picture-in-Picture (PiP) ad that displays in a floating window across screens.
    /// </summary>
    public class PictureInPictureAd
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
        /// Raised when the PiP ad is displayed on the screen.
        /// </summary>
        public event Action OnAdShown;

        /// <summary>
        /// Raised when the PiP ad is hidden from the screen.
        /// </summary>
        public event Action OnAdHidden;

        /// <summary>
        /// Raised when an ad opened full-screen content.
        /// </summary>
        public event Action OnAdFullScreenContentOpened;

        /// <summary>
        /// Raised when the ad closed full-screen content.
        /// </summary>
        public event Action OnAdFullScreenContentClosed;

        protected internal IPictureInPictureAdClient _client;

        protected internal PictureInPictureAd() {}

        internal PictureInPictureAd(IPictureInPictureAdClient client)
        {
            _client = client;
            RegisterAdEvents();
        }

        /// <summary>
        /// Loads a Picture-in-Picture ad.
        /// </summary>
        public static void Load(string adUnitId,
                                AdRequest request,
                                Action<PictureInPictureAd, LoadAdError> adLoadCallback)
        {
            if (adLoadCallback == null)
            {
                Debug.LogError("adLoadCallback is null. No ad was loaded.");
                return;
            }

            var client = MobileAds.GetClientFactory().BuildPictureInPictureAdClient();
            client.CreatePictureInPictureAd();
            client.OnAdLoaded += (sender, args) =>
            {
                var pipAd = new PictureInPictureAd(client);
                MobileAds.RaiseAction(() =>
                {
                    adLoadCallback(pipAd, null);
                });
            };
            client.OnAdFailedToLoad += (sender, error) =>
            {
                var loadAdError = (error != null && error.LoadAdErrorClient != null)
                    ? new LoadAdError(error.LoadAdErrorClient)
                    : null;
                MobileAds.RaiseAction(() =>
                {
                    adLoadCallback(null, loadAdError);
                });
            };
            client.LoadAd(adUnitId, request);
        }

        /// <summary>
        /// Shows the Picture-in-Picture ad on the screen at the specified corner position.
        /// </summary>
        public void Show(PictureInPictureAdPosition position = PictureInPictureAdPosition.Default)
        {
            if (_client != null)
            {
                _client.Show(position);
            }
        }

        /// <summary>
        /// Hides the Picture-in-Picture ad from the screen.
        /// </summary>
        public void Hide()
        {
            if (_client != null)
            {
                _client.Hide();
            }
        }

        /// <summary>
        /// Destroys the Picture-in-Picture ad and cleans up native resources.
        /// </summary>
        public void Destroy()
        {
            if (_client != null)
            {
                _client.Destroy();
            }
        }

        /// <summary>
        /// Returns the ResponseInfo for the loaded ad, or null if unavailable.
        /// </summary>
        public ResponseInfo GetResponseInfo()
        {
            return _client != null ? new ResponseInfo(_client.GetResponseInfoClient()) : null;
        }

        /// <summary>
        /// Returns the current or last known position of the Picture-in-Picture ad.
        /// </summary>
        public PictureInPictureAdPosition GetAdPosition()
        {
            return _client != null ? _client.GetAdPosition() : PictureInPictureAdPosition.Default;
        }

        private void RegisterAdEvents()
        {
            if (_client == null)
            {
                return;
            }

            _client.OnAdShown += (sender, args) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdShown != null)
                    {
                        OnAdShown();
                    }
                });
            };

            _client.OnAdHidden += (sender, args) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdHidden != null)
                    {
                        OnAdHidden();
                    }
                });
            };

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
        }
    }
}
