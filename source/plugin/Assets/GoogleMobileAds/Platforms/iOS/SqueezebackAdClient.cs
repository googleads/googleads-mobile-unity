#if UNITY_IOS
// Copyright 2026 Google LLC
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
    public class SqueezebackAdClient :
        ISqueezebackAdClient, IDisposable
    {
        private IntPtr _squeezebackAdPtr;
        private IntPtr _squeezebackAdClientPtr;

        #region Squeezeback callback types

        internal delegate void GADUSqueezebackAdLoadedCallback(
            IntPtr squeezebackAdClient);

        internal delegate void GADUSqueezebackAdFailToLoadCallback(
            IntPtr squeezebackAdClient, IntPtr error);

        internal delegate void GADUSqueezebackAdShownCallback(
            IntPtr squeezebackAdClient);

        internal delegate void GADUSqueezebackAdHiddenCallback(
            IntPtr squeezebackAdClient);

        internal delegate void
            GADUSqueezebackAdDidRecordImpressionCallback(
                IntPtr squeezebackAdClient);

        internal delegate void GADUSqueezebackAdDidRecordClickCallback(
            IntPtr squeezebackAdClient);

        internal delegate void
            GADUSqueezebackAdDidFailToShowCallback(
                IntPtr squeezebackAdClient, IntPtr error);

        internal delegate void
            GADUSqueezebackAdWillPresentFullScreenContentCallback(
                IntPtr squeezebackAdClient);

        internal delegate void
            GADUSqueezebackAdDidDismissFullScreenContentCallback(
                IntPtr squeezebackAdClient);

        internal delegate void GADUSqueezebackAdPaidEventCallback(
            IntPtr squeezebackAdClient, int precision, long value,
            string currencyCode);

        #endregion

        public event Action OnAdLoaded;
        public event Action<LoadAdErrorClientEventArgs> OnAdFailedToLoad;
        public event Action OnAdShown;
        public event Action OnAdHidden;
        public event Action<AdErrorClientEventArgs> OnAdFailedToShow;
        public event Action OnAdDidRecordImpression;
        public event Action OnAdClicked;
#pragma warning disable 67
        public event Action<AdErrorClientEventArgs>
            OnAdFailedToPresentFullScreenContent;
#pragma warning restore 67
        public event Action OnAdDidPresentFullScreenContent;
        public event Action OnAdDidDismissFullScreenContent;
        public event Action<AdValue> OnPaidEvent;

        public SqueezebackAdClient()
        {
            CreateSqueezebackAd();
        }

        // This property should be used when setting the _squeezebackAdPtr.
        private IntPtr SqueezebackAdPtr
        {
            get
            {
                return this._squeezebackAdPtr;
            }
            set
            {
                Externs.GADURelease(this._squeezebackAdPtr);
                this._squeezebackAdPtr = value;
            }
        }

        public void CreateSqueezebackAd()
        {
            if (this.SqueezebackAdPtr != IntPtr.Zero)
            {
                return;
            }
            if (this._squeezebackAdClientPtr == IntPtr.Zero)
            {
                this._squeezebackAdClientPtr = (IntPtr)GCHandle.Alloc(this);
            }
            this.SqueezebackAdPtr =
                Externs.GADUCreateSqueezebackAd(
                    this._squeezebackAdClientPtr);
            Externs.GADUSetSqueezebackAdCallbacks(
                this.SqueezebackAdPtr,
                SqueezebackAdLoadedCallback,
                SqueezebackAdFailedToLoadCallback,
                SqueezebackAdShownCallback,
                SqueezebackAdHiddenCallback,
                SqueezebackAdDidRecordImpressionCallback,
                SqueezebackAdDidRecordClickCallback,
                SqueezebackAdDidFailToShowCallback,
                SqueezebackAdWillPresentFullScreenContentCallback,
                SqueezebackAdDidDismissFullScreenContentCallback,
                SqueezebackAdPaidEventCallback);
        }

        public void LoadAd(string adUnitId, AdRequest request)
        {
            if (this.SqueezebackAdPtr == IntPtr.Zero)
            {
                CreateSqueezebackAd();
            }
            IntPtr requestPtr = Utils.BuildAdRequest(request);
            Externs.GADULoadSqueezebackAd(
                this.SqueezebackAdPtr, adUnitId, requestPtr);
            Externs.GADURelease(requestPtr);
        }

        public void Show(SqueezebackAdOptions options = null)
        {
            if (options == null || !options.FallbackBackgroundColor.HasValue)
            {
                Externs.GADUShowSqueezebackAdWithOptions(
                    this.SqueezebackAdPtr, IntPtr.Zero);
                return;
            }

            Color color = options.FallbackBackgroundColor.Value;
            IntPtr colorPtr = IntPtr.Zero;
            IntPtr optionsPtr = IntPtr.Zero;
            try
            {
                colorPtr = Externs.GADUCreateUIColor(
                    color.a, color.r, color.g, color.b);
                optionsPtr = Externs.GADUCreateSqueezebackAdOptions();
                Externs.GADUSqueezebackAdOptionsSetFallbackBackgroundColor(
                    optionsPtr, colorPtr);
                Externs.GADUShowSqueezebackAdWithOptions(
                    this.SqueezebackAdPtr, optionsPtr);
            }
            finally
            {
                if (colorPtr != IntPtr.Zero)
                {
                    Externs.GADURelease(colorPtr);
                }
                if (optionsPtr != IntPtr.Zero)
                {
                    Externs.GADURelease(optionsPtr);
                }
            }
        }

        public void Hide()
        {
            Externs.GADUHideSqueezebackAd(this.SqueezebackAdPtr);
        }

        public void Destroy()
        {
            Externs.GADUDestroySqueezebackAd(this.SqueezebackAdPtr);
            this.SqueezebackAdPtr = IntPtr.Zero;
        }

        public string GetAdUnitID()
        {
            return Externs.GADUGetSqueezebackAdUnitID(
                this.SqueezebackAdPtr);
        }

        public IResponseInfoClient GetResponseInfoClient()
        {
            return new ResponseInfoClient(
                ResponseInfoClientType.AdLoaded, this.SqueezebackAdPtr);
        }

        public void Dispose()
        {
            this.Destroy();
            if (this._squeezebackAdClientPtr == IntPtr.Zero)
            {
                return;
            }
            ((GCHandle)this._squeezebackAdClientPtr).Free();
            this._squeezebackAdClientPtr = IntPtr.Zero;
        }

        ~SqueezebackAdClient()
        {
            this.Dispose();
        }

        #region Squeezeback ad callback methods

        [MonoPInvokeCallback(typeof(GADUSqueezebackAdLoadedCallback))]
        private static void SqueezebackAdLoadedCallback(
            IntPtr squeezebackAdClient)
        {
            SqueezebackAdClient client =
                IntPtrToSqueezebackAdClient(squeezebackAdClient);
            if (client != null && client.OnAdLoaded != null)
            {
                client.OnAdLoaded.Invoke();
            }
        }

        [MonoPInvokeCallback(typeof(GADUSqueezebackAdFailToLoadCallback))]
        private static void SqueezebackAdFailedToLoadCallback(
            IntPtr squeezebackAdClient, IntPtr error)
        {
            SqueezebackAdClient client =
                IntPtrToSqueezebackAdClient(squeezebackAdClient);
            if (client != null && client.OnAdFailedToLoad != null)
            {
                LoadAdErrorClientEventArgs args =
                    new LoadAdErrorClientEventArgs()
                {
                    LoadAdErrorClient = new LoadAdErrorClient(error),
                };
                client.OnAdFailedToLoad.Invoke(args);
            }
        }

        [MonoPInvokeCallback(typeof(GADUSqueezebackAdShownCallback))]
        private static void SqueezebackAdShownCallback(
            IntPtr squeezebackAdClient)
        {
            SqueezebackAdClient client =
                IntPtrToSqueezebackAdClient(squeezebackAdClient);
            if (client != null && client.OnAdShown != null)
            {
                client.OnAdShown.Invoke();
            }
        }

        [MonoPInvokeCallback(typeof(GADUSqueezebackAdHiddenCallback))]
        private static void SqueezebackAdHiddenCallback(
            IntPtr squeezebackAdClient)
        {
            SqueezebackAdClient client =
                IntPtrToSqueezebackAdClient(squeezebackAdClient);
            if (client != null && client.OnAdHidden != null)
            {
                client.OnAdHidden.Invoke();
            }
        }

        [MonoPInvokeCallback(
            typeof(GADUSqueezebackAdDidRecordImpressionCallback))]
        private static void SqueezebackAdDidRecordImpressionCallback(
            IntPtr squeezebackAdClient)
        {
            SqueezebackAdClient client =
                IntPtrToSqueezebackAdClient(squeezebackAdClient);
            if (client != null && client.OnAdDidRecordImpression != null)
            {
                client.OnAdDidRecordImpression.Invoke();
            }
        }

        [MonoPInvokeCallback(
            typeof(GADUSqueezebackAdDidRecordClickCallback))]
        private static void SqueezebackAdDidRecordClickCallback(
            IntPtr squeezebackAdClient)
        {
            SqueezebackAdClient client =
                IntPtrToSqueezebackAdClient(squeezebackAdClient);
            if (client != null && client.OnAdClicked != null)
            {
                client.OnAdClicked.Invoke();
            }
        }

        [MonoPInvokeCallback(typeof(
            GADUSqueezebackAdDidFailToShowCallback))]
        private static void
            SqueezebackAdDidFailToShowCallback(
                IntPtr squeezebackAdClient, IntPtr error)
        {
            SqueezebackAdClient client =
                IntPtrToSqueezebackAdClient(squeezebackAdClient);
            if (client != null && client.OnAdFailedToShow != null)
            {
                AdErrorClientEventArgs args = new AdErrorClientEventArgs()
                {
                    AdErrorClient = new AdErrorClient(error),
                };
                client.OnAdFailedToShow.Invoke(args);
            }
        }

        [MonoPInvokeCallback(typeof(
            GADUSqueezebackAdWillPresentFullScreenContentCallback))]
        private static void
            SqueezebackAdWillPresentFullScreenContentCallback(
                IntPtr squeezebackAdClient)
        {
            SqueezebackAdClient client =
                IntPtrToSqueezebackAdClient(squeezebackAdClient);
            if (client != null &&
                client.OnAdDidPresentFullScreenContent != null)
            {
                client.OnAdDidPresentFullScreenContent.Invoke();
            }
        }

        [MonoPInvokeCallback(typeof(
            GADUSqueezebackAdDidDismissFullScreenContentCallback))]
        private static void
            SqueezebackAdDidDismissFullScreenContentCallback(
                IntPtr squeezebackAdClient)
        {
            SqueezebackAdClient client =
                IntPtrToSqueezebackAdClient(squeezebackAdClient);
            if (client != null &&
                client.OnAdDidDismissFullScreenContent != null)
            {
                client.OnAdDidDismissFullScreenContent.Invoke();
            }
        }

        [MonoPInvokeCallback(typeof(GADUSqueezebackAdPaidEventCallback))]
        private static void SqueezebackAdPaidEventCallback(
            IntPtr squeezebackAdClient, int precision, long value,
            string currencyCode)
        {
            SqueezebackAdClient client =
                IntPtrToSqueezebackAdClient(squeezebackAdClient);
            if (client != null && client.OnPaidEvent != null)
            {
                AdValue adValue = new AdValue()
                {
                    Precision = (AdValue.PrecisionType)precision,
                    Value = value,
                    CurrencyCode = currencyCode
                };
                client.OnPaidEvent.Invoke(adValue);
            }
        }

        private static SqueezebackAdClient
            IntPtrToSqueezebackAdClient(IntPtr squeezebackAdClient)
        {
            GCHandle handle = (GCHandle)squeezebackAdClient;
            return handle.Target as SqueezebackAdClient;
        }

        #endregion
    }
}
#endif
