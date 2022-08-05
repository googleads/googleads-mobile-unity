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
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds
{
    /// <summary>
    /// Event interface for all ad formats.
    /// </summary>
    public class BaseAd
    {
        /// <summary>
        /// Raised when the ad is estimated to have earned money.
        /// Available for allowlisted accounts only.
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
        /// Returns ad request Response info.
        /// </summary>
        public ResponseInfo Response
        {
            get { return _client != null ? new ResponseInfo(_client.Response) : null; }
        }

        /// <summary>
        /// Returns true if Destroy() has been called.
        /// </summary>
        public bool IsDestroyed { get { return _client != null; } }

        private IBaseAd _client;

        /// <summary>
        /// Initializes the Ad, binding it to a platform client.
        /// </summary>
        protected void Init(IBaseAd client)
        {
            _client = client;
            _client.OnAdPaid += OnAdPaid;
            _client.OnAdClickRecorded += OnAdClickRecorded;
            _client.OnAdImpressionRecorded += OnAdImpressionRecorded;
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
    }
}
