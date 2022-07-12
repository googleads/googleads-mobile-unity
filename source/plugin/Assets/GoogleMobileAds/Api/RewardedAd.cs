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
    public class RewardedAd : BaseFullScreenAd
    {
        /// <summary>
        /// Loads an RewardedAd.
        /// </summary>
        public static void LoadRewardedAd(string adUnitId,
                                          AdRequest request,
                                          Action<RewardedAd, LoadAdError> callback)
        {
            Action<IRewardedAdClient, ILoadAdErrorClient> apiCallback = (ad, error) =>
            {
                callback(new RewardedAd(ad), new LoadAdError(error));
            };
            var loader = MobileAds.GetClientFactory().BuildRewardedAdClient();
            loader.LoadRewardedAd(adUnitId, request, apiCallback);
        }

        [Obsolete("Use LoadAd().")]
        public event EventHandler<EventArgs> OnAdLoaded = delegate{};
        [Obsolete("Use LoadAd().")]
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad = delegate{};
        [Obsolete("Use OnFullScreenAdOpened.")]
        public event EventHandler<EventArgs> OnAdOpening = delegate{};
        [Obsolete("Use OnFullScreenAdClosed.")]
        public event EventHandler<EventArgs> OnAdClosed = delegate{};
        [Obsolete("Use OnFullScreenAdFailed.")]
        public event EventHandler<AdErrorEventArgs> OnAdFailedToShow = delegate{};
        [Obsolete("Use OnAdImpressionRecorded.")]
        public event EventHandler<EventArgs> OnAdDidRecordImpression;
        [Obsolete("Use OnAdPaid")]
        public event EventHandler<AdValueEventArgs> OnPaidEvent = delegate{};
        [Obsolete("Use OnUserRewardEarned.")]
        public event EventHandler<Reward> OnUserEarnedReward = delegate{};

        /// <summary>
        /// The reward item for the loaded RewardedAd.
        /// Returns null if the ad is not loaded.
        /// </summary>
        public Reward RewardItem
        {
            get { return _client != null ? _client.RewardItem : null; }
        }

        private IRewardedAdClient _client;
        private string _adUnitId;

        [Obsolete("Use LoadRewardedAd().")]
        public RewardedAd(string adUnitId)
        {
            _adUnitId = adUnitId;
        }

        private RewardedAd(IRewardedAdClient client)
        {
            Init(client);
            _client = client;
            OnAdPaid += (adValue) =>
            {
                OnPaidEvent(this, new AdValueEventArgs(adValue));
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

        /// <summary>
        /// Loads an RewardedAd.
        /// </summary>
        [Obsolete("Use LoadRewardedAd().")]
        public void LoadAd(AdRequest request)
        {
            var loader = MobileAds.GetClientFactory().BuildRewardedAdClient();
            loader.LoadRewardedAd(_adUnitId, request, OnLoadAd);
        }

        /// <summary>
        /// Determines whether the rewarded ad has loaded.
        /// </summary>
        [Obsolete("Use RewardedAd.LoadAd.")]
        public bool IsLoaded()
        {
            return _client != null;
        }

        /// <summary>
        /// Returns ad request response info.
        /// </summary>
        [Obsolete("Use ResponseInfo.")]
        public ResponseInfo GetResponseInfo()
        {
            return base.Response;
        }

        /// <summary>
        /// The reward item for the loaded rewarded interstital ad.
        /// Returns null if the ad is not loaded.
        /// </summary>
        [System.Obsolete("Use RewardItem.")]
        public Reward GetRewardItem()
        {
            return this.RewardItem;
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
        /// Shows the rewarded ad.
        /// </summary>
        [Obsolete("Use Show(Action<Reward> userRewardEarnedCallback)")]
        public void Show()
        {
            Show(null);
        }

        /// <summary>
        /// Shows the Rewarded ad.
        /// </summary>
        /// <param name="userRewardEarnedCallback">
        /// Called when the user earns a reward.
        /// </param>
        public void Show(Action<Reward> userRewardEarnedCallback)
        {
            if (_client != null)
            {
                _client.Show(userRewardEarnedCallback);
            }
        }

        private void OnLoadAd(IRewardedAdClient client, ILoadAdErrorClient error)
        {
            if (error != null)
            {
                OnAdFailedToLoad(this, new AdFailedToLoadEventArgs
                {
                    LoadAdError = new LoadAdError(error)
                });
            }
            else
            {
                _client= client;
                Init(client);
                OnAdLoaded(this, EventArgs.Empty);
            }
        }
    }
}
