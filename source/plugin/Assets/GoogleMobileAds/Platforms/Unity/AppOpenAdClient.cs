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
    public class AppOpenAdClient : BaseAdDummyClient, IAppOpenAdClient
    {
        public bool IsDestroyed { get; private set; }

        public event EventHandler<EventArgs> OnAdDidRecordImpression = delegate { };
        public event Action OnAdFullScreenContentOpened = delegate { };
        public event Action OnAdFullScreenContentClosed = delegate { };
        public event Action<IAdErrorClient> OnAdFullScreenContentFailed = delegate { };
        public event Action<AdValue> OnAdPaid = delegate { };
        public event Action OnAdClickRecorded = delegate { };
        public event Action OnAdImpressionRecorded = delegate { };

        private Dictionary<AdSize, string> prefabAds = new Dictionary<AdSize, string>() {
            { new AdSize(768, 1024), "DummyAds/AppOpen/768x1024" },
            { new AdSize(1024, 768), "DummyAds/AppOpen/1024x768" }
        };

        private ButtonBehaviour buttonBehaviour;

        private void AddClickBehavior(GameObject dummy)
        {
            Image[] images = dummy.GetComponentsInChildren<Image>();
            Image adImage = images[1];
            Button button = adImage.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => {
                OnAdClickRecorded();
                buttonBehaviour.OpenURL();
            });

            Button[] innerButtons = adImage.GetComponentsInChildren<Button>();

            innerButtons[1].onClick.AddListener(() =>
            {
                Destroy();
                OnAdFullScreenContentClosed();
                AdBehaviour.ResumeGame();
            });
        }

        private void CreateButtonBehavior()
        {
            buttonBehaviour = base.dummyAd.AddComponent<ButtonBehaviour>();
        }

        public void LoadAd(string adUnitId,
                           ScreenOrientation orientation,
                           AdRequest request,
                           Action<IAppOpenAdClient, ILoadAdErrorClient> callback)
        {
            if (Screen.width > Screen.height) // Landscape
            {
                LoadAndSetPrefabAd(prefabAds[new AdSize(1024, 768)]);
            }
            else
            {
                LoadAndSetPrefabAd(prefabAds[new AdSize(768, 1024)]);
            }


            if (callback == null)
            {
                return;
            }

            callback(
                prefabAd == null ? null : this,
                prefabAd == null ? new LoadAdErrorClient() : null);
        }

        public void ShowAd()
        {
          if (prefabAd != null)
          {
              dummyAd = AdBehaviour.ShowAd(prefabAd, new Vector3(0, 0, 1));
              AdBehaviour.PauseGame();

              CreateButtonBehavior();
              AddClickBehavior(dummyAd);
              OnAdFullScreenContentOpened();
          }
          else
          {
              Debug.Log("No Ad Loaded");
          }
        }

        public void Destroy()
        {
          AdBehaviour.DestroyAd(dummyAd);
          prefabAd = null;
          IsDestroyed = true;
        }
    }
}
