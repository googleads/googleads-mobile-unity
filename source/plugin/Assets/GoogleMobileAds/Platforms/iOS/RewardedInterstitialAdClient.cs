#if UNITY_IOS
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
using System.Runtime.InteropServices;
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    public class RewardedInterstitialAdClient : IRewardedInterstitialAdClient, IDisposable
    {
        private IntPtr rewardedInterstitialAdClientPtr;
        private IntPtr rewardedInterstitialAdPtr;

#region rewarded interstitial ad callback types

        internal delegate void GADURewardedInterstitialAdLoadedCallback(IntPtr rewardedInterstitialAdClient);

        internal delegate void GADURewardedInterstitialAdFailedToLoadCallback(IntPtr rewardedInterstitialAdClient, IntPtr error);

        internal delegate void GADUUserEarnedRewardCallback(
            IntPtr rewardedInterstitialAdClient, string rewardType, double rewardAmount);

        internal delegate void GADURewardedInterstitialAdPaidEventCallback(
            IntPtr rewardedInterstitialAdClient, int precision, long value, string currencyCode);

#endregion

#region full screen content callback types

        internal delegate void GADUFailedToPresentFullScreenContentCallback(IntPtr rewardedInterstitialAdClient, IntPtr error);

        internal delegate void GADUDidPresentFullScreenContentCallback(IntPtr rewardedInterstitialAdClient);

        internal delegate void GADUDidDismissFullScreenContentCallback(IntPtr rewardedInterstitialAdClient);

#endregion

        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;

        public event EventHandler<Reward> OnUserEarnedReward;

        public event EventHandler<AdValueEventArgs> OnPaidEvent;

        public event EventHandler<AdErrorClientEventArgs> OnAdFailedToPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;

#region IRewardedInterstitialAdClient implementation

        public void CreateRewardedInterstitialAd()
        {
            this.rewardedInterstitialAdClientPtr = (IntPtr)GCHandle.Alloc(this);
            this.rewardedInterstitialAdPtr = Externs.GADUCreateRewardedInterstitialAd(this.rewardedInterstitialAdClientPtr);

            Externs.GADUSetRewardedInterstitialAdCallbacks(
                this.rewardedInterstitialAdPtr,
                RewardedInterstitialAdLoadedCallback,
                RewardedInterstitialAdFailedToLoadCallback,
                RewardedInterstitialAdUserDidEarnRewardCallback,
                RewardedInterstitialAdPaidEventCallback,
                AdFailedToPresentFullScreenContentCallback,
                AdDidPresentFullScreenContentCallback,
                AdDidDismissFullScreenContentCallback);
        }

        public void LoadAd(string adUnitID, AdRequest request) {
            IntPtr requestPtr = Utils.BuildAdRequest(request);
            Externs.GADULoadRewardedInterstitialAd(this.rewardedInterstitialAdPtr, adUnitID, requestPtr);
            Externs.GADURelease(requestPtr);
        }

        // Show the rewarded interstitial ad on the screen.
        public void Show()
        {
            Externs.GADUShowRewardedInterstitialAd(this.rewardedInterstitialAdPtr);
        }

        // Sets the server side verification options
        public void SetServerSideVerificationOptions(ServerSideVerificationOptions serverSideVerificationOptions)
        {
            IntPtr optionsPtr = Utils.BuildServerSideVerificationOptions(serverSideVerificationOptions);
            Externs.GADURewardedInterstitialAdSetServerSideVerificationOptions(this.rewardedInterstitialAdPtr, optionsPtr);
            Externs.GADURelease(optionsPtr);
        }

        // Returns the reward item for the loaded rewarded interstitial ad.
        public Reward GetRewardItem()
        {
            string type = Externs.GADURewardedInterstitialAdGetRewardType(this.rewardedInterstitialAdPtr);
            double amount = Externs.GADURewardedInterstitialAdGetRewardAmount(this.rewardedInterstitialAdPtr); ;
            return new Reward()
            {
                Type = type,
                Amount = amount
            };
        }

        public IResponseInfoClient GetResponseInfoClient()
        {
            return new ResponseInfoClient(ResponseInfoClientType.AdLoaded, this.rewardedInterstitialAdPtr);
        }

        // Destroys the rewarded interstitial ad.
        public void DestroyRewardedInterstitialAd()
        {
            this.rewardedInterstitialAdPtr = IntPtr.Zero;
        }

        public void Dispose()
        {
            this.DestroyRewardedInterstitialAd();
            ((GCHandle)this.rewardedInterstitialAdClientPtr).Free();
        }

        ~RewardedInterstitialAdClient()
        {
            this.Dispose();
        }

#endregion

#region rewarded interstitial ad callback methods

        [MonoPInvokeCallback(typeof(GADURewardedInterstitialAdLoadedCallback))]
        private static void RewardedInterstitialAdLoadedCallback(IntPtr rewardedInterstitialAdClient)
        {
            RewardedInterstitialAdClient client = IntPtrToRewardedInterstitialAdClient(rewardedInterstitialAdClient);
            if (client.OnAdLoaded != null)
            {
                client.OnAdLoaded(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADURewardedInterstitialAdFailedToLoadCallback))]
        private static void RewardedInterstitialAdFailedToLoadCallback(
            IntPtr rewardedInterstitialAdClient, IntPtr error)
        {
            RewardedInterstitialAdClient client = IntPtrToRewardedInterstitialAdClient(rewardedInterstitialAdClient);
            if (client.OnAdFailedToLoad != null)
            {
                LoadAdErrorClientEventArgs args = new LoadAdErrorClientEventArgs()
                {
                    LoadAdErrorClient = new LoadAdErrorClient(error),
                    Message = Externs.GADUGetAdErrorMessage(error)
                };
                client.OnAdFailedToLoad(client, args);
            }
        }

        [MonoPInvokeCallback(typeof(GADUUserEarnedRewardCallback))]
        private static void RewardedInterstitialAdUserDidEarnRewardCallback(
            IntPtr rewardedInterstitialAdClient, string rewardType, double rewardAmount)
        {
            RewardedInterstitialAdClient client = IntPtrToRewardedInterstitialAdClient(
                rewardedInterstitialAdClient);
            if (client.OnUserEarnedReward != null)
            {
                Reward args = new Reward()
                {
                    Type = rewardType,
                    Amount = rewardAmount
                };
                client.OnUserEarnedReward(client, args);
            }
        }

        [MonoPInvokeCallback(typeof(GADURewardedInterstitialAdPaidEventCallback))]
        private static void RewardedInterstitialAdPaidEventCallback(
            IntPtr rewardedInterstitialAdClient, int precision, long value, string currencyCode)
        {
            RewardedInterstitialAdClient client = IntPtrToRewardedInterstitialAdClient(rewardedInterstitialAdClient);
            if (client.OnPaidEvent != null)
            {
                AdValue adValue = new AdValue()
                {
                    Precision = (AdValue.PrecisionType)precision,
                    Value = value,
                    CurrencyCode = currencyCode
                };
                AdValueEventArgs args = new AdValueEventArgs()
                {
                    AdValue = adValue
                };

                client.OnPaidEvent(client, args);
            }
        }

        [MonoPInvokeCallback(typeof(GADUFailedToPresentFullScreenContentCallback))]
        private static void AdFailedToPresentFullScreenContentCallback(IntPtr rewardedInterstitialAdClient, IntPtr error)
        {
            RewardedInterstitialAdClient client = IntPtrToRewardedInterstitialAdClient(rewardedInterstitialAdClient);
            if (client.OnAdFailedToPresentFullScreenContent != null)
            {
                AdErrorClientEventArgs args = new AdErrorClientEventArgs()
                {
                    AdErrorClient = new AdErrorClient(error),
                    Message = Externs.GADUGetAdErrorMessage(error)
                };
                client.OnAdFailedToPresentFullScreenContent(client, args);
            }
        }

        [MonoPInvokeCallback(typeof(GADUDidPresentFullScreenContentCallback))]
        private static void AdDidPresentFullScreenContentCallback(IntPtr rewardedInterstitialAdClient)
        {
            RewardedInterstitialAdClient client = IntPtrToRewardedInterstitialAdClient(rewardedInterstitialAdClient);
            if (client.OnAdDidPresentFullScreenContent != null)
            {
                client.OnAdDidPresentFullScreenContent(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUDidDismissFullScreenContentCallback))]
        private static void AdDidDismissFullScreenContentCallback(IntPtr rewardedInterstitialAdClient)
        {
            RewardedInterstitialAdClient client = IntPtrToRewardedInterstitialAdClient(rewardedInterstitialAdClient);
            if (client.OnAdDidDismissFullScreenContent != null)
            {
                client.OnAdDidDismissFullScreenContent(client, EventArgs.Empty);
            }
        }

        private static RewardedInterstitialAdClient IntPtrToRewardedInterstitialAdClient(
            IntPtr rewardedInterstitialAdClient)
        {
            GCHandle handle = (GCHandle)rewardedInterstitialAdClient;
            return handle.Target as RewardedInterstitialAdClient;
        }

#endregion
    }
}
#endif
