// Copyright (C) 2022 Google LLC
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
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    public class RewardedInterstitialAdClient : BaseAdClient,
                                                IRewardedInterstitialAdClient
    {

        public Reward RewardItem
        {
            get
            {
                return GetRewardItem();
            }
        }

        private Action<IRewardedInterstitialAdClient, ILoadAdErrorClient> _loadCallback;
        private Action<Reward> _userRewardEarnedCallback;

        public RewardedInterstitialAdClient() : base(Utils.UnityRewardedInterstitialAdClassName)
        {
        }

        public void LoadRewardedInterstitialAd(string adUnitId,
            AdRequest request,
            Action<IRewardedInterstitialAdClient, ILoadAdErrorClient> callback)
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.RewardInterstitialAd LoadRewardedInterstitialAd");
            //END_DEBUG_STRIP
            _loadCallback = callback;

            if (_ad != null)
            {
                _ad.Call("loadAd", adUnitId, Utils.GetAdRequestJavaObject(request));
            }
        }

        #region Callbacks from IInterstitalAd

        public void Destroy()
        {
            _ad = null;
        }

        public void Show(Action<Reward> userRewardEarnedCallback)
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.RewardInterstitialAd Show");
            //END_DEBUG_STRIP
            _userRewardEarnedCallback = userRewardEarnedCallback;

            if (_ad != null)
            {
                _ad.Call("show");
            }
        }

        public void SetServerSideVerificationOptions(ServerSideVerificationOptions options)
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.RewardInterstitialAd "+
              "SetServerSideVerificationOptions");
            //END_DEBUG_STRIP

            if (_ad != null)
            {
                _ad.Call("setServerSideVerificationOptions",
                    Utils.GetServerSideVerificationOptionsJavaObject(options));
            }
        }

        #endregion

        protected override void OnAdLoaded()
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.RewardInterstitialAd onAdLoaded");
            //END_DEBUG_STRIP
            if (_loadCallback != null)
            {
                _loadCallback(this, null);
            }
            _loadCallback = null;
        }

        protected override void OnAdLoadFailed(AndroidJavaObject error)
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.RewardInterstitialAd onAdLoadFailed");
            //END_DEBUG_STRIP
            if (_loadCallback != null)
            {
                _loadCallback(null, new LoadAdErrorClient(error));
            }
        }

        protected override void OnUserEarnedReward(Reward item)
        {
            if (_userRewardEarnedCallback != null)
            {
                _userRewardEarnedCallback(item);
            }
        }
    }
}
