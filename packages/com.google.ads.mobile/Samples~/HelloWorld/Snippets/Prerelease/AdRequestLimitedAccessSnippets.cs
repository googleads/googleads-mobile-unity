using GoogleMobileAds.Api;
using System.Collections.Generic;
using UnityEngine;

namespace GoogleMobileAds.Snippets
{
    /// <summary>
    /// Code snippets for limited access ad requests.
    /// </summary>
    internal class AdRequestLimitedAccessSnippets
    {

        private void RequestWithCustomReportingParameter()
        {
            // [START request_with_custom_reporting_parameter]
            var adRequest = new AdRequest();
            adRequest.Extras.Add("custom_reporting_parameter", "nano");
            // [END request_with_custom_reporting_parameter]
        }
    }
}
