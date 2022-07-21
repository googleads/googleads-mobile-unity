// Copyright (C) 2020 Google LLC
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
    /// Rewarded interstitial ads can serve without requiring the user to opt-in to viewing.
    /// At any point during the experience, the user can decide to skip the ad.
    /// </summary>
    public class RewardedInterstitialAd : FullScreenAd
    {
        /// <summary>
        /// Loads an rewarded interstitial ad.
        /// </summary>
        public static void LoadRewardedInterstitialAd(
                                  string adUnitId,
                                  AdRequest request,
                                  Action<RewardedInterstitialAd, LoadAdError> callback)
        {
            Action<IRewardedInterstitialAdClient, ILoadAdErrorClient> apiCallback = (ad, error) =>
            {
                callback(
                    ad == null ? null : new RewardedInterstitialAd(ad),
                    error == null ? null : new LoadAdError(error));
            };
            var loader = MobileAds.GetClientFactory().BuildRewardedInterstitialAdClient();
            loader.LoadAd(adUnitId, request, apiCallback);
        }

        [Obsolete("Use RewardedInterstitialAd.LoadRewardedInterstitialAd().")]
        public static void LoadAd(string adUnitId,
                                  AdRequest request,
                                  Action<RewardedInterstitialAd, AdFailedToLoadEventArgs> adLoadCallback)
          {
            Action<RewardedInterstitialAd, LoadAdError> oldCallback = (ad, error) =>
            {
                AdFailedToLoadEventArgs failedToLoadEventArgs = error == null
                  ? null
                  : new AdFailedToLoadEventArgs{ LoadAdError = error };
                adLoadCallback(ad, failedToLoadEventArgs);
            };
            LoadRewardedInterstitialAd(adUnitId, request, oldCallback);
          }

        [Obsolete("Use OnFullScreenAdOpened.")]
        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent = delegate{};
        [Obsolete("Use OnFullScreenAdClosed.")]
        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent = delegate{};
        [Obsolete("Use OnFullScreenAdFailed.")]
        public event EventHandler<AdErrorEventArgs>
                                    OnAdFailedToPresentFullScreenContent = delegate{};
        [Obsolete("Use OnAdImpressionRecorded.")]
        public event EventHandler<EventArgs> OnAdDidRecordImpression = delegate{};
        [Obsolete("Use OnAdPaid.")]
        public event EventHandler<AdValueEventArgs> OnPaidEvent = delegate{};

        IRewardedInterstitialAdClient _client;

        private RewardedInterstitialAd(IRewardedInterstitialAdClient client)
        {
            base.Init(client);
            _client = client;
            OnAdPaid += (adValue) =>
            {
                OnPaidEvent(this, new AdValueEventArgs
                {
                    AdValue = adValue,
                });
            };
            OnAdImpressionRecorded += () =>
            {
                OnAdDidRecordImpression(this, EventArgs.Empty);
            };
            OnAdFullScreenContentFailed += (error) =>
            {
                OnAdFailedToPresentFullScreenContent(this, new AdErrorEventArgs
                {
                    AdError = error
                });
            };
            OnAdFullScreenContentClosed += () =>
            {
                OnAdDidDismissFullScreenContent(this, EventArgs.Empty);
            };
            OnAdFullScreenContentOpened += () =>
            {
                OnAdDidPresentFullScreenContent(this, EventArgs.Empty);
            };
        }

        /// <summary>
        /// The reward item for the loaded rewarded interstital ad.
        /// </summary>
        public Reward GetRewardItem()
        {
            return _client == null ? null : _client.GetRewardItem();
        }

        /// <summary>
        /// Sets the server side verification options
        /// </summary>
        public void SetServerSideVerificationOptions(ServerSideVerificationOptions options)
        {
            if (_client != null)
            {
                _client.SetServerSideVerificationOptions(options);
            }
        }

        /// <summary>
        /// Shows a rewarded interstitial ad.
        /// </summary>
        /// <param name="userRewardEarnedCallback">
        /// An action to be raised when the user earns a reward.
        /// </param>
        public void Show(Action<Reward> userRewardEarnedCallback)
        {
            if (_client != null)
            {
                Action<Reward> proxyCallback = (reward) =>
                {
                    if (userRewardEarnedCallback != null)
                    {
                        userRewardEarnedCallback(reward);
                    }
                };
                _client.ShowAd(proxyCallback);
            }
        }
    }
}
