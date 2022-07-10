#if UNITY_IOS
// Copyright 2015-2022 Google LLC
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
using System.Runtime.InteropServices;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{

    public class RewardedAdClient : BaseAdClient, IRewardedAdClient
    {
        #region RewardedAd externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADURewardedAdCreate(IntPtr adClientPtr);

        [DllImport("__Internal")]
        internal static extern IntPtr GADURewardedAdLoad(IntPtr adBridgePtr, string adUnitID,
            IntPtr request);

        [DllImport("__Internal")]
        internal static extern void GADURewardedAdShow(IntPtr adBridgePtr);


        [DllImport("__Internal")]
        internal static extern void GADURewardedAdSetSSVOptions(IntPtr adBridgePtr, IntPtr options);

        [DllImport("__Internal")]
        internal static extern string GADURewardedAdGetRewardType(IntPtr adBridgePtr);

        [DllImport("__Internal")]
        internal static extern double GADURewardedAdGetRewardAmount(IntPtr adBridgePtr);

        #endregion
        private Action<IRewardedAdClient, ILoadAdErrorClient> _loadCallback;
        private Action<Reward> _rewardCallback;

        public void LoadAd(
            string adUnitId,
            AdRequest request,
            Action<IRewardedAdClient, ILoadAdErrorClient> callback)
        {
            _loadCallback = callback;

            AdClientPtr = (IntPtr)GCHandle.Alloc(this);
            AdBridgePtr = GADURewardedAdCreate(AdClientPtr);

            SetClientCallbacks();

            IntPtr requestPtr = Utils.BuildAdRequest(request);
            GADURewardedAdLoad(AdBridgePtr, adUnitId, requestPtr);
            Externs.GADURelease(requestPtr);
        }


        // Show the rewarded ad on the screen.
        public void Show(Action<Reward> userRewardEarnedCallback)
        {
            _rewardCallback = userRewardEarnedCallback;
            GADURewardedAdShow(AdBridgePtr);
        }

        public void Destroy()
        {
            Dispose();
            _rewardCallback = null;
        }

        public void SetServerSideVerificationOptions(ServerSideVerificationOptions options)
        {
            IntPtr optionsPtr = Utils.BuildServerSideVerificationOptions(options);
            Externs.GADURewardedAdSetServerSideVerificationOptions(AdBridgePtr, optionsPtr);
            Externs.GADURelease(optionsPtr);
        }

        public Reward GetRewardItem()
        {
            string type = GADURewardedAdGetRewardType(AdBridgePtr);
            double amount = GADURewardedAdGetRewardAmount(AdBridgePtr);
            return new Reward()
            {
                Type = type,
                Amount = amount
            };
        }

        protected override void OnUserEarnedReward(Reward reward)
        {
            if (_rewardCallback != null)
            {
                _rewardCallback(reward);
                _rewardCallback = null;
            }
        }

        protected override void OnAdLoaded()
        {
            if (_loadCallback != null)
            {
                _loadCallback(this, null);
                _loadCallback = null;
            }
        }

        protected override void OnAdLoadFailed(ILoadAdErrorClient error)
        {
            if (_loadCallback != null)
            {
                _loadCallback(null, error);
                _loadCallback = null;
            }
        }
    }
}
#endif
