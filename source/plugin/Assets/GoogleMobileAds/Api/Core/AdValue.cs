
// Copyright (C) 2019 Google LLC.
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

using GoogleMobileAds.Api;

namespace GoogleMobileAds.Api
{

    // The monetary value earned from an ad.
    public class AdValue
    {
        public enum PrecisionType
        {
            // An ad value with unknown precision.
            Unknown = 0,
            // An ad value estimated from aggregated data.
            Estimated = 1,
            // A publisher-provided ad value, such as manual CPMs in a mediation group.
            PublisherProvided = 2,
            // The precise value paid for this ad.
            Precise = 3
        }

        // The precision of the reported ad value.
        public PrecisionType Precision { get; set; }

        // The ad's value in micro-units, where 1,000,000 micro-units equal one unit of
        // the currency.
        public long Value { get; set; }

        // The value's ISO 4217 currency code.
        public string CurrencyCode { get; set; }
    }
}

