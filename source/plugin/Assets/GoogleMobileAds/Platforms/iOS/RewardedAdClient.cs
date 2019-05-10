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

#if UNITY_IOS

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    public class RewardedAdClient : IRewardedAdClient, IDisposable
    {
        private IntPtr rewardedAdPtr;
        private IntPtr rewardedAdClientPtr;

        #region rewarded callback types

        internal delegate void GADURewardedAdDidReceiveAdCallback(
            IntPtr rewardedAdClient);

        internal delegate void GADURewardedAdDidFailToReceiveAdWithErrorCallback(
            IntPtr rewardedAdClient, string error);

        internal delegate void GADURewardedAdDidFailToShowAdWithErrorCallback(
            IntPtr rewardedAdClient, string error);

        internal delegate void GADURewardedAdDidOpenCallback(
            IntPtr rewardedAdClient);

        internal delegate void GADURewardedAdDidCloseCallback(
            IntPtr rewardedAdClient);

        internal delegate void GADUUserEarnedRewardCallback(
            IntPtr rewardedAdClient, string rewardType, double rewardAmount);

        #endregion

        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<AdErrorEventArgs> OnAdFailedToLoad;

        public event EventHandler<AdErrorEventArgs> OnAdFailedToShow;

        public event EventHandler<EventArgs> OnAdOpening;

        public event EventHandler<EventArgs> OnAdStarted;

        public event EventHandler<EventArgs> OnAdClosed;

        public event EventHandler<Reward> OnUserEarnedReward;


        // This property should be used when setting the rewardedAdPtr.
        private IntPtr RewardedAdPtr
        {
            get { return this.rewardedAdPtr; }

            set
            {
                Externs.GADURelease(this.rewardedAdPtr);
                this.rewardedAdPtr = value;
            }
        }

        #region IGoogleMobileAdsRewardedAdClient implementation

        // Creates a rewarded ad.
        public void CreateRewardedAd(string adUnitId)
        {
            this.rewardedAdClientPtr = (IntPtr)GCHandle.Alloc(this);
            this.RewardedAdPtr = Externs.GADUCreateRewardedAd(
                this.rewardedAdClientPtr, adUnitId);

            Externs.GADUSetRewardedAdCallbacks(
                this.RewardedAdPtr,
                RewardedAdDidReceiveAdCallback,
                RewardedAdDidFailToReceiveAdWithErrorCallback,
                RewardedAdDidFailToShowAdWithErrorCallback,
                RewardedAdDidOpenCallback,
                RewardedAdDidCloseCallback,
                RewardedAdUserDidEarnRewardCallback);
        }

        // Load an ad.
        public void LoadAd(AdRequest request)
        {
            IntPtr requestPtr = Utils.BuildAdRequest(request);
            Externs.GADURequestRewardedAd(this.RewardedAdPtr, requestPtr);
            Externs.GADURelease(requestPtr);
        }

        // Show the rewarded ad on the screen.
        public void Show()
        {
            Externs.GADUShowRewardedAd(this.RewardedAdPtr);
        }

        // Sets the server side verification options
        public void SetServerSideVerificationOptions(ServerSideVerificationOptions serverSideVerificationOptions)
        {
            IntPtr optionsPtr = Utils.BuildServerSideVerificationOptions(serverSideVerificationOptions);
            Externs.GADURewardedAdSetServerSideVerificationOptions(this.RewardedAdPtr, optionsPtr);
            Externs.GADURelease(optionsPtr);
        }

        public bool IsLoaded()
        {
            return Externs.GADURewardedAdReady(this.RewardedAdPtr);
        }

        // Returns the mediation adapter class name.
        public string MediationAdapterClassName()
        {
            return Utils.PtrToString(
                Externs.GADUMediationAdapterClassNameForRewardedAd(this.RewardedAdPtr));
        }

        // Destroys the rewarded ad.
        public void DestroyRewardedAd()
        {
            this.RewardedAdPtr = IntPtr.Zero;
        }

        public void Dispose()
        {
            this.DestroyRewardedAd();
            ((GCHandle)this.rewardedAdClientPtr).Free();
        }

        ~RewardedAdClient()
        {
            this.Dispose();
        }

        #endregion

        #region Rewarded ad callback methods

        [MonoPInvokeCallback(typeof(GADURewardedAdDidReceiveAdCallback))]
        private static void RewardedAdDidReceiveAdCallback(IntPtr rewardedAdClient)
        {
            RewardedAdClient client = IntPtrToRewardedAdClient(rewardedAdClient);
            if (client.OnAdLoaded != null)
            {
                client.OnAdLoaded(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADURewardedAdDidFailToReceiveAdWithErrorCallback))]
        private static void RewardedAdDidFailToReceiveAdWithErrorCallback(
            IntPtr rewardedAdClient, string error)
        {
            RewardedAdClient client = IntPtrToRewardedAdClient(rewardedAdClient);
            if (client.OnAdFailedToLoad != null)
            {
                AdErrorEventArgs args = new AdErrorEventArgs()
                {
                    Message = error
                };
                client.OnAdFailedToLoad(client, args);
            }
        }

        [MonoPInvokeCallback(typeof(GADURewardedAdDidFailToShowAdWithErrorCallback))]
        private static void RewardedAdDidFailToShowAdWithErrorCallback(
            IntPtr rewardedAdClient, string error)
        {
            RewardedAdClient client = IntPtrToRewardedAdClient(rewardedAdClient);
            if (client.OnAdFailedToShow != null)
            {
                AdErrorEventArgs args = new AdErrorEventArgs()
                {
                    Message = error
                };
                client.OnAdFailedToLoad(client, args);
            }
        }

        [MonoPInvokeCallback(typeof(GADURewardedAdDidOpenCallback))]
        private static void RewardedAdDidOpenCallback(IntPtr rewardedAdClient)
        {
            RewardedAdClient client = IntPtrToRewardedAdClient(rewardedAdClient);
            if (client.OnAdOpening != null)
            {
                client.OnAdOpening(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADURewardedAdDidCloseCallback))]
        private static void RewardedAdDidCloseCallback(IntPtr rewardedAdClient)
        {
            RewardedAdClient client = IntPtrToRewardedAdClient(
                rewardedAdClient);
            if (client.OnAdClosed != null)
            {
                client.OnAdClosed(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUUserEarnedRewardCallback))]
        private static void RewardedAdUserDidEarnRewardCallback(
            IntPtr rewardedAdClient, string rewardType, double rewardAmount)
        {
            RewardedAdClient client = IntPtrToRewardedAdClient(
                rewardedAdClient);
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

        private static RewardedAdClient IntPtrToRewardedAdClient(
            IntPtr rewardedAdClient)
        {
            GCHandle handle = (GCHandle)rewardedAdClient;
            return handle.Target as RewardedAdClient;
        }

        #endregion
    }
}

#endif
