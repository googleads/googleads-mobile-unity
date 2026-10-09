using System;

namespace GoogleMobileAds.Common
{
    public interface IInsightsEmitter
    {
        void Emit(Insight insight);
    }
}
