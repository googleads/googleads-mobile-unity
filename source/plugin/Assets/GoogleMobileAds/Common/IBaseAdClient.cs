// Copyright (C) 2022 Google, LLC
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

namespace GoogleMobileAds.Common
{
    /// <summary>
    /// The base ad client interface for all ad formats.
    /// </summary>
    public interface IBaseAdClient
    {
        /// <summary>
        /// Raised when the ad is estimated to have earned money.
        /// </summary>
        event Action<AdValue> OnAdPaid;

        /// <summary>
        /// Raised when a click is recorded for an ad.
        /// </summary>
        event Action OnAdClickRecorded;

        /// <summary>
        /// Raised when an impression is recorded for an ad.
        /// </summary>
        event Action OnAdImpressionRecorded;

        /// <summary>
        /// Returns the ad request response info client.
        /// </summary>
        IResponseInfoClient GetResponseInfoClient();

        /// <summary>
        /// Returns true if Destroy() has been called.
        /// </summary>
        bool IsDestroyed { get; }

        /// <summary>
        /// Destroys the ad client.
        /// </summary>
        void Destroy();
    }
}
