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

#if UNITY_IOS

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    internal class BannerClient : IBannerClient, IDisposable
    {
        private IntPtr bannerViewPtr;

        private IntPtr bannerClientPtr;

        #region Banner callback types

        internal delegate void GADUAdViewDidReceiveAdCallback(IntPtr bannerClient);

        internal delegate void GADUAdViewDidFailToReceiveAdWithErrorCallback(
                IntPtr bannerClient, string error);

        internal delegate void GADUAdViewWillPresentScreenCallback(IntPtr bannerClient);

        internal delegate void GADUAdViewDidDismissScreenCallback(IntPtr bannerClient);

        internal delegate void GADUAdViewWillLeaveApplicationCallback(IntPtr bannerClient);

        #endregion

        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

        public event EventHandler<EventArgs> OnAdOpening;

        public event EventHandler<EventArgs> OnAdClosed;

        public event EventHandler<EventArgs> OnAdLeavingApplication;

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

            if (adSize.IsSmartBanner)
            {
                this.BannerViewPtr = Externs.GADUCreateSmartBannerView(
                        this.bannerClientPtr, adUnitId, (int)position);
            }
            else
            {
                this.BannerViewPtr = Externs.GADUCreateBannerView(
                        this.bannerClientPtr, adUnitId, adSize.Width, adSize.Height, (int)position);
            }

            Externs.GADUSetBannerCallbacks(
                    this.BannerViewPtr,
                    AdViewDidReceiveAdCallback,
                    AdViewDidFailToReceiveAdWithErrorCallback,
                    AdViewWillPresentScreenCallback,
                    AdViewDidDismissScreenCallback,
                    AdViewWillLeaveApplicationCallback);
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
                IntPtr bannerClient, string error)
        {
            BannerClient client = IntPtrToBannerClient(bannerClient);
            if (client.OnAdFailedToLoad != null)
            {
                AdFailedToLoadEventArgs args = new AdFailedToLoadEventArgs()
                {
                    Message = error
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

        [MonoPInvokeCallback(typeof(GADUAdViewWillLeaveApplicationCallback))]
        private static void AdViewWillLeaveApplicationCallback(IntPtr bannerClient)
        {
            BannerClient client = IntPtrToBannerClient(bannerClient);
            if (client.OnAdLeavingApplication != null)
            {
                client.OnAdLeavingApplication(client, EventArgs.Empty);
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
