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
using UnityEngine;
using UnityEngine.Networking;

namespace GoogleMobileAds.Snippets
{
    // [START multi_platform_utils]
    /// <summary>
    /// Utility class for multi-platform mediation.
    /// </summary>
    public class MultiPlatformUtils
    {
        public const string AUCTION_CONTEXT_KEY = "auction_context";
        public const string USER_ACQUISITION_CHANNEL_KEY = "ua_channel";
        public const string DAYS_SINCE_INSTALL_KEY = "days_since_install";

        public class AuctionContext
        {
            public static readonly AuctionContext Parallel =
                new AuctionContext("parallel");
            public static readonly AuctionContext PostBidFirst =
                new AuctionContext("post_bid_first");
            public static readonly AuctionContext PostBidSecond =
                new AuctionContext("post_bid_second");

            public string Value { get; private set; }

            private AuctionContext(string value)
            {
                Value = value;
            }
        }

        public class UserAcquisitionChannel
        {
            public static readonly string GoogleAds = "google_ads";
            public static readonly string Organic = "organic";
            public static readonly string AppleSearchAds = "apple_search_ads";
            public static readonly string Applovin = "applovin";
            public static readonly string Unity = "unity";
            public static readonly string Meta = "meta";
            public static readonly string Mintegral = "mintegral";
            public static readonly string Moloco = "moloco";
            public static readonly string Liftoff = "liftoff";
            public static readonly string Pangle = "pangle";

            // [START map_ua_channel]
            public static string FromNetwork(string network)
            {
                string lowercaseNetwork = network.ToLower();
                if (lowercaseNetwork.Contains("google")
                    || lowercaseNetwork.Contains("adwords"))
                {
                    return GoogleAds;
                }
                else if (lowercaseNetwork.Contains("meta")
                         || lowercaseNetwork.Contains("facebook"))
                {
                    return Meta;
                }
                else if (lowercaseNetwork.Contains("applovin"))
                {
                    return Applovin;
                }
                else if (lowercaseNetwork.Contains("organic"))
                {
                    return Organic;
                }
                else if (lowercaseNetwork.Contains("apple"))
                {
                    return AppleSearchAds;
                }
                else if (lowercaseNetwork.Contains("unity"))
                {
                    return Unity;
                }
                else if (lowercaseNetwork.Contains("mintegral"))
                {
                    return Mintegral;
                }
                else if (lowercaseNetwork.Contains("moloco"))
                {
                    return Moloco;
                }
                else if (lowercaseNetwork.Contains("liftoff"))
                {
                    return Liftoff;
                }
                else if (lowercaseNetwork.Contains("pangle"))
                {
                    return Pangle;
                }

                // If network does not match predefined values, return as is.
                return network;
            }
            // [END map_ua_channel]
        }

        /// <summary>
        /// Records an auction event with the given parameters.
        /// </summary>
        public static void RecordAuctionEvent(string publisherId,
                                              string responseId,
                                              string highestOtherBid)
        {
            string safePublisherId = UnityWebRequest.EscapeURL(publisherId);
            string safeResponseId = UnityWebRequest.EscapeURL(responseId);
            string safeHob = UnityWebRequest.EscapeURL(highestOtherBid);

            string url = "https://pagead2.googlesyndication.com/pagead/gen_204/"
                + $"?id=ampm&type=auction&response_id={safeResponseId}"
                + $"&wp={safePublisherId}&hob={safeHob}";

            SendRequest(url);
        }

        /// <summary>
        /// Records an impression event with the given parameters.
        /// </summary>
        public static void RecordImpressionEvent(string publisherId,
                                                 string responseId,
                                                 string adValue,
                                                 string precision)
        {
            string safePublisherId = UnityWebRequest.EscapeURL(publisherId);
            string safeResponseId = UnityWebRequest.EscapeURL(responseId);
            string safeAdValue = UnityWebRequest.EscapeURL(adValue);
            string safePrecision = UnityWebRequest.EscapeURL(precision);

            string url = "https://pagead2.googlesyndication.com/pagead/gen_204/"
                + $"?id=ampm&type=impression&response_id={safeResponseId}"
                + $"&wp={safePublisherId}&piv={safeAdValue}&pip={safePrecision}";

            SendRequest(url);
        }

        private static void SendRequest(string url)
        {
            UnityWebRequest uwr = UnityWebRequest.Get(url);
            var op = uwr.SendWebRequest();
            op.completed += (operation) =>
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("DataRequest Response Code: " + uwr.responseCode);
                }
                else
                {
                    Debug.LogError("Error sending request: " + uwr.error);
                }
                uwr.Dispose();
            };
        }
    }
    // [END multi_platform_utils]
}
