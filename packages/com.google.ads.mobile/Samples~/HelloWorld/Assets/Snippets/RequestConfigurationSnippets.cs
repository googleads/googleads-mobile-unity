using UnityEngine;
using GoogleMobileAds.Api;
using System.Collections.Generic;

namespace GoogleMobileAds.Snippets
{
    /// <summary>
    /// Code snippets used for the developer guides.
    /// </summary>
    internal class RequestConfigurationSnippets
    {
        // Replace with your own test device ID.
        private const string TEST_DEVICE_ID = "2077ef9a63d2b398840261c8221a0c9b";

        private void SetTestDeviceIds()
        {
            // [START set_test_device_ids]
            List<string> testDeviceIds = new List<string>();
            testDeviceIds.Add(TEST_DEVICE_ID);

            RequestConfiguration requestConfiguration = new RequestConfiguration
            {
                TestDeviceIds = testDeviceIds
            };
            // [END set_test_device_ids]

            // [START set_request_configuration]
            MobileAds.SetRequestConfiguration(requestConfiguration);
            // [END set_request_configuration]
        }
    }
}