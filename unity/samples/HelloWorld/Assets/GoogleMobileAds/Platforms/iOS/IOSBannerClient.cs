using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    internal class IOSBannerClient : IGoogleMobileAdsBannerClient
    {
        #region Banner callback types

        internal delegate void GADUAdViewDidReceiveAdCallback(IntPtr bannerClient);
        internal delegate void GADUAdViewDidFailToReceiveAdWithErrorCallback(
                IntPtr bannerClient, string error);
        internal delegate void GADUAdViewWillPresentScreenCallback(IntPtr bannerClient);
        internal delegate void GADUAdViewWillDismissScreenCallback(IntPtr bannerClient);
        internal delegate void GADUAdViewDidDismissScreenCallback(IntPtr bannerClient);
        internal delegate void GADUAdViewWillLeaveApplicationCallback(IntPtr bannerClient);

        #endregion

        private IAdListener listener;
        private IntPtr bannerViewPtr;
        private static Dictionary<IntPtr, IOSBannerClient> bannerClients;

        public IOSBannerClient(IAdListener listener)
        {
            this.listener = listener;
        }

        // This property should be used when setting the bannerViewPtr.
        private IntPtr BannerViewPtr
        {
            get
            {
                return bannerViewPtr;
            }
            set
            {
                Externs.GADURelease(bannerViewPtr);
                bannerViewPtr = value;
            }
        }

        #region IGoogleMobileAdsBannerClient implementation

        // Creates a banner view.
        public void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position) {
            IntPtr bannerClientPtr = (IntPtr) GCHandle.Alloc(this);

            if (adSize.IsSmartBanner) {
                BannerViewPtr = Externs.GADUCreateSmartBannerView(
                        bannerClientPtr, adUnitId, (int)position);
            }
            else
            {
                BannerViewPtr = Externs.GADUCreateBannerView(
                        bannerClientPtr, adUnitId, adSize.Width, adSize.Height, (int)position);
            }
            Externs.GADUSetBannerCallbacks(
                    BannerViewPtr,
                    AdViewDidReceiveAdCallback,
                    AdViewDidFailToReceiveAdWithErrorCallback,
                    AdViewWillPresentScreenCallback,
                    AdViewWillDismissScreenCallback,
                    AdViewDidDismissScreenCallback,
                    AdViewWillLeaveApplicationCallback);
        }

        // Load an ad.
        public void LoadAd(AdRequest request)
        {
            IntPtr requestPtr = Externs.GADUCreateRequest();
            foreach (string keyword in request.Keywords)
            {
                Externs.GADUAddKeyword(requestPtr, keyword);
            }
            foreach (string deviceId in request.TestDevices)
            {
                Externs.GADUAddTestDevice(requestPtr, deviceId);
            }
            if (request.Birthday.HasValue)
            {
                DateTime birthday = request.Birthday.GetValueOrDefault();
                Externs.GADUSetBirthday(requestPtr, birthday.Year, birthday.Month, birthday.Day);
            }
            if (request.Gender.HasValue)
            {
                Externs.GADUSetGender(requestPtr, (int)request.Gender.GetValueOrDefault());
            }
            if (request.TagForChildDirectedTreatment.HasValue) {
                Externs.GADUTagForChildDirectedTreatment(
                        requestPtr, request.TagForChildDirectedTreatment.GetValueOrDefault());
            }
            foreach (KeyValuePair<string, string> entry in request.Extras)
            {
                Externs.GADUSetExtra(requestPtr, entry.Key, entry.Value);
            }
            Externs.GADURequestBannerAd(BannerViewPtr, requestPtr);
            Externs.GADURelease(requestPtr);
        }

        // Show the banner view on the screen.
        public void ShowBannerView() {
            Externs.GADUShowBannerView(BannerViewPtr);
        }

        // Hide the banner view from the screen.
        public void HideBannerView()
        {
            Externs.GADUHideBannerView(BannerViewPtr);
        }

        public void DestroyBannerView()
        {
            Externs.GADURemoveBannerView(BannerViewPtr);
            BannerViewPtr = IntPtr.Zero;
        }

        #endregion

        #region Banner callback methods

        [MonoPInvokeCallback(typeof(GADUAdViewDidReceiveAdCallback))]
        private static void AdViewDidReceiveAdCallback(IntPtr bannerClient)
        {
            IntPtrToBannerClient(bannerClient).listener.FireAdLoaded();
        }

        [MonoPInvokeCallback(typeof(GADUAdViewDidFailToReceiveAdWithErrorCallback))]
        private static void AdViewDidFailToReceiveAdWithErrorCallback(
                IntPtr bannerClient, string error)
        {
            IntPtrToBannerClient(bannerClient).listener.FireAdFailedToLoad(error);
        }

        [MonoPInvokeCallback(typeof(GADUAdViewWillPresentScreenCallback))]
        private static void AdViewWillPresentScreenCallback(IntPtr bannerClient)
        {
            IntPtrToBannerClient(bannerClient).listener.FireAdOpened();
        }

        [MonoPInvokeCallback(typeof(GADUAdViewWillDismissScreenCallback))]
        private static void AdViewWillDismissScreenCallback(IntPtr bannerClient)
        {
            IntPtrToBannerClient(bannerClient).listener.FireAdClosing();
        }

        [MonoPInvokeCallback(typeof(GADUAdViewDidDismissScreenCallback))]
        private static void AdViewDidDismissScreenCallback(IntPtr bannerClient)
        {
            IntPtrToBannerClient(bannerClient).listener.FireAdClosed();
        }

        [MonoPInvokeCallback(typeof(GADUAdViewWillLeaveApplicationCallback))]
        private static void AdViewWillLeaveApplicationCallback(IntPtr bannerClient)
        {
            IntPtrToBannerClient(bannerClient).listener.FireAdLeftApplication();
        }

        private static IOSBannerClient IntPtrToBannerClient(IntPtr bannerClient)
        {
            GCHandle handle = (GCHandle) bannerClient;
            return handle.Target as IOSBannerClient;
        }

        #endregion
    }
}
