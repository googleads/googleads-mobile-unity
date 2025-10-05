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
using System.Collections.Generic;
using System.Linq;

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// Information about an ad response.
    /// </summary>
    public class ResponseInfo
    {
        private IResponseInfoClient _client;

        public ResponseInfo(IResponseInfoClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Returns the <see cref="AdapterResponseInfo"/> of the adapter that loaded
        /// the ad or null if the ad failed to load.
        /// </summary>
        public AdapterResponseInfo GetLoadedAdapterResponseInfo()
        {
            if (_client == null)
            {
                return null;
            }
            var adapterResponseInfoClient = _client.GetLoadedAdapterResponseInfo();
            return adapterResponseInfoClient == null
                    ? null
                    : new AdapterResponseInfo(adapterResponseInfoClient);
        }

        /// <summary>
        /// Returns the list of <see cref="AdapterResponseInfo"/> containing metadata
        /// for each adapter included in the ad response. Can be used to debug the
        /// mediation waterfall execution.
        /// </summary>
        public List<AdapterResponseInfo> GetAdapterResponses()
        {
            if (_client == null)
            {
                return new List<AdapterResponseInfo>();
            }
            var responses = _client.GetAdapterResponses();
            if (responses == null)
            {
                return new List<AdapterResponseInfo>();
            }
            return responses.Where(o => o != null).Select(o => new AdapterResponseInfo(o)).ToList();
        }

        /// <summary>
        /// Returns extra information about the ad response.
        /// </summary>
        public Dictionary<string, string> GetResponseExtras()
        {
            if (_client == null)
            {
                return new Dictionary<string, string>();
            }
            return _client.GetResponseExtras();
        }

        /// <summary>
        /// Returns the mediation adapter class name of the ad network that loaded the ad.
        ///
        /// In the case of a mediated ad response, this is the name of the class that was
        /// responsible for performing the ad request and rendering the ad.
        ///
        /// See <seealso href="https://developers.google.com/admob/unity/response-info"/>
        /// for possible return values.
        /// Returns null if the ad failed to load.
        /// </summary>
        public string GetMediationAdapterClassName()
        {
            if (_client == null)
            {
                return null;
            }
            return _client.GetMediationAdapterClassName();
        }

        /// <summary>
        /// Returns the response ID for the loaded ad.
        /// Can be used to look up ads in the Ad Review Center.
        /// Returns null if the ad failed to load.
        /// </summary>
        public string GetResponseId()
        {
            if (_client == null)
            {
                return null;
            }
            return _client.GetResponseId();
        }

        /// <summary>
        /// Returns a log friendly string version of this object.
        /// </summary>
        public override string ToString()
        {
            if (_client == null)
            {
                return string.Empty;
            }
            return _client.ToString();
        }
    }
}
