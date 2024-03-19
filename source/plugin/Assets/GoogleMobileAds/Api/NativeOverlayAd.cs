// Copyright (C) 2024 Google, Inc.
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
using UnityEngine;

using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// Native Overlay ads use the Native ad format to show overlays on top of the application.
    /// They stay on screen while users are interacting with the app.
    /// </summary>
    public class NativeOverlayAd
    {
        /// <summary>
        /// Raised when the ad is estimated to have earned money.
        /// </summary>
        public event Action<AdValue> OnAdPaid;

        /// <summary>
        /// Raised when a click is recorded for an ad.
        /// </summary>
        public event Action OnAdClicked;

        /// <summary>
        /// Raised when an impression is recorded for an ad.
        /// </summary>
        public event Action OnAdImpressionRecorded;

        /// <summary>
        /// Raised when an ad opened full-screen content.
        /// </summary>
        public event Action OnAdFullScreenContentOpened;

        /// <summary>
        /// Raised when the ad closed full-screen content.
        /// On iOS, this event is only raised when an ad opens an overlay, not when opening a new
        /// application such as Safari or the App Store.
        /// </summary>
        public event Action OnAdFullScreenContentClosed;

        private INativeOverlayAdClient _client;

        private NativeOverlayAd(INativeOverlayAdClient client)
        {
            _client = client;
            RegisterAdEvents();
        }

        /// <summary>
        /// Loads a native overlay ad.
        /// </summary>
        /// <param name="adUnitId">
        /// An ad unit ID created in the AdMob or Ad Manager UI.
        /// </param>
        /// <param name="request">
        /// An ad request object containing targeting information used to fetch the ad.
        /// </param>
        /// <param name="options">
        /// Native Ad Options for configuring image and video rendering settings.
        /// </param>
        /// <param name="adLoadCallback">
        /// Callback to inform the app if the load request was successful or failed to fetch the ad.
        /// </param>
        public static void Load(string adUnitId, AdRequest request, NativeAdOptions options,
                                Action<NativeOverlayAd, LoadAdError> adLoadCallback)
        {
            if (adLoadCallback == null)
            {
                UnityEngine.Debug.LogError("adLoadCallback is null. No ad was loaded.");
                return;
            }

            var client = MobileAds.GetClientFactory().BuildNativeOverlayAdClient();
            client.OnAdLoaded += (sender, args) =>
            {
                MobileAds.RaiseAction(() => { adLoadCallback(new NativeOverlayAd(client), null); });
            };
            client.OnAdFailedToLoad += (sender, error) =>
            {
                var loadAdError = new LoadAdError(error.LoadAdErrorClient);
                MobileAds.RaiseAction(() => { adLoadCallback(null, loadAdError); });
            };
            client.Load(adUnitId, request, options);
        }

        /// <summary>
        /// Renders the native overlay ad at provided AdPosition with the given adSize.
        /// </summary>
        /// <param name="nativeTemplateStyle">
        /// The Template styling used to customize the native ad.
        /// </param>
        /// <param name="adSize">
        /// The size of the overlay used to render the native ad.
        /// </param>
        /// <param name="adPosition">
        /// The position of the overlay in screen space.
        /// </param>
        public void RenderTemplate(NativeTemplateStyle nativeTemplateStyle, AdSize adSize,
                                  AdPosition adPosition)
        {
            if (_client != null)
            {
                _client.Render(nativeTemplateStyle, adSize, adPosition);
            }
        }

        /// <summary>
        /// Renders the native overlay ad at x,y coordinates with the provided size.
        /// </summary>
        /// <param name="nativeTemplateStyle">
        /// The Template styling used to customize the native ad.
        /// </param>
        /// <param name="adSize">
        /// The size of the overlay used to render the native ad.
        /// </param>
        /// <param name="x">
        /// The x coordinate position of the overlay in screen space.
        /// </param>
        /// <param name="y">
        /// The y coordinate position of the overlay in screen space.
        /// </param>
        public void RenderTemplate(NativeTemplateStyle nativeTemplateStyle, AdSize adSize, int x,
                                  int y)
        {
            if (_client != null)
            {
                _client.Render(nativeTemplateStyle, adSize, x, y);
            }
        }

        /// <summary>
        /// Renders the native overlay ad at provided AdPosition using the default size.
        /// </summary>
        /// <param name="nativeTemplateStyle">
        /// The Template styling used to customize the native ad.
        /// </param>
        /// <param name="adPosition">
        /// The position of the overlay in screen space.
        /// </param>
        public void RenderTemplate(NativeTemplateStyle nativeTemplateStyle, AdPosition adPosition)
        {
            if (_client != null)
            {
                _client.Render(nativeTemplateStyle, adPosition);
            }
        }

        /// <summary>
        /// Renders the native Overlay ad at x,y coordinates using the default size.
        /// </summary>
        /// <param name="nativeTemplateStyle">
        /// The Template styling used to customize the native ad.
        /// </param>
        /// <param name="x">
        /// The x coordinate position of the overlay in screen space.
        /// </param>
        /// <param name="y">
        /// The y coordinate position of the overlay in screen space.
        /// </param>
        public void RenderTemplate(NativeTemplateStyle nativeTemplateStyle, int x, int y)
        {
            if (_client != null)
            {
                _client.Render(nativeTemplateStyle, x, y);
            }
        }

        /// <summary>
        /// Destroys the native overlay ad.
        /// </summary>
        public void Destroy()
        {
            if (_client != null)
            {
                _client.DestroyAd();
            }
        }

        /// <summary>
        /// Hides the ad from being shown.
        /// </summary>
        public void Hide()
        {
            if (_client != null)
            {
                _client.Hide();
            }
        }

        /// <summary>
        /// Shows the previously hidden ad.
        /// </summary>
        public void Show()
        {
            if (_client != null)
            {
                _client.Show();
            }
        }

        /// <summary>
        /// Sets the position of the native overlay ad using standard position.
        /// </summary>
        /// <param name="position">
        /// The position of the overlay in screen space.
        /// </param>
        public void SetTemplatePosition(AdPosition position)
        {
            if (_client != null)
            {
                _client.SetPosition(position);
            }
        }

        /// <summary>
        /// Sets the position of the native overlay ad using custom position.
        /// </summary>
        /// <param name="x">
        /// The x coordinate position of the overlay in screen space.
        /// </param>
        /// <param name="y">
        /// The y coordinate position of the overlay in screen space.
        /// </param>
        public void SetTemplatePosition(int x, int y)
        {
            if (_client != null)
            {
                _client.SetPosition(x, y);
            }
        }

        /// <summary>
        /// Fetches the ad request response info.
        /// </summary>
        /// <returns>
        /// The ad request response info or null if the ad is not loaded.
        /// </returns>
        public ResponseInfo GetResponseInfo()
        {
            return _client != null ? new ResponseInfo(_client.GetResponseInfoClient()) : null;
        }

        /// <summary>
        /// Returns the height of the native overlay in pixels.
        /// </summary>
        public float GetTemplateHeightInPixels()
        {
            return _client != null ? _client.GetHeightInPixels() : 0;
        }

        /// <summary>
        /// Returns the width of the native overlay in pixels.
        /// </summary>
        public float GetTemplateWidthInPixels()
        {
            return _client != null ? _client.GetWidthInPixels() : 0;
        }

        private void RegisterAdEvents()
        {
            _client.OnAdClicked += () =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdClicked != null)
                    {
                        OnAdClicked();
                    }
                });
            };

            _client.OnAdDidRecordImpression += (sender, args) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdImpressionRecorded != null)
                    {
                        OnAdImpressionRecorded();
                    }
                });
            };

            _client.OnAdDidDismissFullScreenContent += (sender, args) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdFullScreenContentClosed != null)
                    {
                        OnAdFullScreenContentClosed();
                    }
                });
            };

            _client.OnAdDidPresentFullScreenContent += (sender, args) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdFullScreenContentOpened != null)
                    {
                        OnAdFullScreenContentOpened();
                    }
                });
            };

            _client.OnPaidEvent += (adValue) =>
            {
                MobileAds.RaiseAction(() =>
                {
                    if (OnAdPaid != null)
                    {
                        OnAdPaid(adValue);
                    }
                });
            };
        }
    }
}