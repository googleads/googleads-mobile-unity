#if UNITY_IOS
// Copyright (C) 2021 Google LLC
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
using System.Runtime.InteropServices;

using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    public class AppOpenAdClient : IAppOpenAdClient, IDisposable
    {
        private IntPtr appOpenAdPtr;
        private IntPtr appOpenAdClientPtr;

        #region app open callback types

        internal delegate void GADUAppOpenAdLoadedCallback(
            IntPtr appOpenAdClient);

        internal delegate void GADUAppOpenAdFailToLoadCallback(
            IntPtr appOpenAdClient, IntPtr error);

        internal delegate void GADUAppOpenAdPaidEventCallback(
            IntPtr appOpenAdClient, int precision, long value, string currencyCode);

        #endregion

        #region full screen content callback types

        internal delegate void GADUAppOpenAdFailedToPresentFullScreenContentCallback(
            IntPtr appOpenAdClient, IntPtr error);

        internal delegate void GADUAppOpenAdWillPresentFullScreenContentCallback(IntPtr appOpenAdClient);

        internal delegate void GADUAppOpenAdDidDismissFullScreenContentCallback(IntPtr appOpenAdClient);

        internal delegate void GADUAppOpenAdDidRecordImpressionCallback(IntPtr appOpenAdClient);

        internal delegate void GADUAppOpenAdDidRecordClickCallback(IntPtr appOpenAdClient);
        #endregion

        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;

        public event EventHandler<AdValueEventArgs> OnPaidEvent;

        public event EventHandler<AdErrorClientEventArgs> OnAdFailedToPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidRecordImpression;

        public event Action OnAdClicked;

        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;

        // This property should be used when setting the appOpenAdPtr.
        private IntPtr AppOpenAdPtr
        {
            get
            {
                return this.appOpenAdPtr;
            }

            set
            {
                Externs.GADURelease(this.appOpenAdPtr);
                this.appOpenAdPtr = value;
            }
        }

        #region IAppOpenAdClient implementation

        public void CreateAppOpenAd()
        {
            this.appOpenAdClientPtr = (IntPtr)GCHandle.Alloc(this);
            this.AppOpenAdPtr = Externs.GADUCreateAppOpenAd(this.appOpenAdClientPtr);

            Externs.GADUSetAppOpenAdCallbacks(
                    this.AppOpenAdPtr,
                    AppOpenAdLoadedCallback,
                    AppOpenAdFailedToLoadCallback,
                    AppOpenAdPaidEventCallback,
                    AdFailedToPresentFullScreenContentCallback,
                    AdWillPresentFullScreenContentCallback,
                    AdDidDismissFullScreenContentCallback,
                    AdDidRecordImpressionCallback,
                    AdDidRecordClickCallback);
        }

        // Load an ad.
        public void LoadAd(string adUnitID, AdRequest request)
        {
            IntPtr requestPtr = Utils.BuildAdRequest(request);
            Externs.GADULoadAppOpenAdWithAdUnitID(this.AppOpenAdPtr, adUnitID,  requestPtr);
            Externs.GADURelease(requestPtr);
        }

        // Load an ad.
        public void LoadAd(string adUnitID, AdRequest request, ScreenOrientation orientation)
        {
            IntPtr requestPtr = Utils.BuildAdRequest(request);
            Externs.GADULoadAppOpenAd(this.AppOpenAdPtr, adUnitID, (int) orientation, requestPtr);
            Externs.GADURelease(requestPtr);
        }

        // Show the app open ad on the screen.
        public void Show()
        {
            Externs.GADUShowAppOpenAd(this.AppOpenAdPtr);
        }

        public IResponseInfoClient GetResponseInfoClient()
        {
            return new ResponseInfoClient(ResponseInfoClientType.AdLoaded, this.AppOpenAdPtr);
        }

        // Destroys the app open ad.
        public void DestroyAppOpenAd()
        {
            this.AppOpenAdPtr = IntPtr.Zero;
        }

        public void Dispose()
        {
            this.DestroyAppOpenAd();
            ((GCHandle)this.appOpenAdClientPtr).Free();
        }

        ~AppOpenAdClient()
        {
            this.Dispose();
        }

        #endregion

        #region App open ad callback methods

        [MonoPInvokeCallback(typeof(GADUAppOpenAdLoadedCallback))]
        private static void AppOpenAdLoadedCallback(IntPtr appOpenAdClient)
        {
            AppOpenAdClient client = IntPtrToAppOpenAdClient(appOpenAdClient);
            if (client.OnAdLoaded != null)
            {
                client.OnAdLoaded(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUAppOpenAdFailToLoadCallback))]
        private static void AppOpenAdFailedToLoadCallback(
            IntPtr appOpenAdClient, IntPtr error)
        {
            AppOpenAdClient client = IntPtrToAppOpenAdClient(appOpenAdClient);
            if (client.OnAdFailedToLoad != null)
            {
                LoadAdErrorClientEventArgs args = new LoadAdErrorClientEventArgs()
                {
                    LoadAdErrorClient = new LoadAdErrorClient(error),
                };
                client.OnAdFailedToLoad(client, args);
            }
        }

        [MonoPInvokeCallback(typeof(GADUAppOpenAdPaidEventCallback))]
        private static void AppOpenAdPaidEventCallback(
            IntPtr appOpenAdClient, int precision, long value, string currencyCode)
        {
            AppOpenAdClient client = IntPtrToAppOpenAdClient(appOpenAdClient);
            if (client.OnPaidEvent != null)
            {
                AdValue adValue = new AdValue()
                {
                    Precision = (AdValue.PrecisionType) precision,
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

        [MonoPInvokeCallback(typeof(GADUAppOpenAdFailedToPresentFullScreenContentCallback))]
        private static void AdFailedToPresentFullScreenContentCallback(
            IntPtr appOpenAdClient, IntPtr error)
        {
            AppOpenAdClient client = IntPtrToAppOpenAdClient(appOpenAdClient);
            if (client.OnAdFailedToPresentFullScreenContent != null)
            {
                AdErrorClientEventArgs args = new AdErrorClientEventArgs()
                {
                    AdErrorClient = new AdErrorClient(error),
                };
                client.OnAdFailedToPresentFullScreenContent(client, args);
            }
        }

        [MonoPInvokeCallback(typeof(GADUAppOpenAdWillPresentFullScreenContentCallback))]
        private static void AdWillPresentFullScreenContentCallback(IntPtr appOpenAdClient)
        {
            AppOpenAdClient client = IntPtrToAppOpenAdClient(appOpenAdClient);
            if (client.OnAdDidPresentFullScreenContent != null)
            {
                client.OnAdDidPresentFullScreenContent(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUAppOpenAdDidDismissFullScreenContentCallback))]
        private static void AdDidDismissFullScreenContentCallback(IntPtr appOpenAdClient)
        {
            AppOpenAdClient client = IntPtrToAppOpenAdClient(appOpenAdClient);
            if (client.OnAdDidDismissFullScreenContent != null)
            {
                client.OnAdDidDismissFullScreenContent(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUAppOpenAdDidRecordImpressionCallback))]
        private static void AdDidRecordImpressionCallback(IntPtr appOpenAdClient)
        {
            AppOpenAdClient client = IntPtrToAppOpenAdClient(appOpenAdClient);
            if (client.OnAdDidRecordImpression != null)
            {
                client.OnAdDidRecordImpression(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUAppOpenAdDidRecordClickCallback))]
        private static void AdDidRecordClickCallback(IntPtr appOpenAdClient)
        {
            AppOpenAdClient client = IntPtrToAppOpenAdClient(appOpenAdClient);
            if (client.OnAdClicked != null)
            {
                client.OnAdClicked();
            }
        }

        private static AppOpenAdClient IntPtrToAppOpenAdClient(IntPtr appOpenAdClient)
        {
            GCHandle handle = (GCHandle)appOpenAdClient;
            return handle.Target as AppOpenAdClient;
        }

        #endregion
    }
}
#endif
