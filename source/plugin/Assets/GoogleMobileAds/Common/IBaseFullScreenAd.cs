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
    /// Event interface for all full screen ad formats.
    /// </summary>
    public interface IBaseFullScreenAd : IBaseAd
    {
        /// <summary>
        /// Raised when an overlay ad that covers the screen is opened.
        /// </summary>
        event Action OnAdFullScreenContentOpened;

        /// <summary>
        /// Raised when returning to the application after closing an overlay ad.
        /// </summary>
        event Action OnAdFullScreenContentClosed;

        /// <summary>
        /// Raised when failing to open an overlay ad.
        /// </summary>
        event Action<IAdErrorClient> OnAdFullScreenContentFailed;
    }
}
