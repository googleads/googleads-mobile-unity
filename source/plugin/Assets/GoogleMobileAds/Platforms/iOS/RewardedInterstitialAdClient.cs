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
using System.Runtime.InteropServices;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    public class RewardedInterstitialAdClient : BaseAdClient,
                                                IRewardedInterstitialAdClient
    {

        #region RewardedInterstitialAd externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADURewardedInterstitialAdCreate(IntPtr adClientPtr);

        [DllImport("__Internal")]
        internal static extern IntPtr GADURewardedInterstitialAdLoad(IntPtr adBridgePtr,
                                                                     string adUnitID,
                                                                     IntPtr request);

        [DllImport("__Internal")]
        internal static extern void GADURewardedInterstitialAdShow(IntPtr adBridgePtr);

        [DllImport("__Internal")]
        internal static extern void GADURewardedInterstitialAdSetSSVOptions(IntPtr adBridgePtr,
                                                                            IntPtr options);

        [DllImport("__Internal")]
        internal static extern string GADURewardedInterstitialAdGetRewardType(IntPtr adBridgePtr);

        [DllImport("__Internal")]
        internal static extern double GADURewardedInterstitialAdGetRewardAmount(IntPtr adBridgePtr);

        #endregion

        private Action<IRewardedInterstitialAdClient, ILoadAdErrorClient> _loadCallback;
        private Action<Reward> _rewardCallback;

        public void LoadRewardedInterstitialAd(
            string adUnitId,
            AdRequest request,
            Action<IRewardedInterstitialAdClient, ILoadAdErrorClient> callback)
        {
            _loadCallback = callback;

            AdClientPtr = (IntPtr)GCHandle.Alloc(this);
            AdBridgePtr = GADURewardedInterstitialAdCreate(AdClientPtr);

            SetClientCallbacks();

            IntPtr requestPtr = Utils.BuildAdRequest(request);
            GADURewardedInterstitialAdLoad(AdBridgePtr, adUnitId, requestPtr);
            Externs.GADURelease(requestPtr);
        }

        public void Show(Action<Reward> userRewardEarnedCallback)
        {
            _rewardCallback = userRewardEarnedCallback;
            GADURewardedInterstitialAdShow(AdBridgePtr);
        }

        public void Destroy()
        {
            Dispose();
            _rewardCallback = null;
        }

        public void SetServerSideVerificationOptions(ServerSideVerificationOptions options)
        {
            IntPtr optionsPtr = Utils.BuildServerSideVerificationOptions(options);
            GADURewardedInterstitialAdSetSSVOptions(AdBridgePtr, optionsPtr);
            Externs.GADURelease(optionsPtr);
        }

        public Reward GetRewardItem()
        {
            string type = GADURewardedInterstitialAdGetRewardType(AdBridgePtr);
            double amount = GADURewardedInterstitialAdGetRewardAmount(AdBridgePtr);
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
