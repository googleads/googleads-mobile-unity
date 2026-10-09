using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Snippets
{
    /// <summary>
    /// Code snippets used for the developer guides.
    /// </summary>
    internal class AdResponseInfoUtility
    {
        // [START get_ad_source_name]
        private string GetAdSourceName(AdapterResponseInfo loadedAdapterResponseInfo)
        {
            if (loadedAdapterResponseInfo == null)
            {
                return string.Empty;
            }

            string adSourceName = loadedAdapterResponseInfo.AdSourceName;

            if (adSourceName == "Custom Event")
            {

                #if UNITY_ANDROID
                    if (loadedAdapterResponseInfo.AdapterClassName ==
                        "com.google.ads.mediation.sample.customevent.SampleCustomEvent")
                    {
                        adSourceName = "Sample Ad Network (Custom Event)";
                    }
                #elif UNITY_IPHONE
                    if (loadedAdapterResponseInfo.AdapterClassName == "SampleCustomEvent")
                    {
                        adSourceName = "Sample Ad Network (Custom Event)";
                    }
                #endif

            }
            return adSourceName;
        }
        // [END get_ad_source_name]
    }
}
