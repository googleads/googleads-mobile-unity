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
        public bool IsDestroyed { get; private set; }

        private IntPtr rewardedInterstitialAdClientPtr;
        private IntPtr rewardedInterstitialAdPtr;

#region rewarded interstitial ad callback types

        internal delegate void GADURewardedInterstitialAdLoadedCallback(IntPtr rewardedInterstitialAdClient);

        internal delegate void GADURewardedInterstitialAdFailedToLoadCallback(IntPtr rewardedInterstitialAdClient, IntPtr error);

        internal delegate void GADURewardedInterstitialAdUserEarnedRewardCallback(
            IntPtr rewardedInterstitialAdClient, string rewardType, double rewardAmount);

        internal delegate void GADURewardedInterstitialAdPaidEventCallback(
            IntPtr rewardedInterstitialAdClient, int precision, long value, string currencyCode);

#endregion

#region full screen content callback types

        internal delegate void GADURewardedInterstitialAdFailedToPresentFullScreenContentCallback(IntPtr rewardedInterstitialAdClient, IntPtr error);

        internal delegate void GADURewardedInterstitialAdWillPresentFullScreenContentCallback(IntPtr rewardedInterstitialAdClient);

        internal delegate void GADURewardedInterstitialAdDidDismissFullScreenContentCallback(IntPtr rewardedInterstitialAdClient);

        internal delegate void GADURewardedInterstitialAdDidRecordImpressionCallback(IntPtr rewardedInterstitialAdClient);

        internal delegate void GADURewardedInterstitialAdDidRecordClickCallback(IntPtr rewardedInterstitialAdClient);

#endregion

        public event Action OnAdFullScreenContentOpened = delegate { };
        public event Action OnAdFullScreenContentClosed = delegate { };
        public event Action<IAdErrorClient> OnAdFullScreenContentFailed = delegate { };
        public event Action<AdValue> OnAdPaid = delegate { };
        public event Action OnAdClickRecorded = delegate { };
        public event Action OnAdImpressionRecorded = delegate { };

        private Action<IRewardedInterstitialAdClient, ILoadAdErrorClient> _loadCallback;
        private Action<Reward> _rewardCallback;

        // This property should be used when setting the rewardedInterstitialAdPtr.
        private IntPtr RewardedInterstitialAdPtr
        {
            get
            {
                return this.rewardedInterstitialAdPtr;
            }

            set
            {
                Externs.GADURelease(this.rewardedInterstitialAdPtr);
                this.rewardedInterstitialAdPtr = value;
            }
        }

#region IRewardedInterstitialAdClient implementation

        public RewardedInterstitialAdClient()
        {
            this.rewardedInterstitialAdClientPtr = (IntPtr)GCHandle.Alloc(this);
            this.RewardedInterstitialAdPtr = Externs.GADUCreateRewardedInterstitialAd(this.rewardedInterstitialAdClientPtr);

            Externs.GADUSetRewardedInterstitialAdCallbacks(
                this.RewardedInterstitialAdPtr,
                RewardedInterstitialAdLoadedCallback,
                RewardedInterstitialAdFailedToLoadCallback,
                RewardedInterstitialAdUserDidEarnRewardCallback,
                RewardedInterstitialAdPaidEventCallback,
                AdFailedToPresentFullScreenContentCallback,
                AdWillPresentFullScreenContentCallback,
                AdDidDismissFullScreenContentCallback,
                AdDidRecordImpressionCallback,
                AdDidRecordClickCallback);
        }

        public void LoadRewardedInterstitialAd(string adUnitId, AdRequest request,
            Action<IRewardedInterstitialAdClient, ILoadAdErrorClient> callback)
        {
            _loadCallback = callback;
            IntPtr requestPtr = Utils.BuildAdRequest(request);
            Externs.GADULoadRewardedInterstitialAd(this.RewardedInterstitialAdPtr, adUnitId, requestPtr);
            Externs.GADURelease(requestPtr);
        }

        // Show the rewarded interstitial ad on the screen.
        public void Show(Action<Reward> userRewardEarnedCallback)
        {
            _rewardCallback = userRewardEarnedCallback;
            Externs.GADUShowRewardedInterstitialAd(this.RewardedInterstitialAdPtr);
        }

        // Sets the server side verification options
        public void SetServerSideVerificationOptions(ServerSideVerificationOptions serverSideVerificationOptions)
        {
            IntPtr optionsPtr = Utils.BuildServerSideVerificationOptions(serverSideVerificationOptions);
            Externs.GADURewardedInterstitialAdSetServerSideVerificationOptions(this.RewardedInterstitialAdPtr, optionsPtr);
            Externs.GADURelease(optionsPtr);
        }

        // Returns the reward item for the loaded rewarded interstitial ad.
        public Reward GetRewardItem()
        {
            string type = Externs.GADURewardedInterstitialAdGetRewardType(this.RewardedInterstitialAdPtr);
            double amount = Externs.GADURewardedInterstitialAdGetRewardAmount(this.RewardedInterstitialAdPtr); ;
            return new Reward()
            {
                Type = type,
                Amount = amount
            };
        }

        public IResponseInfoClient GetResponseInfoClient()
        {
            return new ResponseInfoClient(ResponseInfoClientType.AdLoaded, this.RewardedInterstitialAdPtr);
        }

        // Destroys the rewarded interstitial ad.
        public void Destroy()
        {
            this.RewardedInterstitialAdPtr = IntPtr.Zero;
            IsDestroyed = true;
        }

        public void Dispose()
        {
            this.Destroy();
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
            if (client._loadCallback != null)
            {
                client._loadCallback(client, null);
                client._loadCallback = null;
            }
        }

        [MonoPInvokeCallback(typeof(GADURewardedInterstitialAdFailedToLoadCallback))]
        private static void RewardedInterstitialAdFailedToLoadCallback(
            IntPtr rewardedInterstitialAdClient, IntPtr error)
        {
            RewardedInterstitialAdClient client = IntPtrToRewardedInterstitialAdClient(
                rewardedInterstitialAdClient);
            if (client._loadCallback != null)
            {
                client._loadCallback(null, new LoadAdErrorClient(error));
                client._loadCallback = null;
            }
        }

        [MonoPInvokeCallback(typeof(GADURewardedInterstitialAdUserEarnedRewardCallback))]
        private static void RewardedInterstitialAdUserDidEarnRewardCallback(
            IntPtr rewardedInterstitialAdClient, string rewardType, double rewardAmount)
        {
            RewardedInterstitialAdClient client = IntPtrToRewardedInterstitialAdClient(
                rewardedInterstitialAdClient);
            if (client._rewardCallback != null)
            {
                Reward reward = new Reward()
                {
                    Type = rewardType,
                    Amount = rewardAmount
                };
                client._rewardCallback(reward);
                client._rewardCallback = null;
            }
        }

        [MonoPInvokeCallback(typeof(GADURewardedInterstitialAdPaidEventCallback))]
        private static void RewardedInterstitialAdPaidEventCallback(
            IntPtr rewardedInterstitialAdClient, int precision, long value, string currencyCode)
        {
            RewardedInterstitialAdClient client = IntPtrToRewardedInterstitialAdClient(
                rewardedInterstitialAdClient);
            AdValue adValue = new AdValue()
            {
                Precision = (AdValue.PrecisionType)precision,
                Value = value,
                CurrencyCode = currencyCode
            };
            client.OnAdPaid(adValue);
        }

        [MonoPInvokeCallback(typeof(GADURewardedInterstitialAdFailedToPresentFullScreenContentCallback))]
        private static void AdFailedToPresentFullScreenContentCallback(IntPtr rewardedInterstitialAdClient, IntPtr error)
        {
            RewardedInterstitialAdClient client = IntPtrToRewardedInterstitialAdClient(rewardedInterstitialAdClient);
            client.OnAdFullScreenContentFailed(new AdErrorClient(error));
        }

        [MonoPInvokeCallback(typeof(GADURewardedInterstitialAdWillPresentFullScreenContentCallback))]
        private static void AdWillPresentFullScreenContentCallback(IntPtr rewardedInterstitialAdClient)
        {
            RewardedInterstitialAdClient client = IntPtrToRewardedInterstitialAdClient(rewardedInterstitialAdClient);
            client.OnAdFullScreenContentOpened();
        }

        [MonoPInvokeCallback(typeof(GADURewardedInterstitialAdDidDismissFullScreenContentCallback))]
        private static void AdDidDismissFullScreenContentCallback(IntPtr rewardedInterstitialAdClient)
        {
            RewardedInterstitialAdClient client = IntPtrToRewardedInterstitialAdClient(rewardedInterstitialAdClient);
            client.OnAdFullScreenContentClosed();
        }

        [MonoPInvokeCallback(typeof(GADURewardedInterstitialAdDidRecordImpressionCallback))]
        private static void AdDidRecordImpressionCallback(IntPtr rewardedInterstitialAdClient)
        {
            RewardedInterstitialAdClient client = IntPtrToRewardedInterstitialAdClient(rewardedInterstitialAdClient);
            client.OnAdImpressionRecorded();
        }

        [MonoPInvokeCallback(typeof(GADURewardedInterstitialAdDidRecordClickCallback))]
        private static void AdDidRecordClickCallback(IntPtr rewardedInterstitialAdClient)
        {
            RewardedInterstitialAdClient client = IntPtrToRewardedInterstitialAdClient(rewardedInterstitialAdClient);
            client.OnAdClickRecorded();
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
