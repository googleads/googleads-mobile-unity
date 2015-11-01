using System;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
    // Interface for the methods to be invoked by the native plugin.
    internal interface IInAppPurchaseListener
    {
        void FireOnInAppPurchaseFinished(IInAppPurchaseResult result);
        bool FireIsValidPurchase(string sku);
    }
}
