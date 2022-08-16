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
    /// Rewarded ad units allow you to reward users with in-app items for interacting with
    /// video ads, playable ads, and surveys.
    /// </summary>
    public class RewardedAd : FullScreenAd
    {
        /// <summary>
        /// Loads an rewarded ad.
        /// </summary>
        public static void LoadAd(string adUnitId,
                                  AdRequest request,
                                  Action<RewardedAd, LoadAdError> callback)
        {
            Action<IRewardedAdClient, ILoadAdErrorClient> apiCallback = (ad, error) =>
            {
                callback(
                    ad == null ? null : new RewardedAd(ad),
                    error == null ? null : new LoadAdError(error));
            };
            var loader = MobileAds.GetClientFactory().BuildRewardedAdClient();
            loader.LoadAd(adUnitId, request, apiCallback);
        }

        [Obsolete("Use RewardedAd.LoadAd().")]
        public event EventHandler<EventArgs> OnAdLoaded = delegate{};
        [Obsolete("Use RewardedAd.LoadAd().")]
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad = delegate{};
        [Obsolete("Use OnFullScreenAdOpened.")]
        public event EventHandler<EventArgs> OnAdOpening = delegate{};
        [Obsolete("Use OnFullScreenAdClosed.")]
        public event EventHandler<EventArgs> OnAdClosed = delegate{};
        [Obsolete("Use OnFullScreenAdFailed.")]
        public event EventHandler<AdErrorEventArgs> OnAdFailedToShow = delegate{};
        [Obsolete("Use OnAdImpressionRecorded.")]
        public event EventHandler<EventArgs> OnAdDidRecordImpression = delegate{};
        [Obsolete("Use OnAdPaid.")]
        public event EventHandler<AdValueEventArgs> OnPaidEvent = delegate{};
        [Obsolete("Use OnUserRewardEarned.")]
        public event EventHandler<Reward> OnUserEarnedReward = delegate{};

        private IRewardedAdClient _client;
        private string _adUnitId;

        [Obsolete("Use RewardedAd.LoadAd().")]
        public RewardedAd(string adUnitId)
        {
            _adUnitId = adUnitId;
        }

        private RewardedAd(IRewardedAdClient client)
        {
            Init(client);
        }

        /// <summary>
        /// Loads an rewarded ad.
        /// </summary>
        [Obsolete("Use RewardedAd.LoadAd().")]
        public void LoadAd(AdRequest request)
        {
            var loader = MobileAds.GetClientFactory().BuildRewardedAdClient();
            loader.LoadAd(_adUnitId, request, OnLoadAd);
        }

        /// <summary>
        /// Determines whether the rewarded ad has loaded.
        /// </summary>
        [Obsolete("Use RewardedAd.LoadAd().")]
        public bool IsLoaded()
        {
            return _client != null;
        }

        /// <summary>
        /// The reward item for the loaded rewarded ad.
        /// </summary>
        public Reward GetRewardItem()
        {
            return _client != null ? _client.GetRewardItem() : null;
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
        /// Shows a rewarded ad.
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
                    if(reward != null)
                    {
                        OnUserEarnedReward(this, reward);
                    }
                };
                _client.ShowAd(proxyCallback);
            }
        }

        private void OnLoadAd(IRewardedAdClient client, ILoadAdErrorClient error)
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

        private void Init(IRewardedAdClient client)
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
                OnAdFailedToShow(this, new AdErrorEventArgs { AdError = error });
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
