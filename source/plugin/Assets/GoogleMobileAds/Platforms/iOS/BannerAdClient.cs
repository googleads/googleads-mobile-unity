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
using System.Runtime.InteropServices;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    public class BannerAdClient : IBannerAdClient, IDisposable
    {
        public bool IsDestroyed { get; private set; }

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

        public event Action OnBannerAdLoaded = delegate { };

        public event Action<ILoadAdErrorClient> OnBannerAdLoadFailed = delegate { };

        public event Action OnAdFullScreenContentOpened = delegate { };

        public event Action OnAdFullScreenContentClosed = delegate { };

        public event Action<AdValue> OnAdPaid = delegate { };

        public event Action OnAdClickRecorded = delegate { };

        public event Action OnAdImpressionRecorded = delegate { };

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

        #region IBannerAdClient implementation

        // Creates a banner view.
        public void CreateBannerAd(string adUnitId, AdSize adSize, AdPosition position)
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

        public void CreateBannerAd(string adUnitId, AdSize adSize, int x, int y)
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
        public void ShowAd()
        {
            Externs.GADUShowBannerView(this.BannerViewPtr);
        }

        // Hides the banner view from the screen.
        public void HideAd()
        {
            Externs.GADUHideBannerView(this.BannerViewPtr);
        }

        // Destroys the banner view.
        public void Destroy()
        {
            Externs.GADURemoveBannerView(this.BannerViewPtr);
            this.BannerViewPtr = IntPtr.Zero;
            IsDestroyed = true;
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
            this.Destroy();
            ((GCHandle)this.bannerClientPtr).Free();
        }

        ~BannerAdClient()
        {
            this.Dispose();
        }

#endregion

#region Banner callback methods

        [MonoPInvokeCallback(typeof(GADUAdViewDidReceiveAdCallback))]
        private static void AdViewDidReceiveAdCallback(IntPtr bannerClient)
        {
            BannerAdClient client = IntPtrToBannerClient(bannerClient);
            client.OnBannerAdLoaded();
        }

        [MonoPInvokeCallback(typeof(GADUAdViewDidFailToReceiveAdWithErrorCallback))]
        private static void AdViewDidFailToReceiveAdWithErrorCallback(
                IntPtr bannerClient, IntPtr error)
        {
            BannerAdClient client = IntPtrToBannerClient(bannerClient);
            client.OnBannerAdLoadFailed(new LoadAdErrorClient(error));
        }

        [MonoPInvokeCallback(typeof(GADUAdViewWillPresentScreenCallback))]
        private static void AdViewWillPresentScreenCallback(IntPtr bannerClient)
        {
            BannerAdClient client = IntPtrToBannerClient(bannerClient);
            client.OnAdFullScreenContentOpened();
        }

        [MonoPInvokeCallback(typeof(GADUAdViewDidDismissScreenCallback))]
        private static void AdViewDidDismissScreenCallback(IntPtr bannerClient)
        {
            BannerAdClient client = IntPtrToBannerClient(bannerClient);
            client.OnAdFullScreenContentClosed();
        }

        [MonoPInvokeCallback(typeof(GADUAdViewPaidEventCallback))]
        private static void AdViewPaidEventCallback(
            IntPtr bannerClient, int precision, long value, string currencyCode)
        {
            BannerAdClient client = IntPtrToBannerClient(bannerClient);
            AdValue adValue = new AdValue()
            {
                Precision = (AdValue.PrecisionType)precision,
                Value = value,
                CurrencyCode = currencyCode
            };
            client.OnAdPaid(adValue);
        }


        [MonoPInvokeCallback(typeof(GADUAdViewClickCallback))]
        private static void AdViewImpressionRecordedCallback(IntPtr adClientRef)
        {
            BannerAdClient client = IntPtrToBannerClient(adClientRef);
            client.OnAdImpressionRecorded();
        }

        [MonoPInvokeCallback(typeof(GADUAdViewImpressionCallback))]
        private static void AdViewClickRecordedCallback(IntPtr adClientRef)
        {
            BannerAdClient client = IntPtrToBannerClient(adClientRef);
            client.OnAdClickRecorded();
        }

        private static BannerAdClient IntPtrToBannerClient(IntPtr bannerClient)
        {
            GCHandle handle = (GCHandle)bannerClient;
            return handle.Target as BannerAdClient;
        }

#endregion
    }
}
#endif
