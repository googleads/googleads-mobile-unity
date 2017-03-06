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
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    internal class InterstitialClient : IInterstitialClient, IDisposable
    {
        private IntPtr interstitialPtr;
        private IntPtr interstitialClientPtr;

        #region Interstitial callback types

        internal delegate void GADUInterstitialDidReceiveAdCallback(IntPtr interstitialClient);

        internal delegate void GADUInterstitialDidFailToReceiveAdWithErrorCallback(
                IntPtr interstitialClient, string error);

        internal delegate void GADUInterstitialWillPresentScreenCallback(IntPtr interstitialClient);

        internal delegate void GADUInterstitialDidDismissScreenCallback(IntPtr interstitialClient);

        internal delegate void GADUInterstitialWillLeaveApplicationCallback(
                IntPtr interstitialClient);

        #endregion

        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

        public event EventHandler<EventArgs> OnAdOpening;

        public event EventHandler<EventArgs> OnAdClosed;

        public event EventHandler<EventArgs> OnAdLeavingApplication;

        // This property should be used when setting the interstitialPtr.
        private IntPtr InterstitialPtr
        {
            get
            {
                return this.interstitialPtr;
            }

            set
            {
                Externs.GADURelease(this.interstitialPtr);
                this.interstitialPtr = value;
            }
        }

        #region IInterstitialClient implementation

        // Creates an interstitial ad.
        public void CreateInterstitialAd(string adUnitId)
        {
            this.interstitialClientPtr = (IntPtr)GCHandle.Alloc(this);
            this.InterstitialPtr = Externs.GADUCreateInterstitial(this.interstitialClientPtr, adUnitId);
            Externs.GADUSetInterstitialCallbacks(
                    this.InterstitialPtr,
                    InterstitialDidReceiveAdCallback,
                    InterstitialDidFailToReceiveAdWithErrorCallback,
                    InterstitialWillPresentScreenCallback,
                    InterstitialDidDismissScreenCallback,
                    InterstitialWillLeaveApplicationCallback);
        }

        // Loads an ad.
        public void LoadAd(AdRequest request)
        {
            IntPtr requestPtr = Utils.BuildAdRequest(request);
            Externs.GADURequestInterstitial(this.InterstitialPtr, requestPtr);
            Externs.GADURelease(requestPtr);
        }

        // Checks if interstitial has loaded.
        public bool IsLoaded()
        {
            return Externs.GADUInterstitialReady(this.InterstitialPtr);
        }

        // Presents the interstitial ad on the screen
        public void ShowInterstitial()
        {
            Externs.GADUShowInterstitial(this.InterstitialPtr);
        }

        // Destroys the interstitial ad.
        public void DestroyInterstitial()
        {
            this.InterstitialPtr = IntPtr.Zero;
        }

        public void Dispose()
        {
            this.DestroyInterstitial();
            ((GCHandle)this.interstitialClientPtr).Free();
        }

        ~InterstitialClient()
        {
            this.Dispose();
        }

        #endregion

        #region Interstitial callback methods

        [MonoPInvokeCallback(typeof(GADUInterstitialDidReceiveAdCallback))]
        private static void InterstitialDidReceiveAdCallback(IntPtr interstitialClient)
        {
            InterstitialClient client = IntPtrToInterstitialClient(interstitialClient);
            if (client.OnAdLoaded != null)
            {
                client.OnAdLoaded(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUInterstitialDidFailToReceiveAdWithErrorCallback))]
        private static void InterstitialDidFailToReceiveAdWithErrorCallback(
                IntPtr interstitialClient, string error)
        {
            InterstitialClient client = IntPtrToInterstitialClient(interstitialClient);
            if (client.OnAdFailedToLoad != null)
            {
                AdFailedToLoadEventArgs args = new AdFailedToLoadEventArgs()
                {
                    Message = error
                };
                client.OnAdFailedToLoad(client, args);
            }
        }

        [MonoPInvokeCallback(typeof(GADUInterstitialWillPresentScreenCallback))]
        private static void InterstitialWillPresentScreenCallback(IntPtr interstitialClient)
        {
            InterstitialClient client = IntPtrToInterstitialClient(interstitialClient);
            if (client.OnAdOpening != null)
            {
                client.OnAdOpening(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUInterstitialDidDismissScreenCallback))]
        private static void InterstitialDidDismissScreenCallback(IntPtr interstitialClient)
        {
            InterstitialClient client = IntPtrToInterstitialClient(interstitialClient);
            if (client.OnAdClosed != null)
            {
                client.OnAdClosed(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUInterstitialWillLeaveApplicationCallback))]
        private static void InterstitialWillLeaveApplicationCallback(IntPtr interstitialClient)
        {
            InterstitialClient client = IntPtrToInterstitialClient(interstitialClient);
            if (client.OnAdLeavingApplication != null)
            {
                client.OnAdLeavingApplication(client, EventArgs.Empty);
            }
        }

        private static InterstitialClient IntPtrToInterstitialClient(IntPtr interstitialClient)
        {
            GCHandle handle = (GCHandle)interstitialClient;
            return handle.Target as InterstitialClient;
        }

        #endregion
    }
}

#endif
