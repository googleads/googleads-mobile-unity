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
    public class BannerAdClient : BaseAdClient, IBannerAdClient
    {
        public event Action OnBannerAdLoaded = delegate { };
        public event Action<ILoadAdErrorClient> OnBannerAdLoadFailed = delegate { };

        private readonly Dictionary<AdSize, string> prefabAds =
            new Dictionary<AdSize, string>()
            {
                {AdSize.Banner, "DummyAds/Banners/BANNER"},
                {AdSize.SmartBanner, "DummyAds/Banners/SMART_BANNER" },
                {AdSize.MediumRectangle, "DummyAds/Banners/MEDIUM_RECTANGLE" },
                {AdSize.IABBanner, "DummyAds/Banners/FULL_BANNER" },
                {AdSize.Leaderboard, "DummyAds/Banners/LEADERBOARD" },
                {new AdSize (320,100), "DummyAds/Banners/LARGE_BANNER" }
            };

        public void LoadBannerAd(string adUnitId, AdSize adSize, AdPosition position)
        {
            if (adSize.AdType == AdSize.Type.AnchoredAdaptive)
            {
                LoadPrefab("DummyAds/Banners/ADAPTIVE");
            }
            else
            {
                LoadPrefab(prefabAds[adSize]);
            }

            if (adPrefab == null)
            {
                OnBannerAdLoadFailed(new AdError());
                return;
            }

            LoadAd(Vector3.zero);
            AddClickBehavior(adInstance);

            if (adSize == AdSize.SmartBanner
                    || adSize.AdType == AdSize.Type.AnchoredAdaptive)
            {
                SetAndStretchAd(adPrefab, position, adSize);
            }
            else
            {
                AnchorAd(adPrefab, position);
            }
            Hide();
        }

        public void LoadBannerAd(string adUnitId, AdSize adSize, int x, int y)
        {
            if (adSize.AdType == AdSize.Type.AnchoredAdaptive)
            {
                LoadPrefab("DummyAds/Banners/ADAPTIVE");
            }
            else
            {
                LoadPrefab(prefabAds[adSize]);
            }

            if (adPrefab == null)
            {
                OnBannerAdLoadFailed(new AdError());
                return;
            }

            LoadAd(Vector3.zero);
            AddClickBehavior(adInstance);

            RectTransform rect = GetRectTransform(adInstance);

            if (adSize == AdSize.SmartBanner
                || adSize.AdType == AdSize.Type.AnchoredAdaptive)
            {
                SetAndStretchAd(adInstance, 0, adSize);
                rect.anchoredPosition = new Vector3(0, y, 1);
            }
            else
            {
                rect.anchoredPosition = new Vector3(x, y, 1);
            }
            Hide();
        }

        public void LoadAd(AdRequest request)
        {
            try
            {
                Show();
                OnBannerAdLoaded();
            }
            catch
            {
                OnBannerAdLoadFailed(new AdError());
            }
        }

        public void Hide()
        {
            adPrefab.SetActive(false);
        }

        public void Show()
        {
            adPrefab.SetActive(true);
            RaiseAdImpressionRecorded();
        }

        public float GetHeightInPixels()
        {
            return adPrefab != null ? GetRectTransform(adPrefab).sizeDelta.x : 0;
        }

        public float GetWidthInPixels()
        {
            return adPrefab != null ? GetRectTransform(adPrefab).sizeDelta.y : 0;
        }

        public void SetPosition(AdPosition adPosition)
        {
            if (adInstance != null)
            {
                AnchorAd(adInstance, adPosition);
            }
            else
            {
                Debug.Log("No existing banner in game");
            }
        }

        public void SetPosition(int x, int y)
        {
            if (adInstance != null)
            {
                RectTransform rect = GetRectTransform(adInstance);
                rect.anchoredPosition = new Vector2(x, y);
            }
            else
            {
                Debug.Log("No existing banner in game");
            }
        }

        private void SetAndStretchAd(GameObject dummyAd, AdPosition pos, AdSize adSize)
        {
            if (dummyAd != null)
            {
                Image myImage = dummyAd.GetComponentInChildren<Image>();
                RectTransform rect = myImage.GetComponentInChildren<RectTransform>();

                rect.pivot = new Vector2(0.5f, 0.5f);

                if (pos == AdPosition.Bottom
                    || pos == AdPosition.BottomLeft
                    || pos == AdPosition.BottomRight)
                {
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom,
                                                       0,
                                                       rect.sizeDelta.y);
                    rect.anchoredPosition = new Vector2(0, (float)rect.sizeDelta.y/2);
                }
                else if (pos == AdPosition.Top
                    || pos == AdPosition.TopLeft
                    || pos == AdPosition.TopRight)
                {
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, rect.sizeDelta.y);
                    rect.anchoredPosition = new Vector2(0, -(float)rect.sizeDelta.y/2);
                }
                else if (pos == AdPosition.Center)
                {
                    LoadPrefab("DummyAds/Banners/CENTER");
                    if (adSize.AdType == AdSize.Type.AnchoredAdaptive)
                    {
                        LoadPrefab("DummyAds/Banners/CENTER");
                        Text adText = adPrefab.GetComponentInChildren<Image>()
                                              .GetComponentInChildren<Text>();
                        adText.text = "This is a Test Adaptive Banner";
                    }
                    else if (adSize == AdSize.SmartBanner)
                    {
                        LoadPrefab("DummyAds/Banners/CENTER");
                        Text adText = adPrefab.GetComponentInChildren<Image>()
                                              .GetComponentInChildren<Text>();
                        adText.text = "This is a Test Smart Banner";
                    }
                    else
                    {
                        rect.anchoredPosition = new Vector2(0, 0);
                    }
                }
                else
                {
                    rect.anchoredPosition = rect.position;
                }
            }
            else
            {
                Debug.Log("Invalid Dummy Ad");
            }
        }

        private void AnchorAd(GameObject dummyAd, AdPosition position)
        {
            if (dummyAd != null)
            {
                Image myImage = dummyAd.GetComponentInChildren<Image>();
                RectTransform rect = myImage.GetComponentInChildren<RectTransform>();

                float x = (float)rect.sizeDelta.x/2;
                float y = (float)rect.sizeDelta.y/2;

                switch (position)
                {
                    case (AdPosition.TopLeft):
                        rect.pivot = new Vector2(0.5f, 0.5f);
                        rect.anchorMin = new Vector2(0, 1);
                        rect.anchorMax = new Vector2(0, 1);
                        rect.anchoredPosition = new Vector2(x, -y);
                        break;
                    case (AdPosition.TopRight):
                        rect.pivot = new Vector2(0.5f, 0.5f);
                        rect.anchorMin = new Vector2(1, 1);
                        rect.anchorMax = new Vector2(1, 1);
                        rect.anchoredPosition = new Vector2(-x, -y);
                        break;
                    case (AdPosition.Top):
                        rect.pivot = new Vector2(0.5f, 0.5f);
                        rect.anchorMin = new Vector2(0.5f, 1);
                        rect.anchorMax = new Vector2(0.5f, 1);
                        rect.anchoredPosition = new Vector2(0, -y);
                        break;
                    case (AdPosition.Bottom):
                        rect.pivot = new Vector2(0.5f, 0.5f);
                        rect.anchorMin = new Vector2(0.5f, 0);
                        rect.anchorMax = new Vector2(0.5f, 0);
                        rect.anchoredPosition = new Vector2(0, y);
                        break;
                    case (AdPosition.BottomRight):
                        rect.pivot = new Vector2(0.5f, 0.5f);
                        rect.anchorMin = new Vector2(1, 0);
                        rect.anchorMax = new Vector2(1, 0);
                        rect.anchoredPosition = new Vector2(-x, y);
                        break;
                    case (AdPosition.BottomLeft):
                        rect.pivot = new Vector2(0.5f, 0.5f);
                        rect.anchorMin = new Vector2(0, 0);
                        rect.anchorMax = new Vector2(0, 0);
                        rect.anchoredPosition = new Vector2(x, y);
                        break;
                    default:
                        rect.pivot = new Vector2(0.5f, 0.5f);
                        rect.anchorMin = new Vector2(0.5f, 0.5f);
                        rect.anchorMax = new Vector2(0.5f, 0.5f);
                        rect.anchoredPosition = new Vector2(0, 0);
                        break;
                }
            }
            else
            {
                Debug.Log("Invalid Dummy Ad");
            }
        }

        private void AddClickBehavior(GameObject dummyAd)
        {
            Image myImage = dummyAd.GetComponentInChildren<Image>();
            Button button = myImage.GetComponentInChildren<Button>();
            button.onClick.AddListener(() =>
            {
                NavigateToAdUrl();
                base.RaiseAdClickRecorded();
                base.RaiseAdFullScreenContentOpened();
            });
        }

        private RectTransform GetRectTransform(GameObject prefabAd)
        {
            Image myImage = prefabAd.GetComponentInChildren<Image>();
            return myImage.GetComponent<RectTransform>();
        }

    }
}
