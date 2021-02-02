// Copyright (C) 2019 Google LLC
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
    public class RewardedAdGameObject : AdGameObject
    {

#pragma warning disable 0649

        [SerializeField]
        private UnityEvent onAdLoaded;

        [SerializeField]
        private AdFailedToLoadEvent onAdFailedToLoad;

        [SerializeField]
        private UnityEvent onAdFailedToShow;

        [SerializeField]
        private UnityEvent onAdOpening;

        [SerializeField]
        private UserEarnedRewardEvent onUserEarnedReward;

        [SerializeField]
        private UnityEvent onAdClosed;

#pragma warning restore 0649

        private RewardedAd ad;

        public RewardedAd RewardedAd
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
            ad = new RewardedAd(AdUnitId);
            AddCallbacks();
            ad.LoadAd(adRequest);
        }

        public void ShowIfLoaded()
        {
            if (ad.IsLoaded())
            {
                ad.Show();
            }
        }

        protected override void AddCallbacks()
        {
            ad.OnAdLoaded += (sender, args) =>
            {
                if (onAdLoaded != null)
                {
                    MobileAdsEventExecutor.InvokeInUpdate(onAdLoaded);
                }
            };
            ad.OnAdFailedToLoad += (sender, args) =>
            {
                if (onAdFailedToLoad != null)
                {
                    MobileAdsEventExecutor.ExecuteInUpdate(() =>
                    {
                        onAdFailedToLoad.Invoke(args.Message);
                    });
                }
            };

            ad.OnAdFailedToShow += (sender, args) =>
            {
                if (onAdFailedToShow != null)
                {
                   MobileAdsEventExecutor.InvokeInUpdate(onAdFailedToShow);
                }
            };

            ad.OnAdOpening += (sender, args) =>
            {
                if (onAdOpening != null)
                {
                    MobileAdsEventExecutor.InvokeInUpdate(onAdOpening);
                }
            };

            ad.OnUserEarnedReward += (sender, args) =>
            {
                if (onUserEarnedReward != null)
                {
                    MobileAdsEventExecutor.ExecuteInUpdate(() =>
                    {
                        onUserEarnedReward.Invoke(args);
                    });
                }
            };

            ad.OnAdClosed += (sender, args) =>
            {
                if (onAdClosed != null)
                {
                    MobileAdsEventExecutor.InvokeInUpdate(onAdClosed);
                }
            };
        }

#endregion
    }
}
