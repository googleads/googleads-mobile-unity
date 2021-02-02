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

using UnityEngine;
using UnityEngine.Events;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Placement
{
    public class RewardedInterstitialAdGameObject : AdGameObject
    {

#pragma warning disable 0649

        [SerializeField]
        private UnityEvent onAdLoaded;

        [SerializeField]
        private AdFailedToLoadEvent onAdFailedToLoad;

        [SerializeField]
        private AdErrorEvent onAdFailedToPresentFullScreenContent;

        [SerializeField]
        private UnityEvent onAdDidPresentFullScreenContent;

        [SerializeField]
        private UnityEvent onAdDidDismissFullScreenContent;

        [SerializeField]
        private UserEarnedRewardEvent onUserEarnedReward;

#pragma warning restore 0649

        private RewardedInterstitialAd ad;

        public RewardedInterstitialAd RewardedInterstitialAd
        {
            get
            {
                return this.ad;
            }
        }

        public void Awake()
        {
            bool added = AddGameObjectToPool();
            if (!added)
            {
                // GameObject was not added to the AdGameObjectPool,
                // since the GameoObject with the same name already exists in the pool.
                // Destroy the object as it will not be managed by the AdGameObjectPool.
                Destroy(this.gameObject);
                return;
            }
            else
            {
                if (persistent)
                {
                    DontDestroyOnLoad(this.gameObject);
                }
            }
        }

        public void OnDestroy()
        {
            bool removed = RemoveGameObjectFromPoolIfNeeded();

            if (removed && ad != null)
            {
                ad = null;
            }
        }

#region Helper methods

        public override void LoadAd(AdRequest adRequest)
        {
            RewardedInterstitialAd.LoadAd(AdUnitId, adRequest, (rewardedInterstitialAd, error) =>
            {
                if (error != null)
                {
                    if (onAdFailedToLoad != null)
                    {
                        MobileAdsEventExecutor.ExecuteInUpdate(() =>
                        {
                            onAdFailedToLoad.Invoke(error.Message);
                        });
                    }
                    return;
                }

                this.ad = rewardedInterstitialAd;
                AddCallbacks();

                if (onAdLoaded != null)
                {
                    MobileAdsEventExecutor.InvokeInUpdate(onAdLoaded);
                }

            });
        }

        public void ShowIfLoaded()
        {
            if (ad != null)
            {
                ad.Show((reward) =>
                {
                    if (onUserEarnedReward != null)
                    {
                        MobileAdsEventExecutor.ExecuteInUpdate(() =>
                        {
                            onUserEarnedReward.Invoke(reward);
                        });
                    }
                });
            }
        }

        protected override void AddCallbacks()
        {
            if (ad == null)
            {
                return;
            }
            // Register for ad events.
            ad.OnAdDidPresentFullScreenContent += (sender, args) =>
            {
                if (onAdDidPresentFullScreenContent != null)
                {
                    MobileAdsEventExecutor.InvokeInUpdate(onAdDidPresentFullScreenContent);
                }
            };
            ad.OnAdDidDismissFullScreenContent += (sender, args) =>
            {
                if (onAdDidDismissFullScreenContent != null)
                {
                    MobileAdsEventExecutor.InvokeInUpdate(onAdDidDismissFullScreenContent);
                }
                ad = null;
            };
            ad.OnAdFailedToPresentFullScreenContent += (sender, args) =>
            {
                if (onAdFailedToPresentFullScreenContent != null)
                {
                    MobileAdsEventExecutor.ExecuteInUpdate(() =>
                    {
                        onAdFailedToPresentFullScreenContent.Invoke(args.Message);
                    });
                }
                ad = null;
            };
        }

#endregion
    }
}
