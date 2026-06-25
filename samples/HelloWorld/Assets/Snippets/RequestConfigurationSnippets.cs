// Copyright 2026 Google LLC

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

        private void SetChildAgeTreatment()
        {
            // [START set_child_age_treatment]
            RequestConfiguration requestConfiguration = new RequestConfiguration
            {
                AgeRestrictedTreatment = AgeRestrictedTreatment.Child
            };
            MobileAds.SetRequestConfiguration(requestConfiguration);
            // [END set_child_age_treatment]
        }
        private void SetChildDirectedTreatment()
        {
            // [START set_child_directed_treatment]
            RequestConfiguration requestConfiguration = new RequestConfiguration
            {
                TagForChildDirectedTreatment = TagForChildDirectedTreatment.True
            };
            MobileAds.SetRequestConfiguration(requestConfiguration);
            // [END set_child_directed_treatment]
        }

        private void SetUnderAgeOfConsent()
        {
            // [START set_under_age_of_consent]
            RequestConfiguration requestConfiguration = new RequestConfiguration
            {
                TagForUnderAgeOfConsent = TagForUnderAgeOfConsent.True
            };
            MobileAds.SetRequestConfiguration(requestConfiguration);
            // [END set_under_age_of_consent]
        }

        private void SetMaxAdContentRating()
        {
            // [START set_max_ad_content_rating]
            RequestConfiguration requestConfiguration = new RequestConfiguration
            {
                MaxAdContentRating = MaxAdContentRating.G
            };
            MobileAds.SetRequestConfiguration(requestConfiguration);
            // [END set_max_ad_content_rating]
        }

        private void AddNetworkExtras()
        {
            // [START add_network_extras]
            var adRequest = new AdRequest();
            adRequest.Extras.Add("collapsible", "bottom");
            // [END add_network_extras]
        }
    }
}