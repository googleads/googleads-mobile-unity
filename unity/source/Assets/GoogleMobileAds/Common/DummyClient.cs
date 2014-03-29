using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
    internal class DummyClient : IGoogleMobileAdsBannerClient
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
    }
}
