using System;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    public class BannerView : IAdListener
    {
        private IGoogleMobileAdsBannerClient client;

        // These are the ad callback events that can be hooked into.
        public event Action AdLoaded = delegate {};
        public event Action<string> AdFailedToLoad = delegate {};
        public event Action AdOpened = delegate {};
        // AdClosing will only fire on iOS.
        public event Action AdClosing = delegate {};
        public event Action AdClosed = delegate {};
        public event Action AdLeftApplication = delegate {};

        // Create a BannerView and add it into the view hierarchy.
        public BannerView(string adUnitId, AdSize adSize, AdPosition position)
        {
            client = GoogleMobileAdsClientFactory.GetGoogleMobileAdsBannerClient(this);
            client.CreateBannerView(adUnitId, adSize, position);
        }

        // Load an ad into the BannerView.
        public void LoadAd(AdRequest request)
        {
            client.LoadAd(request);
        }

        // Hide the BannerView from the screen.
        public void Hide()
        {
            client.HideBannerView();
        }

        // Show the BannerView on the screen.
        public void Show()
        {
            client.ShowBannerView();
        }

        // Destroy the BannerView.
        public void Destroy()
        {
            client.DestroyBannerView();
        }

        #region IAdListener implementation

        // The following methods are invoked from an IGoogleMobileAdsClient. Forward these calls
        // to the developer.
        void IAdListener.FireAdLoaded()
        {
            AdLoaded();
        }

        void IAdListener.FireAdFailedToLoad(string message)
        {
            AdFailedToLoad(message);
        }

        void IAdListener.FireAdOpened()
        {
            AdOpened();
        }

        void IAdListener.FireAdClosing()
        {
            AdClosing();
        }

        void IAdListener.FireAdClosed()
        {
            AdClosed();
        }

        void IAdListener.FireAdLeftApplication()
        {
            AdLeftApplication();
        }

        #endregion
    }
}
