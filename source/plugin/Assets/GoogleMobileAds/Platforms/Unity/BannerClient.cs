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
    public class BannerClient : BaseAdDummyClient, IBannerClient
    {
        // Ad event fired when the banner ad has been received.
        public event EventHandler<EventArgs> OnAdLoaded;
        // Ad event fired when the banner ad has failed to load.
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;
        // Ad event fired when the banner ad is opened.
        public event EventHandler<EventArgs> OnAdOpening;
        // Ad event fired when the banner ad is closed.
        public event EventHandler<EventArgs> OnAdClosed;
        // Ad event fired when the banner ad is leaving the application.
        public event EventHandler<EventArgs> OnAdLeavingApplication;
        // Ad event fired when the banner ad is estimated to have earned money.
        public event EventHandler<AdValueEventArgs> OnPaidEvent;

        private Dictionary<AdSize, string> prefabAds = new Dictionary<AdSize, string>()
        {
            {AdSize.Banner, "DummyAds/Banners/BANNER"},
            {AdSize.SmartBanner, "DummyAds/Banners/SMART_BANNER" },
            {AdSize.MediumRectangle, "DummyAds/Banners/MEDIUM_RECTANGLE" },
            {AdSize.IABBanner, "DummyAds/Banners/FULL_BANNER" },
            {AdSize.Leaderboard, "DummyAds/Banners/LEADERBOARD" },
            {new AdSize (320,100), "DummyAds/Banners/LARGE_BANNER" }
        };

        // Creates a banner view and adds it to the view hierarchy.
        public void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            LoadAndSetPrefabAd(prefabAds[adSize]);
            if (prefabAd != null) {
                if (adSize == AdSize.SmartBanner)
                {
                    AdBehaviour.SetAndStretchAd(prefabAd, position);

                }
                else
                {
                    AdBehaviour.AnchorAd(prefabAd, position);
                }
            }
        }
    
        // Creates a banner view and adds it to the view hierarchy with a custom position.
        public void CreateBannerView(string adUnitId, AdSize adSize, int x, int y)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            
            LoadAndSetPrefabAd(prefabAds[adSize]);
            if (prefabAd != null) {
                RectTransform rect = getRectTransform(prefabAd);

                if (adSize == AdSize.SmartBanner)
                {
                    AdBehaviour.SetAndStretchAd(prefabAd, 0);
                    rect.anchoredPosition = new Vector3(0, y, 1);
                }
                else
                {
                    rect.anchoredPosition = new Vector3(x, y, 1);
                }
            }
        }

        // Requests a new ad for the banner view.
        public void LoadAd(AdRequest request)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            if (prefabAd != null) {
                ShowBannerView();
                OnAdLoaded?.Invoke(this, EventArgs.Empty);
            } else {
                OnAdFailedToLoad?.Invoke(this, new AdFailedToLoadEventArgs()
                {
                    Message = "Prefab Ad is Null"
                });
            }
            
            
        }

        // Shows the banner view on the screen.
        public void ShowBannerView()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            
            dummyAd = AdBehaviour.ShowAd(prefabAd, getRectTransform(prefabAd).anchoredPosition);
            ButtonBehaviour buttonBehaviour = AdBehaviour.GetButtonBehaviour(dummyAd);
            buttonBehaviour.OnAdOpening += OnAdOpening;
            buttonBehaviour.OnLeavingApplication += OnAdLeavingApplication; 
        }

        // Hides the banner view from the screen.
        public void HideBannerView()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);         
            AdBehaviour.DestroyAd(dummyAd);
            
        }

        // Destroys a banner view.
        public void DestroyBannerView()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            AdBehaviour.DestroyAd(dummyAd); 
        }

        // Returns the height of the BannerView in pixels.
        public float GetHeightInPixels()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            if (prefabAd != null) { 
                return getRectTransform(prefabAd).sizeDelta.y;
            }
        }

        // Returns the width of the BannerView in pixels.
        public float GetWidthInPixels()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            if (prefabAd != null) {
                return getRectTransform(prefabAd).sizeDelta.x;
            }
        }

        // Set the position of the banner view using standard position.
        public void SetPosition(AdPosition adPosition)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            if (dummyAd != null)
            {
                AdBehaviour.AnchorAd(dummyAd, adPosition);
            } else
            {
                Debug.Log("No existing banner in game");
            }
        }

        // Set the position of the banner view using custom position.
        public void SetPosition(int x, int y)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            if (dummyAd != null)
            {
                RectTransform rect = getRectTransfrom(dummyAd);
                rect.anchoredPosition = new Vector2(x, y);
            } else
            {
                Debug.Log("No existing banner in game");
            }
        }

        // Returns the mediation adapter class name.
        public string MediationAdapterClassName()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            return null;
        }

        // Returns ad request Response info client.
        public IResponseInfoClient GetResponseInfoClient()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            return null;
        }

    }
}