using System;
using System.Runtime.InteropServices;

namespace GoogleMobileAds.iOS
{
    // Externs used by the iOS component.
    internal class Externs
    {
        #region Common externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateRequest();

        [DllImport("__Internal")]
        internal static extern void GADUAddTestDevice(IntPtr request, string deviceId);

        [DllImport("__Internal")]
        internal static extern void GADUAddKeyword(IntPtr request, string keyword);

        [DllImport("__Internal")]
        internal static extern void GADUSetBirthday(IntPtr request, int year, int month, int day);

        [DllImport("__Internal")]
        internal static extern void GADUSetGender(IntPtr request, int genderCode);

        [DllImport("__Internal")]
        internal static extern void GADUTagForChildDirectedTreatment(
                IntPtr request, bool childDirectedTreatment);

        [DllImport("__Internal")]
        internal static extern void GADUSetExtra(IntPtr request, string key, string value);

        [DllImport("__Internal")]
        internal static extern void GADURelease(IntPtr obj);

        #endregion

        #region Banner externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateBannerView(
                IntPtr bannerClient, string adUnitId, int width, int height, int positionAtTop);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateSmartBannerView(
                IntPtr bannerClient, string adUnitId, int positionAtTop);

        [DllImport("__Internal")]
        internal static extern void GADUSetBannerCallbacks(
                IntPtr bannerView,
                IOSBannerClient.GADUAdViewDidReceiveAdCallback adReceivedCallback,
                IOSBannerClient.GADUAdViewDidFailToReceiveAdWithErrorCallback adFailedCallback,
                IOSBannerClient.GADUAdViewWillPresentScreenCallback willPresentCallback,
                IOSBannerClient.GADUAdViewWillDismissScreenCallback willDismissCallback,
                IOSBannerClient.GADUAdViewDidDismissScreenCallback didDismissCallback,
                IOSBannerClient.GADUAdViewWillLeaveApplicationCallback willLeaveCallback);

        [DllImport("__Internal")]
        internal static extern void GADUHideBannerView(IntPtr bannerView);

        [DllImport("__Internal")]
        internal static extern void GADUShowBannerView(IntPtr bannerView);

        [DllImport("__Internal")]
        internal static extern void GADURemoveBannerView(IntPtr bannerView);

        [DllImport("__Internal")]
        internal static extern void GADURequestBannerAd(IntPtr bannerView, IntPtr request);

        #endregion

        #region Interstitial externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateInterstitial(
                IntPtr interstitialClient, string adUnitId);

        [DllImport("__Internal")]
        internal static extern void GADUSetInterstitialCallbacks(
                IntPtr interstitial,
                IOSInterstitialClient.GADUInterstitialDidReceiveAdCallback adReceivedCallback,
                IOSInterstitialClient.GADUInterstitialDidFailToReceiveAdWithErrorCallback
                        adFailedCallback,
                IOSInterstitialClient.GADUInterstitialWillPresentScreenCallback willPresentCallback,
                IOSInterstitialClient.GADUInterstitialWillDismissScreenCallback willDismissCallback,
                IOSInterstitialClient.GADUInterstitialDidDismissScreenCallback didDismissCallback,
                IOSInterstitialClient.GADUInterstitialWillLeaveApplicationCallback
                        willLeaveCallback);

        [DllImport("__Internal")]
        internal static extern bool GADUInterstitialReady(IntPtr interstitial);

        [DllImport("__Internal")]
        internal static extern void GADUShowInterstitial(IntPtr interstitial);

        [DllImport("__Internal")]
        internal static extern void GADURequestInterstitial(IntPtr interstitial, IntPtr request);

        #endregion
    }
}

