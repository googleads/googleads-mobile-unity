using System;

namespace GoogleMobileAds.Common
{
    // Interface for the methods to be invoked by the native plugin.
    internal interface IAdListener
    {
        void FireAdLoaded();
        void FireAdFailedToLoad(string message);
        void FireAdOpened();
        void FireAdClosing();
        void FireAdClosed();
        void FireAdLeftApplication();
    }
}
