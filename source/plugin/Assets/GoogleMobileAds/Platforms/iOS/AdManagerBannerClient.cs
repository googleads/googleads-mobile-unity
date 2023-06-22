#if UNITY_IOS
// Copyright (C) 2023 Google, Inc.
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
using System.Runtime.InteropServices;
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Api.AdManager;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    public class AdManagerBannerClient : IAdManagerBannerClient, IDisposable
    {

#region Banner callback types

        internal delegate void GADUAdViewDidReceiveAdCallback(IntPtr bannerClient);

        internal delegate void GADUAdViewDidFailToReceiveAdWithErrorCallback(
                IntPtr bannerClient, IntPtr error);

        internal delegate void GADUAdViewWillPresentScreenCallback(IntPtr bannerClient);

        internal delegate void GADUAdViewDidDismissScreenCallback(IntPtr bannerClient);

        internal delegate void GADUAdViewPaidEventCallback(
                IntPtr bannerClient, int precision, long value, string currencyCode);

        internal delegate void GADUAdViewImpressionCallback(IntPtr bannerClient);

        internal delegate void GADUAdViewClickCallback(IntPtr bannerClient);

#endregion

#region Ad Manager banner callback types

        internal delegate void GAMUAdViewAppEventCallback(IntPtr bannerClient, string name,
                                                          string info);

#endregion

        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;

        public event EventHandler<EventArgs> OnAdOpening;

        public event EventHandler<EventArgs> OnAdClosed;

        public event EventHandler<AdValueEventArgs> OnPaidEvent;

        public event Action<AppEvent> OnAppEvent;

        public event Action OnAdClicked;

        public event Action OnAdImpressionRecorded;

        public List<AdSize> ValidAdSizes {
            get { return this._validAdSizes; }
            set
            {
                if (this.BannerViewPtr == IntPtr.Zero)
                {
                    Debug.LogError("Create a banner view before setting ValidAdSizes.");
                    return;
                }
                if (value != null)
                {
                    int[] validAdSizesArray = new int[value.Count * 2];
                    int validAdSizesArrayIndex = 0;
                    for(int adSizeIndex = 0; adSizeIndex < value.Count; adSizeIndex++)
                    {
                        validAdSizesArray[validAdSizesArrayIndex++] = value[adSizeIndex].Width;
                        validAdSizesArray[validAdSizesArrayIndex++] = value[adSizeIndex].Height;
                    }
                    Externs.GAMUBannerViewSetValidAdSizes(this.BannerViewPtr, validAdSizesArray,
                                                          value.Count);
                }
                else
                {
                    Externs.GAMUBannerViewSetValidAdSizes(this.BannerViewPtr, null, 0);
                }
                this._validAdSizes = value;
            }
        }

        private List<AdSize> _validAdSizes;

        private IntPtr _bannerViewPtr;

        private IntPtr _bannerClientPtr;

        // This property should be used when setting the _bannerViewPtr.
        private IntPtr BannerViewPtr
        {
            get
            {
                return this._bannerViewPtr;
            }

            set
            {
                Externs.GADURelease(this._bannerViewPtr);
                this._bannerViewPtr = value;
            }
        }

#region IAdManagerBannerClient implementation

        // Creates an Ad Manager banner view at a preset position.
        public void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position)
        {
            this._bannerClientPtr = (IntPtr)GCHandle.Alloc(this);

            switch (adSize.AdType)
            {
                case AdSize.Type.AnchoredAdaptive:
                    this.BannerViewPtr = Externs.GAMUCreateAnchoredAdaptiveBannerView(
                            this._bannerClientPtr,
                            adUnitId,
                            adSize.Width,
                            (int)adSize.Orientation,
                            (int)position);
                    break;
                case AdSize.Type.Standard:
                    this.BannerViewPtr = Externs.GAMUCreateBannerView(this._bannerClientPtr,
                            adUnitId, adSize.Width, adSize.Height, (int)position);
                    break;
                default:
                    throw new ArgumentException("Invalid AdSize.Type provided.");
            }

            Externs.GAMUSetBannerCallbacks(
                this.BannerViewPtr,
                AdViewDidReceiveAdCallback,
                AdViewDidFailToReceiveAdWithErrorCallback,
                AdViewWillPresentScreenCallback,
                AdViewDidDismissScreenCallback,
                AdViewPaidEventCallback,
                AdViewImpressionRecordedCallback,
                AdViewClickRecordedCallback,
                AdViewAppEventCallback);
        }

        // Creates an Ad Manager banner view at provided coordinates.
        public void CreateBannerView(string adUnitId, AdSize adSize, int x, int y)
        {

            this._bannerClientPtr = (IntPtr)GCHandle.Alloc(this);

            switch (adSize.AdType)
            {
                case AdSize.Type.AnchoredAdaptive:
                    this.BannerViewPtr =
                            Externs.GAMUCreateAnchoredAdaptiveBannerViewWithCustomPosition(
                            this._bannerClientPtr,
                            adUnitId,
                            adSize.Width,
                            (int)adSize.Orientation,
                            x,
                            y);
                    break;
                case AdSize.Type.Standard:
                    this.BannerViewPtr = Externs.GAMUCreateBannerViewWithCustomPosition(
                        this._bannerClientPtr,
                        adUnitId,
                        adSize.Width,
                        adSize.Height,
                        x,
                        y);
                    break;
                default:
                    throw new ArgumentException("Invalid AdSize.Type provided.");
            }

            Externs.GAMUSetBannerCallbacks(
                this.BannerViewPtr,
                AdViewDidReceiveAdCallback,
                AdViewDidFailToReceiveAdWithErrorCallback,
                AdViewWillPresentScreenCallback,
                AdViewDidDismissScreenCallback,
                AdViewPaidEventCallback,
                AdViewImpressionRecordedCallback,
                AdViewClickRecordedCallback,
                AdViewAppEventCallback);
        }

        // Loads an ad into the Ad Manager AdView.
        public void LoadAd(AdRequest request)
        {
            IntPtr requestPtr = Utils.BuildAdManagerAdRequest(request);
            Externs.GADURequestBannerAd(this.BannerViewPtr, requestPtr);
            Externs.GADURelease(requestPtr);
        }

        // Displays the banner view on the screen.
        public void ShowBannerView()
        {
            Externs.GADUShowBannerView(this.BannerViewPtr);
        }

        // Hides the banner view from the screen.
        public void HideBannerView()
        {
            Externs.GADUHideBannerView(this.BannerViewPtr);
        }

        // Destroys the banner view.
        public void DestroyBannerView()
        {
            Externs.GADURemoveBannerView(this.BannerViewPtr);
            this.BannerViewPtr = IntPtr.Zero;
        }

        // Returns the height of the BannerView in pixels.
        public float GetHeightInPixels()
        {
            return Externs.GADUGetBannerViewHeightInPixels(this.BannerViewPtr);
        }

        // Returns the width of the BannerView in pixels.
        public float GetWidthInPixels()
        {
            return Externs.GADUGetBannerViewWidthInPixels(this.BannerViewPtr);
        }

        // Set the position of the banner view using standard position.
        public void SetPosition(AdPosition adPosition)
        {
            Externs.GADUSetBannerViewAdPosition(this.BannerViewPtr, (int)adPosition);
        }

        // Set the position of the banner view using custom position.
        public void SetPosition(int x, int y)
        {
            Externs.GADUSetBannerViewCustomPosition(this.BannerViewPtr, x, y);
        }

        public IResponseInfoClient GetResponseInfoClient()
        {
            return new ResponseInfoClient(ResponseInfoClientType.AdLoaded, this.BannerViewPtr);
        }

        public void Dispose()
        {
            this.DestroyBannerView();
            ((GCHandle)this._bannerClientPtr).Free();
        }

        ~AdManagerBannerClient()
        {
            this.Dispose();
        }

#endregion

#region Banner callback methods

        [MonoPInvokeCallback(typeof(GADUAdViewDidReceiveAdCallback))]
        private static void AdViewDidReceiveAdCallback(IntPtr bannerClient)
        {
            AdManagerBannerClient client = IntPtrToBannerClient(bannerClient);
            if (client.OnAdLoaded != null)
            {
                client.OnAdLoaded(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUAdViewDidFailToReceiveAdWithErrorCallback))]
        private static void AdViewDidFailToReceiveAdWithErrorCallback(
                IntPtr bannerClient, IntPtr error)
        {
            AdManagerBannerClient client = IntPtrToBannerClient(bannerClient);
            if (client.OnAdFailedToLoad != null)
            {
                LoadAdErrorClientEventArgs args = new LoadAdErrorClientEventArgs()
                {
                    LoadAdErrorClient = new LoadAdErrorClient(error)
                };
                client.OnAdFailedToLoad(client, args);
            }
        }

        [MonoPInvokeCallback(typeof(GADUAdViewWillPresentScreenCallback))]
        private static void AdViewWillPresentScreenCallback(IntPtr bannerClient)
        {
            AdManagerBannerClient client = IntPtrToBannerClient(bannerClient);
            if (client.OnAdOpening != null)
            {
                client.OnAdOpening(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUAdViewDidDismissScreenCallback))]
        private static void AdViewDidDismissScreenCallback(IntPtr bannerClient)
        {
            AdManagerBannerClient client = IntPtrToBannerClient(bannerClient);
            if (client.OnAdClosed != null)
            {
                client.OnAdClosed(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUAdViewPaidEventCallback))]
        private static void AdViewPaidEventCallback(
            IntPtr bannerClient, int precision, long value, string currencyCode)
        {
            AdManagerBannerClient client = IntPtrToBannerClient(bannerClient);
            if (client.OnPaidEvent != null)
            {
                AdValue adValue = new AdValue()
                {
                    Precision = (AdValue.PrecisionType)precision,
                    Value = value,
                    CurrencyCode = currencyCode
                };
                AdValueEventArgs args = new AdValueEventArgs()
                {
                    AdValue = adValue
                };

                client.OnPaidEvent(client, args);
            }
        }

        [MonoPInvokeCallback(typeof(GAMUAdViewAppEventCallback))]
        private static void AdViewAppEventCallback(IntPtr bannerClient, string name, string info)
        {
            AdManagerBannerClient client = IntPtrToBannerClient(bannerClient);
            if (client.OnAppEvent != null)
            {
                client.OnAppEvent(new AppEvent()
                {
                    Name = name,
                    Data = info
                });
            }
        }

        [MonoPInvokeCallback(typeof(GADUAdViewImpressionCallback))]
        private static void AdViewImpressionRecordedCallback(IntPtr adClientRef)
        {
            AdManagerBannerClient client = IntPtrToBannerClient(adClientRef);
            if (client.OnAdImpressionRecorded != null)
            {
                client.OnAdImpressionRecorded();
            }
        }

        [MonoPInvokeCallback(typeof(GADUAdViewClickCallback))]
        private static void AdViewClickRecordedCallback(IntPtr adClientRef)
        {
            AdManagerBannerClient client = IntPtrToBannerClient(adClientRef);
            if (client.OnAdClicked != null)
            {
                client.OnAdClicked();
            }
        }

        private static AdManagerBannerClient IntPtrToBannerClient(IntPtr bannerClient)
        {
            GCHandle handle = (GCHandle)bannerClient;
            return handle.Target as AdManagerBannerClient;
        }

#endregion
    }
}
#endif
