#if UNITY_IOS
// Copyright 2015-2021 Google LLC
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
    public class RewardedAdClient : IRewardedAdClient, IDisposable
    {
        public bool IsDestroyed { get; private set; }

        private IntPtr rewardedAdClientPtr;
        private IntPtr rewardedAdPtr;

#region rewarded ad callback types

        internal delegate void GADURewardedAdLoadedCallback(IntPtr rewardedAdClient);

        internal delegate void GADURewardedAdFailedToLoadCallback(IntPtr rewardedAdClient, IntPtr error);

        internal delegate void GADURewardedAdUserEarnedRewardCallback(
            IntPtr rewardedAdClient, string rewardType, double rewardAmount);

        internal delegate void GADURewardedAdPaidEventCallback(
            IntPtr rewardedAdClient, int precision, long value, string currencyCode);

#endregion

#region full screen content callback types

        internal delegate void GADURewardedAdFailedToPresentFullScreenContentCallback(IntPtr rewardedAdClient, IntPtr error);

        internal delegate void GADURewardedAdWillPresentFullScreenContentCallback(IntPtr rewardedAdClient);

        internal delegate void GADURewardedAdDidDismissFullScreenContentCallback(IntPtr rewardedAdClient);

        internal delegate void GADURewardedAdDidRecordImpressionCallback(IntPtr rewardedAdClient);

        internal delegate void GADURewardedAdDidRecordClickCallback(IntPtr rewardedAdClient);

#endregion

        public event Action OnAdFullScreenContentOpened = delegate { };
        public event Action OnAdFullScreenContentClosed = delegate { };
        public event Action<IAdErrorClient> OnAdFullScreenContentFailed = delegate { };
        public event Action<AdValue> OnAdPaid = delegate { };
        public event Action OnAdClickRecorded = delegate { };
        public event Action OnAdImpressionRecorded = delegate { };

        private Action<IRewardedAdClient, ILoadAdErrorClient> _loadCallback;
        private Action<Reward> _rewardCallback;

        // This property should be used when setting the rewardedAdPtr.
        private IntPtr RewardedAdPtr
        {
            get
            {
                return this.rewardedAdPtr;
            }

            set
            {
                Externs.GADURelease(this.rewardedAdPtr);
                this.rewardedAdPtr = value;
            }
        }

        #region IRewardedAdClient implementation

        public RewardedAdClient()
        {
            this.rewardedAdClientPtr = (IntPtr)GCHandle.Alloc(this);
            this.RewardedAdPtr = Externs.GADUCreateRewardedAd(this.rewardedAdClientPtr);

            Externs.GADUSetRewardedAdCallbacks(
                this.RewardedAdPtr,
                RewardedAdLoadedCallback,
                RewardedAdFailedToLoadCallback,
                AdWillPresentFullScreenContentCallback,
                AdFailedToPresentFullScreenContentCallback,
                AdDidDismissFullScreenContentCallback,
                AdDidRecordImpressionCallback,
                AdDidRecordClickCallback,
                RewardedAdUserDidEarnRewardCallback,
                RewardedAdPaidEventCallback);
        }


        public void LoadRewardedAd(string adUnitId, AdRequest request,
            Action<IRewardedAdClient, ILoadAdErrorClient> callback)
        {
            _loadCallback = callback;
            IntPtr requestPtr = Utils.BuildAdRequest(request);
            Externs.GADULoadRewardedAd(this.RewardedAdPtr, adUnitId, requestPtr);
            Externs.GADURelease(requestPtr);
        }

        // Show the rewarded ad on the screen.
        public void Show(Action<Reward> userRewardEarnedCallback)
        {
            _rewardCallback = userRewardEarnedCallback;
            Externs.GADUShowRewardedAd(this.RewardedAdPtr);
        }

        // Sets the server side verification options
        public void SetServerSideVerificationOptions(ServerSideVerificationOptions serverSideVerificationOptions)
        {
            IntPtr optionsPtr = Utils.BuildServerSideVerificationOptions(serverSideVerificationOptions);
            Externs.GADURewardedAdSetServerSideVerificationOptions(this.RewardedAdPtr, optionsPtr);
            Externs.GADURelease(optionsPtr);
        }

        // Returns the reward item for the loaded rewarded ad.
        public Reward GetRewardItem()
        {
            string type = Externs.GADURewardedAdGetRewardType(this.RewardedAdPtr);
            double amount = Externs.GADURewardedAdGetRewardAmount(this.RewardedAdPtr); ;
            return new Reward()
            {
                Type = type,
                Amount = amount
            };
        }

        public IResponseInfoClient GetResponseInfoClient()
        {
            return new ResponseInfoClient(ResponseInfoClientType.AdLoaded, this.RewardedAdPtr);
        }

        // Destroys the rewarded ad.
        public void Destroy()
        {
            this.RewardedAdPtr = IntPtr.Zero;
            IsDestroyed = true;
        }

        public void Dispose()
        {
            this.Destroy();
            ((GCHandle)this.rewardedAdClientPtr).Free();
        }

        ~RewardedAdClient()
        {
            this.Dispose();
        }

#endregion

#region rewarded ad callback methods

        [MonoPInvokeCallback(typeof(GADURewardedAdLoadedCallback))]
        private static void RewardedAdLoadedCallback(IntPtr rewardedAdClient)
        {
            RewardedAdClient client = IntPtrToRewardedAdClient(rewardedAdClient);
            if (client._loadCallback != null)
            {
                client._loadCallback(client, null);
                client._loadCallback = null;
            }
        }

        [MonoPInvokeCallback(typeof(GADURewardedAdFailedToLoadCallback))]
        private static void RewardedAdFailedToLoadCallback(
            IntPtr rewardedAdClient, IntPtr error)
        {
            RewardedAdClient client = IntPtrToRewardedAdClient(rewardedAdClient);
            if (client._loadCallback != null)
            {
                client._loadCallback(null, new LoadAdErrorClient(error));
                client._loadCallback = null;
            }
        }

        [MonoPInvokeCallback(typeof(GADURewardedAdUserEarnedRewardCallback))]
        private static void RewardedAdUserDidEarnRewardCallback(
            IntPtr rewardedAdClient, string rewardType, double rewardAmount)
        {
            RewardedAdClient client = IntPtrToRewardedAdClient(
                rewardedAdClient);
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

        [MonoPInvokeCallback(typeof(GADURewardedAdPaidEventCallback))]
        private static void RewardedAdPaidEventCallback(
            IntPtr rewardedAdClient, int precision, long value, string currencyCode)
        {
            RewardedAdClient client = IntPtrToRewardedAdClient(rewardedAdClient);
            AdValue adValue = new AdValue()
            {
                Precision = (AdValue.PrecisionType)precision,
                Value = value,
                CurrencyCode = currencyCode
            };
            client.OnAdPaid(adValue);
        }

        [MonoPInvokeCallback(typeof(GADURewardedAdFailedToPresentFullScreenContentCallback))]
        private static void AdFailedToPresentFullScreenContentCallback(IntPtr rewardedAdClient, IntPtr error)
        {
            RewardedAdClient client = IntPtrToRewardedAdClient(rewardedAdClient);
            client.OnAdFullScreenContentFailed(new AdErrorClient(error));
        }

        [MonoPInvokeCallback(typeof(GADURewardedAdWillPresentFullScreenContentCallback))]
        private static void AdWillPresentFullScreenContentCallback(IntPtr rewardedAdClient)
        {
          RewardedAdClient client = IntPtrToRewardedAdClient(rewardedAdClient);
          client.OnAdFullScreenContentOpened();
        }

        [MonoPInvokeCallback(typeof(GADURewardedAdDidDismissFullScreenContentCallback))]
        private static void AdDidDismissFullScreenContentCallback(IntPtr rewardedAdClient)
        {
            RewardedAdClient client = IntPtrToRewardedAdClient(rewardedAdClient);
            client.OnAdFullScreenContentClosed();
        }

        [MonoPInvokeCallback(typeof(GADURewardedAdDidRecordImpressionCallback))]
        private static void AdDidRecordImpressionCallback(IntPtr rewardedAdClient)
        {
            RewardedAdClient client = IntPtrToRewardedAdClient(rewardedAdClient);
            client.OnAdImpressionRecorded();
        }

        [MonoPInvokeCallback(typeof(GADURewardedAdDidRecordClickCallback))]
        private static void AdDidRecordClickCallback(IntPtr rewardedAdClient)
        {
            RewardedAdClient client = IntPtrToRewardedAdClient(rewardedAdClient);
            client.OnAdClickRecorded();
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
