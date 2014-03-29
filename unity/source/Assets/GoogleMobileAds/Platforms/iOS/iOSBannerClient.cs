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

        private delegate void GADUAdViewDidReceiveAdCallback(IntPtr bannerClient);
        private delegate void GADUAdViewDidFailToReceiveAdWithErrorCallback(
                IntPtr bannerClient, string error);
        private delegate void GADUAdViewWillPresentScreenCallback(IntPtr bannerClient);
        private delegate void GADUAdViewWillDismissScreenCallback(IntPtr bannerClient);
        private delegate void GADUAdViewDidDismissScreenCallback(IntPtr bannerClient);
        private delegate void GADUAdViewWillLeaveApplicationCallback(IntPtr bannerClient);

        #endregion

        #region iOS entry points

        [DllImport("__Internal")]
        private static extern IntPtr GADUCreateBannerView(IntPtr bannerClient,
                                                          string adUnitId,
                                                          int width,
                                                          int height,
                                                          int positionAtTop);

        [DllImport("__Internal")]
        private static extern IntPtr GADUCreateSmartBannerView(IntPtr bannerClient,
                                                               string adUnitId,
                                                               int positionAtTop);

        [DllImport("__Internal")]
        private static extern void GADUSetCallbacks(
                IntPtr bannerView,
                GADUAdViewDidReceiveAdCallback adReceivedCallback,
                GADUAdViewDidFailToReceiveAdWithErrorCallback adFailedCallback,
                GADUAdViewWillPresentScreenCallback willPresentCallback,
                GADUAdViewWillDismissScreenCallback willDismissCallback,
                GADUAdViewDidDismissScreenCallback didDismissCallback,
                GADUAdViewWillLeaveApplicationCallback willLeaveCallback);

        [DllImport("__Internal")]
        private static extern void GADUHideBannerView(IntPtr bannerView);

        [DllImport("__Internal")]
        private static extern void GADUShowBannerView(IntPtr bannerView);

        [DllImport("__Internal")]
        private static extern IntPtr GADUCreateRequest();

        [DllImport("__Internal")]
        private static extern void GADUAddTestDevice(IntPtr request, string deviceId);

        [DllImport("__Internal")]
        private static extern void GADUAddKeyword(IntPtr request, string keyword);

        [DllImport("__Internal")]
        private static extern void GADUSetBirthday(IntPtr request, int year, int month, int day);

        [DllImport("__Internal")]
        private static extern void GADUSetGender(IntPtr request, int genderCode);

        [DllImport("__Internal")]
        private static extern void GADUTagForChildDirectedTreatment(
                IntPtr request, bool childDirectedTreatment);

        [DllImport("__Internal")]
        private static extern void GADUSetExtra(IntPtr request, string key, string value);

        [DllImport("__Internal")]
        private static extern void GADURequestBannerAd(IntPtr bannerView, IntPtr request);

        [DllImport("__Internal")]
        private static extern void GADURelease(IntPtr bannerView);

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
                GADURelease(bannerViewPtr);
                bannerViewPtr = value;
            }
        }

        #region IGoogleMobileAdsBannerClient implementation

        // Creates a banner view.
        public void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position) {
            IntPtr bannerClientPtr = (IntPtr) GCHandle.Alloc(this);

            if (adSize.IsSmartBanner) {
                BannerViewPtr = GADUCreateSmartBannerView(bannerClientPtr, adUnitId, (int)position);
            }
            else
            {
                BannerViewPtr = GADUCreateBannerView(
                        bannerClientPtr, adUnitId, adSize.Width, adSize.Height, (int)position);
            }
            GADUSetCallbacks(BannerViewPtr,
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
            IntPtr requestPtr = GADUCreateRequest();
            foreach (string keyword in request.Keywords)
            {
                GADUAddKeyword(requestPtr, keyword);
            }
            foreach (string deviceId in request.TestDevices)
            {
                GADUAddTestDevice(requestPtr, deviceId);
            }
            if (request.Birthday.HasValue)
            {
                DateTime birthday = request.Birthday.GetValueOrDefault();
                GADUSetBirthday(requestPtr, birthday.Year, birthday.Month, birthday.Day);
            }
            if (request.Gender.HasValue)
            {
                GADUSetGender(requestPtr, (int)request.Gender.GetValueOrDefault());
            }
            if (request.TagForChildDirectedTreatment.HasValue) {
                GADUTagForChildDirectedTreatment(
                        requestPtr, request.TagForChildDirectedTreatment.GetValueOrDefault());
            }
            foreach (KeyValuePair<string, string> entry in request.Extras)
            {
                GADUSetExtra(requestPtr, entry.Key, entry.Value);
            }
            GADURequestBannerAd(BannerViewPtr, requestPtr);
            GADURelease(requestPtr);
        }

        // Show the banner view on the screen.
        public void ShowBannerView() {
            GADUShowBannerView(BannerViewPtr);
        }

        // Hide the banner view from the screen.
        public void HideBannerView()
        {
            GADUHideBannerView(BannerViewPtr);
        }

        public void DestroyBannerView()
        {
            GADURelease(BannerViewPtr);
        }

        #endregion

        #region Banner callback methods

        [MonoPInvokeCallback(typeof(GADUAdViewDidReceiveAdCallback))]
        private static void AdViewDidReceiveAdCallback(IntPtr bannerClient)
        {
            IntPtrToBannerClient(bannerClient).listener.FireAdLoaded();
        }

        [MonoPInvokeCallback(typeof(GADUAdViewDidFailToReceiveAdWithErrorCallback))]
        private static void AdViewDidFailToReceiveAdWithErrorCallback(IntPtr bannerClient, string error)
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

