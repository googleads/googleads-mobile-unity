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
    public class RewardedAdClient : BaseAdClient,
                                    IRewardedAdClient
    {
        private readonly Dictionary<AdSize, string> prefabAds =
            new Dictionary<AdSize, string>()
            {
                {new AdSize (768,1024), "DummyAds/Rewarded/768x1024" },
                {new AdSize (1024,768), "DummyAds/Rewarded/1024x768"}
            };

        public void LoadAd(string adUnitId, AdRequest request,
            Action<IRewardedAdClient, ILoadAdErrorClient> callback)
        {
            Action<RewardedAdClient, ILoadAdErrorClient> convertedCallback = (ad, error) =>
            {
                if (callback != null)
                {
                    callback(ad, error);
                }
            };
            LoadAd(convertedCallback);
        }

        public void LoadAd(Action<RewardedAdClient, ILoadAdErrorClient> callback)
        {
            //Landscape
            if (Screen.width > Screen.height)
            {
                LoadPrefab(prefabAds[new AdSize(1024, 768)]);
            }
            else
            {
                LoadPrefab(prefabAds[new AdSize(768, 1024)]);
            }

            if (callback == null)
            {
                return;
            }

            if (adPrefab == null)
            {
                callback(null, new LoadAdErrorClient());
            }
            else
            {
                callback(this, null);
            }
        }

        public void Show()
        {
            Show(null);
        }

        public void Show(Action<Reward> userRewardEarnedCallback)
        {
            if (adPrefab == null)
            {
                RaiseAdFullScreenContentFailed(new AdErrorClient());
                return;
            }

            adInstance = LoadAd(Vector3.zero);


            Image[] images = adInstance.GetComponentsInChildren<Image>();
            Image adImage = images[1];
            Button button = adImage.GetComponentInChildren<Button>();
            Button[] innerButtons = adImage.GetComponentsInChildren<Button>();

            if (innerButtons.Length < 2)
            {
                Debug.Log("Invalid Prefab");
                Destroy();
                return;
            }

            Button closeButton = innerButtons[1];
            closeButton.onClick.AddListener(() =>
            {
                Destroy();
                RaiseAdFullScreenContentClosed();
                ResumeGame();
                if (userRewardEarnedCallback != null)
                {
                    userRewardEarnedCallback(GetRewardItem());
                }

            });
            button.onClick.AddListener(() =>
            {
                NavigateToAdUrl();
                RaiseAdClickRecorded();
            });

            RaiseAdFullScreenContentOpened();
            RaiseAdImpressionRecorded();
            adInstance.AddComponent<Countdown>();
            PauseGame();
        }

        public void SetServerSideVerificationOptions(ServerSideVerificationOptions options)
        {
            Debug.Log("Dummy SetServerSideVerificationOptions");
        }
    }
}
