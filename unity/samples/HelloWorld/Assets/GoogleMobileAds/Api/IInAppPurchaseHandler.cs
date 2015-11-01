using System;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    public interface IInAppPurchaseHandler
    {
        void OnInAppPurchaseFinished(IInAppPurchaseResult result);
        bool IsValidPurchase(string sku);
        string AndroidPublicKey { get; }
    }
}
