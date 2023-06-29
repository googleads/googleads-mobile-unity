// Copyright (C) 2021 Google LLC.
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
    public class AppOpenAdClient : BaseAdClient, IAppOpenAdClient
    {
        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;

        public event EventHandler<AdValueEventArgs> OnPaidEvent;

        public event EventHandler<AdErrorClientEventArgs> OnAdFailedToPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidRecordImpression;

        public event Action OnAdClicked;

        private Dictionary<AdSize, string> prefabAds = new Dictionary<AdSize, string>() {
            { new AdSize(768, 1024), "PlaceholderAds/AppOpen/768x1024" },
            { new AdSize(1024, 768), "PlaceholderAds/AppOpen/1024x768" }
        };

        private ButtonBehaviour buttonBehaviour;

        private void AddClickBehavior(GameObject dummy)
        {
            Image[] images = dummy.GetComponentsInChildren<Image>();
            Image adImage = images[1];
            Button button = adImage.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => {
                if (OnAdClicked != null)
                {
                    OnAdClicked();
                }
                buttonBehaviour.OpenURL();
            });

            Button[] innerButtons = adImage.GetComponentsInChildren<Button>();

            innerButtons[1].onClick.AddListener(() =>
            {
                DestroyAppOpenAd();
                if(OnAdDidDismissFullScreenContent != null)
                {
                    OnAdDidDismissFullScreenContent.Invoke(this, new EventArgs());
                }
                AdBehaviour.ResumeGame();
            });
        }

        private void CreateButtonBehavior()
        {
            buttonBehaviour = base.dummyAd.AddComponent<ButtonBehaviour>();
        }

        public void CreateAppOpenAd()
        {
            // Do nothing.
        }

        public void LoadAd(string adUnitID, AdRequest request)
        {
            LoadAndSetPrefabAd(prefabAds[new AdSize(768, 1024)]);
            if (prefabAd != null)
            {
                if(OnAdLoaded != null)
                {
                    OnAdLoaded.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                if(OnAdFailedToLoad != null)
                {
                    OnAdFailedToLoad.Invoke(this, new LoadAdErrorClientEventArgs()
                    {
                        LoadAdErrorClient = new LoadAdErrorClient()
                    });
                }
            }
        }

        public void LoadAd(string adUnitID, AdRequest request, ScreenOrientation orientation)
        {
            if (Screen.width > Screen.height) // Landscape
            {
                LoadAndSetPrefabAd(prefabAds[new AdSize(1024, 768)]);
            }
            else
            {
                LoadAndSetPrefabAd(prefabAds[new AdSize(768, 1024)]);
            }

            if (prefabAd != null)
            {
                if(OnAdLoaded != null)
                {
                    OnAdLoaded.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                if(OnAdFailedToLoad != null)
                {
                    OnAdFailedToLoad.Invoke(this, new LoadAdErrorClientEventArgs()
                    {
                        LoadAdErrorClient = new LoadAdErrorClient()
                    });
                }
            }
        }

        public void Show()
        {
          if (prefabAd != null)
          {
              dummyAd = AdBehaviour.ShowAd(prefabAd, new Vector3(0, 0, 1));
              AdBehaviour.PauseGame();

              CreateButtonBehavior();
              AddClickBehavior(dummyAd);

              if(OnAdDidPresentFullScreenContent != null)
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
              Debug.Log("No Ad Loaded");
          }
        }

        public void DestroyAppOpenAd()
        {
          AdBehaviour.DestroyAd(dummyAd);
          prefabAd = null;
        }
    }
}
