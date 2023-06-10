// Copyright (C) 2018 Google, Inc.
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
    /// Rewarded ads reward users with in-app items for interacting with
    /// video ads, playable ads, or surveys.
    /// </summary>
    public class RewardedAd
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
        /// Raised when the ad failed to load.
        /// </summary>
        public event Action<AdError> OnAdFullScreenContentFailed;

        private IRewardedAdClient _client;
        private bool _canShowAd;
        private Action<Reward> _userRewardEarnedCallback;

        private RewardedAd(IRewardedAdClient client)
        {
            _canShowAd = true;
            _client = client;
            RegisterAdEvents();
        }

        /// <summary>
        /// Loads a rewarded ad.
        /// </summary>
        public static void Load(string adUnitId,
                                AdRequest request,
                                Action<RewardedAd, LoadAdError> adLoadCallback)
        {
            if (adLoadCallback == null)
            {
                UnityEngine.Debug.LogError("adLoadCallback is null. No ad was loaded.");
                return;
            }

            var client = MobileAds.GetClientFactory().BuildRewardedAdClient();
            client.CreateRewardedAd();
            client.OnAdLoaded += (sender, args) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    adLoadCallback(new RewardedAd(client), null);
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
        /// Shows a rewarded ad.
        /// </summary>
        /// <param name="userRewardEarnedCallback">
        /// An action to be raised when the user earns a reward.
        /// </param>
        public void Show(Action<Reward> userRewardEarnedCallback)
        {
            if (CanShowAd())
            {
                _canShowAd = false;
                _userRewardEarnedCallback = userRewardEarnedCallback;
                _client.Show();
            }
        }

        /// <summary>
        /// Sets the server-side verification options.
        /// </summary>
        public void SetServerSideVerificationOptions(ServerSideVerificationOptions options)
        {
            _client.SetServerSideVerificationOptions(options);
        }

        /// <summary>
        /// The reward item for the loaded rewarded ad.
        /// </summary>
        public Reward GetRewardItem()
        {
            return _client == null ? null :  _client.GetRewardItem();
        }

        /// <summary>
        /// Destroys the rewarded ad.
        /// </summary>
        public void Destroy()
        {
            _canShowAd = false;
            if (_client != null)
            {
                _client.DestroyRewardedAd();
            }
        }

        /// <summary>
        /// Returns the ad request response info.
        /// </summary>
        public ResponseInfo GetResponseInfo()
        {
            return _client == null ? null : new ResponseInfo(_client.GetResponseInfoClient());
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

            _client.OnPaidEvent += (sender, args) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdPaid != null)
                    {
                        OnAdPaid(args.AdValue);
                    }
                });
            };

            _client.OnUserEarnedReward += (sender, args) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if(_userRewardEarnedCallback != null)
                    {
                        _userRewardEarnedCallback(args);
                        _userRewardEarnedCallback = null;
                    }
                });
            };
        }
    }
}
