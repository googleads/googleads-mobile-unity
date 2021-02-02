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

using System;
using UnityEngine;

using GoogleMobileAds.Common;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Placement
{
    public abstract class AdGameObject : MonoBehaviour
    {
        [SerializeField]
        public AdPlacement.AdType adType;
        [SerializeField]
        protected int selectedPlacementIndex;
#pragma warning disable 0649

        [SerializeField]
        private string androidAdUnitId;

        [SerializeField]
        private string iOSAdUnitId;

#pragma warning restore 0649

        [SerializeField]
        protected bool persistent;

        [SerializeField]
        protected bool autoLoadEnabled;

        public string AndroidAdUnitId
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Ad unit ID cannot be null");
                }
                this.androidAdUnitId = value;
            }
        }

        public string IOSAdUnitId
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Ad unit ID cannot be null");
                }
                this.iOSAdUnitId = value;
            }
        }
        public string AdUnitId
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    return this.androidAdUnitId;
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    return this.iOSAdUnitId;
                }
                else
                {
                    return "unexpected_platform";
                }
            }
        }

        public virtual void Start()
        {
            if (autoLoadEnabled)
            {
                AutoLoadAd();
            }
        }

        public void LoadAd()
        {
            LoadAd(new AdRequest.Builder().Build());
        }

        public abstract void LoadAd(AdRequest adRequest);

        protected abstract void AddCallbacks();

        protected void AutoLoadAd()
        {
            InitCompleteExecutor.Instance.InvokeOnInitComplete(() =>
            {
                LoadAd();
            });
        }

        protected bool AddGameObjectToPool()
        {
            try
            {
                AdGameObjectPool.Instance.Add(this.GetType(), this.gameObject, persistent);
            }
            catch (ArgumentException e)
            {
                // Something went wrong.
                Debug.LogError(e.Message);
                return false;
            }
#pragma warning disable 0168
            catch (OperationCanceledException e)
            {
#pragma warning restore 0168
                // Redundant instance was created. (Unity's intended behavior)
                // Simply mark newly created instance was not added to the pool.
                return false;
            }

            return true;
        }

        protected bool RemoveGameObjectFromPoolIfNeeded()
        {
            if (!persistent)
            {
                AdGameObjectPool.Instance.Remove(this.GetType(), this.gameObject, persistent);
                return true;
            }
            return false;
        }
    }
}
