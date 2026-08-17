// Copyright 2026 Google LLC

using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Api.AdManager;
using UnityEngine;

namespace GoogleMobileAds.Snippets
{
    /// <summary>
    /// Code snippets used for the developer guides.
    /// </summary>
    internal class AdRequestSnippets
    {
        private void AddNetworkExtras()
        {
            // [START add_network_extras]
            var adRequest = new AdRequest();
            adRequest.Extras.Add("collapsible", "bottom");
            // [END add_network_extras]
        }

        private void AddCustomTargeting()
        {
            // [START add_custom_targeting]
            // Example: Pass custom targeting "age=25".
            AdManagerAdRequest newRequest = new AdManagerAdRequest
            {
                CustomTargeting = new Dictionary<string, string>
                {
                    { "age", "25" }
                }
            };
            // [END add_custom_targeting]
        }

        private void AddCustomTargetingAsList()
        {
            // [START add_custom_targeting_as_list]
            AdManagerAdRequest newRequest = new AdManagerAdRequest
            {
                CustomTargeting = new Dictionary<string, string>
                {
                    { "age", "24, 25, 26" }
                }
            };
            // [END add_custom_targeting_as_list]
        }

        private void AddCategoryExclusions()
        {
            // [START add_category_exclusions]
            AdManagerAdRequest newRequest = new AdManagerAdRequest
            {
                CategoryExclusions = new HashSet<string>
                {
                    "automobile",
                    "boat"
                }
            };
            // [END add_category_exclusions]
        }

        private void SetPublisherProvidedId()
        {
            // [START set_publisher_provided_id]
            AdManagerAdRequest newRequest = new AdManagerAdRequest
            {
                PublisherProvidedId = "AB123456789"
            };
            // [END set_publisher_provided_id]
        }

        private void SetPublisherProvidedSignals()
        {
            // [START set_publisher_provided_signals]
            AdManagerAdRequest newRequest = new AdManagerAdRequest
            {
                Extras = new Dictionary<string, string>
                {
                    // Set the demographic to an audience with an "Age Range" of 30-34
                    // and an interest in mergers and acquisitions.
                    { "IAB_AUDIENCE_1_1", "1, 2, 3, 4, 5" },
                    // Set the content to sedan, station wagon and SUV automotive values.
                    { "IAB_AUDIENCE_2_2", "6, 7, 8, 9, 10" }
                }
            };
            // [END set_publisher_provided_signals]
        }
    }
}
