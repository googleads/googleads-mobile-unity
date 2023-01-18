// Copyright (C) 2020 Google LLC.
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

using System.Reflection;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Unity
{
    // A base client for rewarding ad types for Unity editor platform.
    public class RewardingAdBaseClient : BaseAdClient
    {
        // Ad event fired when the rewarding ad has been received.
        public event EventHandler<EventArgs> OnAdLoaded;
        // Ad event fired when the rewarding ad has failed to load.
        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;
        // Ad event fired when the rewarding ad is estimated to have earned money.
        public event EventHandler<AdValueEventArgs> OnPaidEvent;
        // Ad event fired when the rewarding ad has rewarded the user.
        public event EventHandler<Reward> OnUserEarnedReward;
        // Ad event fired when the full screen content has failed to be presented.
        public event EventHandler<AdErrorClientEventArgs> OnAdFailedToPresentFullScreenContent;
        // Ad event fired when the full screen content has been presented.
        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;
        // Ad event fired when the full screen content has been dismissed.
        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;
        // Ad event fired when an ad impression has been recorded.
        public event EventHandler<EventArgs> OnAdDidRecordImpression;
        // Ad event fired when an ad impression has been clicked.
        public event Action OnAdClicked;

        internal static readonly Dictionary<AdSize, string> prefabAds = new Dictionary<AdSize, string>()
        {
            {new AdSize (768,1024), "PlaceholderAds/Rewarded/768x1024" },
            {new AdSize (1024,768), "PlaceholderAds/Rewarded/1024x768"}
        };

        internal ButtonBehaviour buttonBehaviour;

        internal void AddClickBehavior(GameObject dummyAd)
        {
            Image[] images = dummyAd.GetComponentsInChildren<Image>();
            Image adImage = images[1];
            Button button = adImage.GetComponentInChildren<Button>();
            button.onClick.AddListener(() =>
            {
                buttonBehaviour.OpenURL();
                if (OnAdClicked != null)
                {
                    OnAdClicked();
                }
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
                if (OnAdDidDismissFullScreenContent != null)
                {
                    OnAdDidDismissFullScreenContent.Invoke(this, EventArgs.Empty);
                }
                AdBehaviour.ResumeGame();
                if (OnUserEarnedReward != null)
                {
                    OnUserEarnedReward.Invoke(this, GetRewardItem());
                }
            });
        }

        internal void CreateButtonBehavior()
        {
            buttonBehaviour = base.dummyAd.AddComponent<ButtonBehaviour>();
        }

        // Load a rewarding ad.
        public void LoadAd(string adUnitId, AdRequest request)
        {
            if (Screen.width > Screen.height) //Landscape
            {
                LoadAndSetPrefabAd(prefabAds[new AdSize(1024, 768)]);
            }
            else
            {
                LoadAndSetPrefabAd(prefabAds[new AdSize(768, 1024)]);
            }

            if (prefabAd != null)
            {
                if (OnAdLoaded != null)
                {
                    OnAdLoaded.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                if (OnAdFailedToLoad != null)
                {
                    OnAdFailedToLoad.Invoke(this, new LoadAdErrorClientEventArgs()
                    {
                        LoadAdErrorClient = new LoadAdErrorClient()
                    });
                }
            }
        }

        // Returns the reward item for the loaded rewarded ad.
        public Reward GetRewardItem()
        {
            return new Reward()
            {
                Type = "Reward",
                Amount = 10
            };
        }

        // Shows the rewarding ad on the screen.
        public void Show()
        {
            if (prefabAd != null)
            {
                dummyAd = AdBehaviour.ShowAd(prefabAd, new Vector3(0, 0, 1));
                AdBehaviour.PauseGame();
                CreateButtonBehavior();
                AddClickBehavior(dummyAd);
                dummyAd.AddComponent<Countdown>();
                if (OnAdDidPresentFullScreenContent != null)
                {
                    OnAdDidPresentFullScreenContent.Invoke(this, EventArgs.Empty);
                }
                if (OnAdDidRecordImpression != null)
                {
                    OnAdDidRecordImpression(this, EventArgs.Empty);
                }
            }
            else
            {
                if (OnAdFailedToPresentFullScreenContent != null)
                {
                    OnAdFailedToPresentFullScreenContent.Invoke(this, new AdErrorClientEventArgs()
                    {
                        AdErrorClient = new AdErrorClient()
                    });
                }
            }
        }

        // Sets the server side verification options.
        public void SetServerSideVerificationOptions(ServerSideVerificationOptions serverSideVerificationOptions)
        {

        }
    }
}
