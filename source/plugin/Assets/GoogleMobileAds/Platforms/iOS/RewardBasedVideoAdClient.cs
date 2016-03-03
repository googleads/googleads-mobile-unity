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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    internal class RewardBasedVideoAdClient : IRewardBasedVideoAdClient, IDisposable
    {
        #region reward based video callback types

        internal delegate void GADURewardBasedVideoAdDidReceiveAdCallback(
            IntPtr rewardBasedVideoAdClient);
        internal delegate void GADURewardBasedVideoAdDidFailToReceiveAdWithErrorCallback(
            IntPtr rewardBasedVideoClient, string error);
        internal delegate void GADURewardBasedVideoAdDidOpenCallback(
            IntPtr rewardBasedVideoAdClient);
        internal delegate void GADURewardBasedVideoAdDidStartCallback(
            IntPtr rewardBasedVideoAdClient);
        internal delegate void GADURewardBasedVideoAdDidCloseCallback(
            IntPtr rewardBasedVideoAdClient);
        internal delegate void GADURewardBasedVideoAdDidRewardCallback(
            IntPtr rewardBasedVideoAdClient, string rewardType, double rewardAmount);
        internal delegate void GADURewardBasedVideoAdWillLeaveApplicationCallback(
            IntPtr rewardBasedVideoAdClient);

        #endregion

        public event EventHandler<EventArgs> OnAdLoaded = delegate {};
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad = delegate {};
        public event EventHandler<EventArgs> OnAdOpening = delegate {};
        public event EventHandler<EventArgs> OnAdStarted = delegate {};
        public event EventHandler<EventArgs> OnAdClosed = delegate {};
        public event EventHandler<Reward> OnAdRewarded = delegate {};
        public event EventHandler<EventArgs> OnAdLeavingApplication = delegate {};

        private IntPtr rewardBasedVideoAdPtr;

        // This property should be used when setting the rewardBasedVideoPtr.
        private IntPtr RewardBasedVideoAdPtr {
            get { return rewardBasedVideoAdPtr; }
            set
            {
                Externs.GADURelease(rewardBasedVideoAdPtr);
                rewardBasedVideoAdPtr = value;
            }
        }

        #region IGoogleMobileAdsRewardBasedVideoClient implementation

        // Creates a reward based video.
        public void CreateRewardBasedVideoAd()
        {
            IntPtr rewardBasedVideoAdPtr = (IntPtr)GCHandle.Alloc(this);
            RewardBasedVideoAdPtr = Externs.GADUCreateRewardBasedVideoAd(rewardBasedVideoAdPtr);

            Externs.GADUSetRewardBasedVideoAdCallbacks(
                RewardBasedVideoAdPtr,
                RewardBasedVideoAdDidReceiveAdCallback,
                RewardBasedVideoAdDidFailToReceiveAdWithErrorCallback,
                RewardBasedVideoAdDidOpenCallback,
                RewardBasedVideoAdDidStartCallback,
                RewardBasedVideoAdDidCloseCallback,
                RewardBasedVideoAdDidRewardUserCallback,
                RewardBasedVideoAdWillLeaveApplicationCallback);
        }

        // Load an ad.
        public void LoadAd(AdRequest request, string adUnitId)
        {
            IntPtr requestPtr = Utils.BuildAdRequest(request);
            Externs.GADURequestRewardBasedVideoAd(
                RewardBasedVideoAdPtr, requestPtr, adUnitId);
            Externs.GADURelease(requestPtr);
        }

        // Show the reward based video on the screen.
        public void ShowRewardBasedVideoAd()
        {
            Externs.GADUShowRewardBasedVideoAd(RewardBasedVideoAdPtr);
        }

        public bool IsLoaded()
        {
            return Externs.GADURewardBasedVideoAdReady(RewardBasedVideoAdPtr);
        }

        public void Dispose()
        {
            ((GCHandle)rewardBasedVideoAdPtr).Free();
        }

        ~RewardBasedVideoAdClient()
        {
            Dispose();
        }

        #endregion

        #region Reward based video ad callback methods

        [MonoPInvokeCallback(typeof(GADURewardBasedVideoAdDidReceiveAdCallback))]
        private static void RewardBasedVideoAdDidReceiveAdCallback(IntPtr rewardBasedVideoAdClient)
        {
            RewardBasedVideoAdClient client = IntPtrToRewardBasedVideoClient(
                rewardBasedVideoAdClient);
            client.OnAdLoaded(client, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(GADURewardBasedVideoAdDidFailToReceiveAdWithErrorCallback))]
        private static void RewardBasedVideoAdDidFailToReceiveAdWithErrorCallback(
            IntPtr rewardBasedVideoAdClient, string error)
        {
            RewardBasedVideoAdClient client = IntPtrToRewardBasedVideoClient(
                rewardBasedVideoAdClient);
            AdFailedToLoadEventArgs args = new AdFailedToLoadEventArgs() {
                Message = error
            };
            client.OnAdFailedToLoad(client, args);
        }

        [MonoPInvokeCallback(typeof(GADURewardBasedVideoAdDidOpenCallback))]
        private static void RewardBasedVideoAdDidOpenCallback(IntPtr rewardBasedVideoAdClient)
        {
            RewardBasedVideoAdClient client = IntPtrToRewardBasedVideoClient(
                rewardBasedVideoAdClient);
            client.OnAdOpening(client, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(GADURewardBasedVideoAdDidStartCallback))]
        private static void RewardBasedVideoAdDidStartCallback(IntPtr rewardBasedVideoAdClient)
        {
            RewardBasedVideoAdClient client = IntPtrToRewardBasedVideoClient(
                rewardBasedVideoAdClient);
            client.OnAdStarted(client, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(GADURewardBasedVideoAdDidCloseCallback))]
        private static void RewardBasedVideoAdDidCloseCallback(IntPtr rewardBasedVideoAdClient)
        {
            RewardBasedVideoAdClient client = IntPtrToRewardBasedVideoClient(
                rewardBasedVideoAdClient);
            client.OnAdClosed(client, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(GADURewardBasedVideoAdDidRewardCallback))]
        private static void RewardBasedVideoAdDidRewardUserCallback(
            IntPtr rewardBasedVideoAdClient, string rewardType, double rewardAmount)
        {
            Reward args = new Reward() {
                Type = rewardType,
                Amount = rewardAmount
            };
            RewardBasedVideoAdClient client = IntPtrToRewardBasedVideoClient(
                rewardBasedVideoAdClient);
            client.OnAdRewarded(client, args);
        }

        [MonoPInvokeCallback(typeof(GADURewardBasedVideoAdWillLeaveApplicationCallback))]
        private static void RewardBasedVideoAdWillLeaveApplicationCallback(
            IntPtr rewardBasedVideoAdClient)
        {
            RewardBasedVideoAdClient client = IntPtrToRewardBasedVideoClient(
                rewardBasedVideoAdClient);
            client.OnAdLeavingApplication(client, EventArgs.Empty);
        }

        private static RewardBasedVideoAdClient IntPtrToRewardBasedVideoClient(
            IntPtr rewardBasedVideoAdClient)
        {
            GCHandle handle = (GCHandle)rewardBasedVideoAdClient;
            return handle.Target as RewardBasedVideoAdClient;
        }

        #endregion
    }
}

