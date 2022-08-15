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
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// The base ad for full screen ad formats.
    /// </summary>
    public abstract class FullScreenAd : BaseAd
    {
        /// <summary>
        /// Raised when an ad opened full screen content.
        /// </summary>
        public event Action OnAdFullScreenContentOpened = delegate{};

        /// <summary>
        /// Raised when the ad closed full screen content.
        /// On iOS this does not include ads which open (safari) web browser links.
        /// </summary>
        public event Action OnAdFullScreenContentClosed = delegate{};

        /// <summary>
        /// Raised when the ad failed to open full screen content.
        /// </summary>
        public event Action<AdError> OnAdFullScreenContentFailed = delegate{};

        private bool _isInitialized;

        /// <summary>
        /// Initializes the ad, binding it to a platform client.
        /// </summary>
        internal void Init(IFullScreenAdClient client)
        {
            if (_isInitialized)
            {
                throw new Exception("Ad is already initialized.");
            }
            base.Init(client);
            client.OnAdFullScreenContentOpened += () =>
            {
                OnAdFullScreenContentOpened();
            };
            client.OnAdFullScreenContentClosed += () =>
            {
                OnAdFullScreenContentClosed();
            };
            client.OnAdFullScreenContentFailed += (errorClient) =>
            {
                OnAdFullScreenContentFailed(new AdError(errorClient));
            };
        }
    }
}
