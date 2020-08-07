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

namespace GoogleMobileAds.Unity
{
    public class InterstitialClient : BaseDummyAdClient, IInterstitialClient
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

        private ButtonBehaviour buttonBehaviour;
        private Dictionary<AdSize, string> prefabAds = new Dictionary<AdSize, string>() {
            {new AdSize (320,480), "DummyAds/Interstitials/320x480" },
            {new AdSize (480,320), "DummyAds/Interstitials/480x320"},
            {new AdSize (768,1024), "DummyAds/Interstitials/768x1024" },
            {new AdSize (1024,768), "DummyAds/Interstitials/1024x768"}
        };

        // Creates an InterstitialAd.
        public void CreateInterstitialAd(string adUnitId)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        // Loads a new interstitial request.
        public void LoadAd(AdRequest request)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            if (Screen.width > Screen.height) //Landscape
            {
                if (Screen.width > 800)
                {
                    LoadAndSetPrefabAd(prefabAds[new AdSize(1024, 768)]);
                }
                else
                {
                    LoadAndSetPrefabAd(prefabAds[new AdSize(480, 320)]);
                }
            } else
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
            if (OnAdLoaded != null)
            {
                OnAdLoaded.Invoke(this, EventArgs.Empty);
            }
        }

        // Determines whether the interstitial has loaded.
        public bool IsLoaded()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            if (prefabAd != null) {
                return true;
            } else {
                return false;
            }
        }

        // Shows the InterstitialAd.
        public void ShowInterstitial()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            if (prefabAd != null)
            {
                dummyAd = AdBehaviour.ShowAd(prefabAd, new Vector3(0, 0, 0));
                buttonBehaviour = dummyAd.GetComponentInChildren<ButtonBehaviour>();
                buttonBehaviour.OnAdOpening += OnAdOpening;
                buttonBehaviour.OnLeavingApplication += OnAdLeavingApplication;
                DummyAdBehaviour.instance.OnAdClosed += OnAdClosed;
                AdBehaviour.PauseGame();
            } else
            {
                Debug.Log("No Ad Loaded");
            }
        }

        // Destroys an InterstitialAd.
        public void DestroyInterstitial()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            AdBehaviour.DestroyAd(dummyAd);
        }

        // Returns the mediation adapter class name.
        public string MediationAdapterClassName()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            return new ResponseInfoDummyClient().GetMediationAdapterClassName();
        }

        // Returns ad request Response info client.
        public IResponseInfoClient GetResponseInfoClient()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            return new ResponseInfoDummyClient();
        }

    }
}
