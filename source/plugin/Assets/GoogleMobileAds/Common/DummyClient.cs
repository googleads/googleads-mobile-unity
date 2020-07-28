// Copyright (C) 2015 Google, Inc.
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
using System.Threading.Tasks;

using GoogleMobileAds.Unity;
using GoogleMobileAds.Api;

using UnityEngine;
using UnityEngine.UI;

namespace GoogleMobileAds.Common
{
    public class DummyClient : IBannerClient, IInterstitialClient, IRewardBasedVideoAdClient,
            IAdLoaderClient, IMobileAdsClient
    {
        public GameObject dummyAd;
        private bool inUnityEditor = (Application.platform == RuntimePlatform.OSXEditor) || (Application.platform == RuntimePlatform.WindowsEditor);
        private const float timeDelay = 1000;
        private static GameObject myObject = new GameObject();
        private DummyAdBehaviour AdBehaviour = myObject.AddComponent<DummyAdBehaviour>();
        private string form;
         private Dictionary<AdSize, string> prefabAds = new Dictionary<AdSize, string>()
        {
            {AdSize.Banner, "DummyAds/Banners/BANNER"},
            {AdSize.SmartBanner, "DummyAds/Banners/BANNER"},
            {AdSize.MediumRectangle, "DummyAds/Banners/MEDIUM_RECTANGLE" },
            {AdSize.IABBanner, "DummyAds/Banners/FULL_BANNER" },
            {AdSize.Leaderboard, "DummyAds/Banners/LEADERBOARD" },
            {new AdSize (320,100), "DummyAds/Banners/LARGE_BANNER" },
            {new AdSize (480,32), "DummyAds/Banners/BANNER_480x32"}
        };

        public DummyClient()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            form = null;
            dummyAd = null;
        }

        // Disable warnings for unused dummy ad events.
#pragma warning disable 67

        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

        public event EventHandler<EventArgs> OnAdOpening;

        public event EventHandler<EventArgs> OnAdStarted;

        public event EventHandler<EventArgs> OnAdClosed;

        public event EventHandler<Reward> OnAdRewarded;

        public event EventHandler<EventArgs> OnAdLeavingApplication;

        public event EventHandler<EventArgs> OnAdCompleted;

        public event EventHandler<AdValueEventArgs> OnPaidEvent;

        public event EventHandler<CustomNativeClientEventArgs> OnCustomNativeTemplateAdLoaded;

        public event EventHandler<CustomNativeClientEventArgs> OnCustomNativeTemplateAdClicked;
#pragma warning restore 67

        public string UserId
        {
            get
            {
                Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
                return "UserId";
            }

            set
            {
                Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            }
        }

        public void Initialize(string appId)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void Initialize(Action<IInitializationStatusClient> initCompleteAction)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            var initStatusClient = new InitializationStatusDummyClient();
            initCompleteAction(initStatusClient);
        }

        public void DisableMediationInitialization()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void SetApplicationMuted(bool muted)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void SetRequestConfiguration(RequestConfiguration requestConfiguration)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public RequestConfiguration GetRequestConfiguration()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            return null;

        }

        public void SetApplicationVolume(float volume)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void SetiOSAppPauseOnBackground(bool pause)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public float GetDeviceScale()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            return 0;
        }

        public int GetDeviceSafeWidth()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            return 0;
        }

        public void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            if (inUnityEditor) {
                dummyAd = Resources.Load(prefabAds[adSize]) as GameObject;
                AdBehaviour.AnchorAd(dummyAd,position);
                form = "BANNER";
            }
        }

        public void CreateBannerView(string adUnitId, AdSize adSize, int positionX, int positionY)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            if (inUnityEditor)
            { 
                dummyAd = Resources.Load(prefabAds[adSize]) as GameObject;
                Image myImage = dummyAd.GetComponentInChildren<Image>();
                RectTransform rect = myImage.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector3(positionX, positionY, 1);
                form = "BANNER";
            }
        }

        public void LoadAd(AdRequest request)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            if (inUnityEditor)
            {
                 if (form == "BANNER")
                {
                    Task.Delay(timeDelay).ContinueWith(_ => MobileAdsEventExecutor.ExecuteInUpdate(ShowBannerView));
                }
                else if (form == "INTERSTITIAL")
                {
                    Task.Delay(timeDelay).ContinueWith(_ => MobileAdsEventExecutor.ExecuteInUpdate(ShowInterstitial));
                }
                else if (form == "REWARDED")
                {
                    Task.Delay(timeDelay).ContinueWith(_ => MobileAdsEventExecutor.ExecuteInUpdate(ShowRewardBasedVideoAd));
                }
            }
        }

        public void ShowBannerView()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            if (inUnityEditor)
            {
                Image myImage = dummyAd.GetComponentInChildren<Image>();
                RectTransform rect = myImage.GetComponent<RectTransform>();
                AdBehaviour.ShowAd(dummyAd, rect.anchoredPosition);
            }
        }

        public void HideBannerView()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            if (inUnityEditor && (dummyAd != null))
            {
                AdBehaviour.DestroyAd(dummyAd);
            }
        }

        public void DestroyBannerView()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            if (inUnityEditor && (dummyAd != null))
            {
                adBehaviour.DestroyAd(dummyAd);
            }
        }

        public float GetHeightInPixels()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            return 0;
        }

        public float GetWidthInPixels()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            return 0;
        }

        public void SetPosition(AdPosition adPosition)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void SetPosition(int x, int y)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void CreateInterstitialAd(string adUnitId)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public bool IsLoaded()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            return true;
        }

        public void ShowInterstitial()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void DestroyInterstitial()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void CreateRewardBasedVideoAd()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void SetUserId(string userId)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void LoadAd(AdRequest request, string adUnitId)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void DestroyRewardBasedVideoAd()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void ShowRewardBasedVideoAd()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void CreateAdLoader(AdLoaderClientArgs args)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void Load(AdRequest request)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public void SetAdSize(AdSize adSize)
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
        }

        public string MediationAdapterClassName()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            return null;
        }

        public IResponseInfoClient GetResponseInfoClient()
        {
            Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
            return null;
        }

    }
}
