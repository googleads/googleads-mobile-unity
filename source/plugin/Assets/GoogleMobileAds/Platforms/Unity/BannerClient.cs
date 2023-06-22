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
    public class BannerClient : BaseAdClient, IBannerClient
    {
        // Ad event fired when the banner ad has been received.
        public event EventHandler<EventArgs> OnAdLoaded;
        // Ad event fired when the banner ad has failed to load.
        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;
        // Ad event fired when the banner ad is opened.
        public event EventHandler<EventArgs> OnAdOpening;
        // Ad event fired when the banner ad is closed.
        public event EventHandler<EventArgs> OnAdClosed;
        // Ad event fired when the banner ad is estimated to have earned money.
        public event EventHandler<AdValueEventArgs> OnPaidEvent;

        public event Action OnAdClicked;

        public event Action OnAdImpressionRecorded;

        protected internal Dictionary<AdSize, string> prefabAds = new Dictionary<AdSize, string>()
        {
            {AdSize.Banner, "PlaceholderAds/Banners/BANNER"},
            {AdSize.SmartBanner, "PlaceholderAds/Banners/SMART_BANNER" },
            {AdSize.MediumRectangle, "PlaceholderAds/Banners/MEDIUM_RECTANGLE" },
            {AdSize.IABBanner, "PlaceholderAds/Banners/FULL_BANNER" },
            {AdSize.Leaderboard, "PlaceholderAds/Banners/LEADERBOARD" },
            {new AdSize (320,100), "PlaceholderAds/Banners/LARGE_BANNER" }
        };

        private ButtonBehaviour buttonBehaviour;

        private void AddClickBehavior(GameObject dummyAd)
        {
            Image myImage = dummyAd.GetComponentInChildren<Image>();
            Button button = myImage.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => {
                if (OnAdClicked != null)
                {
                    OnAdClicked();
                }
                buttonBehaviour.OpenURL();
            });
        }

        private void CreateButtonBehavior()
        {
            buttonBehaviour = base.dummyAd.AddComponent<ButtonBehaviour>();
            buttonBehaviour.OnAdOpening += (s, e) =>
            {
                if (OnAdOpening != null)
                {
                    OnAdOpening(this, EventArgs.Empty);
                }
                if (OnAdImpressionRecorded != null)
                {
                    OnAdImpressionRecorded();
                }
            };
        }

        // Creates a banner view and adds it to the view hierarchy.
        public virtual void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position)
        {
            if (adSize.AdType == AdSize.Type.AnchoredAdaptive)
            {
                LoadAndSetPrefabAd("PlaceholderAds/Banners/ADAPTIVE");
            }
            else if (prefabAds.ContainsKey(adSize))
            {
                LoadAndSetPrefabAd(prefabAds[adSize]);
            }
            if (prefabAd == null)
            {
                return;
            }
            if (adSize == AdSize.SmartBanner || adSize.AdType == AdSize.Type.AnchoredAdaptive)
            {
                SetAndStretchAd(prefabAd, position, adSize);
            }
            else
            {
                AnchorAd(prefabAd, position);
            }
        }

        // Creates a banner view and adds it to the view hierarchy with a custom position.
        public virtual void CreateBannerView(string adUnitId, AdSize adSize, int x, int y)
        {
            if (adSize.AdType == AdSize.Type.AnchoredAdaptive)
            {
                LoadAndSetPrefabAd("PlaceholderAds/Banners/ADAPTIVE");
            }
            else if (prefabAds.ContainsKey(adSize))
            {
                LoadAndSetPrefabAd(prefabAds[adSize]);
            }
            if (prefabAd == null)
            {
                return;
            }

            RectTransform rect = getRectTransform(prefabAd);
            if (adSize == AdSize.SmartBanner || adSize.AdType == AdSize.Type.AnchoredAdaptive)
            {
                SetAndStretchAd(prefabAd, 0, adSize);
                rect.anchoredPosition = new Vector3(0, y, 1);
            }
            else
            {
                // Account for banner size and refactor coordinates
                float xWithOffset = (float)rect.sizeDelta.x/2 + x;
                float yWithOffset = (float)rect.sizeDelta.y/2 + y;

                // Anchor the banner relative to the top left
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.anchoredPosition = new Vector2(xWithOffset, -yWithOffset);
            }
        }

        // Requests a new ad for the banner view.
        public virtual void LoadAd(AdRequest request)
        {

            if (prefabAd != null) {
                ShowBannerView();
                if (OnAdLoaded != null)
                {
                  OnAdLoaded.Invoke(this, EventArgs.Empty);
                }
            } else {
                if (OnAdFailedToLoad != null)
                {
                  OnAdFailedToLoad.Invoke(this, new LoadAdErrorClientEventArgs()
                  {
                      LoadAdErrorClient = new LoadAdErrorClient()
                  });
                }
            }
        }

        // Shows the banner view on the screen.
        public void ShowBannerView()
        {
            dummyAd = AdBehaviour.ShowAd(prefabAd, getRectTransform(prefabAd).anchoredPosition);
            CreateButtonBehavior();
            AddClickBehavior(dummyAd);
        }

        // Hides the banner view from the screen.
        public void HideBannerView()
        {
            AdBehaviour.DestroyAd(dummyAd);
        }

        // Destroys a banner view.
        public void DestroyBannerView()
        {
            AdBehaviour.DestroyAd(dummyAd);
            prefabAd = null;
        }

        // Returns the height of the BannerView in pixels.
        public float GetHeightInPixels()
        {
            if (prefabAd != null) {
                return getRectTransform(prefabAd).sizeDelta.y;
            }
            return 0;
        }

        // Returns the width of the BannerView in pixels.
        public float GetWidthInPixels()
        {
            if (prefabAd != null) {
                return getRectTransform(prefabAd).sizeDelta.x;
            }
            return 0;
        }

        // Set the position of the banner view using standard position.
        public void SetPosition(AdPosition adPosition)
        {
            if (dummyAd != null)
            {
                AnchorAd(dummyAd, adPosition);
            } else
            {
                Debug.Log("No existing banner in game");
            }
        }

        // Set the position of the banner view using custom position.
        public void SetPosition(int x, int y)
        {
            if (dummyAd == null)
            {
                Debug.Log("No existing banner in game");
                return;
            }

            // Account for banner size and refactor coordinates
            RectTransform rect = getRectTransform(dummyAd);
            float xWithOffset = (float)rect.sizeDelta.x/2 + x;
            float yWithOffset = (float)rect.sizeDelta.y/2 + y;

            // Anchor the banner relative to the top left
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(xWithOffset, -yWithOffset);
        }

        protected internal void SetAndStretchAd(GameObject dummyAd, AdPosition pos, AdSize adSize)
        {
            if (dummyAd != null) {
                Image myImage = dummyAd.GetComponentInChildren<Image>();
                RectTransform rect = myImage.GetComponentInChildren<RectTransform>();

                rect.pivot = new Vector2(0.5f, 0.5f);

                if (pos == AdPosition.Bottom || pos == AdPosition.BottomLeft || pos == AdPosition.BottomRight)
                {
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, rect.sizeDelta.y);
                    rect.anchoredPosition = new Vector2(0, (float)rect.sizeDelta.y/2);
                } else if (pos == AdPosition.Top || pos == AdPosition.TopLeft || pos == AdPosition.TopRight)
                {
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, rect.sizeDelta.y);
                    rect.anchoredPosition = new Vector2(0, -(float)rect.sizeDelta.y/2);
                } else if (pos == AdPosition.Center)
                {
                    LoadAndSetPrefabAd("PlaceholderAds/Banners/CENTER");
                    if (adSize.AdType == AdSize.Type.AnchoredAdaptive)
                    {
                        LoadAndSetPrefabAd("PlaceholderAds/Banners/CENTER");
                        Text adText = prefabAd.GetComponentInChildren<Image>().GetComponentInChildren<Text>();
                        adText.text = "This is a Test Adaptive Banner";
                    }
                    else if (adSize == AdSize.SmartBanner)
                    {
                        LoadAndSetPrefabAd("PlaceholderAds/Banners/CENTER");
                        Text adText = prefabAd.GetComponentInChildren<Image>().GetComponentInChildren<Text>();
                        adText.text = "This is a Test Smart Banner";
                    }
                    else
                    {
                        rect.anchoredPosition = new Vector2(0, 0);
                    }
                } else
                {
                    rect.anchoredPosition = rect.position;
                }
            } else {
                Debug.Log("Invalid Placeholder Ad");
            }
        }

        protected internal void AnchorAd(GameObject dummyAd, AdPosition position)
        {
            if (dummyAd != null) {
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
            } else {
                Debug.Log("Invalid Placeholder Ad");
            }
        }

    }
}
