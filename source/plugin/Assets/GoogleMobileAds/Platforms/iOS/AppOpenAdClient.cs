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
        public bool IsDestroyed { get; private set; }

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

        public event Action OnAdFullScreenContentOpened = delegate { };
        public event Action OnAdFullScreenContentClosed = delegate { };
        public event Action<IAdErrorClient> OnAdFullScreenContentFailed = delegate { };
        public event Action<AdValue> OnAdPaid = delegate { };
        public event Action OnAdClickRecorded = delegate { };
        public event Action OnAdImpressionRecorded = delegate { };

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

        private Action<IAppOpenAdClient, ILoadAdErrorClient> _loadCallback;

        #region IAppOpenAdClient implementation

        public AppOpenAdClient()
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
        public void LoadAppOpenAd(string adUnitId, ScreenOrientation orientation,
            AdRequest request, Action<IAppOpenAdClient, ILoadAdErrorClient> callback)
        {
            _loadCallback = callback;
            IntPtr requestPtr = Utils.BuildAdRequest(request);
            Externs.GADULoadAppOpenAd(this.AppOpenAdPtr, adUnitId, (int) orientation, requestPtr);
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
        public void Destroy()
        {
            this.AppOpenAdPtr = IntPtr.Zero;
            IsDestroyed = true;
        }

        public void Dispose()
        {
            this.Destroy();
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
            if (client._loadCallback == null)
            {
                return;
            }
            client._loadCallback(client, null);
            client._loadCallback = null;
        }

        [MonoPInvokeCallback(typeof(GADUAppOpenAdFailToLoadCallback))]
        private static void AppOpenAdFailedToLoadCallback(
            IntPtr appOpenAdClient, IntPtr error)
        {
            AppOpenAdClient client = IntPtrToAppOpenAdClient(appOpenAdClient);
            if (client._loadCallback == null)
            {
                return;
            }
            client._loadCallback(null, new LoadAdErrorClient(error));
            client._loadCallback = null;
        }

        [MonoPInvokeCallback(typeof(GADUAppOpenAdPaidEventCallback))]
        private static void AppOpenAdPaidEventCallback(
            IntPtr appOpenAdClient, int precision, long value, string currencyCode)
        {
            AppOpenAdClient client = IntPtrToAppOpenAdClient(appOpenAdClient);
            AdValue adValue = new AdValue()
            {
                Precision = (AdValue.PrecisionType)precision,
                Value = value,
                CurrencyCode = currencyCode
            };
            client.OnAdPaid(adValue);
        }

        [MonoPInvokeCallback(typeof(GADUAppOpenAdFailedToPresentFullScreenContentCallback))]
        private static void AdFailedToPresentFullScreenContentCallback(
            IntPtr appOpenAdClient, IntPtr error)
        {
            AppOpenAdClient client = IntPtrToAppOpenAdClient(appOpenAdClient);
            client.OnAdFullScreenContentFailed(new AdErrorClient(error));
        }

        [MonoPInvokeCallback(typeof(GADUAppOpenAdWillPresentFullScreenContentCallback))]
        private static void AdWillPresentFullScreenContentCallback(IntPtr appOpenAdClient)
        {
            AppOpenAdClient client = IntPtrToAppOpenAdClient(appOpenAdClient);
            client.OnAdFullScreenContentOpened();
        }

        [MonoPInvokeCallback(typeof(GADUAppOpenAdDidDismissFullScreenContentCallback))]
        private static void AdDidDismissFullScreenContentCallback(IntPtr appOpenAdClient)
        {
            AppOpenAdClient client = IntPtrToAppOpenAdClient(appOpenAdClient);
            client.OnAdFullScreenContentClosed();
        }

        [MonoPInvokeCallback(typeof(GADUAppOpenAdDidRecordImpressionCallback))]
        private static void AdDidRecordImpressionCallback(IntPtr appOpenAdClient)
        {
            AppOpenAdClient client = IntPtrToAppOpenAdClient(appOpenAdClient);
            client.OnAdImpressionRecorded();
        }

        [MonoPInvokeCallback(typeof(GADUAppOpenAdDidRecordClickCallback))]
        private static void AdDidRecordClickCallback(IntPtr appOpenAdClient)
        {
            AppOpenAdClient client = IntPtrToAppOpenAdClient(appOpenAdClient);
            client.OnAdClickRecorded();
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
