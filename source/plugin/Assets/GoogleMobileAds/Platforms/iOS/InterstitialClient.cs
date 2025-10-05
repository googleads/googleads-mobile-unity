#if UNITY_IOS
// Copyright 2015-2021 Google LLC
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
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    public class InterstitialClient : IInterstitialClient, IDisposable
    {
        private IntPtr interstitialClientPtr;
        private IntPtr interstitialPtr;

#region interstitial ad callback types

        internal delegate void GADUInterstitialAdLoadedCallback(IntPtr interstitialClient);

        internal delegate void GADUInterstitialAdFailedToLoadCallback(IntPtr interstitialClient, IntPtr error);

        internal delegate void GADUInterstitialPaidEventCallback(
            IntPtr interstitialClient, int precision, long value, string currencyCode);

#endregion

#region full screen content callback types

        internal delegate void GADUInterstitialAdFailedToPresentFullScreenContentCallback(IntPtr interstitialClient, IntPtr error);

        internal delegate void GADUInterstitialAdWillPresentFullScreenContentCallback(IntPtr interstitialClient);

        internal delegate void GADUInterstitialAdDidDismissFullScreenContentCallback(IntPtr interstitialClient);

        internal delegate void GADUInterstitialAdDidRecordImpressionCallback(IntPtr interstitialClient);

        internal delegate void GADUInterstitialAdDidRecordClickCallback(IntPtr interstitialClient);

#endregion

        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;

        public event Action<AdValue> OnPaidEvent;

        public event EventHandler<AdErrorClientEventArgs> OnAdFailedToPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidRecordImpression;

        public event Action OnAdClicked;

        // A long integer provided by the AdMob UI for the configured placement.
        public long PlacementId
        {
            get
            {
                return Externs.GADUGetInterstitialAdPlacementID(InterstitialPtr);
            }
            set
            {
                Externs.GADUSetInterstitialAdPlacementID(InterstitialPtr, value);
            }
        }

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

        internal void CreateInterstitialAdWithReference(IntPtr interstitialAdClientRef,
                                                        IntPtr interstitialAdRef)
        {
            interstitialClientPtr = interstitialAdClientRef;
            InterstitialPtr = interstitialAdRef;

            Externs.GADUSetInterstitialCallbacks(
                InterstitialPtr,
                InterstitialLoadedCallback,
                InterstitialFailedToLoadCallback,
                AdWillPresentFullScreenContentCallback,
                AdFailedToPresentFullScreenContentCallback,
                AdDidDismissFullScreenContentCallback,
                AdDidRecordImpressionCallback,
                AdDidRecordClickCallback,
                InterstitialPaidEventCallback);
        }

#region IInterstitialClient implementation

        public void CreateInterstitialAd()
        {
            this.interstitialClientPtr = (IntPtr)GCHandle.Alloc(this);
            this.InterstitialPtr = Externs.GADUCreateInterstitial(this.interstitialClientPtr);

            Externs.GADUSetInterstitialCallbacks(
                this.InterstitialPtr,
                InterstitialLoadedCallback,
                InterstitialFailedToLoadCallback,
                AdWillPresentFullScreenContentCallback,
                AdFailedToPresentFullScreenContentCallback,
                AdDidDismissFullScreenContentCallback,
                AdDidRecordImpressionCallback,
                AdDidRecordClickCallback,
                InterstitialPaidEventCallback);
        }

        // Verify if an interstitial ad is preloaded and available to show.
        public bool IsAdAvailable(string adUnitId)
        {
            return Externs.GADUInterstitialIsPreloadedAdAvailable(adUnitId);
        }

        // Returns the next pre-loaded interstitial ad and null if no ad is available.
        public IInterstitialClient PollAd(string adUnitId)
        {
            Externs.GADUInterstitialPreloadedAdWithAdUnitID(this.InterstitialPtr, adUnitId);
            return this;
        }

        public void LoadAd(string adUnitID, AdRequest request) {
            IntPtr requestPtr = Utils.BuildAdRequest(request);
            Externs.GADULoadInterstitialAd(this.InterstitialPtr, adUnitID, requestPtr);
            Externs.GADURelease(requestPtr);
        }

        // Show the interstitial ad on the screen.
        public void Show()
        {
            Externs.GADUShowInterstitial(this.InterstitialPtr);
        }

        // Returns the ad unit ID.
        public string GetAdUnitID()
        {
            return Externs.GADUGetInterstitialAdUnitID(this.InterstitialPtr);
        }

        public IResponseInfoClient GetResponseInfoClient()
        {
            return new ResponseInfoClient(ResponseInfoClientType.AdLoaded, this.InterstitialPtr);
        }

        // Destroys the interstitial ad.
        public void DestroyInterstitial()
        {
            this.InterstitialPtr = IntPtr.Zero;
        }

        public void Dispose()
        {
            this.DestroyInterstitial();
            if (this.interstitialClientPtr == IntPtr.Zero)
            {
                return;
            }
            ((GCHandle)this.interstitialClientPtr).Free();
        }

        ~InterstitialClient()
        {
            this.Dispose();
        }

#endregion

#region interstitial ad callback methods

        [MonoPInvokeCallback(typeof(GADUInterstitialAdLoadedCallback))]
        private static void InterstitialLoadedCallback(IntPtr interstitialClient)
        {
            InterstitialClient client = IntPtrToInterstitialClient(interstitialClient);
            if (client.OnAdLoaded != null)
            {
                client.OnAdLoaded(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUInterstitialAdFailedToLoadCallback))]
        private static void InterstitialFailedToLoadCallback(
            IntPtr interstitialClient, IntPtr error)
        {
            InterstitialClient client = IntPtrToInterstitialClient(interstitialClient);
            if (client.OnAdFailedToLoad != null)
            {
                LoadAdErrorClientEventArgs args = new LoadAdErrorClientEventArgs()
                {
                    LoadAdErrorClient = new LoadAdErrorClient(error)
                };
                client.OnAdFailedToLoad(client, args);
            }
        }

        [MonoPInvokeCallback(typeof(GADUInterstitialPaidEventCallback))]
        private static void InterstitialPaidEventCallback(
            IntPtr interstitialClient, int precision, long value, string currencyCode)
        {
            InterstitialClient client = IntPtrToInterstitialClient(interstitialClient);
            if (client.OnPaidEvent != null)
            {
                AdValue adValue = new AdValue()
                {
                    Precision = (AdValue.PrecisionType)precision,
                    Value = value,
                    CurrencyCode = currencyCode
                };
                client.OnPaidEvent(adValue);
            }
        }

        [MonoPInvokeCallback(typeof(GADUInterstitialAdFailedToPresentFullScreenContentCallback))]
        private static void AdFailedToPresentFullScreenContentCallback(IntPtr interstitialClient, IntPtr error)
        {
            InterstitialClient client = IntPtrToInterstitialClient(interstitialClient);
            if (client.OnAdFailedToPresentFullScreenContent != null)
            {
                AdErrorClientEventArgs args = new AdErrorClientEventArgs()
                {
                    AdErrorClient = new AdErrorClient(error)
                };
                client.OnAdFailedToPresentFullScreenContent(client, args);
            }
        }

        [MonoPInvokeCallback(typeof(GADUInterstitialAdWillPresentFullScreenContentCallback))]
        private static void AdWillPresentFullScreenContentCallback(IntPtr interstitialClient)
        {
            InterstitialClient client = IntPtrToInterstitialClient(interstitialClient);
            if (client.OnAdDidPresentFullScreenContent != null)
            {
                client.OnAdDidPresentFullScreenContent(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUInterstitialAdDidDismissFullScreenContentCallback))]
        private static void AdDidDismissFullScreenContentCallback(IntPtr interstitialClient)
        {
            InterstitialClient client = IntPtrToInterstitialClient(interstitialClient);
            if (client.OnAdDidDismissFullScreenContent != null)
            {
                client.OnAdDidDismissFullScreenContent(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUInterstitialAdDidRecordImpressionCallback))]
        private static void AdDidRecordImpressionCallback(IntPtr interstitialClient)
        {
            InterstitialClient client = IntPtrToInterstitialClient(interstitialClient);
            if (client.OnAdDidRecordImpression != null)
            {
                client.OnAdDidRecordImpression(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUInterstitialAdDidRecordClickCallback))]
        private static void AdDidRecordClickCallback(IntPtr interstitialClient)
        {
            InterstitialClient client = IntPtrToInterstitialClient(interstitialClient);
            if (client.OnAdClicked != null)
            {
                client.OnAdClicked();
            }
        }

        private static InterstitialClient IntPtrToInterstitialClient(
            IntPtr interstitialClient)
        {
            GCHandle handle = (GCHandle)interstitialClient;
            return handle.Target as InterstitialClient;
        }

#endregion
    }
}
#endif
