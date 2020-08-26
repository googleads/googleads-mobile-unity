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
            {new AdSize (320,480), "DummyAds/Rewarded/320x480" },
            {new AdSize (480,320), "DummyAds/Rewarded/480x320"},
            {new AdSize (768,1024), "DummyAds/Rewarded/768x1024" },
            {new AdSize (1024,768), "DummyAds/Rewarded/1024x768"}
        };

        private ButtonBehaviour buttonBehaviour;

        private void AddClickBehavior(GameObject dummyAd)
        {
            Image myImage = dummyAd.GetComponentInChildren<Image>();
            Button button = myImage.GetComponentInChildren<Button>();
            button.onClick.AddListener(() =>
            {
                buttonBehaviour.OpenURL();
            });
            Button[] innerButtons = dummyAd.GetComponentsInChildren<Button>();

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
                if (Screen.width >= 1080)
                {
                    LoadAndSetPrefabAd(prefabAds[new AdSize(1024, 768)]);
                }
                else
                {
                    LoadAndSetPrefabAd(prefabAds[new AdSize(480, 320)]);
                }
            }
            else
            {
                if (Screen.height < 1080)
                {
                    LoadAndSetPrefabAd(prefabAds[new AdSize(320, 480)]);
                }
                else
                {
                    LoadAndSetPrefabAd(prefabAds[new AdSize(768, 1024)]);
                }
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
