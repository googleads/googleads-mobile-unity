using System;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    // Event that occurs when an ad fails to load.
    public class AdFailedToLoadEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
}
