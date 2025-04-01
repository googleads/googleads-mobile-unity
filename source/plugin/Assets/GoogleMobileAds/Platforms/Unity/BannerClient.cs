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
using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

using UnityEngine;
using UnityEngine.UI;

namespace GoogleMobileAds.Unity
{
    public class BannerClient : IBannerClient
    {
        public event Action<AdValue> OnPaidEvent;
        public event Action OnAdClicked;
        public event Action OnAdImpressionRecorded;
        public event EventHandler<EventArgs> OnAdLoaded;
        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;
        public event EventHandler<EventArgs> OnAdOpening;
        public event EventHandler<EventArgs> OnAdClosed;

        private bool _isHidden = false;
        private bool _didImpress = false;
        private bool _loaded = false;
        private GameObject _gameObject;
        private GameObject _prefab;
        private string _adUnitId;

        protected internal Dictionary<AdSize, string> _prefabAds = new Dictionary<AdSize, string>()
        {
            {AdSize.Banner, "PlaceholderAds/Banners/BANNER"},
            {AdSize.SmartBanner, "PlaceholderAds/Banners/SMART_BANNER" },
            {AdSize.MediumRectangle, "PlaceholderAds/Banners/MEDIUM_RECTANGLE" },
            {AdSize.IABBanner, "PlaceholderAds/Banners/FULL_BANNER" },
            {AdSize.Leaderboard, "PlaceholderAds/Banners/LEADERBOARD" },
            {new AdSize (320,100), "PlaceholderAds/Banners/LARGE_BANNER" }
        };

        public void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position)
        {
            if(CreateBannerView(adSize, position))
            {
                _adUnitId = adUnitId;

                if (adSize == AdSize.SmartBanner || adSize.AdType == AdSize.Type.AnchoredAdaptive)
                {
                    SetAndStretchAd(position, adSize);
                }
                else
                {
                    AnchorAd(position);
                }

                _gameObject.SetActive(false);
            }
        }

        public void CreateBannerView(string adUnitId, AdSize adSize, int x, int y)
        {
            if(CreateBannerView(adSize, null))
            {
                _adUnitId = adUnitId;
                RectTransform rect = GetRectTransform();
                if (adSize == AdSize.SmartBanner || adSize.AdType == AdSize.Type.AnchoredAdaptive)
                {
                    SetAndStretchAd(0, adSize);
                    rect.anchoredPosition = new Vector3(0, y, 1);
                }
                else
                {
                    AnchorAd(x, y);
                }

                _gameObject.SetActive(false);
            }
        }

        bool CreateBannerView(AdSize adSize, AdPosition? position)
        {
            string prefabName;

            if (adSize == AdSize.SmartBanner || adSize.AdType == AdSize.Type.AnchoredAdaptive)
            {
                if (position.HasValue && position.Value == AdPosition.Center)
                {
                     prefabName = "PlaceholderAds/Banners/CENTER";
                }
                else
                {
                    prefabName = "PlaceholderAds/Banners/ADAPTIVE";
                }
            }
            else if(!_prefabAds.TryGetValue(adSize, out prefabName))
            {
                Debug.LogError("No Prefab found");
                return false;
            }
            _prefab = Resources.Load(prefabName) as GameObject;
            if (_prefab == null)
            {
                Debug.LogError("No Prefab found");
                return false;
            }
            _gameObject = GameObject.Instantiate(_prefab);

            // Setting the maximum sortingOrder ensures highest priority for rendering the ad.
            Canvas canvas = _prefab.GetComponent<Canvas>();
            if (canvas != null)
            {
                // sortingOrder is a 16 bit int so the maximum value is 32767.
                canvas.sortingOrder = 32767;
            }

            // Wire button.
            var button = _gameObject.GetComponentInChildren<Button>();
            button.onClick.AddListener(() =>
            {
                if (OnAdClicked != null)
                {
                    OnAdClicked();
                }
                if (OnAdOpening != null)
                {
                    OnAdOpening(this, EventArgs.Empty);
                }
                Application.OpenURL("https://google.com");
                if (OnPaidEvent != null)
                {
                    OnPaidEvent(new AdValue{
                        Value = 0,
                        CurrencyCode = "Google",
                        Precision = AdValue.PrecisionType.Unknown});
                }
                if (OnAdClosed != null)
                {
                    OnAdClosed(this, EventArgs.Empty);
                }
            });

            return true;
        }

        public void LoadAd(AdRequest request)
        {
            if (_gameObject == null)
            {
                return;
            }

            if (request == null || string.IsNullOrEmpty(_adUnitId))
            {
                if(OnAdFailedToLoad != null)
                {
                    OnAdFailedToLoad.Invoke(this, new LoadAdErrorClientEventArgs {
                            LoadAdErrorClient = new LoadAdErrorClient()
                        });
                }
            }
            else
            {
                _loaded = true;
                _didImpress = false;

                if(OnAdLoaded != null)
                {
                    OnAdLoaded.Invoke(this, EventArgs.Empty);
                }

                if (!_isHidden)
                {
                    _gameObject.SetActive(true);

                    if(OnAdImpressionRecorded != null)
                    {
                        OnAdImpressionRecorded();
                    }
                    _didImpress = true;
                }
            }
        }

        public void ShowBannerView()
        {
            if(_gameObject != null)
            {
                _gameObject.SetActive(_loaded);

                if (!_didImpress)
                {
                    if(OnAdImpressionRecorded != null)
                    {
                        OnAdImpressionRecorded();
                    }
                }
            }
            _isHidden = false;
        }

        public void HideBannerView()
        {
            if(_gameObject != null)
            {
                _gameObject.SetActive(false);
            }
            _isHidden = true;
        }

        public void DestroyBannerView()
        {
            if (_gameObject != null)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(_gameObject);
                }
                else
                {
                    GameObject.DestroyImmediate(_gameObject);
                }
                _gameObject = null;
            }
        }

        public string GetAdUnitID()
        {
            return _adUnitId;
        }

        public float GetHeightInPixels()
        {
            var rect = GetRectTransform();
            return rect.sizeDelta.y;
        }

        public float GetWidthInPixels()
        {
            var rect = GetRectTransform();
            return rect.sizeDelta.x;
        }

        public void SetPosition(AdPosition adPosition)
        {
            if (_gameObject != null)
            {
                AnchorAd(adPosition);
            }
        }

        public void SetPosition(int x, int y)
        {
            if (_gameObject != null)
            {
                AnchorAd(x, y);
            }
        }

        public bool IsCollapsible()
        {
            return false;
        }

        public IResponseInfoClient GetResponseInfoClient()
        {
            return new ResponseInfoClient();
        }

        private RectTransform GetRectTransform()
        {
            Image myImage = _gameObject.GetComponentInChildren<Image>();
            if (myImage != null)
            {
                return myImage.GetComponent<RectTransform>();
            }
            return null;
        }

        private void SetAndStretchAd(AdPosition pos, AdSize adSize)
        {
            if (_gameObject != null)
            {
                RectTransform rect = GetRectTransform();

                rect.pivot = new Vector2(0.5f, 0.5f);

                if (pos == AdPosition.Bottom || pos == AdPosition.BottomLeft || pos == AdPosition.BottomRight)
                {
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, rect.sizeDelta.y);
                    rect.anchoredPosition = new Vector2(0, (float)rect.sizeDelta.y/2);
                }
                else if (pos == AdPosition.Top || pos == AdPosition.TopLeft || pos == AdPosition.TopRight)
                {
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, rect.sizeDelta.y);
                    rect.anchoredPosition = new Vector2(0, -(float)rect.sizeDelta.y/2);
                }
                else if (pos == AdPosition.Center)
                {
                    if (adSize.AdType == AdSize.Type.AnchoredAdaptive)
                    {
                        Text adText = _gameObject.GetComponentInChildren<Text>();
                        adText.text = "This is a Test Adaptive Banner";
                    }
                    else if (adSize == AdSize.SmartBanner)
                    {
                        Text adText = _gameObject.GetComponentInChildren<Text>();
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
                Debug.Log("Invalid Placeholder Ad");
            }
        }

        private void AnchorAd(int x, int y)
        {
            var rect = GetRectTransform();
            // Account for banner size and refactor coordinates
            float xWithOffset = (float)rect.sizeDelta.x/2 + x;
            float yWithOffset = (float)rect.sizeDelta.y/2 + y;

            // Anchor the banner relative to the top left
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(xWithOffset, -yWithOffset);
        }

        private void AnchorAd(AdPosition position)
        {
            if (_gameObject != null) 
            {
                Image myImage = _gameObject.GetComponentInChildren<Image>();
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
                Debug.Log("Invalid Placeholder Ad");
            }
        }
    }
}