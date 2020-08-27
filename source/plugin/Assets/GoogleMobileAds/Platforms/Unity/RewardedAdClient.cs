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
    public class RewardedAdClient : BaseAdDummyClient, IRewardedAdClient
    {
        // Ad event fired when the rewarded ad has been received.
        public event EventHandler<EventArgs> OnAdLoaded;
        // Ad event fired when the rewarded ad has failed to load.
        public event EventHandler<AdErrorEventArgs> OnAdFailedToLoad;
        // Ad event fired when the rewarded ad has failed to show.
        public event EventHandler<AdErrorEventArgs> OnAdFailedToShow;
        // Ad event fired when the rewarded ad is opened.
        public event EventHandler<EventArgs> OnAdOpening;
        // Ad event fired when the rewarded ad has rewarded the user.
        public event EventHandler<Reward> OnUserEarnedReward;
        // Ad event fired when the rewarded ad is closed.
        public event EventHandler<EventArgs> OnAdClosed;
        // Ad event fired when the rewarded ad is estimated to have earned money.
        public event EventHandler<AdValueEventArgs> OnPaidEvent;

        private static readonly Dictionary<AdSize, string> prefabAds = new Dictionary<AdSize, string>()
        {
            {new AdSize (768,1024), "DummyAds/Rewarded/768x1024 1" },
            {new AdSize (1024,768), "DummyAds/Rewarded/1024x768 1"}
        };

        private ButtonBehaviour buttonBehaviour;

        private void AddClickBehavior(GameObject dummyAd)
        {
            Image[] images = dummyAd.GetComponentsInChildren<Image>();
            Image adImage = images[1];
            Button button = adImage.GetComponentInChildren<Button>();
            button.onClick.AddListener(() =>
            {
                buttonBehaviour.OpenURL();
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
                if (OnAdClosed != null)
                {
                    OnAdClosed.Invoke(this, EventArgs.Empty);
                }
                AdBehaviour.ResumeGame();
                if (OnUserEarnedReward != null)
                {
                    OnUserEarnedReward.Invoke(this, GetRewardItem());
                }
            });
        }

        private void CreateButtonBehavior()
        {
            buttonBehaviour = new ButtonBehaviour();
            buttonBehaviour.OnAdOpening += OnAdOpening;
        }

        // Creates a rewarded ad.
        public void CreateRewardedAd(string adUnitId)
        {

        }

        // Load a rewarded ad.
        public void LoadAd(AdRequest request)
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
                    OnAdFailedToLoad.Invoke(this, new AdErrorEventArgs()
                    {
                        Message = "Prefab Ad is Null"
                    });
                }
            }
        }

        // Determines whether the rewarded ad has loaded.
        public bool IsLoaded()
        {
            if (prefabAd != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Returns the mediation adapter class name.
        public string MediationAdapterClassName()
        {
            return new ResponseInfoDummyClient().GetMediationAdapterClassName();
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

        // Shows the rewarded ad on the screen.
        public void Show()
        {
            if (IsLoaded() == true)
            {
                dummyAd = AdBehaviour.ShowAd(prefabAd, new Vector3(0, 0, 1));
                AdBehaviour.PauseGame();
                CreateButtonBehavior();
                AddClickBehavior(dummyAd);
                dummyAd.AddComponent<Countdown>();
            }
            else
            {
                if (OnAdFailedToShow != null)
                {
                    OnAdFailedToShow.Invoke(this, new AdErrorEventArgs()
                    {
                        Message = "No Ad Loaded"
                    });
                }
            }
        }

        // Sets the server side verification options.
        public void SetServerSideVerificationOptions(ServerSideVerificationOptions serverSideVerificationOptions)
        {

        }

        // Returns ad request Response info client.
        public IResponseInfoClient GetResponseInfoClient()
        {
            return new ResponseInfoDummyClient();
        }
    }
}
