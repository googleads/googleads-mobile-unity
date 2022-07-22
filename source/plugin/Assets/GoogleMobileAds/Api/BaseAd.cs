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
    /// The base ad for all ad formats.
    /// </summary>
    public class BaseAd
    {
        /// <summary>
        /// Raised when the ad is estimated to have earned money.
        /// </summary>
        public event Action<AdValue> OnAdPaid = delegate{};

        /// <summary>
        /// Raised when a click is recorded for an ad.
        /// </summary>
        public event Action OnAdClickRecorded = delegate{};

        /// <summary>
        /// Raised when an impression is recorded for an ad.
        /// </summary>
        public event Action OnAdImpressionRecorded = delegate{};

        /// <summary>
        /// Returns true if Destroy() has been called.
        /// </summary>
        public bool IsDestroyed { get { return _client == null || _client.IsDestroyed; } }

        private IBaseAdClient _client;
        private bool _isInitialized;

        /// <summary>
        /// Initializes the ad, binding it to a platform client.
        /// </summary>
        internal void Init(IBaseAdClient client)
        {
            if (_isInitialized)
            {
                throw new Exception("Ad is already initialized.");
            }

            _isInitialized = true;
            _client = client;
            _client.OnAdPaid += (adValue) =>
            {
                OnAdPaid(adValue);
            };
            _client.OnAdClickRecorded += () =>
            {
                OnAdClickRecorded();
            };
            _client.OnAdImpressionRecorded += () =>
            {
                OnAdImpressionRecorded();
            };
        }

        /// <summary>
        /// Destroys the ad.
        /// </summary>
        public virtual void Destroy()
        {
            if (_client != null)
            {
                _client.Destroy();
                _client = null;
            }
        }

        /// <summary>
        /// Returns the ad request Response info.
        /// </summary>
        public ResponseInfo GetResponseInfo()
        {
            return _client != null ? new ResponseInfo(_client.GetResponseInfoClient()) : null;
        }
    }
}
