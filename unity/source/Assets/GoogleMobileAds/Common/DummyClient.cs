using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
    internal class DummyClient : IGoogleMobileAdsBannerClient, IGoogleMobileAdsInterstitialClient
    {
        public DummyClient(IAdListener listener)
        {
            Debug.Log("Created DummyClient");
        }

        public void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position)
        {
            Debug.Log("Dummy CreateBannerView");
        }

        public void LoadAd(AdRequest request)
        {
            Debug.Log("Dummy LoadAd");
        }

        public void ShowBannerView()
        {
            Debug.Log("Dummy ShowBannerView");
        }

        public void HideBannerView()
        {
            Debug.Log("Dummy HideBannerView");
        }

        public void DestroyBannerView()
        {
            Debug.Log("Dummy DestroyBannerView");
        }

        public void CreateInterstitialAd(string adUnitId) {
            Debug.Log("Dummy CreateIntersitialAd");
        }

        public bool IsLoaded() {
            Debug.Log("Dummy IsLoaded");
            return true;
        }

        public void ShowInterstitial() {
            Debug.Log("Dummy ShowInterstitial");
        }

        public void DestroyInterstitial() {
            Debug.Log("Dummy DestroyInterstitial");
        }

        public void SetInAppPurchaseParams(IInAppPurchaseListener listener, string androidPublicKey)
        {
            Debug.Log("Dummy SetInAppPurchaseParams");
        }
    }
}
