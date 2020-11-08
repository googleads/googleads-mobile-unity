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
using System.Collections.Generic;

using GoogleMobileAds;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    public class RewardedInterstitialAd
    {
        private IRewardedInterstitialAdClient rewardedInterstitialAdClient;
        private static HashSet<IRewardedInterstitialAdClient> loadingClients = new HashSet<IRewardedInterstitialAdClient>();

        private RewardedInterstitialAd(IRewardedInterstitialAdClient client)
        {
            this.rewardedInterstitialAdClient = client;

            this.rewardedInterstitialAdClient.OnAdFailedToPresentFullScreenContent += (sender, args) =>
            {
                if (this.OnAdFailedToPresentFullScreenContent != null)
                {
                    this.OnAdFailedToPresentFullScreenContent(this, args);
                }
            };

            this.rewardedInterstitialAdClient.OnAdDidPresentFullScreenContent += (sender, args) =>
            {
                if (this.OnAdDidPresentFullScreenContent != null)
                {
                    this.OnAdDidPresentFullScreenContent(this, args);
                }
            };

            this.rewardedInterstitialAdClient.OnAdDidDismissFullScreenContent += (sender, args) =>
            {
                if (this.OnAdDidDismissFullScreenContent != null)
                {
                   this.OnAdDidDismissFullScreenContent(this, args);
                }
            };
        }

        // Called when the ad is estimated to have earned money.
        public event EventHandler<AdValueEventArgs> OnPaidEvent;

        // Full screen content events.
        public event EventHandler<AdErrorEventArgs> OnAdFailedToPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;


        // Loads a new rewarded interstitial ad.
        // TODO(jillsong): Apply AdError.
        public static void LoadAd(string adUnitID, AdRequest request, Action<RewardedInterstitialAd, string> adLoadCallback)
        {
            IRewardedInterstitialAdClient client = MobileAds.GetClientFactory().BuildRewardedInterstitialAdClient();
            loadingClients.Add(client);
            client.CreateRewardedInterstitialAd();

            client.OnAdLoaded += (sender, args) =>
            {
                if (adLoadCallback != null)
                {
                    adLoadCallback(new RewardedInterstitialAd(client), null);
                    loadingClients.Remove(client);
                }
            };

            client.OnAdFailedToLoad += (sender, args) =>
            {
                if (adLoadCallback != null)
                {
                    adLoadCallback(null, args.Message);
                    loadingClients.Remove(client);
                }
            };

            client.LoadAd(adUnitID, request);
        }

        // Shows the rewarded interstitial ad.
        public void Show(Action<Reward> userEarnedRewardCallback)
        {
            if (rewardedInterstitialAdClient != null)
            {
                this.rewardedInterstitialAdClient.OnUserEarnedReward += (sender, args) =>
                {
                    if (userEarnedRewardCallback != null)
                    {
                        userEarnedRewardCallback(args);
                    }
                };
                rewardedInterstitialAdClient.Show();
            }
        }

        // Sets the server side verification options
        public void SetServerSideVerificationOptions(ServerSideVerificationOptions serverSideVerificationOptions)
        {
            rewardedInterstitialAdClient.SetServerSideVerificationOptions(serverSideVerificationOptions);
        }

        // Returns the reward item for the loaded rewarded interstitial ad.
        public Reward GetRewardItem()
        {
            return rewardedInterstitialAdClient.GetRewardItem();
        }

        // Returns ad request response info.
        public ResponseInfo GetResponseInfo()
        {
            return new ResponseInfo(rewardedInterstitialAdClient.GetResponseInfoClient());
        }

    }
}
