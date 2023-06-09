// Copyright (C) 2020 Google, LLC
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
    /// Error information about why an ad operation failed.
    /// </summary>
    public class AdError
    {
        private IAdErrorClient _client;

        /// <summary>
        /// Creates a <see cref="AdError"/>.
        /// </summary>
        /// <param name="client">
        /// A platform level <see cref="IAdErrorClient"/> implementation.
        /// </param>
        public AdError(IAdErrorClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Returns the error code.
        /// </summary>
        /// <returns>The error code.</returns>
        public int GetCode()
        {
            return _client.GetCode();
        }

        /// <summary>
        /// Return "MobileAds" for Google Mobile Ads SDK errors, or a domain defined by
        /// mediation networks for mediation errors.
        /// </summary>
        /// <returns>The error domain.</returns>
        public string GetDomain()
        {
            return _client.GetDomain();
        }

        /// <summary>
        /// Returns the error message.
        /// <seealso href="https://support.google.com/admob/answer/9905175"/>
        /// for explanations of common errors.
        /// <summary>
        /// <returns>Ad Error Message. </returns>
        public string GetMessage()
        {
            return _client.GetMessage();
        }

        ///<summary>
        /// Returns the cause of this error or <c>null</c> if the cause is nonexistent or unknown.
        /// <summary>
        /// <returns>Returns the cause of this error.</returns>
        public AdError GetCause()
        {
            var cause = _client.GetCause();
            return cause == null ? null : new AdError(_client.GetCause());
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

