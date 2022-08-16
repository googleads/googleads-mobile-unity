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

namespace GoogleMobileAds.Common
{
    /// <summary>
    /// The ad client interface for full screen ad formats.
    /// </summary>
    public interface IFullScreenAdClient : IBaseAdClient
    {
        /// <summary>
        /// Raised when an ad opened full screen content.
        /// </summary>
        event Action OnAdFullScreenContentOpened;

        /// <summary>
        /// Raised when the ad closed full screen content.
        /// On iOS this does not include ads which open (safari) web browser links.
        /// </summary>
        event Action OnAdFullScreenContentClosed;

        /// <summary>
        /// Raised when the ad failed to open full screen content.
        /// </summary>
        event Action<IAdErrorClient> OnAdFullScreenContentFailed;
    }
}