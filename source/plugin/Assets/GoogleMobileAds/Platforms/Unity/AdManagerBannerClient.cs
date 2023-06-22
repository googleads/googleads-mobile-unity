// Copyright (C) 2023 Google LLC.
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
using UnityEngine;
using UnityEngine.UI;

using GoogleMobileAds.Api;
using GoogleMobileAds.Api.AdManager;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Unity
{
    public class AdManagerBannerClient : BannerClient, IAdManagerBannerClient
    {
        public event Action<AppEvent> OnAppEvent;
        public List<AdSize> ValidAdSizes {
            get { return this._validAdSizes; }
            set
            {
                if (value != null && value.Count < 1)
                {
                    Debug.LogError(
                            "ValidAdSizes must contain at least one valid ad size.");
                    return;
                }
                this._validAdSizes = value;
            }
        }

        private List<AdSize> _validAdSizes = new List<AdSize>();
        private string adUnitId;
        private AdSize adSize;
        private AdPosition position;
        private int positionX;
        private int positionY;

        // Creates a banner view and adds it to the view hierarchy.
        public override void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position)
        {
            this.adUnitId = adUnitId;
            this.adSize = adSize;
            this.position = position;
            if (ValidAdSizes != null && ValidAdSizes.Count > 0)
            {
                foreach(AdSize requestedAdSize in ValidAdSizes)
                {
                    if (prefabAds.ContainsKey(requestedAdSize))
                    {
                        LoadAndSetPrefabAd(prefabAds[requestedAdSize]);
                        break;
                    }
                }
                if (prefabAd == null)
                {
                    base.CreateBannerView(adUnitId, adSize, position);
                    return;
                }
            }
            else
            {
                base.CreateBannerView(adUnitId, adSize, position);
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

        // Creates an Ad Manager banner and adds it to the view hierarchy with a custom position.
        public override void CreateBannerView(string adUnitId, AdSize adSize, int x, int y)
        {
            this.adUnitId = adUnitId;
            this.adSize = adSize;
            this.positionX = x;
            this.positionY = y;
            if (ValidAdSizes != null && ValidAdSizes.Count > 0)
            {
                foreach(AdSize requestedAdSize in ValidAdSizes)
                {
                    if (prefabAds.ContainsKey(requestedAdSize))
                    {
                        LoadAndSetPrefabAd(prefabAds[requestedAdSize]);
                        break;
                    }
                }
                if (prefabAd == null)
                {
                    base.CreateBannerView(adUnitId, adSize, x, y);
                    return;
                }
            }
            else
            {
                base.CreateBannerView(adUnitId, adSize, x, y);
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
        public override void LoadAd(AdRequest request)
        {
            if (ValidAdSizes != null && ValidAdSizes.Count > 0)
            {
                if (position != null) CreateBannerView(adUnitId, adSize, position);
                else CreateBannerView(adUnitId, adSize, positionX, positionY);
            }
            base.LoadAd(request);
        }
    }
}
