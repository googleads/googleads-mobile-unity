using System;

namespace GoogleMobileAds.Common
{
    public interface ITracer
    {
        ITrace StartTrace(string name);
    }
}
