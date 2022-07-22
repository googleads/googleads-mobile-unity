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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Unity
{
    public class BaseRewardedAdClient : BaseAdDummyClient
    {
        protected static readonly Dictionary<AdSize, string> prefabAds =
            new Dictionary<AdSize, string>()
            {
                { new AdSize (768,1024), "DummyAds/Rewarded/768x1024" },
                { new AdSize (1024,768), "DummyAds/Rewarded/1024x768" }
            };

        public bool IsDestroyed { get; private set; }

        public event Action OnAdFullScreenContentOpened = delegate { };
        public event Action OnAdFullScreenContentClosed = delegate { };
        public event Action<IAdErrorClient> OnAdFullScreenContentFailed = delegate { };
        public event Action<AdValue> OnAdPaid = delegate { };
        public event Action OnAdClickRecorded = delegate { };
        public event Action OnAdImpressionRecorded = delegate { };

        private ButtonBehaviour _buttonBehaviour;
        private Action<Reward> _rewardCallback;

        public Reward GetRewardItem()
        {
            return new Reward()
            {
                Type = "Reward",
                Amount = 10
            };
        }

        public void Show(Action<Reward> userRewardEarnedCallback)
        {
            _rewardCallback = userRewardEarnedCallback;
            if (prefabAd != null)
            {
                dummyAd = AdBehaviour.ShowAd(prefabAd, new Vector3(0, 0, 1));
                AdBehaviour.PauseGame();
                CreateButtonBehavior();
                AddClickBehavior(dummyAd);
                dummyAd.AddComponent<Countdown>();
                OnAdFullScreenContentOpened();
            }
            else
            {
                OnAdFullScreenContentFailed(new AdErrorClient());
            }
        }

        public void SetServerSideVerificationOptions(ServerSideVerificationOptions options)
        {
            Debug.Log("BaseRewardedAdClient.SetServerSideVerificationOptions");
        }

        public void Destroy()
        {
            IsDestroyed = true;
        }

        private void CreateButtonBehavior()
        {
            _buttonBehaviour = base.dummyAd.AddComponent<ButtonBehaviour>();
        }

        private void AddClickBehavior(GameObject dummyAd)
        {
            Image[] images = dummyAd.GetComponentsInChildren<Image>();
            Image adImage = images[1];
            Button button = adImage.GetComponentInChildren<Button>();
            button.onClick.AddListener(() =>
            {
                OnAdClickRecorded();
                _buttonBehaviour.OpenURL();
            });
            Button[] innerButtons = adImage.GetComponentsInChildren<Button>();

            if (innerButtons.Length < 2)
            {
                Debug.Log("Invalid Prefab");
                return;
            }

            Button closeButton = innerButtons[1];

            closeButton.onClick.AddListener(() =>
            {
                AdBehaviour.DestroyAd(dummyAd);
                prefabAd = null;
                OnAdFullScreenContentClosed();
                AdBehaviour.ResumeGame();
                if (_rewardCallback != null)
                {
                    _rewardCallback(GetRewardItem());
                }
            });
        }
    }
}
