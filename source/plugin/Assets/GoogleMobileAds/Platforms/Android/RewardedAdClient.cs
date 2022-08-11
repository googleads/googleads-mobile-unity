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
    public class RewardedAdClient : BaseAdClient,
                                    IRewardedAdClient
    {
        public Reward RewardItem
        {
            get
            {
                return GetRewardItem();
            }
        }

        private Action<IRewardedAdClient, ILoadAdErrorClient> _loadCallback;
        private Action<Reward> _userRewardEarnedCallback;

        public RewardedAdClient() : base(Utils.UnityRewardedAdClassName)
        {
        }

        public void LoadAd(string adUnitId,
            AdRequest request,
            Action<IRewardedAdClient, ILoadAdErrorClient> callback)
        {
            _loadCallback = callback;

            if (_ad != null)
            {
                _ad.Call("loadAd",
                    adUnitId,
                    Utils.GetAdRequestJavaObject(request));
            }
        }

        #region Callbacks from IInterstitalAd

        public void Destroy()
        {
            _ad = null;
        }

        public void Show(Action<Reward> userRewardEarnedCallback)
        {
            _userRewardEarnedCallback = userRewardEarnedCallback;

            if (_ad != null)
            {
                _ad.Call("show");
            }
        }

        public void SetServerSideVerificationOptions(ServerSideVerificationOptions options)
        {
            if (_ad != null)
            {
                _ad.Call("setServerSideVerificationOptions",
                    Utils.GetServerSideVerificationOptionsJavaObject(options));
            }
        }

        #endregion

        protected override void OnAdLoaded()
        {
            if (_loadCallback != null)
            {
                _loadCallback(this, null);
            }
            _loadCallback = null;
        }

        protected override void OnAdLoadFailed(AndroidJavaObject error)
        {
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
