#if UNITY_IOS
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
using System.Collections.Generic;
using System.Runtime.InteropServices;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    public class BannerClient : IBannerClient, IDisposable
    {
        private IntPtr bannerViewPtr;

        private IntPtr bannerClientPtr;

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

        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;

        public event EventHandler<EventArgs> OnAdOpening;

        public event EventHandler<EventArgs> OnAdClosed;

        public event EventHandler<AdValueEventArgs> OnPaidEvent;

        public event Action OnAdClicked;

        public event Action OnAdImpressionRecorded;

        // This property should be used when setting the bannerViewPtr.
        private IntPtr BannerViewPtr
        {
            get
            {
                return this.bannerViewPtr;
            }

            set
            {
                Externs.GADURelease(this.bannerViewPtr);
                this.bannerViewPtr = value;
            }
        }

#region IBannerClient implementation

        // Creates a banner view.
        public void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position)
        {
            this.bannerClientPtr = (IntPtr)GCHandle.Alloc(this);

            switch (adSize.AdType)
            {
                case AdSize.Type.SmartBanner:
                    this.BannerViewPtr = Externs.GADUCreateSmartBannerView(
                            this.bannerClientPtr, adUnitId, (int)position);
                    break;
                case AdSize.Type.AnchoredAdaptive:
                    this.BannerViewPtr = Externs.GADUCreateAnchoredAdaptiveBannerView(
                            this.bannerClientPtr,
                            adUnitId,
                            adSize.Width,
                            (int)adSize.Orientation,
                            (int)position);
                    break;
                case AdSize.Type.Standard:
                    this.BannerViewPtr = Externs.GADUCreateBannerView(
                            this.bannerClientPtr, adUnitId, adSize.Width, adSize.Height, (int)position);
                    break;
                default:
                    throw new ArgumentException("Invalid AdSize.Type provided.");
            }

            Externs.GADUSetBannerCallbacks(
                this.BannerViewPtr,
                AdViewDidReceiveAdCallback,
                AdViewDidFailToReceiveAdWithErrorCallback,
                AdViewWillPresentScreenCallback,
                AdViewDidDismissScreenCallback,
                AdViewPaidEventCallback,
                AdViewImpressionRecordedCallback,
                AdViewClickRecordedCallback
                );
        }

        public void CreateBannerView(string adUnitId, AdSize adSize, int x, int y)
        {

            this.bannerClientPtr = (IntPtr)GCHandle.Alloc(this);

            switch (adSize.AdType)
            {
                case AdSize.Type.SmartBanner:
                    this.BannerViewPtr = Externs.GADUCreateSmartBannerViewWithCustomPosition(
                    this.bannerClientPtr,
                    adUnitId,
                    x,
                    y);
                    break;
                case AdSize.Type.AnchoredAdaptive:
                    this.BannerViewPtr = Externs.GADUCreateAnchoredAdaptiveBannerViewWithCustomPosition(
                        this.bannerClientPtr,
                        adUnitId,
                        adSize.Width,
                        (int)adSize.Orientation,
                        x,
                        y);
                    break;
                case AdSize.Type.Standard:
                    this.BannerViewPtr = Externs.GADUCreateBannerViewWithCustomPosition(
                        this.bannerClientPtr,
                        adUnitId,
                        adSize.Width,
                        adSize.Height,
                        x,
                        y);
                    break;
                default:
                    throw new ArgumentException("Invalid AdSize.Type provided.");
            }

            Externs.GADUSetBannerCallbacks(
                this.BannerViewPtr,
                AdViewDidReceiveAdCallback,
                AdViewDidFailToReceiveAdWithErrorCallback,
                AdViewWillPresentScreenCallback,
                AdViewDidDismissScreenCallback,
                AdViewPaidEventCallback,
                AdViewImpressionRecordedCallback,
                AdViewClickRecordedCallback
                );
        }

        // Loads an ad.
        public void LoadAd(AdRequest request)
        {
            IntPtr requestPtr = Utils.BuildAdRequest(request);
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
            ((GCHandle)this.bannerClientPtr).Free();
        }

        ~BannerClient()
        {
            this.Dispose();
        }

#endregion

#region Banner callback methods

        [MonoPInvokeCallback(typeof(GADUAdViewDidReceiveAdCallback))]
        private static void AdViewDidReceiveAdCallback(IntPtr bannerClient)
        {
            BannerClient client = IntPtrToBannerClient(bannerClient);
            if (client.OnAdLoaded != null)
            {
                client.OnAdLoaded(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUAdViewDidFailToReceiveAdWithErrorCallback))]
        private static void AdViewDidFailToReceiveAdWithErrorCallback(
                IntPtr bannerClient, IntPtr error)
        {
            BannerClient client = IntPtrToBannerClient(bannerClient);
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
            BannerClient client = IntPtrToBannerClient(bannerClient);
            if (client.OnAdOpening != null)
            {
                client.OnAdOpening(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUAdViewDidDismissScreenCallback))]
        private static void AdViewDidDismissScreenCallback(IntPtr bannerClient)
        {
            BannerClient client = IntPtrToBannerClient(bannerClient);
            if (client.OnAdClosed != null)
            {
                client.OnAdClosed(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUAdViewPaidEventCallback))]
        private static void AdViewPaidEventCallback(
            IntPtr bannerClient, int precision, long value, string currencyCode)
        {
            BannerClient client = IntPtrToBannerClient(bannerClient);
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


        [MonoPInvokeCallback(typeof(GADUAdViewImpressionCallback))]
        private static void AdViewImpressionRecordedCallback(IntPtr adClientRef)
        {
            BannerClient client = IntPtrToBannerClient(adClientRef);
            if (client.OnAdImpressionRecorded != null)
            {
                client.OnAdImpressionRecorded();
            }
        }

        [MonoPInvokeCallback(typeof(GADUAdViewClickCallback))]
        private static void AdViewClickRecordedCallback(IntPtr adClientRef)
        {
            BannerClient client = IntPtrToBannerClient(adClientRef);
            if (client.OnAdClicked != null)
            {
                client.OnAdClicked();
            }
        }

        private static BannerClient IntPtrToBannerClient(IntPtr bannerClient)
        {
            GCHandle handle = (GCHandle)bannerClient;
            return handle.Target as BannerClient;
        }

#endregion
    }
}
#endif
