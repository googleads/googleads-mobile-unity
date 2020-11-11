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

using System;
using System.Reflection;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleMobileAds.Unity
{
    public class InterstitialClient : BaseAdDummyClient, IInterstitialClient
    {
        // Ad event fired when the interstitial ad has been received.
        public event EventHandler<EventArgs> OnAdLoaded;
        // Ad event fired when the interstitial ad has failed to load.
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;
        // Ad event fired when the interstitial ad is opened.
        public event EventHandler<EventArgs> OnAdOpening;
        // Ad event fired when the interstitial ad is closed.
        public event EventHandler<EventArgs> OnAdClosed;
        // Ad event fired when the interstitial ad is leaving the application.
        public event EventHandler<EventArgs> OnAdLeavingApplication;
        // Ad event fired when the interstitial ad is estimated to have earned money.
        public event EventHandler<AdValueEventArgs> OnPaidEvent;

        private Dictionary<AdSize, string> prefabAds = new Dictionary<AdSize, string>() {
            {new AdSize (768,1024), "DummyAds/Interstitials/768x1024" },
            {new AdSize (1024,768), "DummyAds/Interstitials/1024x768"}
        };

        private ButtonBehaviour buttonBehaviour;

        private void AddClickBehavior(GameObject dummyAd)
        {
            Image[] images = dummyAd.GetComponentsInChildren<Image>();
            Image adImage = images[1];
            Button button = adImage.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => {
                buttonBehaviour.OpenURL();
            });

            Button[] innerButtons = adImage.GetComponentsInChildren<Button>();

            innerButtons[1].onClick.AddListener(() =>
            {
                DestroyInterstitial();
                if (OnAdClosed != null)
                {
                    OnAdClosed.Invoke(this, new EventArgs());
                }
                AdBehaviour.ResumeGame();
            });
        }

        private void CreateButtonBehavior()
        {
            buttonBehaviour = new ButtonBehaviour();
            buttonBehaviour.OnLeavingApplication += OnAdLeavingApplication;
        }

        // Creates an InterstitialAd.
        public void CreateInterstitialAd(string adUnitId)
        {

        }

        // Loads a new interstitial request.
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
                    OnAdFailedToLoad.Invoke(this, new AdFailedToLoadEventArgs()
                    {
                        Message = "Prefab Ad is Null"
                    });
                }
            }
        }

        // Determines whether the interstitial has loaded.
        public bool IsLoaded()
        {
            if (prefabAd != null) {
                return true;
            } else {
                return false;
            }
        }

        // Shows the InterstitialAd.
        public void ShowInterstitial()
        {
            if (IsLoaded() == true)
            {
                dummyAd = AdBehaviour.ShowAd(prefabAd, new Vector3(0, 0, 1));
                CreateButtonBehavior();
                AddClickBehavior(dummyAd);
                AdBehaviour.PauseGame();
                if (OnAdOpening != null)
                {
                  OnAdOpening.Invoke(this, EventArgs.Empty);
                }
            } else
            {
                Debug.Log("No Ad Loaded");
            }
        }

        // Destroys an InterstitialAd.
        public void DestroyInterstitial()
        {
            AdBehaviour.DestroyAd(dummyAd);
            prefabAd = null;
        }

    }
}
