// Copyright (C) 2022 Google LLC.
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

namespace GoogleMobileAds.Unity
{
    public class BaseAdClient
    {
        public event Action<AdValue> OnAdPaid = delegate{};
        public event Action OnAdClickRecorded = delegate{};
        public event Action OnAdImpressionRecorded = delegate{};
        public event Action OnAdFullScreenContentOpened = delegate{};
        public event Action OnAdFullScreenContentClosed = delegate{};
        public event Action<IAdErrorClient> OnAdFullScreenContentFailed = delegate{};

        public bool IsDestroyed { get { return adPrefab == null; } }

        public IResponseInfoClient Response { get; set; }
        public Reward RewardItem { get; set; }

        protected GameObject adPrefab;
        protected GameObject adInstance;

        public BaseAdClient()
        {
            Response = new ResponseInfo();
            RewardItem = new Reward { Amount = 10, Type = "Reward" };
        }

        public virtual void Destroy()
        {
            GameObject.Destroy(adInstance);
            adPrefab = null;
            adInstance = null;

        }
        public void RaiseAdPaid(AdValue args)
        {
            OnAdPaid(args);
        }

        public void RaiseAdClickRecorded()
        {
            OnAdClickRecorded();
        }

        public void RaiseAdImpressionRecorded()
        {
            OnAdImpressionRecorded();
        }

        public void RaiseAdFullScreenContentOpened()
        {
            OnAdFullScreenContentOpened();
        }

        public void RaiseAdFullScreenContentClosed()
        {
            OnAdFullScreenContentClosed();
        }

        public void RaiseAdFullScreenContentFailed(IAdErrorClient error)
        {
            OnAdFullScreenContentFailed(error);
        }

        protected void LoadPrefab(string prefabName)
        {
            adPrefab = Resources.Load(prefabName) as GameObject;
            if (adPrefab == null)
            {
                Debug.LogWarning("No Prefab found.");
            }
        }

        protected GameObject LoadAd(Vector3 position)
        {
            adInstance = GameObject.Instantiate(
                adPrefab,
                position,
                Quaternion.identity);
            return adInstance;
        }

        protected void PauseGame()
        {
            Time.timeScale = 0;
            Debug.Log("Pause Game");
        }

        protected void ResumeGame()
        {
            Time.timeScale = 1;
            Debug.Log("Resume Game");
        }

        protected void NavigateToAdUrl()
        {
            Debug.Log("Opened URL");
            Application.OpenURL("http://google.com");
        }
    }
}
