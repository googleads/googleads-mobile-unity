// Copyright (C) 2020 Google LLC
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

using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// Error information about why an ad load operation failed.
    /// </summary>
    public class LoadAdError : AdError
    {
        ILoadAdErrorClient _client;

        /// <summary>
        /// Creates a <see cref="LoadAdError"/>.
        /// </summary>
        /// <param name="client">
        /// A platform level <see cref="ILoadAdErrorClient"/> implementation.
        /// </param>
        public LoadAdError(ILoadAdErrorClient client) : base(client)
        {
            _client = client;
        }

        /// <summary>
        /// Returns ResponseInfo object for the failed request.
        /// <seealso href="https://developers.google.com/admob/unity/response-info">
        /// response-info</seealso> for more inforomation about Response Info.
        /// <summary>
        /// <returns>The <see cref="ResponseInfo"/> of this error.</returns>
        public ResponseInfo GetResponseInfo()
        {
            return new ResponseInfo(_client.GetResponseInfoClient());
        }

        /// <summary>
        /// Returns a log friendly string version of this object.
        /// </summary>
        public override string ToString()
        {
            return _client.ToString();
        }
    }
}
