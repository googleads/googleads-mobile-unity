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
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    public class PictureInPictureAdClient :
        IPictureInPictureAdClient, IDisposable
    {
        private IntPtr _pictureInPictureAdPtr;
        private IntPtr _pictureInPictureAdClientPtr;

        #region Picture-in-Picture callback types

        internal delegate void GADUPictureInPictureAdLoadedCallback(
            IntPtr pipAdClient);

        internal delegate void GADUPictureInPictureAdFailToLoadCallback(
            IntPtr pipAdClient, IntPtr error);

        internal delegate void GADUPictureInPictureAdShownCallback(
            IntPtr pipAdClient);

        internal delegate void GADUPictureInPictureAdHiddenCallback(
            IntPtr pipAdClient);

        internal delegate void
            GADUPictureInPictureAdDidRecordImpressionCallback(
                IntPtr pipAdClient);

        internal delegate void GADUPictureInPictureAdDidRecordClickCallback(
            IntPtr pipAdClient);

        internal delegate void
            GADUPictureInPictureAdFailedToPresentFullScreenContentCallback(
                IntPtr pipAdClient, IntPtr error);

        internal delegate void
            GADUPictureInPictureAdWillPresentFullScreenContentCallback(
                IntPtr pipAdClient);

        internal delegate void
            GADUPictureInPictureAdDidDismissFullScreenContentCallback(
                IntPtr pipAdClient);

        internal delegate void GADUPictureInPictureAdPaidEventCallback(
            IntPtr pipAdClient, int precision, long value,
            string currencyCode);

        #endregion

        public event Action OnAdLoaded;
        public event Action<LoadAdErrorClientEventArgs> OnAdFailedToLoad;
        public event Action OnAdShown;
        public event Action OnAdHidden;
        public event Action OnAdDidRecordImpression;
        public event Action OnAdClicked;
        public event Action<AdErrorClientEventArgs>
            OnAdFailedToPresentFullScreenContent;
        public event Action OnAdDidPresentFullScreenContent;
        public event Action OnAdDidDismissFullScreenContent;
        public event Action<AdValue> OnPaidEvent;

        // This property should be used when setting the _pictureInPictureAdPtr.
        private IntPtr PictureInPictureAdPtr
        {
            get
            {
                return this._pictureInPictureAdPtr;
            }
            set
            {
                Externs.GADURelease(this._pictureInPictureAdPtr);
                this._pictureInPictureAdPtr = value;
            }
        }

        public void CreatePictureInPictureAd()
        {
            this._pictureInPictureAdClientPtr = (IntPtr)GCHandle.Alloc(this);
            this.PictureInPictureAdPtr =
                Externs.GADUCreatePictureInPictureAd(
                    this._pictureInPictureAdClientPtr);
            Externs.GADUSetPictureInPictureAdCallbacks(
                this.PictureInPictureAdPtr,
                PictureInPictureAdLoadedCallback,
                PictureInPictureAdFailedToLoadCallback,
                PictureInPictureAdShownCallback,
                PictureInPictureAdHiddenCallback,
                PictureInPictureAdDidRecordImpressionCallback,
                PictureInPictureAdDidRecordClickCallback,
                PictureInPictureAdFailedToPresentFullScreenContentCallback,
                PictureInPictureAdWillPresentFullScreenContentCallback,
                PictureInPictureAdDidDismissFullScreenContentCallback,
                PictureInPictureAdPaidEventCallback);
        }

        public void LoadAd(string adUnitId, AdRequest request)
        {
            IntPtr requestPtr = Utils.BuildAdRequest(request);
            Externs.GADULoadPictureInPictureAd(
                this.PictureInPictureAdPtr, adUnitId, requestPtr);
            Externs.GADURelease(requestPtr);
        }

        public void Show(PictureInPictureAdPosition position)
        {
            Externs.GADUShowPictureInPictureAd(
                this.PictureInPictureAdPtr, (int)position);
        }

        public void Hide()
        {
            Externs.GADUHidePictureInPictureAd(this.PictureInPictureAdPtr);
        }

        public void Destroy()
        {
            this.PictureInPictureAdPtr = IntPtr.Zero;
        }

        public string GetAdUnitID()
        {
            return Externs.GADUGetPictureInPictureAdUnitID(
                this.PictureInPictureAdPtr);
        }

        public PictureInPictureAdPosition GetAdPosition()
        {
            return (PictureInPictureAdPosition)
                Externs.GADUGetPictureInPictureAdPosition(
                    this.PictureInPictureAdPtr);
        }

        public IResponseInfoClient GetResponseInfoClient()
        {
            return new ResponseInfoClient(
                ResponseInfoClientType.AdLoaded, this.PictureInPictureAdPtr);
        }

        public void Dispose()
        {
            this.Destroy();
            if (this._pictureInPictureAdClientPtr == IntPtr.Zero)
            {
                return;
            }
            ((GCHandle)this._pictureInPictureAdClientPtr).Free();
            this._pictureInPictureAdClientPtr = IntPtr.Zero;
        }

        ~PictureInPictureAdClient()
        {
            this.Dispose();
        }

        #region Picture-in-Picture ad callback methods

        [MonoPInvokeCallback(typeof(GADUPictureInPictureAdLoadedCallback))]
        private static void PictureInPictureAdLoadedCallback(
            IntPtr pipAdClient)
        {
            PictureInPictureAdClient client =
                IntPtrToPictureInPictureAdClient(pipAdClient);
            if (client != null && client.OnAdLoaded != null)
            {
                client.OnAdLoaded.Invoke();
            }
        }

        [MonoPInvokeCallback(typeof(GADUPictureInPictureAdFailToLoadCallback))]
        private static void PictureInPictureAdFailedToLoadCallback(
            IntPtr pipAdClient, IntPtr error)
        {
            PictureInPictureAdClient client =
                IntPtrToPictureInPictureAdClient(pipAdClient);
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

        [MonoPInvokeCallback(typeof(GADUPictureInPictureAdShownCallback))]
        private static void PictureInPictureAdShownCallback(
            IntPtr pipAdClient)
        {
            PictureInPictureAdClient client =
                IntPtrToPictureInPictureAdClient(pipAdClient);
            if (client != null && client.OnAdShown != null)
            {
                client.OnAdShown.Invoke();
            }
        }

        [MonoPInvokeCallback(typeof(GADUPictureInPictureAdHiddenCallback))]
        private static void PictureInPictureAdHiddenCallback(
            IntPtr pipAdClient)
        {
            PictureInPictureAdClient client =
                IntPtrToPictureInPictureAdClient(pipAdClient);
            if (client != null && client.OnAdHidden != null)
            {
                client.OnAdHidden.Invoke();
            }
        }

        [MonoPInvokeCallback(
            typeof(GADUPictureInPictureAdDidRecordImpressionCallback))]
        private static void PictureInPictureAdDidRecordImpressionCallback(
            IntPtr pipAdClient)
        {
            PictureInPictureAdClient client =
                IntPtrToPictureInPictureAdClient(pipAdClient);
            if (client != null && client.OnAdDidRecordImpression != null)
            {
                client.OnAdDidRecordImpression.Invoke();
            }
        }

        [MonoPInvokeCallback(
            typeof(GADUPictureInPictureAdDidRecordClickCallback))]
        private static void PictureInPictureAdDidRecordClickCallback(
            IntPtr pipAdClient)
        {
            PictureInPictureAdClient client =
                IntPtrToPictureInPictureAdClient(pipAdClient);
            if (client != null && client.OnAdClicked != null)
            {
                client.OnAdClicked.Invoke();
            }
        }

        [MonoPInvokeCallback(typeof(
            GADUPictureInPictureAdFailedToPresentFullScreenContentCallback))]
        private static void
            PictureInPictureAdFailedToPresentFullScreenContentCallback(
                IntPtr pipAdClient, IntPtr error)
        {
            PictureInPictureAdClient client =
                IntPtrToPictureInPictureAdClient(pipAdClient);
            if (client != null &&
                client.OnAdFailedToPresentFullScreenContent != null)
            {
                AdErrorClientEventArgs args = new AdErrorClientEventArgs()
                {
                    AdErrorClient = new AdErrorClient(error),
                };
                client.OnAdFailedToPresentFullScreenContent.Invoke(args);
            }
        }

        [MonoPInvokeCallback(typeof(
            GADUPictureInPictureAdWillPresentFullScreenContentCallback))]
        private static void
            PictureInPictureAdWillPresentFullScreenContentCallback(
                IntPtr pipAdClient)
        {
            PictureInPictureAdClient client =
                IntPtrToPictureInPictureAdClient(pipAdClient);
            if (client != null &&
                client.OnAdDidPresentFullScreenContent != null)
            {
                client.OnAdDidPresentFullScreenContent.Invoke();
            }
        }

        [MonoPInvokeCallback(typeof(
            GADUPictureInPictureAdDidDismissFullScreenContentCallback))]
        private static void
            PictureInPictureAdDidDismissFullScreenContentCallback(
                IntPtr pipAdClient)
        {
            PictureInPictureAdClient client =
                IntPtrToPictureInPictureAdClient(pipAdClient);
            if (client != null &&
                client.OnAdDidDismissFullScreenContent != null)
            {
                client.OnAdDidDismissFullScreenContent.Invoke();
            }
        }

        [MonoPInvokeCallback(typeof(GADUPictureInPictureAdPaidEventCallback))]
        private static void PictureInPictureAdPaidEventCallback(
            IntPtr pipAdClient, int precision, long value,
            string currencyCode)
        {
            PictureInPictureAdClient client =
                IntPtrToPictureInPictureAdClient(pipAdClient);
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

        private static PictureInPictureAdClient
            IntPtrToPictureInPictureAdClient(IntPtr pipAdClient)
        {
            GCHandle handle = (GCHandle)pipAdClient;
            return handle.Target as PictureInPictureAdClient;
        }

        #endregion
    }
}
#endif
