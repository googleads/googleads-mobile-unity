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
    public class InterstitialAdGameObject : AdGameObject
    {

#pragma warning disable 0649

        [SerializeField]
        private UnityEvent onAdLoaded;

        [SerializeField]
        private AdFailedToLoadEvent onAdFailedToLoad;

        [SerializeField]
        private UnityEvent onAdOpening;

        [SerializeField]
        private UnityEvent onAdClosed;

        [SerializeField]
        private UnityEvent onAdLeavingApplication;

#pragma warning restore 0649

        private InterstitialAd ad;

        public InterstitialAd InterstitialAd
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
                ad.Destroy();
                ad = null;
            }
        }

#region Helper methods

        public override void LoadAd(AdRequest adRequest)
        {
            if (ad != null)
            {
                ad.Destroy();
            }

            ad = new InterstitialAd(AdUnitId);
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

        public void DestroyAd()
        {
            if (ad != null)
            {
                ad.Destroy();
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
                if (onAdFailedToLoad != null) {
                    MobileAdsEventExecutor.ExecuteInUpdate(() =>
                    {
                        onAdFailedToLoad.Invoke(args.Message);
                    });
                }
            };
            ad.OnAdOpening += (sender, args) =>
            {
                if (onAdOpening != null)
                {
                    MobileAdsEventExecutor.InvokeInUpdate(onAdOpening);
                }
            };
            ad.OnAdClosed += (sender, args) =>
            {
                if (onAdClosed != null)
                {
                    MobileAdsEventExecutor.InvokeInUpdate(onAdClosed);
                }
            };
            ad.OnAdLeavingApplication += (sender, args) =>
            {
                if (onAdLeavingApplication != null)
                {
                    MobileAdsEventExecutor.InvokeInUpdate(onAdLeavingApplication);
                }
            };
        }

#endregion
    }
}
