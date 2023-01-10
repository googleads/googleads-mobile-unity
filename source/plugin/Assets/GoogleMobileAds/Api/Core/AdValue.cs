
// Copyright (C) 2019 Google LLC
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

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// The monetary value earned from an ad.
    /// </summary>
    public class AdValue
    {
        /// <summary>
        /// The precision of the reported ad value.
        /// </summary>
        public enum PrecisionType
        {
            /// <summary>
            /// An ad value with unknown precision.
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// An ad value estimated from aggregated data.
            /// </summary>
            Estimated = 1,
            /// <summary>
            /// A publisher-provided ad value, such as manual CPMs in a mediation group.
            /// </summary>
            PublisherProvided = 2,
            /// <summary>
            /// The precise value paid for this ad.
            /// </summary>
            Precise = 3
        }

        /// <summary>
        /// The precision of the reported ad value.
        /// </summary>
        public PrecisionType Precision { get; set; }

        /// <summary>
        /// The ad's value in micro-units, where 1,000,000 micro-units equal one unit of
        /// the currency.
        /// </summary>
        public long Value { get; set; }

        /// <summary>
        /// The value's ISO 4217 currency code.
        /// </summary>
        public string CurrencyCode { get; set; }
    }
}

