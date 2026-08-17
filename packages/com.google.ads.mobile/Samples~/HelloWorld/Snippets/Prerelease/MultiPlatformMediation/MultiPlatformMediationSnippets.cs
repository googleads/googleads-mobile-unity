// Copyright 2026 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Snippets
{
    public class MultiPlatformMediationSnippets
    {
        // [START get_days_since_install]
        /// <summary>
        /// Returns the number of days since the application was first installed.
        /// </summary>
        public int? GetDaysSinceInstall()
        {
            try
            {
                string installTimestampStr = PlayerPrefs.GetString(
                    "INSTALL_DATE_KEY", null);
                if (string.IsNullOrEmpty(installTimestampStr))
                {
                    return null;
                }
                long installTimestamp = long.Parse(installTimestampStr);
                if (installTimestamp == 0)
                {
                    return null;
                }
                var installDate = DateTimeOffset
                    .FromUnixTimeMilliseconds(installTimestamp).UtcDateTime;
                return (int)(DateTime.UtcNow - installDate).TotalDays;
            }
            catch (Exception)
            {
                return null;
            }
        }
        // [END get_days_since_install]

        public void AddNetworkExtras()
        {
            // [START add_network_extras]
            var adRequest = new AdRequest();
            // TODO: This is an example. Replace with your own auction context.
            adRequest.Extras.Add(
                MultiPlatformUtils.AUCTION_CONTEXT_KEY,
                MultiPlatformUtils.AuctionContext.Parallel.Value);
            adRequest.Extras.Add(
                MultiPlatformUtils.USER_ACQUISITION_CHANNEL_KEY,
                MultiPlatformUtils.UserAcquisitionChannel.FromNetwork(
                    "ATTRIBUTION_NETWORK"));
            int? daysSinceInstall = GetDaysSinceInstall();
            if (daysSinceInstall.HasValue)
            {
                adRequest.Extras.Add(
                    MultiPlatformUtils.DAYS_SINCE_INSTALL_KEY,
                    daysSinceInstall.Value.ToString());
            }
            // [END add_network_extras]
        }

        // [START on_auction_complete]
        /// <summary>
        /// Call inside your code after internal multi-platform mediation runs.
        /// </summary>
        public void OnAuctionComplete(InterstitialAd interstitialAd)
        {
            ResponseInfo responseInfo = interstitialAd.GetResponseInfo();
            if (responseInfo == null)
            {
                return;
            }
            string responseId = responseInfo.GetResponseId();
            if (string.IsNullOrEmpty(responseId))
            {
                return;
            }

            // Get standard decimal USD bid from other platform. Use "0" if null.
            string highestOtherBid =
                GetMultiPlatformMediationHighestOtherBidUSD() ?? "0";

            // Arguments supply query parameters for the auction request:
            // - publisherId: Your AdMob publisher ID.
            // - responseId: The response ID for the loaded ad.
            // - highestOtherBid: The highest bid in USD from a non-AdMob source.
            MultiPlatformUtils.RecordAuctionEvent(
                "PUBLISHER_ID", responseId, highestOtherBid);
        }

        public string GetMultiPlatformMediationHighestOtherBidUSD()
        {
            // TODO: Fill with highest bid in standard decimal USD not from AdMob.
            return null;
        }
        // [END on_auction_complete]

        // [START on_ad_paid]
        /// <summary>
        /// Set the OnAdPaid event handler.
        /// </summary>
        public void SetOnPaidEventListener(InterstitialAd interstitialAd)
        {
            interstitialAd.OnAdPaid += (AdValue adValue) =>
            {
                ResponseInfo responseInfo = interstitialAd.GetResponseInfo();
                if (responseInfo == null)
                {
                    return;
                }
                string responseId = responseInfo.GetResponseId();
                if (string.IsNullOrEmpty(responseId))
                {
                    return;
                }

                // Convert the impression value from micros to standard decimal USD
                // (e.g., 7500 / 1000000m = 0.0075).
                string value = (adValue.Value / 1000000m).ToString(
                    System.Globalization.CultureInfo.InvariantCulture);
                string precision = ((int)adValue.Precision).ToString();

                // Arguments supply query parameters for the impression request:
                // - publisherId: Your AdMob publisher ID.
                // - responseId: The response ID for the loaded ad.
                // - value: The impression value in USD.
                // - precision: The precision type of the impression value.
                MultiPlatformUtils.RecordImpressionEvent(
                    "PUBLISHER_ID", responseId, value, precision);
            };
        }
        // [END on_ad_paid]
    }
}
