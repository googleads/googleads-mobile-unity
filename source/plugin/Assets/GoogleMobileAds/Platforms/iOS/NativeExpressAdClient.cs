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
    internal class NativeExpressAdClient : IDisposable, INativeExpressAdClient
    {
        private IntPtr nativeExpressAdViewPtr;
        private IntPtr nativeExpressAdClientPtr;

        #region Native Express Ad callback types

        internal delegate void GADUNativeExpressAdViewDidReceiveAdCallback(IntPtr nativeExpressAdClient);

        internal delegate void GADUNativeExpressAdViewDidFailToReceiveAdWithErrorCallback(
            IntPtr nativeExpressAdClient, string error);

        internal delegate void GADUNativeExpressAdViewWillPresentScreenCallback(IntPtr nativeExpressAdClient);

        internal delegate void GADUNativeExpressAdViewDidDismissScreenCallback(IntPtr nativeExpressAdClient);

        internal delegate void GADUNativeExpressAdViewWillLeaveApplicationCallback(IntPtr nativeExpressAdClient);

        #endregion

        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

        public event EventHandler<EventArgs> OnAdOpening;

        public event EventHandler<EventArgs> OnAdClosed;

        public event EventHandler<EventArgs> OnAdLeavingApplication;

        // This property should be used when setting the nativeExpressAdViewPtr.
        private IntPtr NativeExpressAdViewPtr
        {
            get
            {
                return this.nativeExpressAdViewPtr;
            }

            set
            {
                Externs.GADURelease(this.nativeExpressAdViewPtr);
                this.nativeExpressAdViewPtr = value;
            }
        }

        #region INativeExpressAdClient implementation

        // Creates a native express ad view.
        public void CreateNativeExpressAdView(string adUnitId, AdSize adSize, AdPosition position)
        {
            nativeExpressAdClientPtr = (IntPtr)GCHandle.Alloc(this);
            this.NativeExpressAdViewPtr = Externs.GADUCreateNativeExpressAdView(
                nativeExpressAdClientPtr, adUnitId, adSize.Width, adSize.Height, (int)position);

            Externs.GADUSetNativeExpressAdCallbacks(
                    this.NativeExpressAdViewPtr,
                    NativeExpressAdViewDidReceiveAdCallback,
                    NativeExpressAdViewDidFailToReceiveAdWithErrorCallback,
                    NativeExpressAdViewWillPresentScreenCallback,
                    NativeExpressAdViewDidDismissScreenCallback,
                    NativeExpressAdViewWillLeaveApplicationCallback);
        }

        // Loads an ad.
        public void LoadAd(AdRequest request)
        {
            IntPtr requestPtr = Utils.BuildAdRequest(request);
            Externs.GADURequestNativeExpressAd(this.NativeExpressAdViewPtr, requestPtr);
            Externs.GADURelease(requestPtr);
        }

        // Displays the native express ad view on the screen.
        public void ShowNativeExpressAdView()
        {
            Externs.GADUShowNativeExpressAdView(this.NativeExpressAdViewPtr);
        }

        // Hides the native express ad view from the screen.
        public void HideNativeExpressAdView()
        {
            Externs.GADUHideNativeExpressAdView(this.NativeExpressAdViewPtr);
        }

        // Destroys the native express ad view.
        public void DestroyNativeExpressAdView()
        {
            Externs.GADURemoveNativeExpressAdView(this.NativeExpressAdViewPtr);
            this.NativeExpressAdViewPtr = IntPtr.Zero;
        }

        public void Dispose()
        {
            this.DestroyNativeExpressAdView();
            ((GCHandle)this.nativeExpressAdClientPtr).Free();
        }

        ~NativeExpressAdClient()
        {
            this.Dispose();
        }

        #endregion

        #region Native Express Ad callback methods

        [MonoPInvokeCallback(typeof(GADUNativeExpressAdViewDidReceiveAdCallback))]
        private static void NativeExpressAdViewDidReceiveAdCallback(IntPtr nativeExpressClient)
        {
            NativeExpressAdClient client = IntPtrToNativeExpressAdClient(nativeExpressClient);
            if(client.OnAdLoaded != null)
            {
                client.OnAdLoaded(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUNativeExpressAdViewDidFailToReceiveAdWithErrorCallback))]
        private static void NativeExpressAdViewDidFailToReceiveAdWithErrorCallback(
            IntPtr nativeExpressClient, string error)
        {
            NativeExpressAdClient client = IntPtrToNativeExpressAdClient(nativeExpressClient);
            if(client.OnAdFailedToLoad != null)
            {
                AdFailedToLoadEventArgs args = new AdFailedToLoadEventArgs()
                {
                    Message = error
                };
                client.OnAdFailedToLoad(client, args);
            }
        }

        [MonoPInvokeCallback(typeof(GADUNativeExpressAdViewWillPresentScreenCallback))]
        private static void NativeExpressAdViewWillPresentScreenCallback(IntPtr nativeExpressClient)
        {
            NativeExpressAdClient client = IntPtrToNativeExpressAdClient(nativeExpressClient);
            if(client.OnAdOpening != null)
            {
                client.OnAdOpening(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUNativeExpressAdViewDidDismissScreenCallback))]
        private static void NativeExpressAdViewDidDismissScreenCallback(IntPtr nativeExpressClient)
        {
            NativeExpressAdClient client = IntPtrToNativeExpressAdClient(nativeExpressClient);
            if(client.OnAdClosed != null)
            {
                client.OnAdClosed(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUNativeExpressAdViewWillLeaveApplicationCallback))]
        private static void NativeExpressAdViewWillLeaveApplicationCallback(IntPtr nativeExpressClient)
        {
            NativeExpressAdClient client = IntPtrToNativeExpressAdClient(nativeExpressClient);
            if(client.OnAdLeavingApplication != null)
            {
                client.OnAdLeavingApplication(client, EventArgs.Empty);
            }
        }

        private static NativeExpressAdClient IntPtrToNativeExpressAdClient(IntPtr nativeExpressAdClient)
        {
            GCHandle handle = (GCHandle)nativeExpressAdClient;
            return handle.Target as NativeExpressAdClient;
        }

        #endregion
    }
}

#endif
