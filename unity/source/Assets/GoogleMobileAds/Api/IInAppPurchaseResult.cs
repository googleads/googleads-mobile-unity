using System;

namespace GoogleMobileAds.Api
{
    public interface IInAppPurchaseResult
    {
        void FinishPurchase();
        string ProductId { get; }
    }
}
